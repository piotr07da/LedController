/*
 * Communication.c
 *
 * Created: 2013-02-01 19:01:26
 *  Author: pbejger
 */ 

#include "Communication.h"

void ReadFrameByte(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    frame->ReceivedByte = byte;
    
    if (!ReadFrameBegin(byte, frame))
    {
        if (!ReadFrameType(byte, frame))
        {
            if (!ReadFrameDataSize(byte, frame))
            {
                ReadFrameDataByte(byte, frame, colorTimeLine);
            }
        }
    }
}

uint8_t ReadFrameBegin(uint8_t byte, DataFrame_t *frame)
{
    if (frame->State != com_fs_HDR_BYTE_RECEIVED && frame->State != com_fs_TYPE_BYTE_RECEIVED && frame->DataBytesToReadCount == 0 && byte == com_rb_FRAME_HDR_BYTE)
    {
        // If header byte was not received in last read AND
        // If type byte was not received in last read AND
        // If number of bytes to read is zero AND
        // If header byte is received in current read
        
        frame->State = com_fs_HDR_BYTE_RECEIVED;
        
        return TRUE;
    }
    
    return FALSE;
}

uint8_t ReadFrameType(uint8_t byte, DataFrame_t *frame)
{
    if (frame->State == com_fs_HDR_BYTE_RECEIVED)
    {
        frame->State = com_fs_TYPE_BYTE_RECEIVED;
        frame->Type = byte;
        
        // TODO - CHECK IF TYPE IS VALID
        
        // Resetting values
        switch (byte)
        {
            case com_mt_SET_TIME_SPAN:
                frame->SetTimeSpanData = 0;
                break;
            case com_mt_SET_TIME_PROGRESS:
                frame->SetTimeProgressData = 0;
                break;
        }
        
        return TRUE;
    }
    
    return FALSE;
}

uint8_t ReadFrameDataSize(uint8_t byte, DataFrame_t *frame)
{
    if (frame->State == com_fs_TYPE_BYTE_RECEIVED)
    {
        frame->State = com_fs_LEN_BYTE_RECEIVED;
        frame->DataBytesToReadCount = byte;
        frame->DataByteIndex = -1;
        
        // TODO - CHECK IF LENGTH IS VALID FOR GIVEN TYPE
        
        return TRUE;
    }
    
    return FALSE;
}

void ReadFrameDataByte(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    if (frame->State != com_fs_LEN_BYTE_RECEIVED && frame->State != com_fs_DATA_BYTE_RECEIVED)
        return;
    
    frame->State = com_fs_DATA_BYTE_RECEIVED;
    ++frame->DataByteIndex;
    --frame->DataBytesToReadCount;
    
    switch (frame->Type)
    {
        case com_mt_ADD_MARKER:
            ReadFrameDataByteForAddMarker(byte, frame, colorTimeLine);
            break;
            
        case com_mt_REM_MARKER:
            ReadFrameDataByteForRemMarker(byte, frame, colorTimeLine);
            break;
            
        case com_mt_SET_MARKER:
            ReadFrameDataByteForSetMarker(byte, frame, colorTimeLine);
            break;
            
        case com_mt_GET_MARKER:
            ReadFrameDataByteForGetMarker(byte, frame, colorTimeLine);
            break;
            
        case com_mt_GET_MARKER_COUNT:
            break;
            
        case com_mt_SET_TIME_SPAN:
            ReadFrameDataByteForSetTimeSpan(byte, frame, colorTimeLine);
            break;
            
        case com_mt_GET_TIME_SPAN:
            break;
            
        case com_mt_SET_TIME_PROGRESS:
            ReadFrameDataByteForSetTimeProgress(byte, frame, colorTimeLine);
            break;
            
        case com_mt_GET_TIME_PROGRESS:
            break;
    }
    
    if (frame->DataBytesToReadCount == 0)
        frame->State = com_fs_END;
}

void ReadFrameDataByteForAddMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    uint8_t mIx;
    uint8_t markerCount = colorTimeLine->PointCount;
    for (mIx = 0; mIx < markerCount; ++mIx)
        colorTimeLine->Points[mIx].TimePoint = (colorTimeLine->Points[mIx].TimePoint * (markerCount - 1)) / markerCount;
    
    SetAsLastPoint(&colorTimeLine->Points[markerCount], colorTimeLine);
    ++colorTimeLine->PointCount;
}

void ReadFrameDataByteForRemMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    if (colorTimeLine->PointCount <= ctl_MIN_POINT_COUNT)
        return;
    
    uint8_t mIx;
    uint8_t markerCount = --colorTimeLine->PointCount;
    for (mIx = 0; mIx < markerCount - 1; ++mIx)
        colorTimeLine->Points[mIx].TimePoint = (colorTimeLine->Points[mIx].TimePoint * markerCount) / (markerCount - 1);
        
    SetAsLastPoint(&colorTimeLine->Points[markerCount - 1], colorTimeLine);
}

void ReadFrameDataByteForSetMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    if (frame->DataByteIndex == com_dbix_addMarker_INDEX)
    {
        if (byte < colorTimeLine->PointCount)
            frame->SetMarkerData.Index = byte;
        else
        {
            frame->State = com_fs_err_MARKER_INDEX_OUT_OF_RANGE;
            return;
        }            
    }        
    else if (frame->DataByteIndex == com_dbix_addMarker_TIME_POINT)
        frame->SetMarkerData.ColorTimePoint.TimePoint = byte;
    else
        frame->SetMarkerData.ColorTimePoint.ColorBytes[frame->DataByteIndex - com_dbix_addMarker_R] = byte;
        
    if (frame->DataByteIndex == com_dbix_addMarker_B)
    {
        colorTimeLine->Points[frame->SetMarkerData.Index] = frame->SetMarkerData.ColorTimePoint;
        
        // First and last color have to be the same
        if (frame->SetMarkerData.Index == 0)
            colorTimeLine->Points[colorTimeLine->PointCount - 1].Color = frame->SetMarkerData.ColorTimePoint.Color;
        else if (frame->SetMarkerData.Index == colorTimeLine->PointCount - 1)
            colorTimeLine->Points[0].Color = frame->SetMarkerData.ColorTimePoint.Color;
    }        
}

void ReadFrameDataByteForGetMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    //frame->GetMarkerData.Index = byte;
    //frame->DataByteIndex = -1;
    //frame->DataBytesToSendCount = ctl_COLOR_TIME_POINT_SIZE;
    //frame->State = com_fs_END;
}

void ReadFrameDataByteForSetTimeSpan(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    frame->SetTimeSpanData |= ((uint16_t)byte) << (frame->DataByteIndex * 8);
    if (frame->DataByteIndex == ctl_TIME_SPAN_SIZE - 1)
        colorTimeLine->TimeSpan = frame->SetTimeSpanData;
}

void ReadFrameDataByteForSetTimeProgress(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    frame->SetTimeProgressData |= ((uint16_t)byte) << (frame->DataByteIndex * 8);
    if (frame->DataByteIndex == ctl_TIME_PROGRESS_SIZE - 1)
        colorTimeLine->TimeProgress = frame->SetTimeProgressData;
}

uint8_t GetResponseFrameByte(DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    //++frame->DataByteIndex;
    //--frame->DataBytesToSendCount;
    //
    //switch (frame->Type)
    //{
        //case com_mt_GET_MARKER:
            //return GetResponseFrameDataByteForGetMarker(frame, colorTimeLine);
            //
        //case com_mt_GET_MARKER_COUNT:
            //break;
            //
        //case com_mt_GET_TIME_SPAN:
            //break;
            //
        //case com_mt_GET_TIME_PROGRESS:
            //break;
    //}
    
    return 0;
}

uint8_t GetResponseFrameDataByteForGetMarker(DataFrame_t *frame, ColorTimeLine_t *colorTimeLine)
{
    if (frame->DataByteIndex == 0)
        return colorTimeLine->Points[frame->GetMarkerData.Index].TimePoint;
    return colorTimeLine->Points[frame->GetMarkerData.Index].ColorBytes[frame->DataByteIndex - 1];
}

void SetAsLastPoint(ColorTimePoint_t *point, ColorTimeLine_t *colorTimeLine)
{
    point->TimePoint = UINT8_MAX;
    point->Color = colorTimeLine->Points[0].Color;
}
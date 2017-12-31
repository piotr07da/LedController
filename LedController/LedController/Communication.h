/*
 * Communication.h
 *
 * Created: 2013-02-01 18:46:00
 *  Author: pbejger
 */ 

#include "GlobalConstants.h"
#include "ColorTimeLine.h"

#ifndef COMMUNICATION_H_
#define COMMUNICATION_H_

// RECEIVED BYTE
#define com_rb_FRAME_HDR_BYTE '<'

// FRAME STATES
#define com_fs_HDR_BYTE_RECEIVED 1
#define com_fs_TYPE_BYTE_RECEIVED 2
#define com_fs_LEN_BYTE_RECEIVED 3
#define com_fs_DATA_BYTE_RECEIVED 4
#define com_fs_END 20
#define com_fs_err_UNKOWN_ERROR 100
#define com_fs_err_WRONG_TYPE 101
#define com_fs_err_WRONG_LEN = 102
#define com_fs_err_MARKER_INDEX_OUT_OF_RANGE 103

// MESSAGE TYPES
#define com_mt_ADD_MARKER 1
#define com_mt_REM_MARKER 2
#define com_mt_SET_MARKER 3
#define com_mt_GET_MARKER 4
#define com_mt_GET_MARKER_COUNT 5
#define com_mt_SET_TIME_SPAN 6
#define com_mt_GET_TIME_SPAN 7
#define com_mt_SET_TIME_PROGRESS 8
#define com_mt_GET_TIME_PROGRESS 9

// DATA BYTE INDICES
#define com_dbix_addMarker_INDEX 0
#define com_dbix_addMarker_TIME_POINT 1
#define com_dbix_addMarker_R 2
#define com_dbix_addMarker_G 3
#define com_dbix_addMarker_B 4

struct MarkerData;
struct DataFrame;

typedef struct MarkerData MarkerData_t;
typedef struct DataFrame DataFrame_t;

struct MarkerData
{
    uint8_t Index;
    ColorTimePoint_t ColorTimePoint;
};

struct DataFrame
{
    uint8_t ReceivedByte;
    
    uint8_t State;
    uint8_t Type;
    uint8_t DataBytesToReadCount;
    uint8_t DataBytesToSendCount;
    uint8_t DataByteIndex;
    
    union
    {
        MarkerData_t SetMarkerData;
        MarkerData_t GetMarkerData;
    };
    union
    {
        uint8_t SetTimeSpanData;
        uint8_t SetTimeProgressData;
    };
};

void ReadFrameByte(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
uint8_t ReadFrameBegin(uint8_t byte, DataFrame_t *frame);
uint8_t ReadFrameType(uint8_t byte, DataFrame_t *frame);
uint8_t ReadFrameDataSize(uint8_t byte, DataFrame_t *frame);
void ReadFrameDataByte(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
void ReadFrameDataByteForAddMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
void ReadFrameDataByteForRemMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
void ReadFrameDataByteForSetMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
void ReadFrameDataByteForGetMarker(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
// TODO - ReadFrameDataByteForGetMarkerCount
void ReadFrameDataByteForSetTimeSpan(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
// TODO - ReadFrameDataByteForGetTimeSpan
void ReadFrameDataByteForSetTimeProgress(uint8_t byte, DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
// TODO - ReadFrameDataByteForGetTimeProgress
uint8_t GetResponseFrameByte(DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
uint8_t GetResponseFrameDataByteForGetMarker(DataFrame_t *frame, ColorTimeLine_t *colorTimeLine);
void SetAsLastPoint(ColorTimePoint_t *point, ColorTimeLine_t *colorTimeLine);

#endif /* COMMUNICATION_H_ */
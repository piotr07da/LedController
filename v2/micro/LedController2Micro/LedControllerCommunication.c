/*
 * LedControllerCommunication.c
 *
 * Created: 2013-04-07 13:32:04
 *  Author: pbejger
 */ 

#include "LedControllerCommunication.h"

void AnalyzeReceivedFrame(LedControllerManager_t *lcManager, ReceiverState_t *rState, TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine)
{
    ColorTimeMarker_t marker;
    uint16_t timeSpan;
    uint16_t timeProgress;
    
    switch ((MessageType_t)rState->TypeLengthBuff[0])
    {
        case lcc_mt_ADD_MARKER:
            AddColorMarker(colorTimeLine);
            break;
        
        case lcc_mt_REM_MARKER:
            RemColorMarker(colorTimeLine);
            break;
        
        case lcc_mt_SET_MARKER:
            marker.TimePoint = rState->PayloadBuff[lcc_dbix_addMarker_TIME_POINT];
            marker.Color.R = rState->PayloadBuff[lcc_dbix_addMarker_R];
            marker.Color.G = rState->PayloadBuff[lcc_dbix_addMarker_G];
            marker.Color.B = rState->PayloadBuff[lcc_dbix_addMarker_B];
            SetColorMarker(colorTimeLine, marker, rState->PayloadBuff[lcc_dbix_addMarker_INDEX]);
            break;
        
        case lcc_mt_GET_MARKER:
            SendResponseForGetMarker(tState, colorTimeLine, rState->PayloadBuff[0]);
            break;
        
        case lcc_mt_GET_MARKER_COUNT:
            SendResponseForGetMarkerCount(tState, colorTimeLine);
            break;
        
        case lcc_mt_SET_TIME_SPAN:
            timeSpan = (uint16_t)rState->PayloadBuff[1] << 8;
            timeSpan |= (uint16_t)rState->PayloadBuff[0];
            SetTimeSpan(colorTimeLine, timeSpan);
            break;
        
        case lcc_mt_GET_TIME_SPAN:
            SendResponseForGetTimeSpan(tState, colorTimeLine);
            break;
        
        case lcc_mt_SET_TIME_PROGRESS:
            timeProgress = (uint16_t)rState->PayloadBuff[1] << 8;
            timeProgress |= (uint16_t)rState->PayloadBuff[0];
            colorTimeLine->TimeProgress = timeProgress;
            break;
        
        case lcc_mt_GET_TIME_PROGRESS:
            SendResponseForGetTimeProgress(tState, colorTimeLine);
            break;
            
		case lcc_mt_TURN_ON:
			if (lcManager->TurnOn != NULL)
				lcManager->TurnOn();
			break;
		
		case lcc_mt_TURN_OFF:
			if (lcManager->TurnOff != NULL)
				lcManager->TurnOff();
			break;
			
        case lcc_mt_PAUSE_ON:
			break;
            
        case lcc_mt_PAUSE_OFF:
            break;
            
        case lcc_mt_SOUND_ON:
            if (lcManager->SoundOn != NULL)
				lcManager->SoundOn();
			break;
            
        case lcc_mt_SOUND_OFF:
			if (lcManager->SoundOff != NULL)
				lcManager->SoundOff();
			break;
			
		case lcc_mt_GET_STATE_FLAGS:
			SendResponseForGetStateFlags(tState, lcManager);
			break;
    }
}

void SendResponseForGetMarker(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine, int markerIndex)
{
    WriteTypeLength(tState->ResponseBuffer, lcc_mt_GET_MARKER, lcc_rps_GET_MARKER);
    WriteResponsePayloadForGetMarker(&(tState->ResponseBuffer[com_FRAME_PAYLOAD_OFFSET]), colorTimeLine, markerIndex);
    WriteCRC4(tState->ResponseBuffer, lcc_rps_GET_MARKER);
    
    SendBytes(tState, lcc_rcfs_GET_MARKER);
}

void SendResponseForGetMarkerCount(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine)
{
    WriteTypeLength(tState->ResponseBuffer, lcc_mt_GET_MARKER_COUNT, lcc_rps_GET_MARKER_COUNT);
    WriteResponsePayloadForGetMarkerCount(&(tState->ResponseBuffer[com_FRAME_PAYLOAD_OFFSET]), colorTimeLine);
    WriteCRC4(tState->ResponseBuffer, lcc_rps_GET_MARKER_COUNT);
    
    SendBytes(tState, lcc_rcfs_GET_MARKER_COUNT);
}

void SendResponseForGetTimeSpan(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine)
{
    WriteTypeLength(tState->ResponseBuffer, lcc_mt_GET_TIME_SPAN, lcc_rps_GET_TIME_SPAN);
    WriteResponsePayloadForGetTimeSpan(&(tState->ResponseBuffer[com_FRAME_PAYLOAD_OFFSET]), colorTimeLine);
    WriteCRC4(tState->ResponseBuffer, lcc_rps_GET_TIME_SPAN);
    
    SendBytes(tState, lcc_rcfs_GET_TIME_SPAN);
}

void SendResponseForGetTimeProgress(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine)
{
    WriteTypeLength(tState->ResponseBuffer, lcc_mt_GET_TIME_PROGRESS, lcc_rps_GET_TIME_PROGRESS);
    WriteResponsePayloadForGetTimeProgress(&(tState->ResponseBuffer[com_FRAME_PAYLOAD_OFFSET]), colorTimeLine);
    WriteCRC4(tState->ResponseBuffer, lcc_rps_GET_TIME_PROGRESS);
    
    SendBytes(tState, lcc_rcfs_GET_TIME_PROGRESS);
    
}

void SendResponseForGetStateFlags(TransmitterState_t *tState, LedControllerManager_t *lcManager)
{
	WriteTypeLength(tState->ResponseBuffer, lcc_mt_GET_STATE_FLAGS, lcc_rps_GET_STATE_FLAGS);
	WriteResponsePayloadForGetStateFlags(&(tState->ResponseBuffer[com_FRAME_PAYLOAD_OFFSET]), lcManager);
	WriteCRC4(tState->ResponseBuffer, lcc_rps_GET_STATE_FLAGS);
	
	SendBytes(tState, lcc_rcfs_GET_STATE_FLAGS);
}

void WriteResponsePayloadForGetMarker(uint8_t *response, ColorTimeLine_t *colorTimeLine, int markerIndex)
{
    response[0] = colorTimeLine->Markers[markerIndex].TimePoint;
    for (int ccIx = 0; ccIx < 3; ++ccIx)
        response[1 + ccIx] = colorTimeLine->Markers[markerIndex].ColorBytes[ccIx];
}

void WriteResponsePayloadForGetMarkerCount(uint8_t *response, ColorTimeLine_t *colorTimeLine)
{
    response[0] = colorTimeLine->MarkerCount;
}

void WriteResponsePayloadForGetTimeSpan(uint8_t *response, ColorTimeLine_t *colorTimeLine)
{
    response[0] = (uint8_t)(colorTimeLine->TimeSpan);
    response[1] = (uint8_t)(colorTimeLine->TimeSpan >> 8);
}

void WriteResponsePayloadForGetTimeProgress(uint8_t *response, ColorTimeLine_t *colorTimeLine)
{
    response[0] = (uint8_t)(colorTimeLine->TimeProgress);
    response[1] = (uint8_t)(colorTimeLine->TimeProgress >> 8);
}

void WriteResponsePayloadForGetStateFlags(uint8_t *response, LedControllerManager_t *lcManager)
{
	// last 4 bytes are not used now. They are declared for future use.
	for (int i = 0; i < lcc_rps_GET_STATE_FLAGS; ++i)
		response[i] = 0;
		
	if (lcManager->IsSystemEnabled == TRUE)
		response[0] |= _BV(lcm_sfix_IS_SYSTEM_ENABLED);
	
	if (lcManager->IsSoundEnabled == TRUE)
		response[0] |= _BV(lcm_sfix_IS_SOUND_ENABLED);
}
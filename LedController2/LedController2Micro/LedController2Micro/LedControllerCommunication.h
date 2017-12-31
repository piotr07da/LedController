/*
 * LedControllerCommunication.h
 *
 * Created: 2013-04-06 21:39:40
 *  Author: pbejger
 */ 

#ifndef LEDCONTROLLERCOMMUNICATION_H_
#define LEDCONTROLLERCOMMUNICATION_H_


#include <avr/io.h>
#include <avr/iom32a.h>
#include <avr/sfr_defs.h>

#include "LedControllerManager.h"
#include "Communication.h"
#include "ColorTimeLine.h"
#include "BeatDetector.h"

// RESPONSE PAYLOAD SIZES
#define lcc_rps_GET_MARKER 4
#define lcc_rps_GET_MARKER_COUNT 1
#define lcc_rps_GET_TIME_SPAN 2
#define lcc_rps_GET_TIME_PROGRESS 2
#define lcc_rps_GET_STATE_FLAGS 5

// RESPONSE COMPLETE FRAME SIZES
#define lcc_rcfs_GET_MARKER com_FRAME_WITHOUT_PAYLOAD_SIZE + lcc_rps_GET_MARKER
#define lcc_rcfs_GET_MARKER_COUNT com_FRAME_WITHOUT_PAYLOAD_SIZE + lcc_rps_GET_MARKER_COUNT
#define lcc_rcfs_GET_TIME_SPAN com_FRAME_WITHOUT_PAYLOAD_SIZE + lcc_rps_GET_TIME_SPAN
#define lcc_rcfs_GET_TIME_PROGRESS com_FRAME_WITHOUT_PAYLOAD_SIZE + lcc_rps_GET_TIME_PROGRESS
#define lcc_rcfs_GET_STATE_FLAGS com_FRAME_WITHOUT_PAYLOAD_SIZE + lcc_rps_GET_STATE_FLAGS

typedef enum MessageType
{
    lcc_mt_ADD_MARKER = 1,
    lcc_mt_REM_MARKER = 2,
    lcc_mt_SET_MARKER = 3,
    lcc_mt_GET_MARKER = 4,
    lcc_mt_GET_MARKER_COUNT = 5,
    lcc_mt_SET_TIME_SPAN = 6,
    lcc_mt_GET_TIME_SPAN = 7,
    lcc_mt_SET_TIME_PROGRESS = 8,
    lcc_mt_GET_TIME_PROGRESS = 9,
    lcc_mt_PAUSE_ON = 10,
    lcc_mt_PAUSE_OFF = 11,
    lcc_mt_SOUND_ON = 12,
    lcc_mt_SOUND_OFF = 13,
	lcc_mt_TURN_ON = 14,
	lcc_mt_TURN_OFF = 15,
	lcc_mt_GET_STATE_FLAGS = 16,
} __attribute__((packed)) MessageType_t;


typedef enum DataByteIndexes
{
    lcc_dbix_addMarker_INDEX = 0,
    lcc_dbix_addMarker_TIME_POINT = 1,
    lcc_dbix_addMarker_R = 2,
    lcc_dbix_addMarker_G = 3,
    lcc_dbix_addMarker_B = 4,
} __attribute__((packed)) DataByteIndexes_t;

void AnalyzeReceivedFrame(LedControllerManager_t *lcManager, ReceiverState_t *rState, TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine);

void SendResponseForGetMarker(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine, int markerIndex);
void SendResponseForGetMarkerCount(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine);
void SendResponseForGetTimeSpan(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine);
void SendResponseForGetTimeProgress(TransmitterState_t *tState, ColorTimeLine_t *colorTimeLine);
void SendResponseForGetStateFlags(TransmitterState_t *tState, LedControllerManager_t *lcManager);

void WriteResponsePayloadForGetMarker(uint8_t *response, ColorTimeLine_t *colorTimeLine, int markerIndex);
void WriteResponsePayloadForGetMarkerCount(uint8_t *response, ColorTimeLine_t *colorTimeLine);
void WriteResponsePayloadForGetTimeSpan(uint8_t *response, ColorTimeLine_t *colorTimeLine);
void WriteResponsePayloadForGetTimeProgress(uint8_t *response, ColorTimeLine_t *colorTimeLine);
void WriteResponsePayloadForGetStateFlags(uint8_t *response, LedControllerManager_t *lcManager);

#endif /* LEDCONTROLLERCOMMUNICATION_H_ */
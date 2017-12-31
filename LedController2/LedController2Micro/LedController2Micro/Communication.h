/*
 * Communication.h
 *
 * Created: 2013-02-01 18:46:00
 *  Author: pbejger
 */ 

// GRAIN OF SAND COMMUNICATION PROTOCOL //

#ifndef COMMUNICATION_H_
#define COMMUNICATION_H_

#include "GlobalConstants.h"

#include <stdint.h>
#include <stddef.h>

#define com_FRAME_SYNCWORD_SIZE 3
#define com_FRAME_TYPE_LENGTH_SIZE 2
#define com_FRAME_PAYLOAD_MAX_SIZE 16
#define com_FRAME_CHECK_SEQUENCE_SIZE 1
#define com_FRAME_WITHOUT_PAYLOAD_SIZE com_FRAME_SYNCWORD_SIZE + com_FRAME_TYPE_LENGTH_SIZE + com_FRAME_CHECK_SEQUENCE_SIZE
#define com_FRAME_PAYLOAD_OFFSET com_FRAME_SYNCWORD_SIZE + com_FRAME_TYPE_LENGTH_SIZE

typedef enum FramePart
{
    com_fp_SYNCWORD = -1,
    com_fp_TYPE_LENGTH = 0,
    com_fp_PAYLOAD = 1,
    com_fp_FRAME_CHECK_SEQUENCE = 2,
} __attribute__((packed)) FramePart_t;

struct ReceiverState;
struct TransmitterState;

typedef struct ReceiverState ReceiverState_t;
typedef struct TransmitterState TransmitterState_t;

struct ReceiverState
{
    // Indexes of end bytes of each frame parts.
    uint8_t EndByteIndexes[3];
    
    // Index of current byte or number of readen bytes.
    uint8_t ByteIndex;
    
    // Number of bytes to read.
    uint8_t BytesToReadCount;
    
    // Type/Length information about payload.
    uint8_t TypeLengthBuff[com_FRAME_TYPE_LENGTH_SIZE];
    
    // Payload buffer.
    uint8_t PayloadBuff[com_FRAME_PAYLOAD_MAX_SIZE];
    
    // Frame check sequence buffer.
    uint8_t FrameCheckSequenceBuff[com_FRAME_CHECK_SEQUENCE_SIZE];
    
    // Indicates whether complete frame is ready.
    uint8_t FrameIsReady;
};

struct TransmitterState
{
    uint8_t ResponseBuffer[com_FRAME_WITHOUT_PAYLOAD_SIZE + com_FRAME_PAYLOAD_MAX_SIZE];
    
    void (*ByteBufferPushed)(uint8_t *bytes, uint8_t byteCount);
};

void InitReceiverState(ReceiverState_t *rState);
void ReceiveByte(ReceiverState_t *rState, uint8_t byte);
void Sync(ReceiverState_t *rState, uint8_t byte);
void FinishReceiving(ReceiverState_t *rState);
void ResetReceiverState(ReceiverState_t *rState);

void InitTransmitterState(TransmitterState_t *tState);
void SendBytes(TransmitterState_t *tState, uint8_t byteCount);

void WriteTypeLength(uint8_t *response, uint8_t type, uint8_t length);
void WriteCRC4(uint8_t *response, uint8_t payloadSize);

#endif /* COMMUNICATION_H_ */
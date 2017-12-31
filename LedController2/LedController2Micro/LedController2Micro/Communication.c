/*
 * Communication.c
 *
 * Created: 2013-02-01 19:01:26
 *  Author: pbejger
 */ 

#include "Communication.h"

void InitReceiverState(ReceiverState_t *rState)
{
    rState->EndByteIndexes[com_fp_TYPE_LENGTH] = com_FRAME_TYPE_LENGTH_SIZE - 1;
    rState->ByteIndex = 0;
    rState->BytesToReadCount = 0;
    rState->FrameIsReady = FALSE;
}

void ReceiveByte(ReceiverState_t *rState, uint8_t byte)
{
    if (rState->FrameIsReady)
        return;
    
    if (rState->BytesToReadCount == 0)
    {
        // IF NOT SYNCED
        
        Sync(rState, byte);
    }
    else
    {
        // IF SYNCED
        
        //
        // Save received byte.
        
        if (rState->ByteIndex <= rState->EndByteIndexes[com_fp_TYPE_LENGTH])
        {
            rState->TypeLengthBuff[rState->ByteIndex] = byte;
            if (rState->ByteIndex == rState->EndByteIndexes[com_fp_TYPE_LENGTH])
            {
                if (byte > com_FRAME_PAYLOAD_MAX_SIZE)
                {
                    ResetReceiverState(rState);
                    return;
                }
                
                rState->BytesToReadCount += byte;
                rState->EndByteIndexes[com_fp_PAYLOAD] = rState->EndByteIndexes[com_fp_TYPE_LENGTH] + byte;
                rState->EndByteIndexes[com_fp_FRAME_CHECK_SEQUENCE] = rState->EndByteIndexes[com_fp_PAYLOAD] + com_FRAME_CHECK_SEQUENCE_SIZE;
            }
        }
        else if (rState->ByteIndex <= rState->EndByteIndexes[com_fp_PAYLOAD])
        {
            int payloadByteIndex = rState->ByteIndex - rState->EndByteIndexes[com_fp_TYPE_LENGTH] - 1;
            rState->PayloadBuff[payloadByteIndex] = byte;
        }
        else if (rState->ByteIndex <= rState->EndByteIndexes[com_fp_FRAME_CHECK_SEQUENCE])
        {
            int fcsByteIndex = rState->ByteIndex - rState->EndByteIndexes[com_fp_PAYLOAD] - 1;
            rState->FrameCheckSequenceBuff[fcsByteIndex] = byte;
        }
        
        //
        // Check for end of frame and finish receiving if true
        // or increment byte index if false.
        
        if (rState->ByteIndex == rState->BytesToReadCount - 1)
        {
            FinishReceiving(rState);
        }
        else
        {
            ++rState->ByteIndex;
        }
    }
}

void Sync(ReceiverState_t *rState, uint8_t byte)
{
    if (rState->ByteIndex < com_FRAME_SYNCWORD_SIZE - 1)
    {
        if (byte == 0xAA)
            ++rState->ByteIndex;
        else
            rState->ByteIndex = 0;
    }            
    else
    {
        if (byte == 0xAB)
        {
            // If received byte is last byte of syncword, then:
            
            // Set byte index to 0.
            rState->ByteIndex = 0;
            // Set number of bytes to read to total frame size excluding payload size which is readen later.
            rState->BytesToReadCount = com_FRAME_TYPE_LENGTH_SIZE + com_FRAME_CHECK_SEQUENCE_SIZE;
        }
        else if (byte == 0xAA)
        {
            rState->ByteIndex = com_FRAME_SYNCWORD_SIZE - 1;
        }
        else
        {
            rState->ByteIndex = 0;
        }
    }
}

void FinishReceiving(ReceiverState_t *rState)
{
    rState->FrameIsReady = TRUE;
}

void ResetReceiverState(ReceiverState_t *rState)
{
    rState->ByteIndex = 0;
    rState->BytesToReadCount = 0;
    rState->FrameIsReady = FALSE;
}


void InitTransmitterState(TransmitterState_t *tState)
{
    tState->ResponseBuffer[0] = 0xAA;
    tState->ResponseBuffer[1] = 0xAA;
    tState->ResponseBuffer[2] = 0xAB;
}    

void SendBytes(TransmitterState_t *tState, uint8_t byteCount)
{
    if (tState->ByteBufferPushed != NULL)
        tState->ByteBufferPushed(tState->ResponseBuffer, byteCount);
}

void WriteTypeLength(uint8_t *response, uint8_t type, uint8_t length)
{
    response[com_FRAME_SYNCWORD_SIZE] = type;
    response[com_FRAME_SYNCWORD_SIZE + 1] = length;
}

void WriteCRC4(uint8_t *response, uint8_t payloadSize)
{
    response[com_FRAME_PAYLOAD_OFFSET + payloadSize] = 0;
}
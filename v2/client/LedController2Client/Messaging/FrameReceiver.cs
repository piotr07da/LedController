using System;

namespace LedController2Client
{
    public class FrameReceiverState
    {
        public event Action<FrameReceiverState> FrameReceived;
        public event Action<FrameReceiverState> FrameError;

        private byte[] _endByteIndexes;
        private byte _byteIndex;
        private byte _bytesToReadCount;

        public byte[] TypeLengthBuff;
        public byte[] PayloadBuff;
        public byte[] FrameCheckSequenceBuff;

        public void Init()
        {
            _endByteIndexes = new byte[3];
            _endByteIndexes[(byte)FramePart.com_fp_TYPE_LENGTH] = FrameConsts.com_FRAME_TYPE_LENGTH_SIZE - 1;
            _byteIndex = 0;
            _bytesToReadCount = 0;

            TypeLengthBuff = new byte[FrameConsts.com_FRAME_TYPE_LENGTH_SIZE];
            PayloadBuff = new byte[FrameConsts.com_FRAME_PAYLOAD_MAX_SIZE];
            FrameCheckSequenceBuff = new byte[FrameConsts.com_FRAME_CHECK_SEQUENCE_SIZE];
        }

        public void ReceiveByte(byte @byte)
        {
            if (_bytesToReadCount == 0)
            {
                // IF NOT SYNCED

                Sync(@byte);
            }
            else
            {
                // IF SYNCED

                //
                // Save received byte.

                if (_byteIndex <= _endByteIndexes[(byte)FramePart.com_fp_TYPE_LENGTH])
                {
                    TypeLengthBuff[_byteIndex] = @byte;
                    if (_byteIndex == _endByteIndexes[(byte)FramePart.com_fp_TYPE_LENGTH])
                    {
                        _bytesToReadCount += @byte;

                        if (@byte > FrameConsts.com_FRAME_PAYLOAD_MAX_SIZE)
                        {
                            BreakReceiveing();
                            return;
                        }

                        _endByteIndexes[(byte)FramePart.com_fp_PAYLOAD] = (byte)(_endByteIndexes[(byte)FramePart.com_fp_TYPE_LENGTH] + @byte);
                        _endByteIndexes[(byte)FramePart.com_fp_FRAME_CHECK_SEQUENCE] = (byte)(_endByteIndexes[(byte)FramePart.com_fp_PAYLOAD] + FrameConsts.com_FRAME_CHECK_SEQUENCE_SIZE);
                    }
                }
                else if (_byteIndex <= _endByteIndexes[(byte)FramePart.com_fp_PAYLOAD])
                {
                    int payloadByteIndex = _byteIndex - _endByteIndexes[(byte)FramePart.com_fp_TYPE_LENGTH] - 1;
                    PayloadBuff[payloadByteIndex] = @byte;
                }
                else if (_byteIndex <= _endByteIndexes[(byte)FramePart.com_fp_FRAME_CHECK_SEQUENCE])
                {
                    int fcsByteIndex = _byteIndex - _endByteIndexes[(byte)FramePart.com_fp_PAYLOAD] - 1;
                    FrameCheckSequenceBuff[fcsByteIndex] = @byte;
                }

                if (_byteIndex == _bytesToReadCount - 1)
                {
                    FinishReceiving();
                }
                else
                {
                    ++_byteIndex;
                }
            }
        }


        private void Sync(byte @byte)
        {
            if (_byteIndex < FrameConsts.com_FRAME_SYNCWORD_SIZE - 1)
            {
                if (@byte == 0xAA)
                    ++_byteIndex;
                else
                    _byteIndex = 0;
            }
            else
            {
                if (@byte == 0xAB)
                {
                    // If received byte is last byte of syncword, then:

                    // Set byte index to 0.
                    _byteIndex = 0;
                    // Set number of bytes to read to total frame size excluding payload size which is readen later.
                    _bytesToReadCount = FrameConsts.com_FRAME_TYPE_LENGTH_SIZE + FrameConsts.com_FRAME_CHECK_SEQUENCE_SIZE;
                }
                else if (@byte == 0xAA)
                {
                    _byteIndex = FrameConsts.com_FRAME_SYNCWORD_SIZE - 1;
                }
                else
                {
                    _byteIndex = 0;
                }
            }
        }

        private void BreakReceiveing()
        {
            FinishReceiving(false);
        }

        private void FinishReceiving(bool withSuccess = true)
        {
            if (withSuccess)
            {
                if (FrameReceived != null)
                    FrameReceived(this);
            }
            else
            {
                if (FrameError != null)
                    FrameError(this);
            }

            //

            _byteIndex = 0;
            _bytesToReadCount = 0;
        }
    }
}

using System;

namespace LedController2Client
{
    public class AudioDataReceiver
    {
        private const int __HEADER_SEQUENCE_LENGTH = 5;

        public AudioDataReceiver(int reservedSize, int sampleSize, int spectrumSize, int octavesSize)
        {
            _reservedSize = reservedSize;
            _samplesSize = sampleSize;
            _spectrumSize = spectrumSize;
            _octavesSize = octavesSize;
            Init();
        }

        private int _reservedSize;
        private int _samplesSize;
        private int _spectrumSize;
        private int _octavesSize;
        private State _state;
        private byte[] _reserved;
        private byte[] _samples;
        private byte[] _spectrum;
        private byte[] _octaves;
        private int _byteIndex;

        public event Action<byte[], byte[], byte[]> DataReceived;

        public void ReadByte(byte @byte)
        {
            if (_state == State.ReadHeader)
            {
                if (@byte == (1 - (_byteIndex % 2)) * 255)
                    ++_byteIndex;
                else
                {
                    if (@byte == 255)
                        _byteIndex = 1;
                    else
                        _byteIndex = 0;
                }

                if (_byteIndex == __HEADER_SEQUENCE_LENGTH)
                {
                    InitReadData(State.ReadReserved);
                }
            }
            else if (_state == State.ReadReserved)
            {
                _reserved[_byteIndex++] = @byte;

                if (_byteIndex == _reservedSize)
                    InitReadData(State.ReadSamples);
            }
            else if (_state == State.ReadSamples)
            {
                _samples[_byteIndex++] = @byte;

                if (_byteIndex == _samplesSize)
                    InitReadData(State.ReadSpectrum);
            }
            else if (_state == State.ReadSpectrum)
            {
                _spectrum[_byteIndex++] = @byte;

                if (_byteIndex == _spectrumSize)
                    InitReadData(State.ReadOctaves);
            }
            else if (_state == State.ReadOctaves)
            {
                _octaves[_byteIndex++] = @byte;

                if (_byteIndex == _octavesSize)
                {
                    if (DataReceived != null)
                        DataReceived(_samples, _spectrum, _octaves);

                    InitReadHeader();
                }
            }
        }

        public virtual void Init()
        {
            InitReadHeader();
            _reserved = new byte[_reservedSize];
            _samples = new byte[_samplesSize];
            _spectrum = new byte[_spectrumSize];
            _octaves = new byte[_octavesSize];
        }

        private void InitReadHeader()
        {
            _state = State.ReadHeader;
            _byteIndex = 0;
        }

        private void InitReadData(State state)
        {
            _state = state;
            _byteIndex = 0;
        }

        private enum State
        {
            ReadHeader,
            ReadReserved,
            ReadSamples,
            ReadSpectrum,
            ReadOctaves,
        }
    }
}

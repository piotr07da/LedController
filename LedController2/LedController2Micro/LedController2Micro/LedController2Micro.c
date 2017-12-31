/*
 * LedController2Micro.c
 *
 * Created: 2013-02-12 08:10:54
 *  Author: pbejger
 */ 

#include "LedController2Micro.h"

volatile uint8_t _sreg;

volatile uint8_t _byte;
volatile uint8_t _byteReady;

volatile LedControllerManager_t _lcManager;
volatile ColorTimeLine_t _timeLine;
volatile ReceiverState_t _receiverState;
volatile TransmitterState_t _transmitterState;
volatile BeatDetector_t _beatDetector;
volatile BeatDetectorConfiguration_t _beatDetectorConfig;
BeatDetectorState_t _fuFreqBeatDetectorState;
BeatDetectorState_t _loFreqBeatDetectorState;
BeatDetectorState_t _hiFreqBeatDetectorState;

uint8_t _clockCycleIndex;
uint16_t _indexOfClockResolutionSpanInSecond;
uint16_t _indexOfClockResolutionSpanInColorUpdate;

uint16_t _timeProgressDelta;
Color_t _currentColor;


//
uint8_t _color;
uint8_t _allColorsEq0;
//
//
uint8_t _stopColor;

uint8_t _soundOn;

volatile uint8_t _tmpCounter;
uint8_t _tmpMCUCSR;
int main(void)
{
    _tmpMCUCSR = MCUCSR;
 
 
 
    
 
    
    
    _stopColor = 0;
	SoundOff();
    
    
    _byte = 0;
    _byteReady = FALSE;
    
    
    
    // -----------
    
    DisableJTAG();
    InitUSART(__lc_UBRR);
    InitTimer();
    InitADC();
    InitPWM();
	InitLedControllerManager(&_lcManager);
	_lcManager.TurnOn = TurnOn;
	_lcManager.TurnOff = TurnOff;
	_lcManager.SoundOn = SoundOn;
	_lcManager.SoundOff = SoundOff;
    InitReceiverState(&_receiverState);
    InitTransmitterState(&_transmitterState);
    InitBeatDetectorConfiguration(&_beatDetectorConfig);
    InitBeatDetector(&_beatDetector, &_beatDetectorConfig);
    InitTimeLine(&_timeLine, &TimeSpanUpdated);
    
    _transmitterState.ByteBufferPushed = &SendBytesUSART;
    
    // -------------------------------------------------------------------------------
    
    
        //for (uint8_t sampleIndex = 0; sampleIndex < bd_DEF_NUMBER_OF_SAMPLES_IN_ENERGY_SET; ++sampleIndex)
            //ProcessSample(&_beatDetector, 0.0000001);
        //
        //BeatDetectorState_t beatDetectorState1;
        //
        //GetBeatDetectorState(&_beatDetector, &_beatDetector.Configuration->FuFreqBandConfig, &_beatDetector.FuFreqSoundEnergyData, &beatDetectorState1);
    
    
    
    DDRC |= 255;
    PORTC |= (_BV(PC0) | _BV(PC1) | _BV(PC2) | _BV(PC3) | _BV(PC4)) & _tmpMCUCSR;
    
    PORTC &= ~(31);
    
    //PORTC |= _BV(PC1);
    
    sei();
    
    //_color = 255;
    //_currentColor.R = 0;
    //_currentColor.G = 0;
    //_currentColor.B = 255;
    //
    int seqState = 0;
    int seqMaxE = 10;
    int seqMaxD = 280;
    int seqIx = 0;
    
    _byteReady = FALSE;
    uint8_t tmpByteReady;// = FALSE;
    
    while (1)
    {
        CalcCurrentColor(&_timeLine, &_currentColor);
        
        double rf = (double)_currentColor.R / 255.0;
        double gf = (double)_currentColor.G / 255.0;
        double bf = (double)_currentColor.B / 255.0;
        
        //rf = 0;
        
        //_color = 16;
		if (_soundOn == FALSE)
			_color = 255;
        if (_stopColor)
            _color = 0;
		
		if (_lcManager.IsSystemEnabled)
		{
			uint8_t pwmR = _color * rf;
			uint8_t pwmG = _color * gf;
			uint8_t pwmB = _color * bf;
			
			if (pwmR == 0 && pwmG == 0 && pwmB == 0)
			{
				// OC1A, OC1B, OC2 (PWM outputs)
				DDRD &= ~(_BV(DDRD4) | _BV(DDRD5) | _BV(DDRD7));
				_allColorsEq0 = TRUE;
			}
			else
			{
				if (_allColorsEq0 == TRUE)
				{
					// OC1A, OC1B, OC2 (PWM outputs)
					DDRD |= (_BV(DDRD4) | _BV(DDRD5) | _BV(DDRD7));
					
					_allColorsEq0 = FALSE;
				}
				
				OCR2 = pwmR;
				OCR1A = pwmG;
				OCR1B = pwmB;
			}
		}
        
		//if (_currentColor.B == 0) DDRD &= ~_BV(DDRD4);
		//else DDRD |= _BV(DDRD4);
		//
		//if (_currentColor.G == 0) DDRD &= ~_BV(DDRD5);
		//else DDRD |= _BV(DDRD5);
		//
		//if (_currentColor.R == 0) DDRD &= ~_BV(DDRD7);
		//else DDRD |= _BV(DDRD7);
		
        //if (seqState == 0 && seqIx++ == seqMaxD)
        //{
            //OCR1A = 255;
            //OCR1B = 255;
            //OCR2 = 255;
            //seqIx = 0;
            //seqState = 1;
        //}
        //else if (seqState == 1 && seqIx++ == seqMaxE)
        //{
            //OCR1A = 0;
            //OCR1B = 0;
            //OCR2 = 0;
            //seqIx = 0;
            //seqState = 0;
        //}
        
        
        // -----
        
        GetBeatDetectorState(&_beatDetector, &_beatDetector.Configuration->FuFreqBandConfig, &_beatDetector.FuFreqSoundEnergyData, &_fuFreqBeatDetectorState);
        GetBeatDetectorState(&_beatDetector, &_beatDetector.Configuration->LoFreqBandConfig, &_beatDetector.LoFreqSoundEnergyData, &_loFreqBeatDetectorState);
        GetBeatDetectorState(&_beatDetector, &_beatDetector.Configuration->HiFreqBandConfig, &_beatDetector.HiFreqSoundEnergyData, &_hiFreqBeatDetectorState);
        
        if(_fuFreqBeatDetectorState.IsReady)
        {
            if(_fuFreqBeatDetectorState.IsDetected)
            {
                _color = 255;
            }
            else
            {
                _color = 0;
            }
        }
        
        //if (TRUE)
        //{
            //if (_receiverState.FrameIsReady)
            //{
                //AnalyzeReceivedFrame(&_receiverState, &_transmitterState, &_timeLine);
                //ResetReceiverState(&_receiverState);
                ////PORTC ^= _BV(PC0);
            //}
            //else
            //{
                //tmpByteReady = _byteReady;
                //if (tmpByteReady == TRUE)
                //{
                    //ReceiveByte(&_receiverState, _byte);
                    //SendByteUSART(_byte);
                    //tmpByteReady = FALSE;
                    //
                    ////PORTC ^= _BV(PC1);
                    //
                    //if (_tmpCounter < 31) ++_tmpCounter;
                    //else _tmpCounter = 0;
                    //
                    //PORTC &= ~(31);
                    //PORTC |= _tmpCounter;
                    //
                //}
                //_byteReady = tmpByteReady;
            //}
        //}
        
        
        
    }
}

ISR(USART_RXC_vect)
{
    uint8_t udr = UDR;
    
    if (_byteReady == FALSE)
    {
        _byte = udr;
        _byteReady = TRUE;
        
        
    }
    
    ReceiveByte(&_receiverState, _byte);
    SendByteUSART(_byte);
    
    if (_receiverState.FrameIsReady)
    {
        AnalyzeReceivedFrame(&_lcManager, &_receiverState, &_transmitterState, &_timeLine);
        ResetReceiverState(&_receiverState);
    }
    
    if (_tmpCounter < 31) ++_tmpCounter;
    else _tmpCounter = 0;
            
    PORTC &= ~(31);
    PORTC |= _tmpCounter;
    
    _byteReady = FALSE;
}


ISR(TIMER0_OVF_vect)
{
    ++_clockCycleIndex;
    if (_clockCycleIndex == __lc_TC0_CLOCK_CYCLE_COUNT)
    {
        _clockCycleIndex = 0;
        
        // ----------------- seconds
        ++_indexOfClockResolutionSpanInSecond;
        if (_indexOfClockResolutionSpanInSecond == __lc_NUMBER_OF_RESOLUTION_SPAN_PER_SECOND)
        {
            _indexOfClockResolutionSpanInSecond = 0;
            //
            PORTC ^= _BV(PC7);
        }
        
        // ----------------- color update
        ++_indexOfClockResolutionSpanInColorUpdate;
        if (_indexOfClockResolutionSpanInColorUpdate == __lc_NUMBER_OF_RESOLUTION_SPAN_PER_COLOR_UPDATE)
        {
            _indexOfClockResolutionSpanInColorUpdate = 0;
            //
            _timeLine.TimeProgress += _timeProgressDelta;
        }
    }
}

ISR(ADC_vect)
{
    uint8_t adcl = ADCL;
    uint8_t adch = ADCH;
    uint16_t sampleValue = ((uint16_t)adch << 8) | (uint16_t)adcl;
    
    // ----------------------------------------------------------------
    
    double normalizedSampleValue = 2.0 * sampleValue / __sa_MAX_SIGNAL_VALUE - 1.0;
    ProcessSample(&_beatDetector, normalizedSampleValue);
    
    // ----------------------------------------------------------------
}

void DisableJTAG()
{
    // To disable JTAG set JTD bit twice.
    MCUCSR |= _BV(JTD);
    MCUCSR |= _BV(JTD);
}

void InitUSART(uint8_t ubrr)
{
    // Set the baud rate.
    UBRRH = (unsigned char)(ubrr >> 8);
    UBRRL = (unsigned char)(ubrr);
    
    // Enable UART receiver and transmitter and receiver interrupt handlings
    UCSRB = _BV(RXEN) | _BV(TXEN) | _BV(RXCIE);
    
    // Set to 8 data bits, 1 stop bit, async
    UCSRC = _BV(URSEL) | _BV(UCSZ1) | _BV(UCSZ0);
}

void InitTimer()
{
    _clockCycleIndex = __lc_TC0_CLOCK_CYCLE_COUNT - 1;
    
    _indexOfClockResolutionSpanInSecond = 0;
    _indexOfClockResolutionSpanInColorUpdate = 0;
    
    // --- TIMER/COUNTER 0 SETTINGS
    
    // With prescaling set to __lc_TC0_PRESCALER
    TCCR0 |= __lc_TC0_TCCR0_CLOCK_SELECT;
    
    // Enable interrupt
    TIMSK |= _BV(TOIE0);
}

void InitADC()
{
    // Set AVCC as AREF, enable ADC0 chanel
    ADMUX = _BV(REFS0) | (0b0000 << MUX0);
    
    // Enable free running mode (with connection with ADTS[2:0]=0 in SFIOR register), enable ADC interrupt.
    ADCSRA = _BV(ADATE) | _BV(ADIE);
    
    // Set prescaler to 128, which gives 115.2kHz ADC clock frequency and approx 8.862kSps (at 14.7456MHz uC clock frequency)
    ADCSRA |= _BV(ADPS1) | _BV(ADPS2) | _BV(ADPS0);
    
    // Enable ADC, start first conversion.
    ADCSRA |= _BV(ADEN) | _BV(ADSC);
}

void InitPWM()
{
    // --- OUTPUT SETTINGS
    
    // OC1A, OC1B, OC2 (PWM outputs)
    DDRD |= _BV(DDRD4) | _BV(DDRD5) | _BV(DDRD7);
	_lcManager.IsSystemEnabled = TRUE;
    
    
    // --- TIMER/COUNTER 1 SETTINGS
    
    // Clear OC1A and OC1B on compare match
    TCCR1A |= _BV(COM1A1) | _BV(COM1B1);
    // Fast PWM, 8-bit, top = 0x00FF
    TCCR1A |= _BV(WGM10);
    TCCR1B |= _BV(WGM12);
    // With prescaling set to __lc_TC1_PRESCALER
    TCCR1B |= __lc_TC1_TCCR1B_CLOCK_SELECT;
    
    
    // --- TIMER/COUNTER 2 SETTINGS
    
    // Clear OC2 on compare match
    TCCR2 |= _BV(COM21);
    // Fast PWM, 8-bit, top = 0xFF
    TCCR2 |= _BV(WGM20) | _BV(WGM21);
    // With prescaling set to __lc_TC2_PRESCALER
    TCCR2 |= __lc_TC2_TCCR2_CLOCK_SELECT;
}

void TimeSpanUpdated(ColorTimeLine_t *colorTimeLine, uint16_t newTimeSpanValue)
{
    _timeProgressDelta = (UINT16_MAX * __lc_COLOR_UPDATE_TIME_DELTA * ctl_TIME_SPAN_SCALE) / (_timeLine.TimeSpan * 1000UL);
}

void SendByteUSART(uint8_t byte)
{
    while (!(UCSRA & _BV(UDRE))) ;
    UDR = byte;
}

void SendBytesUSART(uint8_t *bytes, uint8_t byteCount)
{
    for (int bIx = 0; bIx < byteCount; ++bIx)
        SendByteUSART(bytes[bIx]);
}

void TurnOn()
{
	DDRD |= _BV(DDRD4) | _BV(DDRD5) | _BV(DDRD7);
	_lcManager.IsSystemEnabled = TRUE;
}

void TurnOff()
{
	DDRD &= ~(_BV(DDRD4) | _BV(DDRD5) | _BV(DDRD7));
	_lcManager.IsSystemEnabled = FALSE;
}

void SoundOn()
{
	_soundOn = TRUE;
	_lcManager.IsSoundEnabled = TRUE;
}

void SoundOff()
{
	_soundOn = FALSE;
	_lcManager.IsSoundEnabled = FALSE;
}

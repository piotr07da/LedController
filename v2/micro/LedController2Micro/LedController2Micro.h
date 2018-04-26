/*
 * LedController2Micro.h
 *
 * Created: 2013-03-03 11:10:12
 *  Author: pbejger
 */ 


#ifndef LEDCONTROLLER2MICRO_H_
#define LEDCONTROLLER2MICRO_H_


#include "GlobalConstants.h"

#include <avr/io.h>
#include <avr/iom32a.h>
#include <avr/sfr_defs.h>
#include <avr/interrupt.h>
#include <avr/cpufunc.h>
#include <util/delay.h>
#include <math.h>

#include "LedControllerManager.h"
#include "ColorTimeLine.h"
#include "Communication.h"
#include "LedControllerCommunication.h"
#include "SpectralAnalysis.h"
#include "BeatDetector.h"

//#define __lc_BR 4800UL // bit rate
#define __lc_BR 230400UL // bit rate

#define __lc_UBRR (F_CPU / (16UL * __lc_BR)) - 1 // usart baud rate register

#define __lc_TC0_PRESCALER 8UL // prescaler for 8-bit timer/counter0, used to measure time
#define __lc_TC0_TCCR0_CLOCK_SELECT _BV(CS01)
#define __lc_TC1_PRESCALER 8UL // prescaler for 16-bit timer/counter1, used to generate PWM signal
#define __lc_TC1_TCCR1B_CLOCK_SELECT _BV(CS10)
#define __lc_TC2_PRESCALER 8UL // prescaler for 08-bit timer/counter2, used to generate PWM signal
#define __lc_TC2_TCCR2_CLOCK_SELECT _BV(CS20)

// Number of timer pulses is not useful because Timer/Counter0 does not support compare match. Number of full timer cycles is calculated instead.
// Number of full time cycles is pre-calculated from following equation:
// cycles = ((fcpu / (prescaler * 256)) * resolution) / 1000000  :: resolution measured in microseconds
// Examples (for fcpu = 14745600) of correct options (starting from minimal possible resolution for Timer/Counter0):
// resolution (uSec) :: prescaler :: clock cycles
// 00625 :: 0001 :: 36
// 01250 :: 0001 :: 72
//       :: 0001 :: 9
// 02500 :: 0001 :: 144
//       :: 0008 :: 18
// 05000 :: 0001 :: 288
//       :: 0008 :: 36
// 10000 :: 0001 :: 576
//       :: 0008 :: 72
//       :: 0064 :: 9

#define __lc_TC0_RESOLUTION_TIME 5000UL // microseconds of timer resolution
#define __lc_TC0_CLOCK_CYCLE_COUNT 36UL // number of clock cycles needed for resolution time

#define __lc_COLOR_UPDATE_TIME_DELTA 35UL
#define __lc_NUMBER_OF_RESOLUTION_SPAN_PER_SECOND 1000000UL / __lc_TC0_RESOLUTION_TIME
#define __lc_NUMBER_OF_RESOLUTION_SPAN_PER_COLOR_UPDATE __lc_COLOR_UPDATE_TIME_DELTA * 1000 / __lc_TC0_RESOLUTION_TIME





// FUNCTION DECLARATIONS

void DisableJTAG();
void InitUSART(uint8_t ubrr);
void InitTimer();
void InitADC();
void InitPWM();

void TimeSpanUpdated(ColorTimeLine_t *colorTimeLine, uint16_t newTimeSpanValue);

void SendByteUSART(uint8_t byte);
void SendBytesUSART(uint8_t *bytes, uint8_t byteCount);

void TurnOn();
void TurnOff();
void SoundOn();
void SoundOff();

#endif /* LEDCONTROLLER2MICRO_H_ */
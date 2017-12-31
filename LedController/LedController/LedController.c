/*
 * LedController.c
 *
 * Created: 2013-01-15 13:37:37
 *  Author: pbejger
 */ 

#include "GlobalConstants.h"

#include <avr/io.h>
#include <avr/sfr_defs.h>
#include <avr/interrupt.h>
#include <avr/cpufunc.h>
#include <avr/iotn2313.h>
#include <util/delay.h>
#include <math.h>

#include "ColorTimeLine.h"
#include "Communication.h"

#define __lc_BR 4800UL // bit rate
#define __lc_UBRR (F_CPU / (16UL * __lc_BR)) - 1 // usart baud rate register

#define __lc_TIME_DELTA 32UL

// FUNCTION PROTOTYPES
void InitUSART(unsigned int baudrate);
void InitPWM();

ColorTimeLine_t _timeLine;
DataFrame_t _frame;

uint16_t _timeProgressDelta;
Color_t _currentColor;

//uint8_t data[] =
//{
    //60,
    //1,
    //1,
    //0
//};
//uint8_t i;

int main(void)
{
    InitUSART(__lc_UBRR);
    InitPWM();
    
    ResetTimeLine(&_timeLine);
    
    //for (i = 0; i < 4; ++i)
        //ReadFrameByte(data[i], &_frame, &_timeLine);
    
    while (1)
    {
        cli();
        
        //
        
        _timeProgressDelta = (UINT16_MAX * __lc_TIME_DELTA * ctl_TIME_SPAN_SCALE) / (_timeLine.TimeSpan * 1000UL);
        _timeLine.TimeProgress += _timeProgressDelta;
        
        CalcCurrentColor(&_timeLine, &_currentColor);
        OCR0A = 120 * _currentColor.R / 255;
        OCR0B = 120 * _currentColor.G / 255;
        OCR1A = 120 * _currentColor.B / 255;
        
        _delay_ms(__lc_TIME_DELTA);
        
        //
        
        sei();
    }
}

ISR(USART_RX_vect)
{
    uint8_t byte = UDR;
    
    ReadFrameByte(byte, &_frame, &_timeLine);
    if (_frame.State >= com_fs_err_UNKOWN_ERROR)
    {
        // In case of error, response byte is incremented, otherwise response byte remains unchanged.
        
        if (byte < 255)
            byte += 1;
        else
            byte = 0;
    }    
    
    UDR = byte;
}

void InitUSART(unsigned int ubrr)
{
    // Set the baud rate.
    UBRRH = (unsigned char)(ubrr >> 8);
    UBRRL = (unsigned char)(ubrr);
    
    // Enable UART receiver and transmitter and receiver interrupt handlings
    UCSRB |= _BV(RXEN) | _BV(TXEN) | _BV(RXCIE);
    
    // Set to 8 data bits, 1 stop bit
    UCSRC |= _BV(UCSZ1) | _BV(UCSZ0);
}

void InitPWM()
{
    // Output settings
    
    DDRB |= _BV(PB2) | _BV(PB3); // OCR0A, OCR1A
    DDRD |= _BV(PD5); // OCR0B
    
    // Timer/Counter 0 settings
    
    TCCR0A |= _BV(COM0A1) | _BV(COM0B1); // Clear OC0A and OC0B on compare match
    TCCR0A |= _BV(WGM00) | _BV(WGM01); // Fast PWM mode
    TCCR0B |= _BV(CS00); // No prescaling
    
    // Timer/Counter 1 settings
    
    TCCR1A |= _BV(COM1A1); // Clear OC1A on compare match
    TCCR1A |= _BV(WGM10); // Fast PWM (together with following line)
    TCCR1B |= _BV(WGM12); // Fast PWM
    TCCR1B |= _BV(CS10); // No prescaling
}











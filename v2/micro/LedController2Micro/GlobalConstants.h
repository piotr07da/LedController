/*
 * GlobalConstants.h
 *
 * Created: 2013-01-16 20:57:54
 *  Author: pbejger
 */ 


#ifndef GLOBALCONSTANTS_H_
#define GLOBALCONSTANTS_H_

              
#define F_CPU 14745600UL

#define ADC_PRESCALER 128UL
#define ADC_CLOCK_FREQ (uint64_t)(F_CPU / ADC_PRESCALER)
#define ADC_SAMPLING_FREQUENCY (double)((double)ADC_CLOCK_FREQ / (double)13.0)
#define ADC_DT (double)((double)1.0 / (double)ADC_SAMPLING_FREQUENCY)

#define TRUE 1
#define FALSE 0


#endif /* GLOBALCONSTANTS_H_ */
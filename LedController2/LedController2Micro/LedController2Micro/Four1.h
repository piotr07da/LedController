/*
 * Four1.h
 *
 * Created: 2013-02-14 22:59:06
 *  Author: pbejger
 */ 


#ifndef FOUR1_H_
#define FOUR1_H_

#include <stdint.h>
#include <math.h>

#define SWAP(a,b) tempr=(a);(a)=(b);(b)=tempr
#define __four1_PI2 6.28318530717959

void Four1(double *data, uint16_t nn, uint8_t isign);

#endif /* FOUR1_H_ */
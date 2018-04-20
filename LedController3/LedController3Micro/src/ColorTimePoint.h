#ifndef __COLOR_TIME_POINT__
#define __COLOR_TIME_POINT__

#include "Particle.h"
#include "Color.h"

typedef struct ColorTimePoint ColorTimePoint_t;

struct ColorTimePoint
{
  union Color
  {
      Color_t Color;
      uint8_t ColorBytes[3];
  };
  uint8_t TimePoint;
};

#endif

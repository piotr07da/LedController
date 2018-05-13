#ifndef __COLOR__
#define __COLOR__

#include "Particle.h"

struct Color
{
  uint8_t R;
  uint8_t G;
  uint8_t B;

  Color()
  {
    R = 0;
    G = 0;
    B = 0;
  }

  Color(uint8_t r, uint8_t g, uint8_t b)
  {
    R = r;
    G = g;
    B = b;
  }

  uint8_t MaxComp()
  {
    uint8_t max = R;
    if (G > max)
      max = G;
    if (B > max)
      max = B;
    return max;
  }
};

typedef Color Color_t;

#endif

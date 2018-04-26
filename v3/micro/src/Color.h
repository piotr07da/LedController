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
};

typedef Color Color_t;

#endif

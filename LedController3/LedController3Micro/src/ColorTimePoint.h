#ifndef __COLOR_TIME_POINT__
#define __COLOR_TIME_POINT__

#include "Particle.h"
#include "Color.h"

class ColorTimePoint
{
private:
  uint8_t _id;
  Color_t _color;
  uint8_t _time;

public:
  ColorTimePoint();
  ColorTimePoint(uint8_t id, Color_t color, uint8_t time);
  uint8_t Id();
  Color_t Color();
  uint8_t Time();
};

#endif

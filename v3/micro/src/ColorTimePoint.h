#ifndef __COLOR_TIME_POINT__
#define __COLOR_TIME_POINT__

#include "Particle.h"
#include "Color.h"

class ColorTimePoint
{
private:
  uint8_t _id;
  Color_t _color;
  float _time;

public:
  ColorTimePoint();
  ColorTimePoint(uint8_t id, Color_t color, float time);
  uint8_t GetId();
  Color_t GetColor();
  void SetColor(Color_t color);
  float GetTime();
  void SetTime(float time);
};

#endif

#include "ColorTimePoint.h"

ColorTimePoint::ColorTimePoint()
{
  _id = 0;
  _color = Color_t();
  _time = 0;
}

ColorTimePoint::ColorTimePoint(uint8_t id, Color_t color, uint8_t time)
{
  _id = id;
  _color = color;
  _time = time;
}

uint8_t ColorTimePoint::Id()
{
  return _id;
}

Color_t ColorTimePoint::Color()
{
  return _color;
}

uint8_t ColorTimePoint::Time()
{
  return _time;
}

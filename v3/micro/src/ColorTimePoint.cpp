#include "ColorTimePoint.h"

ColorTimePoint::ColorTimePoint()
{
  _id = 0;
  _color = Color_t();
  _time = 0;
}

ColorTimePoint::ColorTimePoint(uint8_t id, Color_t color, float time)
{
  _id = id;
  _color = color;
  _time = time;
}

uint8_t ColorTimePoint::GetId()
{
  return _id;
}

Color_t ColorTimePoint::GetColor()
{
  return _color;
}

void ColorTimePoint::SetColor(Color_t color)
{
  _color = color;
}

float ColorTimePoint::GetTime()
{
  return _time;
}

void ColorTimePoint::SetTime(float time)
{
  _time = time;
}

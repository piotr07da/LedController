#include "ColorTimeLine.h"
#include "Particle.h"

void ColorTimeLine::Setup()
{
  Particle.variable("pointCount", _DGB_pointCount);
  _DBG_published = false;

  Color_t c0(255, 0, 0);
  Color_t c1(0, 255, 0);
  Color_t c2(0, 0, 255);
  uint8_t t0 = 85;
  uint8_t t1 = 170;
  uint8_t t2 = 0;

  _timeSpan = ColorTimeLine_DefaultTimeSpan;
  _currentColor = c0;
  _currentTime = 0;

  AddPoint(c0, t0);
  AddPoint(c1, t1);
  AddPoint(c2, t2);
}

void ColorTimeLine::IncreaseCurrentTime(int32_t timeStep)
{
  _currentTime += timeStep;

  int32_t diff = _currentTime - _timeSpan;
  if (diff >= 0)
  {
    _currentTime = diff;
  }

  _normalizedCurrentTime = 255 * _currentTime / _timeSpan;

  RefreshCurrentColor();
}

uint32_t ColorTimeLine::GetCurrentTime()
{
  return _currentTime;
}

void ColorTimeLine::SetCurrentTime(uint32_t currentTime)
{

}
uint32_t ColorTimeLine::GetTimeSpan()
{
  return _timeSpan;
}
void ColorTimeLine::SetTimeSpan(uint32_t timeSpan)
{

}
Color_t ColorTimeLine::GetCurrentColor()
{
  return _currentColor;
}

uint8_t ColorTimeLine::GetPointCount()
{
  return _points.Count();
}

ColorTimePoint* ColorTimeLine::GetPoints()
{
  return _points.GetAll();
}

uint8_t ColorTimeLine::AddPoint(Color_t color, uint8_t time)
{
  uint8_t newId = _points.NextFreeId();
  ColorTimePoint ctp(newId, color, time);
  _points.TryAdd(ctp);
}

ColorTimePoint ColorTimeLine::GetPoint(uint8_t index)
{
  ColorTimePoint ctp;
  _points.TryGetAtIndex(index, ctp);
  return ctp;
}
void ColorTimeLine::SetPointColor(uint8_t index, Color_t color)
{

}
void ColorTimeLine::SetPointTime(uint8_t index, uint8_t time)
{

}
void ColorTimeLine::RemovePoint(uint8_t index)
{

}

void ColorTimeLine::RefreshCurrentColor()
{
  uint8_t pointCount = _points.Count();

  if (pointCount == 0)
  {
    _currentColor = Color_t(127, 127, 127);
    return;
  }

  if (pointCount == 1)
  {
    ColorTimePoint ctp;
    _points.TryGetAtIndex(0, ctp);
    _currentColor = ctp.Color();
    return;
  }

  ColorTimePoint lctp;
  ColorTimePoint rctp;

  _points.TryGetAtIndex(0, lctp);
  _points.TryGetAtIndex(pointCount - 1, rctp);

  if (_normalizedCurrentTime <= lctp.Time() || _normalizedCurrentTime >= rctp.Time())
  {
    ColorTimePoint tmp = lctp;
    lctp = rctp;
    rctp = tmp;
  }
  else
  {
    for (uint8_t i = 1; i < pointCount - 1; ++i)
    {
      ColorTimePoint ctp;
      _points.TryGetAtIndex(i, ctp);
      uint8_t ctpTime = ctp.Time();

      if (ctpTime <= _normalizedCurrentTime && ctpTime > lctp.Time())
        lctp = ctp;
      if (ctpTime >= _normalizedCurrentTime && ctpTime < rctp.Time())
        rctp = ctp;
    }
  }

  uint8_t lctpTime = lctp.Time();
  uint8_t rctpTime = rctp.Time();
  uint8_t progressDiff = _normalizedCurrentTime - lctpTime;
  uint8_t pointToPointDiff = rctpTime - lctpTime;
  uint8_t ratio;
  if (pointToPointDiff > 0)
  {
    ratio = 255 * progressDiff / pointToPointDiff;
  }
  else
  {
    ratio = 0;
  }

  InterpolateColors(lctp.Color(), rctp.Color(), ratio, _currentColor);
}

void ColorTimeLine::InterpolateColors(Color_t lColor, Color_t rColor, uint8_t ratio, Color_t& outColor)
{
  InterpolateColorsComponents(lColor.R, rColor.R, ratio, outColor.R);
  InterpolateColorsComponents(lColor.G, rColor.G, ratio, outColor.G);
  InterpolateColorsComponents(lColor.B, rColor.B, ratio, outColor.B);
}

void ColorTimeLine::InterpolateColorsComponents(uint8_t lColorComponent, uint8_t rColorComponent, uint8_t ratio, uint8_t& outColorComponent)
{
  outColorComponent = (uint8_t)((lColorComponent * (255 - ratio) + rColorComponent * ratio) / 255);
}

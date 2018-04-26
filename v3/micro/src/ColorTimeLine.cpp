#include "ColorTimeLine.h"
#include "Particle.h"

void ColorTimeLine::Setup()
{
  Color_t c0(255, 0, 0);
  Color_t c1(0, 255, 0);
  Color_t c2(0, 0, 255);
  uint8_t t0 = 85;
  uint8_t t1 = 170;
  uint8_t t2 = 0;

  _cycleTime = ColorTimeLine_DefaultCycleTime;
  _currentColor = c0;
  _currentTime = 0;

  AddPoint(c0, t0);
  AddPoint(c1, t1);
  AddPoint(c2, t2);
}

uint32_t ColorTimeLine::GetCycleTime()
{
  return _cycleTime;
}

void ColorTimeLine::SetCycleTime(uint32_t cycleTime)
{
  _currentTime = _currentTime * cycleTime / _cycleTime; // Rescale current time to new time span.
  _cycleTime = cycleTime;
}

void ColorTimeLine::IncreaseCurrentTime(int32_t delta)
{
  _currentTime += delta;

  int32_t diff = _currentTime - _cycleTime;
  if (diff >= 0)
  {
    _currentTime = diff;
  }

  RefreshCurrentColor();
}

uint32_t ColorTimeLine::GetCurrentTimeProgress()
{
  return 255 * _currentTime / _cycleTime;
}

void ColorTimeLine::SetCurrentTimeProgress(uint32_t currentTimeProgress)
{
  _currentTime = _cycleTime * currentTimeProgress / 255;
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

ColorTimePoint ColorTimeLine::GetPoint(uint8_t id)
{
  ColorTimePoint ctp;
  _points.TryGetById(id, ctp);
  return ctp;
}
void ColorTimeLine::SetPointColor(uint8_t id, Color_t color)
{
  ColorTimePoint ctp;
  if (_points.TryGetById(id, ctp))
  {
    ctp.SetColor(color);
  }
}

void ColorTimeLine::SetPointTime(uint8_t id, uint8_t time)
{
  ColorTimePoint ctp;
  if (_points.TryGetById(id, ctp))
  {
    ctp.SetTime(time);
  }
}

void ColorTimeLine::RemovePoint(uint8_t id)
{
  _points.TryRemoveById(id);
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
    _currentColor = ctp.GetColor();
    return;
  }

  ColorTimePoint lctp;
  ColorTimePoint rctp;

  _points.TryGetAtIndex(0, lctp);
  _points.TryGetAtIndex(pointCount - 1, rctp);

  uint8_t currentTimeProgress = GetCurrentTimeProgress();

  if (currentTimeProgress <= lctp.GetTime() || currentTimeProgress >= rctp.GetTime())
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
      uint8_t ctpTime = ctp.GetTime();

      if (ctpTime <= currentTimeProgress && ctpTime > lctp.GetTime())
        lctp = ctp;
      if (ctpTime >= currentTimeProgress && ctpTime < rctp.GetTime())
        rctp = ctp;
    }
  }

  uint8_t ratio = InverseLerp(lctp.GetTime(), rctp.GetTime(), currentTimeProgress);

  InterpolateColors(lctp.GetColor(), rctp.GetColor(), ratio, _currentColor);
}

uint8_t ColorTimeLine::InverseLerp(uint8_t lValue, uint8_t rValue, uint8_t value)
{
  uint8_t progress = value - lValue;
  uint8_t range = rValue - lValue;
  if (range > 0)
  {
    return 255 * progress / range;
  }
  return 127;
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

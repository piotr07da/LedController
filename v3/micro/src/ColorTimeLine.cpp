#include "ColorTimeLine.h"
#include "Particle.h"

void ColorTimeLine::Setup()
{
  Color_t c0(255, 0, 0);
  Color_t c1(0, 255, 0);
  Color_t c2(0, 0, 255);
  float t0 = 0.43;
  float t1 = 0.76;
  float t2 = 0.10;

  _cycleTime = ColorTimeLine_DefaultCycleTime;
  _currentTime = 0;

  AddPoint(c0, t0);
  AddPoint(c1, t1);
  AddPoint(c2, t2);
}

int32_t ColorTimeLine::GetCycleTime()
{
  return _cycleTime;
}

void ColorTimeLine::SetCycleTime(int32_t cycleTime)
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

float ColorTimeLine::GetCurrentTimeProgress()
{
  return _currentTime / (float)_cycleTime;
}

void ColorTimeLine::SetCurrentTimeProgress(float currentTimeProgress)
{
  _currentTime = (int32_t)(_cycleTime * currentTimeProgress);
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

uint8_t ColorTimeLine::AddPoint(Color_t color, float time)
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

void ColorTimeLine::SetPointTime(uint8_t id, float time)
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

  float currentTimeProgress = GetCurrentTimeProgress();

  if (currentTimeProgress < lctp.GetTime() || currentTimeProgress > rctp.GetTime())
  {
    Swap(lctp, rctp);
  }
  else
  {
    for (uint8_t i = 1; i < pointCount - 1; ++i)
    {
      ColorTimePoint ctp;
      _points.TryGetAtIndex(i, ctp);
      float ctpTime = ctp.GetTime();

      if (ctpTime <= currentTimeProgress && ctpTime > lctp.GetTime())
        lctp = ctp;
      if (ctpTime >= currentTimeProgress && ctpTime < rctp.GetTime())
        rctp = ctp;
    }
  }

  float ratio = InverseLerp(lctp.GetTime(), rctp.GetTime(), currentTimeProgress);

  InterpolateColors(lctp.GetColor(), rctp.GetColor(), ratio, _currentColor);
}

float ColorTimeLine::InverseLerp(float lValue, float rValue, float value)
{
  float progress = value - lValue;
  float range = rValue - lValue;

  if (progress < 0)
    progress = 1 + progress;

  if (range < 0)
    range = 1 + range;

  if (range > 0.0001)
  {
    float ratio = progress / range;
    return ratio;
  }
  return 0.5;
}

void ColorTimeLine::InterpolateColors(Color_t lColor, Color_t rColor, float ratio, Color_t& outColor)
{
  InterpolateColorsComponents(lColor.R, rColor.R, ratio, outColor.R);
  InterpolateColorsComponents(lColor.G, rColor.G, ratio, outColor.G);
  InterpolateColorsComponents(lColor.B, rColor.B, ratio, outColor.B);
}

void ColorTimeLine::InterpolateColorsComponents(uint8_t lColorComponent, uint8_t rColorComponent, float ratio, uint8_t& outColorComponent)
{
  outColorComponent = (uint8_t)((float)lColorComponent * (1.0 - ratio) + (float)rColorComponent * ratio);
}

void ColorTimeLine::Swap(ColorTimePoint& lhs, ColorTimePoint& rhs)
{
  ColorTimePoint tmp = lhs;
  lhs = rhs;
  rhs = tmp;
}

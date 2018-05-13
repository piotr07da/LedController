#include "ColorTimeLine.h"
#include "ColorsInterpolator.h"
#include "Particle.h"

void ColorTimeLine::Setup()
{
  Color_t c0(255, 0, 0);
  Color_t c1(0, 255, 0);
  Color_t c2(0, 0, 255);
  float t0 = 0.05;
  float t1 = 0.38;
  float t2 = 0.71;

  _cycleTime = ColorTimeLine_DefaultCycleTime;
  _currentTime = 0;

  AddPoint(c0, t0);
  AddPoint(c1, t1);
  AddPoint(c2, t2);

  RefreshCurrentColor();
}

int32_t ColorTimeLine::GetCycleTime()
{
  return _cycleTime;
}

void ColorTimeLine::SetCycleTime(int32_t cycleTime)
{
  _currentTime = (int32_t)(_currentTime * (cycleTime / (float)_cycleTime)); // Rescale current time to new time span.
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

float ColorTimeLine::GetTimeProgress()
{
  return _currentTime / (float)_cycleTime;
}

void ColorTimeLine::SetTimeProgress(float timeProgress)
{
  _currentTime = (int32_t)(_cycleTime * timeProgress);
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

  float timeProgress = GetTimeProgress();

  if (timeProgress < lctp.GetTime() || timeProgress > rctp.GetTime())
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

      if (ctpTime <= timeProgress && ctpTime > lctp.GetTime())
        lctp = ctp;
      if (ctpTime >= timeProgress && ctpTime < rctp.GetTime())
        rctp = ctp;
    }
  }

  float ratio = InverseLerp(lctp.GetTime(), rctp.GetTime(), timeProgress);

  ColorsInterpolator ci;
  ci.InterpolateColors(lctp.GetColor(), rctp.GetColor(), ratio, _currentColor);
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

void ColorTimeLine::Swap(ColorTimePoint& lhs, ColorTimePoint& rhs)
{
  ColorTimePoint tmp = lhs;
  lhs = rhs;
  rhs = tmp;
}

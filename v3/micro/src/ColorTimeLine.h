#ifndef __COLOR_TIME_LINE__
#define __COLOR_TIME_LINE__

#include "Particle.h"
#include "Color.h"
#include "ColorTimePoint.h"
#include "ColorTimePointList.h"

#define ColorTimeLine_DefaultCycleTime 30000

class ColorTimeLine
{
private:
  int32_t _cycleTime;
  int32_t _currentTime;
  Color_t _currentColor;
  ColorTimePointList _points;

public:
  void Setup();
  int32_t GetCycleTime();
  void SetCycleTime(int32_t cycleTime);
  void IncreaseCurrentTime(int32_t delta);
  float GetTimeProgress();
  void SetTimeProgress(float timeProgress);
  Color_t GetCurrentColor();
  uint8_t GetPointCount();
  ColorTimePoint* GetPoints();
  uint8_t AddPoint(Color_t color, float time);
  ColorTimePoint GetPoint(uint8_t id);
  void SetPointColor(uint8_t id, Color_t color);
  void SetPointTime(uint8_t id, float time);
  void RemovePoint(uint8_t id);

private:
  void RefreshCurrentColor();
  float InverseLerp(float lValue, float rValue, float value);
  void Swap(ColorTimePoint& lhs, ColorTimePoint& rhs);
};

#endif

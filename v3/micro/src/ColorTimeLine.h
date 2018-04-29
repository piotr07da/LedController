#ifndef __COLOR_TIME_LINE__
#define __COLOR_TIME_LINE__

#include "Particle.h"
#include "Color.h"
#include "ColorTimePoint.h"
#include "ColorTimePointList.h"

#define ColorTimeLine_DefaultCycleTime 3000

class ColorTimeLine
{
private:
  double _DBG_progress0;
  double _DBG_range0;
  double _DBG_progress1;
  double _DBG_range1;
  double _DBG_ratio;
  
  uint32_t _cycleTime;
  uint32_t _currentTime;
  Color_t _currentColor;
  ColorTimePointList _points;

public:
  void Setup();
  uint32_t GetCycleTime();
  void SetCycleTime(uint32_t cycleTime);
  void IncreaseCurrentTime(int32_t delta);
  float GetCurrentTimeProgress();
  void SetCurrentTimeProgress(float currentTimeProgress);
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
  void InterpolateColors(Color_t lColor, Color_t rColor, float ratio, Color_t& outColor);
  void InterpolateColorsComponents(uint8_t lColorComponent, uint8_t rColorComponent, float ratio, uint8_t& outColorComponent);
  void Swap(ColorTimePoint& lhs, ColorTimePoint& rhs);
};

#endif

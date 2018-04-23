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
  uint32_t _cycleTime;
  uint32_t _currentTime;
  Color_t _currentColor;
  ColorTimePointList _points;

public:
  void Setup();
  uint32_t GetCycleTime();
  void SetCycleTime(uint32_t cycleTime);
  void IncreaseCurrentTime(int32_t delta);
  uint32_t GetCurrentTimeProgress();
  void SetCurrentTimeProgress(uint32_t currentTimeProgress);
  Color_t GetCurrentColor();
  uint8_t GetPointCount();
  ColorTimePoint* GetPoints();
  uint8_t AddPoint(Color_t color, uint8_t time);
  ColorTimePoint GetPoint(uint8_t id);
  void SetPointColor(uint8_t id, Color_t color);
  void SetPointTime(uint8_t id, uint8_t time);
  void RemovePoint(uint8_t id);

private:
  void RefreshCurrentColor();
  uint8_t InverseLerp(uint8_t lValue, uint8_t rValue, uint8_t value);
  void InterpolateColors(Color_t lColor, Color_t rColor, uint8_t ratio, Color_t& outColor);
  void InterpolateColorsComponents(uint8_t lColorComponent, uint8_t rColorComponent, uint8_t ratio, uint8_t& outColorComponent);
};

#endif

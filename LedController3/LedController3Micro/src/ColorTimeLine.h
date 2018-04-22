#ifndef __COLOR_TIME_LINE__
#define __COLOR_TIME_LINE__

#include "Particle.h"
#include "Color.h"
#include "ColorTimePoint.h"
#include "ColorTimePointList.h"

#define ColorTimeLine_DefaultTimeSpan 10000

class ColorTimeLine
{
private:
  int _DGB_pointCount;
  bool _DBG_published;
  uint32_t _timeSpan;
  Color_t _currentColor;
  uint32_t _currentTime;
  uint8_t _normalizedCurrentTime;
  ColorTimePointList _points;

public:
  void Setup();
  void IncreaseCurrentTime(int32_t timeStep);
  uint32_t GetCurrentTime();
  void SetCurrentTime(uint32_t currentTime);
  uint32_t GetTimeSpan();
  void SetTimeSpan(uint32_t timeSpan);
  Color_t GetCurrentColor();
  uint8_t GetPointCount();
  ColorTimePoint* GetPoints();
  uint8_t AddPoint(Color_t color, uint8_t time);
  ColorTimePoint GetPoint(uint8_t index);
  void SetPointColor(uint8_t index, Color_t color);
  void SetPointTime(uint8_t index, uint8_t time);
  void RemovePoint(uint8_t index);

private:
  void RefreshCurrentColor();
  void InterpolateColors(Color_t lColor, Color_t rColor, uint8_t ratio, Color_t& outColor);
  void InterpolateColorsComponents(uint8_t lColorComponent, uint8_t rColorComponent, uint8_t ratio, uint8_t& outColorComponent);
};

#endif

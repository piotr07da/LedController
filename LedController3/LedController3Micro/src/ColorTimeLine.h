#ifndef __COLOR_TIME_LINE__
#define __COLOR_TIME_LINE__

#define ColorTimeLine_MaxColorTimePointCount 10

#include "Color.h"
#include "ColorTimePoint.h"

class ColorTimeLine
{
private:
  uint32_t _timeSpan;
  uint32_t _currentTime;
  ColorTimePoint_t _points[ColorTimeLine_MaxColorTimePointCount];

public:
  void Setup();
  void AddTime(uint32_t timeStep);
  uint32_t GetCurrentTime();
  void SetCurrentTime(uint32_t currentTime);
  uint32_t GetTimeSpan();
  void SetTimeSpan(uint32_t timeSpan);
  Color_t GetCurrentColor();
  void AddPoint(ColorTimePoint_t point);
};

#endif

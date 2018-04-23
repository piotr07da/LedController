#ifndef __COLOR_TIME_POINT_LIST__
#define __COLOR_TIME_POINT_LIST__

#include "ColorTimePoint.h"

#define ColorTimePointList_MaxCount 10

class ColorTimePointList
{
private:
  ColorTimePoint _orderedPoints[ColorTimePointList_MaxCount];
  uint8_t _orderedIds[ColorTimePointList_MaxCount];
  uint8_t _count;

public:
  ColorTimePointList();
  uint8_t Count();
  ColorTimePoint* GetAll();
  uint8_t NextFreeId();
  bool TryAdd(ColorTimePoint point);
  bool TryRemoveById(uint8_t id);
  bool TryGetAtIndex(uint8_t index, ColorTimePoint& point);
  bool TryGetById(uint8_t id, ColorTimePoint& point);

private:
  uint8_t FindIndexToInsert(uint8_t (ColorTimePointList::*currentValuesGetter)(uint8_t), uint8_t valueToInsert);
  uint8_t GetOrderedPointTimeValue(uint8_t index);
  uint8_t GetOrderedIdValue(uint8_t index);
};

#endif

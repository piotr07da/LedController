#include "ColorTimePointList.h"
#include "Particle.h"

ColorTimePointList::ColorTimePointList()
{
  _count = 0;
}

uint8_t ColorTimePointList::Count()
{
  return _count;
}

ColorTimePoint* ColorTimePointList::GetAll()
{
  return _orderedPoints;
}

uint8_t ColorTimePointList::NextFreeId()
{
  uint8_t id = 0;
  for (int16_t i = 0; i < _count; ++i)
  {
    if (id != _orderedIds[i])
    {
      return id;
    }
    else
    {
      ++id;
    }

  }
  return id;
}

bool ColorTimePointList::TryAdd(ColorTimePoint point)
{
  if (_count == ColorTimePointList_MaxCount)
    return false;

  uint8_t indexToInsertPoint = FindIndexToInsert(&ColorTimePointList::GetOrderedPointTimeValue, point.GetTime());

  if (_count > 0)
  {
    for (int16_t i = _count - 1; i >= indexToInsertPoint; --i)
    {
      _orderedPoints[i + 1] = _orderedPoints[i];
    }
  }
  _orderedPoints[indexToInsertPoint] = point;

  uint8_t indexToInsertId = FindIndexToInsert(&ColorTimePointList::GetOrderedIdValue, point.GetId());
  if (_count > 0)
  {
    for (int16_t i = _count - 1; i >= indexToInsertId; --i)
    {
      _orderedIds[i + 1] = _orderedIds[i];
    }
  }
  _orderedIds[indexToInsertPoint] = point.GetId();

  ++_count;

  return true;
}

bool ColorTimePointList::TryRemoveById(uint8_t id)
{
  uint8_t indexToRemovePoint;
  bool indexToRemovePointFound = false;
  for (int16_t i = 0; i < _count; ++i)
  {
    if (_orderedPoints[i].GetId() == id)
    {
      indexToRemovePoint = i;
      indexToRemovePointFound = true;
      break;
    }
  }

  if (!indexToRemovePointFound)
    return false;

  uint8_t indexToRemoveId;
  bool indexToRemoveIdFound = false;
  for (int16_t i = 0; i < _count; ++i)
  {
    if (_orderedIds[i] == id)
    {
      indexToRemoveId = i;
      indexToRemoveIdFound = true;
      break;
    }
  }

  if (!indexToRemoveIdFound)
    return false;

  for (int16_t i = indexToRemovePoint; i < _count; ++i)
  {
    _orderedPoints[i] = _orderedPoints[i + 1];
  }

  for (int16_t i = indexToRemoveId; i < _count; ++i)
  {
    _orderedIds[i] = _orderedIds[i + 1];
  }

  --_count;

  return true;
}

bool ColorTimePointList::TryGetAtIndex(uint8_t index, ColorTimePoint& point)
{
  if (index < 0 || index >= _count)
  {
    return false;
  }
  point = _orderedPoints[index];
  return true;
}

bool ColorTimePointList::TryGetById(uint8_t id, ColorTimePoint& point)
{
  for (uint8_t i = 0; i < _count; ++i)
  {
    ColorTimePoint ctp = _orderedPoints[i];
    if (ctp.GetId() == id)
    {
      point = ctp;
      return true;
    }
  }
}

uint8_t ColorTimePointList::FindIndexToInsert(float (ColorTimePointList::*currentValuesGetter)(uint8_t), float valueToInsert)
{
  if (_count == 0)
    return 0;

  uint8_t indexToInsert = 0;

  for (int16_t i = 0; i < _count; ++i)
  {
    if (valueToInsert > (this->*currentValuesGetter)(i))
    {
      ++indexToInsert;
    }
    else
    {
      break;
    }
  }

  return indexToInsert;
}

float ColorTimePointList::GetOrderedPointTimeValue(uint8_t index)
{
  return _orderedPoints[index].GetTime();
}

float ColorTimePointList::GetOrderedIdValue(uint8_t index)
{
  return _orderedIds[index];
}

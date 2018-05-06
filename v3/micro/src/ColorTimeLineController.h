#ifndef __COLOR_TIME_LINE_CONTROLLER__
#define __COLOR_TIME_LINE_CONTROLLER__

#include "MillisTimer.h"
#include "ColorTimeLine.h"

class ColorTimeLineController
{
private:
  uint32_t _rOutPin;
  uint32_t _gOutPin;
  uint32_t _bOutPin;
  MillisTimer _millisTimer;
  ColorTimeLine _colorTimeLine;

public:
  void Start();
  void Update();
  ColorTimeLine* GetColorTimeLine();

private:
  void WriteCurrentColorToPwmOutputPins();
  void SetPwmOutputPin(uint32_t pin, uint8_t value);
};

#endif

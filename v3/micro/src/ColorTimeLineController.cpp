#include "Particle.h"
#include "ColorTimeLineController.h"

void ColorTimeLineController::Start()
{
  _rOutPin = D0;
  _gOutPin = D1;
  _bOutPin = D2;
  pinMode(_rOutPin, OUTPUT);
  pinMode(_gOutPin, OUTPUT);
  pinMode(_bOutPin, OUTPUT);

  _millisTimer.Setup(10);
  _colorTimeLine.Setup();
}

void ColorTimeLineController::Update()
{
  uint32_t lastValidCheckInterval;
  if (_millisTimer.Check(lastValidCheckInterval))
  {
    _colorTimeLine.IncreaseCurrentTime(lastValidCheckInterval);
    WriteCurrentColorToPwmOutputPins();
  }
}

void ColorTimeLineController::WriteCurrentColorToPwmOutputPins()
{
  Color_t color = _colorTimeLine.GetCurrentColor();
  SetPwmOutputPin(_rOutPin, color.R);
  SetPwmOutputPin(_gOutPin, color.G);
  SetPwmOutputPin(_bOutPin, color.B);
}

void ColorTimeLineController::SetPwmOutputPin(uint32_t pin, uint8_t value)
{
  analogWrite(pin, value, 65535);
}

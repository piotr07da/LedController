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

ColorTimeLine* ColorTimeLineController::GetColorTimeLine()
{
  return &_colorTimeLine;
}

void ColorTimeLineController::WriteCurrentColorToPwmOutputPins()
{
  Color_t color = _colorTimeLine.GetCurrentColor();

  // Based on this articles:
  // LED Backlight Color Measurements
  // https://people.xiph.org/~xiphmont/thinkpad/led-gamut.shtml
  // Understand RGB LED mixing ratios to realize optimal color in signs and displays (MAGAZINE)
  // http://www.ledsmagazine.com/articles/print/volume-10/issue-6/features/understand-rgb-led-mixing-ratios-to-realize-optimal-color-in-signs-and-displays-magazine.html

  // D65 (6500K temperature of blackbody) Ideal White RGB mixing ratios:
  float wrr = 6.4; // mixing ratio for red color for ideal white
  float wgr = 4.9; // mixing ratio for green color for ideal white
  float wbr = 1; // mixing ratio for blue color for ideal white

  // Multiply by mixing ratios
  float rr = (float)color.R * wrr;
  float gr = (float)color.G * wgr;
  float br = (float)color.B * wbr;

  // Normalize to 1 ...
  float rs = rr + gr + br;
  rr /= rs;
  gr /= rs;
  br /= rs;

  uint8_t mc = color.MaxComp();

  // ... and convert to one byte (uint8_t) scale needed for PWM. But not using 255 max range. Instead max component value from input color is used.
  uint8_t r = (uint8_t)(mc * rr);
  uint8_t g = (uint8_t)(mc * gr);
  uint8_t b = (uint8_t)(mc * br);

  // Set PWM outputs.
  SetPwmOutputPin(_rOutPin, r);
  SetPwmOutputPin(_gOutPin, g);
  SetPwmOutputPin(_bOutPin, b);
}

void ColorTimeLineController::SetPwmOutputPin(uint32_t pin, uint8_t value)
{
  analogWrite(pin, value, 65535);
}

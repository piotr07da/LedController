#include "LedHeartBeat.h"
#include "Particle.h"

void LedHeartBeat::Start()
{
  _millisTimer.Setup(500);
  _ledOutpin = D7;
  pinMode(_ledOutpin, OUTPUT);
}

void LedHeartBeat::Update()
{
  if (_millisTimer.Check())
  {
    _ledState = !_ledState;
    if (_ledState)
      digitalWrite(_ledOutpin, HIGH);
    else
      digitalWrite(_ledOutpin, LOW);
  }
}

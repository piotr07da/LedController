#include "ColorTimeLineController.h"

void ColorTimeLineController::Start()
{
  _millisTimer.Setup(10);
}

void ColorTimeLineController::Update()
{
  
  if (_millisTimer.Check())
  {

  }
  // - Pobrac kolor z timeline'u
  // - Ustawic PWM
}

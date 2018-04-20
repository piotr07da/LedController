/*
 * Project LedController3Micro
 * Description:
 * Author: Piotr Bejger
 * Created: 2018-04-19
 */

#include "ColorTimeLineController.h"
#include "LedHeartBeat.h"

ColorTimeLineController _colorTimeLineController;
LedHeartBeat _ledHeartBeat;

void setup()
{
  _colorTimeLineController.Start();
  _ledHeartBeat.Start();
}

void loop()
{
  _colorTimeLineController.Update();
  _ledHeartBeat.Update();
}

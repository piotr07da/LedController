#ifndef __LED_HEART_BEAT__
#define __LED_HEART_BEAT__

#include "MillisTimer.h"

class LedHeartBeat
{
private:
  int _ledOutpin;
  bool _ledState;
  MillisTimer _millisTimer;

public:
  void Start();
  void Update();

};

#endif

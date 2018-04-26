#ifndef __MILLIS_TIMER__
#define __MILLIS_TIMER__

#include "Particle.h"

class MillisTimer
{
private:
  uint32_t _interval;
  uint32_t _lastTime;

public:
  void Setup(uint32_t interval);
  bool Check();
  bool Check(uint32_t& lastValidCheckInterval);
};

#endif

#include "Particle.h"
#include "MillisTimer.h"

void MillisTimer::Setup(uint32_t interval)
{
  _interval = interval;
  _lastTime = millis();
}



bool MillisTimer::Check()
{
  uint32_t lastCheckInterval;
  return Check(lastCheckInterval);
}

bool MillisTimer::Check(uint32_t &lastValidCheckInterval)
{
  uint32_t currentTime = millis();

  // If millis rolls over which means it is less than _lastMillis this always returns positive value (due to unsigned arithmetic).
  // This value will be always correct value.
  // ie.:
  // 1 - 253 = 4
  uint32_t diff = currentTime - _lastTime;
  lastValidCheckInterval = diff;

	if (diff >= _interval)
  {
		_lastTime = currentTime;
		return true;
	}
  return false;
}

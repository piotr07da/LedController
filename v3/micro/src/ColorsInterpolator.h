#ifndef __COLORS_INTERPOLATOR__
#define __COLORS_INTERPOLATOR__

#include "Color.h"

class ColorsInterpolator
{
public:
  void InterpolateColors(Color_t lColor, Color_t rColor, float ratio, Color_t& outColor);

private:
  void InterpolateColorsComponents(uint8_t lColorComponent, uint8_t rColorComponent, float ratio, uint8_t& outColorComponent);
};

#endif

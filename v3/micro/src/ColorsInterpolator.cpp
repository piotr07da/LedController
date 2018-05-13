#include "ColorsInterpolator.h"
#include "Particle.h"

void ColorsInterpolator::InterpolateColors(Color_t lColor, Color_t rColor, float ratio, Color_t& outColor)
{
  uint8_t lColorMax = lColor.MaxComp();
  uint8_t rColorMax = rColor.MaxComp();
  uint8_t cColorMax = (lColorMax + rColorMax) / 2;

  Color_t cColor = Color_t(
    (uint8_t)((lColor.R + rColor.R) / 2),
    (uint8_t)((lColor.G + rColor.G) / 2),
    (uint8_t)((lColor.B + rColor.B) / 2));

  uint8_t colorMax = cColor.MaxComp();
  if (colorMax > 0)
  {
    float mr = cColorMax / (float)colorMax;
    cColor = Color_t(
      (uint8_t)(mr * cColor.R),
      (uint8_t)(mr * cColor.G),
      (uint8_t)(mr * cColor.B));
  }

  Color_t iColor;
  if (ratio < 0.5)
  {
    iColor = lColor;
  }
  else
  {
    iColor = rColor;
    ratio = 1 - ratio;
  }

  ratio *= 2;

  InterpolateColorsComponents(iColor.R, cColor.R, ratio, outColor.R);
  InterpolateColorsComponents(iColor.G, cColor.G, ratio, outColor.G);
  InterpolateColorsComponents(iColor.B, cColor.B, ratio, outColor.B);
}

void ColorsInterpolator::InterpolateColorsComponents(uint8_t lColorComponent, uint8_t rColorComponent, float ratio, uint8_t& outColorComponent)
{
  outColorComponent = (uint8_t)((float)lColorComponent * (1.0 - ratio) + (float)rColorComponent * ratio);
}

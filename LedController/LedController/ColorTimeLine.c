/*
 * ColorTimeLine.c
 *
 * Created: 2013-01-17 22:14:54
 *  Author: pbejger
 */ 

#include "ColorTimeLine.h"

// Reset time line to its defaults.
void ResetTimeLine(ColorTimeLine_t *colorTimeLine)
{
    colorTimeLine->TimeProgress = 0;
    colorTimeLine->TimeSpan = 5;
    
    colorTimeLine->PointCount = 3;
    
    colorTimeLine->Points[0].TimePoint = ctl_TIME_LINE_BEG_POINT;
    SetColor(&colorTimeLine->Points[0].Color, 0xFF00FF);
    
    colorTimeLine->Points[1].TimePoint = ctl_TIME_LINE_END_POINT / 2;
    SetColor(&colorTimeLine->Points[1].Color, 0x66FF66);
    
    colorTimeLine->Points[2].TimePoint = ctl_TIME_LINE_END_POINT;
    SetColor(&colorTimeLine->Points[2].Color, 0xFF00FF);
}

// Sets color value.
void SetColor(Color_t *color, uint32_t value)
{
    color->R = (uint8_t)(value >> 16);
    color->G = (uint8_t)(value >> 8);
    color->B = (uint8_t)(value);
}

// Calculates current color.
void CalcCurrentColor(ColorTimeLine_t *colorTimeLine, Color_t *currentColor)
{
    ColorTimePoint_t *ctpL;
    ColorTimePoint_t *ctpR;
    
    uint16_t ctpLTimePoint = 0;
    uint16_t ctpRTimePoint = 0;
    
    for (uint8_t ctpIx = 0; ctpIx < colorTimeLine->PointCount - 1; ++ctpIx)
    {
        ctpL = &colorTimeLine->Points[ctpIx];
        ctpR = &colorTimeLine->Points[ctpIx + 1];
        
        ctpLTimePoint = TimePointToTimeProgress(&ctpL->TimePoint);
        ctpRTimePoint = TimePointToTimeProgress(&ctpR->TimePoint);
        
        if (colorTimeLine->TimeProgress >= ctpLTimePoint &&
            colorTimeLine->TimeProgress <= ctpRTimePoint)
        {
            break;
        }
    }
    
    uint8_t betweenColorRatio = (uint8_t)((255 * (uint32_t)(colorTimeLine->TimeProgress - ctpLTimePoint)) / (uint32_t)(ctpRTimePoint - ctpLTimePoint));
    
    InterpolateColorComponent(ctpL->Color.R, ctpR->Color.R, betweenColorRatio, &currentColor->R);
    InterpolateColorComponent(ctpL->Color.G, ctpR->Color.G, betweenColorRatio, &currentColor->G);
    InterpolateColorComponent(ctpL->Color.B, ctpR->Color.B, betweenColorRatio, &currentColor->B);
}

// Interpolates color component of two colors L and R.
void InterpolateColorComponent(uint8_t lColorComponent, uint8_t rColorComponent, uint8_t ratio, uint8_t *interpolatedColorComponent)
{
    *interpolatedColorComponent = (uint8_t)((lColorComponent * (255 - ratio) + rColorComponent * ratio) / 255);
}

// Converts time point to time progress.
uint16_t TimePointToTimeProgress(uint8_t *timePoint)
{
    return ((uint16_t)(*timePoint) << 8) | (*timePoint);
}
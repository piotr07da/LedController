/*
 * ColorTimeLine.c
 *
 * Created: 2013-01-17 22:14:54
 *  Author: pbejger
 */ 

#include "ColorTimeLine.h"

// Reset time line to its defaults.
void InitTimeLine(ColorTimeLine_t *colorTimeLine, void (*timeSpanUpdated)(ColorTimeLine_t *colorTimeLine, uint16_t newTimeSpanValue))
{
    colorTimeLine->TimeSpanUpdated = timeSpanUpdated;
    
    colorTimeLine->TimeProgress = 0;
    SetTimeSpan(colorTimeLine, 200);
    
    colorTimeLine->MarkerCount = 4;
    
    colorTimeLine->Markers[0].TimePoint = ctl_TIME_LINE_BEG_POINT;
    SetColor(&colorTimeLine->Markers[0].Color, 0xFF0000);
    
    colorTimeLine->Markers[1].TimePoint = ctl_TIME_LINE_END_POINT / 3;
    SetColor(&colorTimeLine->Markers[1].Color, 0x00FF00);
    
    colorTimeLine->Markers[2].TimePoint = 2 * ctl_TIME_LINE_END_POINT / 3;
    SetColor(&colorTimeLine->Markers[2].Color, 0x0000FF);
    
    colorTimeLine->Markers[3].TimePoint = ctl_TIME_LINE_END_POINT;
    SetColor(&colorTimeLine->Markers[3].Color, 0xFF0000);
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
    ColorTimeMarker_t *ctpL = NULL;
    ColorTimeMarker_t *ctpR = NULL;
    
    uint16_t ctpLTimePoint = 0;
    uint16_t ctpRTimePoint = 0;
    
    for (uint8_t ctpIx = 0; ctpIx < colorTimeLine->MarkerCount - 1; ++ctpIx)
    {
        ctpL = &colorTimeLine->Markers[ctpIx];
        ctpR = &colorTimeLine->Markers[ctpIx + 1];
        
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

// Adds new color marker.
void AddColorMarker(ColorTimeLine_t *colorTimeLine)
{
    if (colorTimeLine->MarkerCount == ctl_MAX_MARKER_COUNT)
        return;
    
    uint8_t mIx;
    uint8_t markerCount = colorTimeLine->MarkerCount;
    
    for (mIx = 0; mIx < markerCount; ++mIx)
        colorTimeLine->Markers[mIx].TimePoint = (colorTimeLine->Markers[mIx].TimePoint * (markerCount - 1)) / markerCount;
    
    SetAsLastMarker(&colorTimeLine->Markers[markerCount], colorTimeLine);
    ++colorTimeLine->MarkerCount;
}

// Removes last color marker.
void RemColorMarker(ColorTimeLine_t *colorTimeLine)
{
    if (colorTimeLine->MarkerCount == ctl_MIN_MARKER_COUNT)
        return;
    
    uint8_t mIx;
    uint8_t markerCount = --colorTimeLine->MarkerCount;
    
    for (mIx = 0; mIx < markerCount - 1; ++mIx)
        colorTimeLine->Markers[mIx].TimePoint = (colorTimeLine->Markers[mIx].TimePoint * markerCount) / (markerCount - 1);
    
    SetAsLastMarker(&colorTimeLine->Markers[markerCount - 1], colorTimeLine);
}

// Sets color marker.
void SetColorMarker(ColorTimeLine_t *colorTimeLine, ColorTimeMarker_t marker, uint8_t index)
{
    colorTimeLine->Markers[index] = marker;
    
    // First and last color have to be the same
    if (index == 0)
        colorTimeLine->Markers[colorTimeLine->MarkerCount - 1].Color = marker.Color;
    else if (index == colorTimeLine->MarkerCount - 1)
        colorTimeLine->Markers[0].Color = marker.Color;
}

// Sets marker as last marker in time line.
void SetAsLastMarker(ColorTimeMarker_t *marker, ColorTimeLine_t *colorTimeLine)
{
    marker->TimePoint = UINT8_MAX;
    marker->Color = colorTimeLine->Markers[0].Color;
}

void SetTimeSpan(ColorTimeLine_t *colorTimeLine, uint16_t timeSpan)
{
    colorTimeLine->TimeSpan = timeSpan;
    
    if (colorTimeLine->TimeSpanUpdated != NULL)
        colorTimeLine->TimeSpanUpdated(colorTimeLine, timeSpan);
}
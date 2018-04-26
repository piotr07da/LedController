/*
 * ColorTimeLine.h
 *
 * Created: 2013-01-17 22:08:08
 *  Author: pbejger
 */ 

#ifndef COLORTIMELINE_H_
#define COLORTIMELINE_H_

#include <stdint.h>
#include <avr/cpufunc.h>
#include <stddef.h>

#define ctl_TIME_SPAN_SCALE 5 // time span is measured as number of 200ms time spans.

#define ctl_COLOR_TIME_MARKER_SIZE 4
#define ctl_TIME_SPAN_SIZE 2
#define ctl_TIME_PROGRESS_SIZE 2
#define ctl_MARKER_COUNT_SIZE 1 // number of bytes needed to store number of color markers

#define ctl_TIME_LINE_BEG_POINT 0
#define ctl_TIME_LINE_END_POINT UINT8_MAX

#define ctl_MIN_MARKER_COUNT 3
#define ctl_MAX_MARKER_COUNT 16

// TYPE PROTOTYPES

struct Color;
struct ColorTimeMarker;
struct ColorTimeLine;

typedef struct Color Color_t;
typedef struct ColorTimeMarker ColorTimeMarker_t;
typedef struct ColorTimeLine ColorTimeLine_t;

// TYPE DEFINITIONS

// Color
struct Color
{
    // Red component.
    uint8_t R;
    
    // Green component.
    uint8_t G;
    
    // Blue component.
    uint8_t B;
};

// Point in time which defines color.
struct ColorTimeMarker
{
    // Time point defined by progress on time line [0, 255].
    uint8_t TimePoint;
    
    union
    {
        // Color
        Color_t Color;
        
        // Color bytes.
        uint8_t ColorBytes[3];
    };
};

// Time line which defines color changes in time.
struct ColorTimeLine
{
    // Array of color time points.
    ColorTimeMarker_t Markers[ctl_MAX_MARKER_COUNT];
    
    // Number of color time markers.
    uint8_t MarkerCount;
    
    // Progress on time line - current time.
    uint16_t TimeProgress;
    
    // Time span measured in (1 / ctl_TIME_SPAN_SCALE) seconds.
    uint16_t TimeSpan;
    
    // Called in case of time span update.
    void (*TimeSpanUpdated)(ColorTimeLine_t *colorTimeLine, uint16_t newTimeSpanValue);
};


// FUNCTIONS

void InitTimeLine(ColorTimeLine_t *colorTimeLine, void (*timeSpanUpdated)(ColorTimeLine_t *colorTimeLine, uint16_t newTimeSpanValue));
void SetColor(Color_t *color, uint32_t value);
void CalcCurrentColor(ColorTimeLine_t *colorTimeLine, Color_t *currentColor);
void InterpolateColorComponent(uint8_t lColorComponent, uint8_t rColorComponent, uint8_t ratio, uint8_t *interpolatedColorComponent);
uint16_t TimePointToTimeProgress(uint8_t *timePoint);
void AddColorMarker(ColorTimeLine_t *colorTimeLine);
void RemColorMarker(ColorTimeLine_t *colorTimeLine);
void SetColorMarker(ColorTimeLine_t *colorTimeLine, ColorTimeMarker_t marker, uint8_t index);
void SetAsLastMarker(ColorTimeMarker_t *marker, ColorTimeLine_t *colorTimeLine);
void SetTimeSpan(ColorTimeLine_t *colorTimeLine, uint16_t timeSpan);

#endif /* COLORTIMELINE_H_ */
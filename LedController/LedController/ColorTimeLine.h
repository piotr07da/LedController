/*
 * ColorTimeLine.h
 *
 * Created: 2013-01-17 22:08:08
 *  Author: pbejger
 */ 

#include <stdint.h>
#include <avr/cpufunc.h>

#ifndef COLORTIMELINE_H_
#define COLORTIMELINE_H_

#define ctl_TIME_SPAN_SCALE 5 // time span is measured as number of 200ms time spans.

#define ctl_COLOR_TIME_POINT_SIZE 4
#define ctl_TIME_SPAN_SIZE 2
#define ctl_TIME_PROGRESS_SIZE 2
#define ctl_POINT_COUNT_SIZE 1 // number of bytes needed to store number of color points (markers)

#define ctl_TIME_LINE_BEG_POINT 0
#define ctl_TIME_LINE_END_POINT UINT8_MAX

#define ctl_MIN_POINT_COUNT 3
#define ctl_MAX_POINT_COUNT 16

// TYPE PROTOTYPES

struct Color;
struct ColorTimePoint;
struct ColorTimeLine;

typedef struct Color Color_t;
typedef struct ColorTimePoint ColorTimePoint_t;
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
struct ColorTimePoint
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
    ColorTimePoint_t Points[ctl_MAX_POINT_COUNT];
    
    // Number of color time points.
    uint8_t PointCount;
    
    // Progress on time line - current time.
    uint16_t TimeProgress;
    
    // Time span measured in (1 / ctl_TIME_SPAN_SCALE) seconds.
    uint16_t TimeSpan;
};


// FUNCTIONS

void ResetTimeLine(ColorTimeLine_t *colorTimeLine);
void SetColor(Color_t *color, uint32_t value);
void CalcCurrentColor(ColorTimeLine_t *colorTimeLine, Color_t *currentColor);
void InterpolateColorComponent(uint8_t lColorComponent, uint8_t rColorComponent, uint8_t ratio, uint8_t *interpolatedColorComponent);
uint16_t TimePointToTimeProgress(uint8_t *timePoint);


#endif /* COLORTIMELINE_H_ */
#ifndef __COLOR_TIME_LINE_CONTROLLER__
#define __COLOR_TIME_LINE_CONTROLLER__

#include "MillisTimer.h"
//#include "ColorTimeLine.h"

class ColorTimeLineController
{
    private:
      MillisTimer _millisTimer;
        //ColorTimeLine _colorTimeLine;

    public:
        void Start();
        void Update();

};

#endif

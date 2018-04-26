using System;
using System.Collections.Generic;

namespace LedController2Client
{
    [Serializable]
    public class ColorSchemeGroup
    {
        public virtual ColorSchemeCategory Category { get; set; }

        public virtual List<ColorScheme> Schemes { get; set; }
    }
}

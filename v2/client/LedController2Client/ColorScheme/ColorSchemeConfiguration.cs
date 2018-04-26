using System;
using System.Collections.Generic;

namespace LedController2Client
{
    [Serializable]
    public class ColorSchemeConfiguration
    {
        public virtual List<ColorSchemeGroup> ColorSchemeGroups { get; set; }
    }
}

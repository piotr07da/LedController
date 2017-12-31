using System;
using System.Collections.Generic;

namespace LedController2Client
{
    public class FakeColorSchemeResource : ColorSchemeResourceBase
    {
        #region ColorSchemeResourceBase Members

        private ColorSchemeConfiguration _config;
        protected override ColorSchemeConfiguration Config
        {
            get { return _config; }
        }

        protected override void LoadConfig()
        {
            ColorMarker[,] samples = new ColorMarker[2, 2];
            samples[0, 0] = new ColorMarker() { R = 255, G = 0, B = 0, TimePoint = 0 };
            samples[0, 1] = new ColorMarker() { R = 0, G = 0, B = 255, TimePoint = Byte.MaxValue };
            samples[1, 0] = new ColorMarker() { R = 0, G = 0, B = 255, TimePoint = 0 };
            samples[1, 1] = new ColorMarker() { R = 0, G = 255, B = 0, TimePoint = Byte.MaxValue };

            _config = new ColorSchemeConfiguration()
            {
                ColorSchemeGroups = new List<ColorSchemeGroup>()
                {
                    new ColorSchemeGroup()
                    {
                        Category = new ColorSchemeCategory() { Name = "Test category 1" },
                        Schemes = new List<ColorScheme>()
                        {
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                        },
                    },
                    new ColorSchemeGroup()
                    {
                        Category = new ColorSchemeCategory() { Name = "Test category 2" },
                        Schemes = new List<ColorScheme>()
                        {
                            new ColorScheme() { Name = "Test scheme 1.1", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                            new ColorScheme() { Name = "Test scheme 1.2", Gradient = new ColorMarker[2] { samples[1,0], samples[1,1] } },
                            new ColorScheme() { Name = "Test scheme 1.3", Gradient = new ColorMarker[2] { samples[0,0], samples[0,1] } },
                        },
                    },
                },
            };
        }

        protected override void SaveConfig()
        {
            //
        }

        #endregion


    }
}

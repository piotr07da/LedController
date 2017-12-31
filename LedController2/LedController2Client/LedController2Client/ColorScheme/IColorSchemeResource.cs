using System.Collections.Generic;

namespace LedController2Client
{
    public interface IColorSchemeResource
    {
        List<ColorSchemeCategory> GetCategories();

        List<ColorScheme> GetSchemes(ColorSchemeCategory category);

        ColorSchemeCategory NewCategory();

        void UpdateCategory(ColorSchemeCategory category);

        void RemoveCategory(ColorSchemeCategory category);

        ColorScheme NewScheme(ColorSchemeCategory category);

        void UpdateScheme(ColorScheme scheme);

        void RemoveScheme(ColorSchemeCategory category, ColorScheme scheme);
    }
}

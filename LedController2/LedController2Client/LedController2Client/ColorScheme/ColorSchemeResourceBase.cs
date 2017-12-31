using System;
using System.Collections.Generic;

namespace LedController2Client
{
    public abstract class ColorSchemeResourceBase : IColorSchemeResource
    {
        #region Attributes

        private Dictionary<ColorSchemeCategory, ColorSchemeGroup> _groupsByCategories;

        #endregion

        #region Properties

        protected abstract ColorSchemeConfiguration Config { get; }

        #endregion

        #region Methods

        private void AddToGroupByCategories(ColorSchemeGroup group)
        {
            if (!_groupsByCategories.ContainsKey(group.Category))
                _groupsByCategories.Add(group.Category, group);
        }

        protected virtual void EnsureConfigExists()
        {
            if (Config == null) LoadConfig();
            if (Config == null) throw new InvalidOperationException("Cannot load color scheme configuration.");
        }

        protected abstract void LoadConfig();

        protected abstract void SaveConfig();

        #endregion

        #region IColorSchemeResource Members

        public virtual List<ColorSchemeCategory> GetCategories()
        {
            EnsureConfigExists();

            _groupsByCategories = new Dictionary<ColorSchemeCategory, ColorSchemeGroup>();

            List<ColorSchemeCategory> cats = new List<ColorSchemeCategory>();
            foreach (ColorSchemeGroup gr in Config.ColorSchemeGroups)
            {
                cats.Add(gr.Category);
                AddToGroupByCategories(gr);
            }
            return cats;
        }

        public virtual List<ColorScheme> GetSchemes(ColorSchemeCategory category)
        {
            EnsureConfigExists();

            ColorSchemeGroup gr;
            if (_groupsByCategories.TryGetValue(category, out gr))
                return gr.Schemes;

            return null;
        }

        public virtual ColorSchemeCategory NewCategory()
        {
            EnsureConfigExists();

            ColorSchemeCategory cat = new ColorSchemeCategory() { Name = "CSC " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            ColorSchemeGroup group = new ColorSchemeGroup() { Category = cat, Schemes = new List<ColorScheme>() };
            Config.ColorSchemeGroups.Add(group);
            AddToGroupByCategories(group);

            SaveConfig();
            return cat;
        }

        public virtual void UpdateCategory(ColorSchemeCategory category)
        {
            EnsureConfigExists();

            SaveConfig();
        }

        public virtual void RemoveCategory(ColorSchemeCategory category)
        {
            EnsureConfigExists();

            ColorSchemeGroup group;
            if (_groupsByCategories.TryGetValue(category, out group))
            {
                Config.ColorSchemeGroups.Remove(group);
                _groupsByCategories.Remove(category);

                SaveConfig();
            }
        }

        public virtual ColorScheme NewScheme(ColorSchemeCategory category)
        {
            EnsureConfigExists();

            ColorSchemeGroup group;
            if (_groupsByCategories.TryGetValue(category, out group))
            {
                ColorScheme scheme = new ColorScheme() { Name = "CS " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                group.Schemes.Add(scheme);

                SaveConfig();
                return scheme;
            }

            return null;
        }

        public virtual void UpdateScheme(ColorScheme scheme)
        {
            EnsureConfigExists();

            SaveConfig();
        }

        public virtual void RemoveScheme(ColorSchemeCategory category, ColorScheme scheme)
        {
            EnsureConfigExists();

            ColorSchemeGroup group;
            if (_groupsByCategories.TryGetValue(category, out group))
            {
                group.Schemes.Remove(scheme);

                SaveConfig();
            }
        }

        #endregion
    }
}

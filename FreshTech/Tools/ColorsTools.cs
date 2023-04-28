using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshTech.Tools
{
    public static class ColorsTools
    {
        public static SolidColorBrush PrimaryBrush = Application.Current.Resources.MergedDictionaries.First()["PrimaryBrush"] as SolidColorBrush;
        public static SolidColorBrush SecondaryBrush = Application.Current.Resources.MergedDictionaries.First()["SecondaryBrush"] as SolidColorBrush;
        public static SolidColorBrush TertiaryBrush = Application.Current.Resources.MergedDictionaries.First()["TertiaryBrush"] as SolidColorBrush;
        public static SolidColorBrush DangerBrush = Application.Current.Resources.MergedDictionaries.First()["DangerBrush"] as SolidColorBrush;
        public static SolidColorBrush DangerDarkBrush = Application.Current.Resources.MergedDictionaries.First()["DangerDarkBrush"] as SolidColorBrush;
        public static SolidColorBrush WarningBrush = Application.Current.Resources.MergedDictionaries.First()["WarningBrush"] as SolidColorBrush;
        public static SolidColorBrush WarningDarkBrush = Application.Current.Resources.MergedDictionaries.First()["WarningDarkBrush"] as SolidColorBrush;
        public static SolidColorBrush SuccessBrush = Application.Current.Resources.MergedDictionaries.First()["SuccessBrush"] as SolidColorBrush;
        public static SolidColorBrush SuccessDarkBrush = Application.Current.Resources.MergedDictionaries.First()["SuccessDarkBrush"] as SolidColorBrush;

        public static Color Primary = Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
        public static Color Secondary = Application.Current.Resources.MergedDictionaries.First()["Secondary"] as Color;
        public static Color Tertiary = Application.Current.Resources.MergedDictionaries.First()["Tertiary"] as Color;
        public static Color Danger = Application.Current.Resources.MergedDictionaries.First()["Danger"] as Color;
        public static Color DangerDark = Application.Current.Resources.MergedDictionaries.First()["DangerDark"] as Color;
        public static Color Warning = Application.Current.Resources.MergedDictionaries.First()["Warning"] as Color;
        public static Color WarningDark = Application.Current.Resources.MergedDictionaries.First()["WarningDark"] as Color;
        public static Color Success = Application.Current.Resources.MergedDictionaries.First()["Success"] as Color;
        public static Color SuccessDark = Application.Current.Resources.MergedDictionaries.First()["SuccessDark"] as Color;
    }
}

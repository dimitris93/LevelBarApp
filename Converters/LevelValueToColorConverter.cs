using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LevelBarApp.Converters
{
    /// <summary>
    /// Converts between a LevelBar value and a color (in the Green->Yellow->Red range spectrum).
    /// </summary>
    public class LevelValueToColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert LevelBar value to color.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float level)
            {
                byte r = (byte)(255 * level);
                byte g = (byte)(255 * (1 - level));
                byte b = 0;

                return new SolidColorBrush(Color.FromRgb(r, g, b));
            }

            return Binding.DoNothing;
        }

        /// <summary>
        /// Convert color to LevelBar value.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

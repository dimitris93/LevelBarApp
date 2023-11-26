using System;
using System.Globalization;
using System.Windows.Data;

namespace LevelBarApp.Converters
{
    /// <summary>
    /// Converts between the MaxLevel value and Peakhold height.
    /// </summary>
    public class MaxLevelValueToPeakholdHeightConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a MaxLevel value to the corresponding Peakhold height.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is float maxLevel &&
                values[1] is double leverBarHeight)
            {
                return maxLevel * System.Convert.ToDouble(leverBarHeight);
            }

            return Binding.DoNothing;
        }

        /// <summary>
        /// Converts a Peakhold height to the corresponding MaxLevel value.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

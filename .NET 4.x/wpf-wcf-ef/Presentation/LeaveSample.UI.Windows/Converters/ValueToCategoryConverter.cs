using LeaveSample.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LeaveSample.UI.Windows.Converters
{
    /// <summary>
    /// Converts a string value to LeaveCategories enum and vice versa.
    /// </summary>
    public class ValueToCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LeaveCategories result;

            if (!Enum.TryParse<LeaveCategories>(value.ToString(), out result))
                result = LeaveCategories.Annual;

            return result;
        }
    }
}

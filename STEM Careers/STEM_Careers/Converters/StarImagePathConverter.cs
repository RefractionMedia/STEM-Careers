using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STEM_Careers.Converters
{
    class StarImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return "gold_star_empty.png";
            }
            if ((bool)value)
            {
                return "gold_star_full.png";
            }
            else return "gold_star_empty.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FileImageSource fileImageSource = value as FileImageSource;
            if ((string)fileImageSource.File == "gold_star_full.png")
            {
                return true;
            }
            else return false;
        }
    }
}

using fmath.controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ui
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class TexFormulaToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string formula = value as string;
            if(formula != null && targetType == typeof(ImageSource))
            {
                BitmapSource ret;

                Bitmap bmp = MathMLFormulaControl.generateBitmapFromLatex(string.Format("${0}$", formula));
                ret = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero,
                                                             Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                bmp.Dispose();

                return ret;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Globalization;
using System.Windows.Data;
using FloatingB.Shared.Helpers;

namespace FloatingB.Shared.Converters
{
    /// <summary>
    /// Binds an icon string to the right font (see GlyphFont).
    /// </summary>
    public class GlyphFontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GlyphFont.For(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

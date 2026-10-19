using System.Windows.Media;

namespace FloatingB.Shared.Helpers
{
    /// <summary>
    /// Picks the font for an icon string: Segoe MDL2 Assets for icon
    /// glyphs (Unicode private use area), Segoe UI for normal text.
    /// </summary>
    public static class GlyphFont
    {
        public static readonly FontFamily Icons = new("Segoe MDL2 Assets");
        public static readonly FontFamily Text = new("Segoe UI Semibold");

        public static FontFamily For(string? glyph)
        {
            if (string.IsNullOrEmpty(glyph)) return Text;

            foreach (char c in glyph)
            {
                if (c >= '\uE000' && c <= '\uF8FF') return Icons;
            }

            return Text;
        }
    }
}

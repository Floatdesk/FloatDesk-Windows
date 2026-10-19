using System.Runtime.InteropServices;
using System.Windows;

namespace FloatingB.Infrastructure.Windows
{
    /// <summary>
    /// Reads the mouse position anywhere on screen, even when it is
    /// not over one of our windows.
    /// </summary>
    internal static class CursorPosition
    {
        public static Point GetScreenPosition()
        {
            return GetCursorPos(out var point)
                ? new Point(point.X, point.Y)
                : new Point(double.NaN, double.NaN);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT point);
    }
}

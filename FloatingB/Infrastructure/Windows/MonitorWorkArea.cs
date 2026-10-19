using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace FloatingB.Infrastructure.Windows
{
    /// <summary>
    /// Finds the work area (screen minus taskbar) of the monitor
    /// a window is currently on. SystemParameters.WorkArea only
    /// knows about the primary monitor.
    /// </summary>
    internal static class MonitorWorkArea
    {
        private const uint MONITOR_DEFAULTTONEAREST = 2;

        public static Rect For(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;

            if (handle == IntPtr.Zero)
            {
                return SystemParameters.WorkArea;
            }

            var monitor = MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);

            var info = new MONITORINFO
            {
                cbSize = Marshal.SizeOf<MONITORINFO>()
            };

            if (!GetMonitorInfo(monitor, ref info))
            {
                return SystemParameters.WorkArea;
            }

            // Win32 gives device pixels; WPF positions windows in DIPs.
            var source = PresentationSource.FromVisual(window);
            Matrix toDips = source?.CompositionTarget?.TransformFromDevice ?? Matrix.Identity;

            var topLeft = toDips.Transform(new Point(info.rcWork.Left, info.rcWork.Top));
            var bottomRight = toDips.Transform(new Point(info.rcWork.Right, info.rcWork.Bottom));

            return new Rect(topLeft, bottomRight);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
    }
}

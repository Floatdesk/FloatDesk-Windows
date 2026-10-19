using FloatingB.Shared.Helpers;

namespace FloatingB.Features.Screenshot.Services
{
    public static class ScreenshotService
    {
        // Opens the Snipping Tool overlay (same as Win+Shift+S)
        public static void Start() => ShellLauncher.Open("ms-screenclip:");
    }
}

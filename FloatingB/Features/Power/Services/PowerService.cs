using System.Diagnostics;

namespace FloatingB.Features.Power.Services
{
    public static class PowerService
    {
        public static void ShutDown() => RunShutdown("/s /t 0");

        public static void Restart() => RunShutdown("/r /t 0");

        private static void RunShutdown(string arguments)
        {
            Process.Start(new ProcessStartInfo("shutdown.exe", arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
    }
}

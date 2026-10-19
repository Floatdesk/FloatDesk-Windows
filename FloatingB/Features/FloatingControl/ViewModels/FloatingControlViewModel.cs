using System.Diagnostics;

namespace FloatingB.Features.FloatingControl.ViewModels
{
    public class FloatingControlViewModel
    {
        public string Icon => "Ω";

        public void Lock()
        {
            Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
        }

        public void ShutDown()
        {
            Process.Start(new ProcessStartInfo("shutdown.exe", "/s /t 0") { CreateNoWindow = true });
        }

        public void Screenshot()
        {
            // Opens the Snipping Tool (same as Win+Shift+S)
            Process.Start(new ProcessStartInfo("ms-screenclip:") { UseShellExecute = true });
        }
    }
}
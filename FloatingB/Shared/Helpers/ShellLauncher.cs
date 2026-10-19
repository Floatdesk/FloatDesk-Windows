using System.Diagnostics;
using System.Reflection;

namespace FloatingB.Shared.Helpers
{
    public static class ShellLauncher
    {
        /// <summary>
        /// Opens a program, file or URI the same way the Run box does.
        /// </summary>
        public static void Open(string target)
        {
            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
        }

        /// <summary>
        /// Same as Win+D.
        /// </summary>
        public static void ToggleDesktop()
        {
            var shellType = Type.GetTypeFromProgID("Shell.Application");
            if (shellType == null) return;

            object? shell = Activator.CreateInstance(shellType);
            shellType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, shell, null);
        }
    }
}

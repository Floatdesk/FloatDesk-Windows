using System.Runtime.InteropServices;

namespace FloatingB.Features.Lock.Services
{
    public static class LockService
    {
        public static void Lock() => LockWorkStation();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LockWorkStation();
    }
}

using System.Runtime.InteropServices;

namespace xApt
{
    public static class Shell
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        static extern int system(string command);

        public static void Execute(string cmd) => system(cmd);
    }
}

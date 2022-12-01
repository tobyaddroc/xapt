using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xApt
{
    public unsafe static class Assembler
    {
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(int* lpAddress, uint dwSize, uint flNewProtect, uint* lpflOldProtect);

        public static void* _asm(byte[] code)
        {
            int i = 0;
            int* p = &i;
            p += 0x14 / 4 + 1;
            i = *p;
            fixed (byte* b = code)
            {
                *p = (int)b;
                uint prev;
                VirtualProtect((int*)b, (uint)code.Length, 0x40, &prev);
            }
            return (void*)i;
        }
    }
}

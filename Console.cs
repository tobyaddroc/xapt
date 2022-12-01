using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xApt
{
    public static class Output
    {
        public static void Log() => Console.ForegroundColor = ConsoleColor.DarkCyan;
        public static void Error() => Console.ForegroundColor = ConsoleColor.Red;
        public static void Success() => Console.ForegroundColor = ConsoleColor.Green;
        public static void Warning() => Console.ForegroundColor = ConsoleColor.Yellow;
        public static void ExtraWarning()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Question() => Console.ForegroundColor = ConsoleColor.Gray;
    }
}

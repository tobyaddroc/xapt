using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xApt
{
    public static class CustomScripts
    {
        public static class CSHelper
        {
            private static bool IsPackageExists()
            {
                if (Core.xapt.IsPackageExists("dhelper"))
                    return true;
                return false;
            }
            public static void CallScript()
            {
                if (!IsPackageExists())
                {
                    Output.Error();
                    Console.WriteLine("[-] xApt: Package dcontrol isn't found, please install it");
                    Console.ResetColor();
                    return;
                }
            }
        }

        public static bool IsProtectedPackage(string Package)
        {
            return Package switch
            {
                "cshelper" => true,
                _ => false
            };
        }

        public static bool ProtectedPackages(string? Passwd, string Package)
        {
            return Package switch
            {
                "cshelper" => Passwd == "7319547896321",
                _ => false
            };
        }
    }
}

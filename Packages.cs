using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using xApt.Globals;

namespace xApt.Library
{
    public class Program
    {
        public string PackageName { get; set; }
        public string Version { get; set; }
        public string Link { get; set; }
        public string MainProcess { get; set; }
        public string PostInstallshell { get; set; }

        public Program(string packageName, string version, string link, string mainProcess, string postInstallshell)
        {
            PackageName = packageName;
            Version = version;
            Link = link;
            MainProcess = mainProcess;
            PostInstallshell = postInstallshell;
        }
    }

    public class Packages
    {
        [JsonPropertyName("names")]
        public string[] NameMassive { get; set; }
        [JsonPropertyName("versions")]
        public string[] VersionMassive { get; set; }
        [JsonPropertyName("links")]
        public string[] LinkMassive { get; set; }
        [JsonPropertyName("exenames")]
        public string[] ExeNameMassive { get; set; }
        [JsonPropertyName("postshells")]
        public string[] PostInstallMassive { get; set; }

        public Packages(string[] nm, string[] vm, string[] lm, string[] em, string[] pm)
        {
            NameMassive = nm;
            VersionMassive = vm;
            LinkMassive = lm;
            ExeNameMassive = em;
            PostInstallMassive = pm;
        }

        public static Packages GetPackages() => new(
            PackageManager.DEFAULTNameMassive, 
            PackageManager.DEFAULTVersionMassive, 
            PackageManager.DEFAULTLinkMassive,
            PackageManager.DEFAULTExeNameMassive,
            PackageManager.DEFAULTPostInstallMassive);
    }

    public class PackageManager
    {
        public static string[] DEFAULTNameMassive = {
            "victoria",
            "sharex",
            "cheatengine",
            "hxd",
            "goodbyedpi",
            "kmsauto",
            "dcontrol",
            "cshelper"
        };

        public static string[] DEFAULTVersionMassive = {
            "434",
            "13.5",
            "8.1.2",
            "2.5",
            "0.2.2",
            "1.0",
            "1.0",
            "1.0"
        };

        public static string[] DEFAULTLinkMassive = {
            "https://github.com/tobyaddroc/xapt_library/raw/main/Victoria454.zip",
            "https://github.com/tobyaddroc/xapt_library/raw/main/sharex.zip",
            "https://github.com/tobyaddroc/xapt_library/raw/main/cheatengine.zip",
            "https://github.com/tobyaddroc/xapt_library/raw/main/hxd.zip",
            "https://github.com/tobyaddroc/xapt_library/raw/main/goodbyedpi-0.2.2.zip",
            "https://github.com/tobyaddroc/xapt_library/raw/main/W10DigitalActivation.zip",
            "https://github.com/tobyaddroc/xapt_library/raw/main/dControl.zip",
            "https://github.com/tobyaddroc/xapt_library/raw/main/cshelper.zip"
        };

        public static string[] DEFAULTExeNameMassive = {
            "DirSmart",
            "ShareX",
            "ce812x86_xaptedition",
            "HxD",
            "1_russia_blacklist_dnsredir",
            "W10DigitalActivation",
            "dControl",
            "cshelper"
        };

        public static string[] DEFAULTPostInstallMassive = {
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[0]}\\{DEFAULTExeNameMassive[0]}.exe",
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[1]}\\{DEFAULTExeNameMassive[1]}.exe",
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[2]}\\{DEFAULTExeNameMassive[2]}.exe",
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[3]}\\{DEFAULTExeNameMassive[3]}.exe",
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[4]}\\{DEFAULTExeNameMassive[4]}.cmd",
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[5]}\\{DEFAULTExeNameMassive[5]}.exe",
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[6]}\\{DEFAULTExeNameMassive[6]}.exe",
            "start " + Global.xAptPackageData + $"\\{DEFAULTNameMassive[7]}\\{DEFAULTExeNameMassive[7]}.exe"
        };
    }
}

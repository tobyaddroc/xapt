using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using xApt.Globals;
using xApt.Library;

namespace xApt.Core
{
    public static class xapt
    {
        public enum XRESULT : int
        {
            SUCCESS,
            ENVIRONMENT_NOT_INSTALLED,
            ENVIRONMENT_IS_CURRENTLY_INSTALLED,
            NO_PACKAGES_IN_DATABASE,
            BAD_PACKAGE,
            PACKAGE_ALREADY_EXISTS,
            PACKAGE_IS_NOT_CALLBACKED,
            PACKAGE_NOT_FOUND,
            XAPT_IS_UP_TO_DATE,
            NO_START_CANDIDATE,
            NO_INSTALLATION_CANDIDATE,
            NO_REMOVAL_CANDIDATE,
            SCRIPT_EXCEPTION,
            UPDATE_AVAILABLE,
            NULL,
            CANCELED_BY_USER
        }

        public static XRESULT Init()
        {
            if (!Directory.Exists(Global.xAptDir))
            {
                Directory.CreateDirectory(Global.xAptDir);
                Directory.CreateDirectory(Global.xAptPackageData);
                Directory.CreateDirectory(Global.xAptScripts);
                if (!File.Exists(Global.xAptConfig))
                    Config.InitConfig();
                return XRESULT.SUCCESS;
            }
            return XRESULT.ENVIRONMENT_IS_CURRENTLY_INSTALLED;
        }

        public static bool CheckEnvironment() => Directory.Exists(Global.xAptDir);

        private static bool DeletionWarning()
        {
            Output.ExtraWarning();
            Console.WriteLine("[!!!] xApt: This action will delete all data in the package library");
            Console.ResetColor();
            Output.Question();
            Console.Write("[?] xApt: Are you sure? (y or n): ");
            Console.ResetColor();
            return Console.ReadLine() == "y";
        }

        public static string? DirectoryToPackageName(string Directory) => Path.GetFileName(Directory);

        public static XRESULT CleanPackages()
        {
            if (!Directory.Exists(Global.xAptDir))
                return XRESULT.ENVIRONMENT_NOT_INSTALLED;
            else if (!DeletionWarning())
                return XRESULT.CANCELED_BY_USER;
            else if (Directory.GetDirectories(Global.xAptDir).Length == 0)
                return XRESULT.NO_PACKAGES_IN_DATABASE;
            foreach (string file in Directory.GetDirectories(Global.xAptPackageData))
            {
                try { 
                    UninstallScript(GetLogicCoordinates(DirectoryToPackageName(file)));
                } catch (Exception e) { Console.WriteLine(e); }
            }
            return XRESULT.SUCCESS;
        }
        public static XRESULT RemoveEnvironment()
        {
            if (!Directory.Exists(Global.xAptDir))
                return XRESULT.ENVIRONMENT_NOT_INSTALLED;
            if (CleanPackages() == XRESULT.CANCELED_BY_USER)
                return XRESULT.CANCELED_BY_USER;
            Directory.Delete(Global.xAptDir, true);
            return XRESULT.SUCCESS;
        }

        public static void DrawProgressBar(int Progress)
        {
            int i = 0;
            string ProgressBar = "[";
            for (i = 0; i < (Progress / 2); i++)
                ProgressBar += "o";
            while (ProgressBar.Length < 52)
                ProgressBar += " ";
            Console.Write("[*] xApt: Downloading... " + ProgressBar + "] "+i*2+"%\r");
        }

        public static XRESULT CheckForUpdates()
        {
            if (Global.buildInfo?.Patch_Client != Global.buildInfo?.Patch)
                return XRESULT.UPDATE_AVAILABLE;
            else if (Global.buildInfo?.Version_Client != Global.buildInfo?.Version)
                return XRESULT.UPDATE_AVAILABLE;
            else
                return XRESULT.XAPT_IS_UP_TO_DATE;
        }

        public static bool IsPackageExists(string Package)
        {
            foreach (string file in Directory.GetDirectories(Global.xAptPackageData))
            {
                if (DirectoryToPackageName(file) == Package.ToLower())
                    return true;
            }
            return false;
        }

        public static XRESULT RegisterAutorun(string _Package)
        {
            Packages packages = Packages.GetPackages();
            if (!CheckEnvironment())
                return XRESULT.ENVIRONMENT_NOT_INSTALLED;
            int logiclist = GetLogicCoordinates(_Package);
            if (logiclist == -1 || !IsPackageExists(_Package))
                return XRESULT.PACKAGE_NOT_FOUND;
            string packagename = packages.NameMassive[logiclist];
            string postshell = packages.PostInstallMassive[logiclist];
            string fpostshell = postshell.Replace("start ", null);
            RegistryKey autorun = Registry.CurrentUser;
            RegistryKey autorun_key = autorun.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            autorun_key.SetValue(_Package, $"\"{fpostshell}\"");
            autorun_key.Close();
            return XRESULT.SUCCESS;
        }

        public static XRESULT UnRegisterAutorun(string _Package)
        {
            if (GetLogicCoordinates(_Package) == -1 || !IsPackageExists(_Package))
                return XRESULT.PACKAGE_NOT_FOUND;
            RegistryKey autorun = Registry.CurrentUser;
            RegistryKey autorun_key = autorun.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            try
            {
                autorun_key.DeleteValue(_Package);
            } catch { autorun_key.Close(); return XRESULT.PACKAGE_IS_NOT_CALLBACKED; }
            autorun_key.Close();
            return XRESULT.SUCCESS;
        }

        public static void OpenXAptShell() => Shell.Execute("start xapt_shell.exe");

        private static XRESULT InstallScript(int LogicListCoordinates)
        {
            Packages packages = Packages.GetPackages();
            string hPackageName = packages.NameMassive[LogicListCoordinates];
            string hVersion = packages.VersionMassive[LogicListCoordinates];
            string hLink = packages.LinkMassive[LogicListCoordinates];
            string hProcess = packages.ExeNameMassive[LogicListCoordinates];
            string hPostInstallShell = packages.PostInstallMassive[LogicListCoordinates];
            if (hLink == "_XAPT_NO_LINK_DEFINED")
            {
                Output.Error();
                Console.Write("[-] xApt: " + hPackageName + "has no installation candidate\n");
                return XRESULT.NO_INSTALLATION_CANDIDATE;
            }
            if (Directory.Exists(Global.xAptPackageData + @"\" + hPackageName))
                return XRESULT.PACKAGE_ALREADY_EXISTS;
            Output.Log();
            Console.Write("[*] xApt: Installing {0}\n", hPackageName);
            if (CustomScripts.IsProtectedPackage(hPackageName))
            {
                Output.Question();
                Console.Write("[?] xApt: This package requires a password: ");
                if (!CustomScripts.ProtectedPackages(Console.ReadLine(), hPackageName))
                {
                    Output.Error();
                    Console.WriteLine("[-] xApt: Wrong installation password, aborting");
                    return XRESULT.NULL;
                }
                Output.Success();
                Console.Write("[+] xApt: Password accepted\n");
                Console.ResetColor();
            }
            Directory.CreateDirectory(Global.xAptPackageData + @"\" + hPackageName);
            bool Success = false;
            using (WebClient wc = new())
            {
                wc.DownloadProgressChanged += (s, a) => {
                    DrawProgressBar(a.ProgressPercentage);
                };
                wc.DownloadFileAsync(new Uri(hLink), Global.xAptPackageData + @"\" + hPackageName + @"\" + hPackageName + @".zip");
                wc.DownloadFileCompleted += (s, a) => {
                    if (a.Error != null)
                    {
                        Output.Error();
                        Console.WriteLine("[-] xApt: Some error while downloading {0} package", hPackageName);
                        Console.WriteLine("[-] xApt: Bug report log => {0}", a.Error.ToString());
                        return;
                    }
                    Success = true;
                    Output.Success();
                    Console.WriteLine("[+] xApt: Downloaded                                                                         \r");
                    Output.Log();
                };
                while (!Success)
                    Thread.Sleep(50);
            }
            Console.Write("[*] xApt: Expanding archives...\r");
            ZipFile.ExtractToDirectory(Global.xAptPackageData + @"\" + hPackageName + @"\" + hPackageName + @".zip", Global.xAptPackageData + @"\" + hPackageName);
            Output.Success();
            Console.Write("[+] xApt: Expanded archives      \n");
            Output.Log();
            Console.Write("[*] xApt: Executing post-shell...\r", hPackageName);
            if (hPostInstallShell == "_XAPT_NO_POST_INSTALL")
            {
                Output.Error();
                Console.Write("[-] xApt: " + hPackageName + "has no start candidate\n");
            }

            else
                Shell.Execute(hPostInstallShell);
            Output.Success();
            Console.Write("[+] xApt: Executed post-shell         \n", hPackageName);
            Output.Log();
            Console.Write("[*] xApt: Cleaning...\r");
            Console.ResetColor();
            File.Delete(Global.xAptPackageData + @"\" + hPackageName + @"\" + hPackageName + @".zip");
            return XRESULT.SUCCESS;
        }

        public static string Help(string CMD)
        {
            return CMD switch
            {
                "init" => "The \"xapt init\" command is used to install the xapt environment, initialize the installation location of the packages and create a configuration file, without installing the environment, everything related to packages will not work.",
                "deinit" => "The \"xapt deinit\" command is used to UNinstall the xapt environment, for more info view \"xapt help init\"",
                "install" => "The \"xapt install <package>\" used to install a package from the xapt library if it has a installation candidate, after a successful installation the package will open automatically if it has a run candidate.",
                "remove" => "The \"xapt remove <package>\" used to remove a package you have installed, the package will be closed automatically before removal if it has a candidate for removal.",
                "clean" => "The \"xapt clean\" command cleans up all the packages you have installed. Be careful! You can break some packages if you don't disable them properly.",
                "run" => "The \"xapt run <package>\" used to run a package you have installed if the package has a candidate to run.",
                "directory" => "The \"xapt directory\" opens xapt service directory",
                "reinstall" => "The \"xapt reinstall <package>\" used to reinstall a package, a cast of two commands, remove and install.",
                "packages" => "The \"xapt packages <optional: installed>\" is used to display available packages for installation, the suffix to the command (installed) allows you to see the packages you installed and not installed by you.",
                "version" => "The \"xapt version\" shows you version of xapt",
                "update" => "The \"xapt update\" updates your xapt, if updates are available",
                "repair" => "The \"xapt repair\" reinstalling your xapt, using for fast reinstallation process if you broke xapt",
                "checkupdates" => "The \"xapt checkupdates\" checking xapt for available updates",
                "shell" => "The \"xapt shell\" used to open a command prompt with admin rights, and work comfortably with xapt, since it always requires admin rights",
                "config" => "The \"xapt config\" used to open a config file in custom editor",
                "autorun" => "The \"xapt autorun <register/unregister> <package> used to register the startup program in the registry, to run it along with Windows\"",
                _ => "_XAPT_UNKNOWN_CMD"
            };
        }

        public static XRESULT Install(string _Package)
        {
            if (!Directory.Exists(Global.xAptDir))
                return XRESULT.ENVIRONMENT_NOT_INSTALLED;
            int Coordinate = GetLogicCoordinates(_Package);
            Output.Log();
            Console.Write("[*] xApt: Allocating package\n");
            Console.ResetColor();
            return Coordinate switch
            {
                -1 => XRESULT.BAD_PACKAGE,
                _ => InstallScript(Coordinate),
            };
        }

        public static XRESULT Run(string _Package)
        {
            Packages packages = Packages.GetPackages();
            if (!Directory.Exists(Global.xAptDir))
                return XRESULT.ENVIRONMENT_NOT_INSTALLED;
            Output.Log();
            Console.Write("[*] xApt: Allocating package\n");
            int Coordinate = GetLogicCoordinates(_Package);
            if (!Directory.Exists(Global.xAptPackageData + @"\" + packages.NameMassive[Coordinate]))
            {
                Console.ResetColor();
                return XRESULT.PACKAGE_NOT_FOUND;
            }
            switch (Coordinate)
            {
                case -1:
                    Console.ResetColor();
                    return XRESULT.PACKAGE_NOT_FOUND;
                default:
                    Console.Write("[*] xApt: Starting {0}\r", _Package);
                    Shell.Execute(packages.PostInstallMassive[Coordinate]);
                    Console.ResetColor();
                    return XRESULT.SUCCESS;
            }
        }
        private static XRESULT UninstallScript(int LogicListCoordinates)
        {
            if (LogicListCoordinates == -1)
                return XRESULT.BAD_PACKAGE;
            if (!Directory.Exists(Global.xAptDir))
                return XRESULT.ENVIRONMENT_NOT_INSTALLED;
            Packages packages = Packages.GetPackages();
            string hPackageName = packages.NameMassive[LogicListCoordinates];
            string hVersion = packages.VersionMassive[LogicListCoordinates];
            string hLink = packages.LinkMassive[LogicListCoordinates];
            string hProcess = packages.ExeNameMassive[LogicListCoordinates];
            string hPostInstallShell = packages.PostInstallMassive[LogicListCoordinates];
            if (!Directory.Exists(Global.xAptPackageData + @"\" + hPackageName))
            {
                Console.ResetColor();
                return XRESULT.PACKAGE_NOT_FOUND;
            }
            Output.Log();
            Console.Write("[*] xApt: Killing {0}\r", hPackageName);
            foreach (Process pkgProc in Process.GetProcessesByName(hProcess))
                pkgProc.Kill();
            Output.Success();
            Console.Write("[+] xApt: Killed {0}           \n", hPackageName);
            Output.Log();
            Console.Write("[*] xApt: Uninstalling {0}\r", hPackageName);
            Thread.Sleep(1000);
            Directory.Delete(Global.xAptPackageData + @"\" + hPackageName, true);
            Console.ResetColor();
            return XRESULT.SUCCESS;
        }

        public static int GetLogicCoordinates(string? _PackageName)
        {
            Packages packages = Packages.GetPackages();
            int Coordinate = 0;
            foreach (string LogicName in packages.NameMassive)
            {
                if (LogicName == _PackageName)
                    return Coordinate;
                else
                    Coordinate++;
            }
            return -1;
        }

        public static XRESULT Uninstall(string _Package)
        {
            int LogicCoordinate = GetLogicCoordinates(_Package);
            Output.Log();
            Console.Write("[*] xApt: Allocating package\n");
            return LogicCoordinate switch
            {
                -1 => XRESULT.PACKAGE_NOT_FOUND,
                _ => UninstallScript(LogicCoordinate)
            };
        }
        public static void PackageList()
        {
            Packages packages = Packages.GetPackages();
            Output.Log();
            Console.WriteLine("[*] xApt: Package List:");
            foreach (string pkg in packages.NameMassive)
                Console.WriteLine("[*] => {0}", pkg);
            Console.ResetColor();
        }

        public static void InstalledPackageList()
        {
            Packages packages = Packages.GetPackages();
            Output.Log();
            Console.WriteLine("[*] xApt: Installed/Not installed Package List: ");
            foreach (string src in packages.NameMassive)
            {
                bool pkg = Directory.Exists(Global.xAptPackageData + $@"\{src}");
                Console.ForegroundColor = pkg ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(pkg ? "[+] => " + src : "[-] => " + src);
                Console.ResetColor();
            }
            return;
        }

        public static XRESULT Update(bool Repair)
        {
            if (Repair)
            {
                Console.ResetColor();
                Output.Log();
                Console.Write(
                "[*] xApt: Fixing xapt after crooked handles...\n");
                Console.ResetColor();
                RemoveEnvironment();
                Init();
                Shell.Execute(
                "start powershell.exe -c \"" +
                "(New-Object Net.WebClient).DownloadFile('" +
                Global.buildInfo?.UpdateLink +
                "', " +
                "$env:temp+'\\update.zip');Expand-Archive $($env:temp+'\\update.zip') " +
                "C:\\Windows\\System32 -Force; del $($env:temp+'\\update.zip')");
                Environment.Exit(0);
                return XRESULT.NULL;
            }
            if (!Directory.Exists(Global.xAptDir))
                return XRESULT.ENVIRONMENT_NOT_INSTALLED;
            if (Global.buildInfo?.Patch == Global.buildInfo?.Patch_Client)
                return XRESULT.XAPT_IS_UP_TO_DATE;
            Shell.Execute(
                "start powershell.exe -c \"" +
                "(New-Object Net.WebClient).DownloadFile('" +
                Global.buildInfo?.UpdateLink +
                "', " +
                "$env:temp+'\\update.zip');Expand-Archive $($env:temp+'\\update.zip') " +
                "C:\\Windows\\System32 -Force; del $($env:temp+'\\update.zip')");
            Environment.Exit(0);
            return XRESULT.SUCCESS;
        }
    }
}

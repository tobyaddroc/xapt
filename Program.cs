using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using xApt.Globals;
using xApt.Core;
using System.Net;

namespace xApt
{
    public static class Entry
    {
        static void Main(string[] args)
        {
            if (!Global.xAptBeta)
            {
                if (Global.Cfg.EnableUpdateWarnings)
                {
                    if (xapt.CheckForUpdates() == xapt.XRESULT.UPDATE_AVAILABLE)
                    {
                        Output.Warning();
                        Console.WriteLine("[!] xApt: Your xapt is outdated, please update it to {0} {1}", Global.buildInfo?.Version, Global.buildInfo?.Patch);
                        Console.ResetColor();
                    }
                }
            }
            if (args.Length == 0)
            {
                Output.Log();
                Console.Write(
                    "[*] xApt: Available arguments: \n" +
                    "[*]: init, clean, deinit, install, reinstall, run\n" +
                    "[*]: directory, remove, packages, version, update, repair\n" +
                    "[*]: config, checkupdates, shell, autorun\n" +
                    " ----------- // ----------- // ----------- // ----------- \n" +
                    "[*] xApt: To get help on a command, use: xapt help \"command\"\n");
                Console.ResetColor();
                return;
            }
            if (Global.xAptBeta)
            {
                Output.Warning();
                Console.WriteLine("[!] xApt: You're using unstable beta-version of xApt");
                Console.ResetColor();
            }
            if (args[0] == "init")
            {
                if (xapt.Init() == xapt.XRESULT.SUCCESS)
                {
                    Output.Success();
                    Console.Write(
                        "[+] xApt: Environment installed succesfully\n");
                    Console.ResetColor();
                    return;
                }
                else if (xapt.Init() == xapt.XRESULT.ENVIRONMENT_IS_CURRENTLY_INSTALLED)
                {
                    Output.Error();
                    Console.Write(
                        "[-] xApt: Environment is already installed\n");
                    Console.ResetColor();
                    return;
                }
            }
            else if (args[0] == "script")
            {
                if (args.Length == 1)
                {
                    Output.Error();
                    Console.Write(
                        "[-] xApt: Bad script call arguments\n");
                    Console.ResetColor();
                    return;
                }
                else if (args.Length == 2)
                {
                    switch (aebx.LoadScript(args[1]))
                    {
                        case AEBXRESULT.SCRIPT_NOT_FOUND:
                            Output.Error();
                            Console.Write(
                                "[-] xApt: Script " + args[1] +" not found in library\n");
                            Console.ResetColor();
                            return;
                        case AEBXRESULT.BAD_SCRIPT:
                            Output.Error();
                            Console.Write(
                                "[-] xApt: Script " + args[1] + " returned with code 0x1 (Unable to load, please check your \"?aebx\" label)\n");
                            Console.ResetColor();
                            return;
                        case AEBXRESULT.SUCCESS:
                            Output.Success();
                            Console.Write(
                                "[+] xApt: Script " + args[1] + " returned with code 0x0 (Success)\n");
                            Console.ResetColor();
                            return;
                        case AEBXRESULT.SCRIPT_EXIT:
                            Output.Success();
                            Console.Write(
                                "[+] xApt: Script " + args[1] + " returned with code 0x4 (Script was aborted or ended)\n");
                            Console.ResetColor();
                            return;
                    }
                }
                else
                {
                    Output.Error();
                    Console.Write(
                        "[-] xApt: Bad script call arguments\n");
                    Console.ResetColor();
                    return;
                }
            }
            else if (args[0] == "help")
            {
                string Answer = "_VIEW_DEFAULT_HELP";
                if (args.Length != 1)
                {
                    Answer = xapt.Help(args[1]);
                }
                if (Answer == "_XAPT_UNKNOWN_CMD")
                {
                    Output.Error();
                    Console.WriteLine("[-] xApt: Command not found in handbook");
                    Console.ResetColor();
                    return;
                }
                else if (Answer == "_VIEW_DEFAULT_HELP")
                {
                    Output.Log();
                    Console.Write(
                        "[*] xApt: Available arguments: \n" +
                        "[*]: init, clean, deinit, install, reinstall, run\n" +
                        "[*]: directory, remove, packages, version, update, repair\n" +
                        "[*]: config, checkupdates, shell, autorun\n" +
                        " ----------- // ----------- // ----------- // ----------- \n" +
                        "[*] xApt: To get help on a command, use: xapt help \"command\"\n");
                    Console.ResetColor();
                    return;
                }
                else
                {
                    Output.Success();
                    Console.WriteLine("[+] xApt: Command " + args[1] +" found in handbook");
                    Output.Question();
                    Console.WriteLine("[.] xApt: " + Answer);
                    Console.ResetColor();
                    return;
                }
            }
            else if (args[0] == "autorun")
            {
                if (args.Length == 1 || args.Length > 3)
                {
                    Output.Log();
                    Console.WriteLine("[*] xApt: Usage: xapt autorun <register/unregister> <package>");
                    Console.ResetColor();
                    return;
                }
                else if (args.Length == 2)
                {
                    Output.Log();
                    Console.WriteLine("[*] xApt: Usage: xapt autorun <register/unregister> <package>");
                    Console.ResetColor();
                    return;
                }
                else if (args.Length == 3)
                {
                    if (args[1] == "register")
                    {
                        switch (xapt.RegisterAutorun(args[2]))
                        {
                            case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                                Output.Error();
                                Console.Write(
                                "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                                Console.ResetColor();
                                return;
                            case xapt.XRESULT.PACKAGE_NOT_FOUND:
                                Output.Error();
                                Console.Write(
                                "[-] xApt: Unable to allocate package " + args[2] + "\n[-] xApt: Is package installed?\n");
                                Console.ResetColor();
                                return;
                            case xapt.XRESULT.SUCCESS:
                                Output.Success();
                                Console.Write(
                                "[+] xApt: Registered autorun for " + args[2] + "\n");
                                Console.ResetColor();
                                return;
                        }
                    }
                    else if (args[1] == "unregister")
                    {
                        switch (xapt.UnRegisterAutorun(args[2]))
                        {
                            case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                                Output.Error();
                                Console.Write(
                                "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                                Console.ResetColor();
                                return;
                            case xapt.XRESULT.PACKAGE_NOT_FOUND:
                                Output.Error();
                                Console.Write(
                                "[-] xApt: Unable to allocate package " + args[2] + "\n[-] xApt: Is package installed?");
                                Console.ResetColor();
                                return;
                            case xapt.XRESULT.PACKAGE_IS_NOT_CALLBACKED:
                                Output.Error();
                                Console.Write(
                                "[-] xApt: Unable to find autorun callback of " + args[2] + "\n");
                                Console.ResetColor();
                                return;
                            case xapt.XRESULT.SUCCESS:
                                Output.Success();
                                Console.Write(
                                "[+] xApt: Unregistered autorun for " + args[2] + "\n");
                                Console.ResetColor();
                                return;
                        }
                    }
                    else
                    {
                        Output.Log();
                        Console.WriteLine("[*] xApt: Usage: xapt autorun <register/unregister> <package>");
                        Console.ResetColor();
                        return;
                    }
                }
            }
            /*
            else if (args[0] == "eval")
            {
                if (args.Length < 2)
                {
                    Output.Log();
                    Console.Write(
                        "[*] xApt: Usage: xapt eval <script>\n");
                    Console.ResetColor();
                    return;
                }
                switch (xapt.OpenEval(args[1]))
                {
                    case xapt.XRESULT.PACKAGE_NOT_FOUND:
                        Output.Error();
                        Console.Write(
                            "[-] xApt: Unknown script\n");
                        Console.ResetColor();
                        return;
                    case xapt.XRESULT.SUCCESS:
                        Output.Success();
                        Console.Write(
                        "[+] xApt: Script " + args[1] +" thrown successfully\n");
                        Console.ResetColor();
                        return;
                }
            }*/
            else if (args[0] == "shell")
            {
                xapt.OpenXAptShell();
            }
            else if (args[0] == "clean")
            {
                try
                {
                    switch (xapt.CleanPackages())
                    {
                        case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.NO_PACKAGES_IN_DATABASE:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Nothing to delete\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.CANCELED_BY_USER:
                            Output.Log();
                            Console.Write(
                            "[*] xApt: Canceled by user\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.SUCCESS:
                            Output.Success();
                            Console.Write(
                            "[+] xApt: All packages has been removed\n");
                            Console.ResetColor();
                            return;
                    }
                }
                catch (Exception e)
                {
                    /*ReportBot.Report(ReportBot.XSTATUS.ERROR, (args.Length > 0 ? $"xapt {args[0]}{(args.Length != 1 ? $" {args[1]}" : "")}" : "xapt"), e);*/
                    return;
                }
            }
            else if (args[0] == "deinit")
            {
                try
                {
                    switch (xapt.RemoveEnvironment())
                    {
                        case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.CANCELED_BY_USER:
                            Output.Log();
                            Console.Write(
                            "[*] xApt: Canceled by user\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.SUCCESS:
                            Output.Success();
                            Console.Write(
                            "[+] xApt: Environment successfully removed from your PC\n");
                            Console.ResetColor();
                            return;
                    }
                }
                catch (Exception e)
                {
                    /*ReportBot.Report(ReportBot.XSTATUS.ERROR, (args.Length > 0 ? $"xapt {args[0]}{(args.Length != 1 ? $" {args[1]}" : "")}" : "xapt"), e);*/
                    Output.Error();
                    Console.Write(
                    "[-] xApt: Operation failed. Most likely one or more packages are open now.\n");
                    Console.ResetColor();
                    return;
                }
            }
            else if (args[0] == "install")
            {
                try
                {
                    if (args.Length == 1)
                    {
                        Output.Log();
                        Console.Write(
                            "[*] xApt: Usage: xapt install <package-name>\n");
                        Console.ResetColor();
                        return;
                    }
                    switch (xapt.Install(args[1].ToLower()))
                    {
                        case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.SUCCESS:
                            Output.Success();
                            Console.Write(
                            "[+] xApt: Package {0} was succesfully installed\n", args[1]);
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.BAD_PACKAGE:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Unable to allocate package {0}\n", args[1]);
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.PACKAGE_ALREADY_EXISTS:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Package {0} already exists\n", args[1]);
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.NULL:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: CustomScript engine aborted this operation\n", args[1]);
                            Console.ResetColor();
                            return;
                    }
                }
                catch
                {
                    return;
                }
            }
            else if (args[0] == "config")
            {
                Shell.Execute("start " + Global.xAptConfig);
            }
            else if (args[0] == "run")
            {
                try
                {
                    if (args.Length == 1)
                    {
                        Output.Log();
                        Console.Write(
                            "[*] xApt: Usage: xapt run <package-name>\n");
                        Console.ResetColor();
                        return;
                    }
                    switch (xapt.Run(args[1]))
                    {
                        case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.SUCCESS:
                            Output.Success();
                            Console.Write(
                            "[+] xApt: Package {0} was succesfully started on your PC\n", args[1]);
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.PACKAGE_NOT_FOUND:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Unable to allocate package {0}\n", args[1]);
                            Console.ResetColor();
                            return;
                    }
                }
                catch (Exception e)
                {
                    return;
                }
            }
            else if (args[0] == "directory")
            {
                try
                {
                    if (xapt.CheckEnvironment())
                    {
                        Shell.Execute("start " + Global.xAptDir);
                        Console.ResetColor();
                        return;
                    }
                    Output.Error();
                    Console.Write(
                    "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                    Console.ResetColor();
                    return;
                }
                catch
                {
                    return;
                }
            }
            else if (args[0] == "remove")
            {
                try
                {
                    if (args.Length == 1)
                    {
                        Output.Log();
                        Console.Write(
                            "[*] xApt: Usage: xapt remove <package-name>\n");
                        Console.ResetColor();
                        return;
                    }
                    switch (xapt.Uninstall(args[1].ToLower()))
                    {
                        case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.SUCCESS:
                            Output.Success();
                            Console.Write(
                            "[+] xApt: Package {0} was succesfully removed from your PC\n", args[1]);
                            Console.ResetColor();
                            return;
                        case xapt.XRESULT.PACKAGE_NOT_FOUND:
                            Output.Error();
                            Console.Write(
                            "[-] xApt: Unable to allocate package {0}\n", args[1]);
                            Console.ResetColor();
                            return;
                    }
                }
                catch (Exception e)
                {
                    return;
                }
            }
            else if (args[0] == "reinstall")
            {
                if (args.Length == 1)
                {
                    Output.Log();
                    Console.Write(
                        "[*] xApt: Usage: xapt reinstall <package-name>\n");
                    Console.ResetColor();
                    return;
                }
                Output.Log();
                Console.Write(
                    "[*] xApt: Trying to reinstall {0}\n", args[0]);
                Console.ResetColor();
                switch (xapt.Uninstall(args[1].ToLower()))
                {
                    case xapt.XRESULT.ENVIRONMENT_NOT_INSTALLED:
                        Output.Error();
                        Console.Write(
                        "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                        Console.ResetColor();
                        return;
                    case xapt.XRESULT.SUCCESS:
                        Output.Success();
                        Console.Write(
                        "[+] xApt: Package {0} was succesfully removed from your PC\n", args[1]);
                        Console.ResetColor();
                        break;
                    case xapt.XRESULT.PACKAGE_NOT_FOUND:
                        Output.Error();
                        Console.Write(
                        "[-] xApt: Unable to allocate package {0}\n", args[1]);
                        Console.ResetColor();
                        return;
                }
                if (xapt.Install(args[1].ToLower()) == xapt.XRESULT.SUCCESS)
                {
                    Output.Success();
                    Console.Write(
                    "[+] xApt: Package {0} was succesfully installed\n", args[1]);
                    Console.ResetColor();
                    return;
                }

            }
            else if (args[0] == "packages")
            {
                if (xapt.CheckEnvironment())
                {
                    if (args.Length < 2)
                    {
                        xapt.PackageList();
                        Console.ResetColor();
                        return;
                    }
                    else if (args.Length == 2 && args[1] == "installed")
                    {
                        xapt.InstalledPackageList();
                        return;
                    }
                    else
                    {
                        Output.Error();
                        Console.Write(
                            "[-] xApt: Invalid argument(s)\n");
                        Console.ResetColor();
                        return;
                    }
                }
                Output.Error();
                Console.Write(
                "[-] xApt: Environment is not installed, install it by \"xapt init\"\n");
                Console.ResetColor();
                return;
            }
            else if (args[0] == "version")
            {
                Output.Log();

                Console.Write("[*] xApt: Version: {0} ", Global.buildInfo?.Version_Client);
                if (Global.buildInfo?.Version != Global.buildInfo?.Version_Client)
                {
                    Output.Error();
                    Console.WriteLine(Global.xAptBeta ? "[client version doesn't match with server]" : "[outdated]");
                } 
                else
                {
                    Output.Success();
                    Console.WriteLine(Global.xAptBeta ? "[client version doesn't match with server]" : "[up-to-date]");
                }
                Output.Log();
                Console.Write("[*] xApt: Patch {0} ", Global.buildInfo?.Patch_Client);
                if (Global.buildInfo?.Patch != Global.buildInfo?.Patch_Client)
                {
                    Output.Error();
                    Console.WriteLine(Global.xAptBeta ? "[client patch doesn't match with server]" : "[outdated]");
                }
                else
                {
                    Output.Success();
                    Console.WriteLine(Global.xAptBeta ? "[client patch doesn't match with server]" : "[up-to-date]");
                }
                Output.Log();
                switch (Global.xAptBeta)
                {
                    case true:
                        Console.Write("[*] xApt: Build Type: ");
                        Output.Warning();
                        Console.Write("[beta]");
                        break;
                    case false:
                        Console.Write("[*] xApt: Build Type: ");
                        Output.Success();
                        Console.Write("[stable]");
                        break;
                }

                Console.WriteLine();
                Console.ResetColor();
                return;
            }
            else if (args[0] == "update")
            {
                Output.Error();
                if (xapt.Update(false) == xapt.XRESULT.XAPT_IS_UP_TO_DATE)
                    Console.Write("[-] xApt: xapt is up to date, nothing to update\n");
                Console.ResetColor();
                return;
            }
            else if (args[0] == "checkupdates")
            {
                switch (xapt.CheckForUpdates())
                {
                    case xapt.XRESULT.XAPT_IS_UP_TO_DATE:
                        Output.Log();
                        Console.Write("[*] xApt: No available updates\n");
                        Console.ResetColor();
                        break;
                    case xapt.XRESULT.UPDATE_AVAILABLE:
                        Output.Warning();
                        Console.Write("[!] xApt: Update available\n");
                        Console.ResetColor();
                        break;
                }
                return;
            }
            else if (args[0] == "repair")
            {
                xapt.Update(true);
            }
            else
            {
                Output.Error();
                Console.Write(
                    "[-] xApt: Invalid argument(s)\n");
                Console.ResetColor();
                return;
            }
        }
    }
}

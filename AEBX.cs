using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xApt
{
    public enum AEBXDELEGATES
    {
        console,
        file,
        webclient,
        shell
    }

    public enum AEBXRESULT
    {
        SCRIPT_NOT_FOUND,
        BAD_SCRIPT,
        EXCEPTION_WHILE_LOADING,
        SUCCESS,
        SCRIPT_EXIT
    }

    public class aex
    {
        public string[]? script { get; set; }
        public string? str32 { get; set; }
        public uint? ui32 { get; set; }
        public ulong? ui64 { get; set; }
        public int? si32 { get; set; }
        public long? si64 { get; set; }

        //public aex(string[] _script, string? _str32, uint? _ui32, ulong? _ui64, int? _si32, long? _si64)
    }

    public class aebx
    {
        public enum Offsets : int
        {
            Command = 0x0,
            Type = 0x0001,
            Src = 0x0002,
            Print_Src = 0x0001
        }

        public static bool IsValidScript(aex _aex)
        {
            if (_aex.script[0].Contains("?aebx"))
                return true;
            return false;
        }

        public static bool IsScriptExists(string ScriptName)
        {
            foreach (string file in Directory.GetFiles(Globals.Global.xAptScripts))
            {
                if (Path.GetFileName(file) == ScriptName)
                    return true;
            }
            return false;
        }

        public static aex Mov(string type, object src, aex scriptsrc)
        {
            switch (type)
            {
                case "str32":
                    scriptsrc.str32 = src.ToString();
                    break;
                case "ui32":
                    scriptsrc.ui32 = (uint?)src;
                    break;
                case "ui64":
                    scriptsrc.ui64 = (ulong?)src;
                    break;
                case "si32":
                    scriptsrc.si32 = (int?)src;
                    break;
                case "si64":
                    scriptsrc.si64 = (long?)src;
                    break;
            }
            return scriptsrc;
        }

        public static void Print(object str, aex scriptsrc)
        {
            for (int i = 0; i < 64; i++)
            {
                str = str.ToString().Replace("+str32+", scriptsrc.str32);
                str = str.ToString().Replace("+ui32+", scriptsrc.ui32.ToString());
                str = str.ToString().Replace("+ui64+", scriptsrc.ui64.ToString());
                str = str.ToString().Replace("+si32+", scriptsrc.si32.ToString());
                str = str.ToString().Replace("+si64+", scriptsrc.si64.ToString());
            }
            Console.WriteLine(str);
        }

        public static AEBXRESULT ScriptToByteCode(aex scriptsrc)
        {
            int lines = scriptsrc.script.Length;
            for (int i = 0; i < lines; i++)
            {
                switch (scriptsrc.script[i])
                {
                    case "mov":
                        scriptsrc = Mov(scriptsrc.script[i + (int)Offsets.Type], scriptsrc.script[i + (int)Offsets.Src], scriptsrc);
                        break;
                    case "print":
                        Print(scriptsrc.script[i + (int)Offsets.Print_Src], scriptsrc);
                        break;
                    case "abort":
                        return AEBXRESULT.SCRIPT_EXIT;
                }
            }
            return AEBXRESULT.SUCCESS;
        }

        public static AEBXRESULT RunScript(aex script) => ScriptToByteCode(script);

        public static AEBXRESULT LoadScript(string ScriptName)
        {
            aex _aex = new();
            if (!IsScriptExists(ScriptName))
                return AEBXRESULT.SCRIPT_NOT_FOUND;

            using (StreamReader sr = new(Globals.Global.xAptScripts + @"\" + ScriptName))
            {
                Console.WriteLine(Globals.Global.xAptScripts + @"\" + ScriptName);
                int counter = 0;
                while (!sr.EndOfStream)
                {
                    _aex.script[counter] = sr.ReadLine();
                    counter++;
                }
            }

            if (!IsValidScript(_aex))
                return AEBXRESULT.BAD_SCRIPT;

            return RunScript(_aex);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Nodes;
using System.Net;
using System.Text.Json;
using xApt.Core;
using System.Text.Json.Serialization;

namespace xApt.Globals
{
    public class BuildInfo
    {
        public string Version { get; set; }
        public string Patch { get; set; }
        public string UpdateLink { get; set; }
        public string Version_Client { get; set; }
        public string Patch_Client { get; set; }
        public BuildInfo(string version, string patch, string updateLink)
        {
            Version = version;
            Patch = patch;
            UpdateLink = updateLink;
            Version_Client = "2.1";
            Patch_Client = "2010.1";
        }
    }

    public class Config
    {
        [JsonPropertyName("enablepostshells")]
        public bool EnablePostShells { get; set; }
        [JsonPropertyName("autoupdates")]
        public bool EnableAutoUpdates { get; set; }
        [JsonPropertyName("updatewarns")]
        public bool EnableUpdateWarnings { get; set; }

        public Config(bool enableAutoUpdates, bool enablepostshells, bool enableupdatewarnings)
        {
            EnablePostShells = enablepostshells;
            EnableAutoUpdates = enableAutoUpdates;
            EnableUpdateWarnings = enableupdatewarnings;
        }

        public static Config ParseConfig()
        {
            try
            {
                using StreamReader sr = new (Global.xAptConfig);
                string readen = sr.ReadToEnd();
                sr.Close();
                return JsonSerializer.Deserialize<Config>(readen);
            }
            catch { return new Config(true, true, true); }
        }
        public static void InitConfig()
        {
            using (StreamWriter sw = new(Global.xAptConfig))
            {
                sw.Write(@"{
    ""enablepostshells"": true,
    ""autoupdates"": false,
    ""updatewarns"": true
}");
                sw.Flush();
                sw.Close();
            }
        }
    }

    public static class Global
    {
        private static readonly WebClient webClient = new();
        public static readonly Config Cfg = Config.ParseConfig();

        public static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static readonly string xAptDir = @"C:\xapt";
        public static readonly string xAptPackageData = xAptDir + @"\packages";
        public static readonly string xAptConfig = xAptDir + @"\config.json";
        public static readonly string xAptPackageList = xAptDir + @"\packages.json";
        public static readonly string xAptScripts = xAptDir + @"\scripts";
        
        
        public static readonly BuildInfo? buildInfo = JsonSerializer.Deserialize<BuildInfo>(webClient.DownloadString("https://pastebin.com/raw/2F9cxKfF"));

        public static readonly bool xAptBeta = true;

        public static readonly string[] PCData = {
            Environment.MachineName.ToString(),
            Environment.OSVersion.ToString(),
            Environment.Is64BitProcess ? "x64" : "x86",
            Environment.UserName,
            Guid.NewGuid().ToString(),
            Environment.ProcessorCount.ToString(),
            webClient.DownloadString("https://ifconfig.me/ip")
        };
    }
}

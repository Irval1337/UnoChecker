using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace VkChecker2
{
    public enum BypassMethod { RuCapctha, Proxy, Manual, Continue };

    public class BypassInfo
    {
        public MethodsPriority methodsPriority { get; set; }

        public string ruCaptchaToken { get; set; }
        public List<string> ruCaptchaData { get; set; }

        public List<string> proxyList { get; set; }

        public void LoadSettings()
        {
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\UnoChecker\Bypass.json";
            if (File.Exists(FilePath))
            {
                BypassInfo settingsInfo = JsonConvert.DeserializeObject<BypassInfo>(File.ReadAllText(FilePath));
                this.Clone(settingsInfo);
            }
            else
            {
                BypassInfo bypassInfo = new BypassInfo();
                bypassInfo.methodsPriority = new MethodsPriority() {First = BypassMethod.Continue };
                bypassInfo.ruCaptchaToken = null;
                bypassInfo.ruCaptchaData = new List<string>();
                bypassInfo.proxyList = new List<string>();

                this.Clone(bypassInfo);

                File.WriteAllText(FilePath, JsonConvert.SerializeObject(bypassInfo));
            }
        }

        public void SaveSettings()
        {
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\UnoChecker\Bypass.json";
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }

        public void Clone(BypassInfo bypassInfo)
        {
            methodsPriority = bypassInfo.methodsPriority;
            ruCaptchaToken = bypassInfo.ruCaptchaToken;
            ruCaptchaData = bypassInfo.ruCaptchaData;
            proxyList = bypassInfo.proxyList;
        }

        public BypassInfo()
        {

        }
    }

    public class MethodsPriority
    {
        public BypassMethod? First { get; set; }
        public BypassMethod? Second { get; set; }
        public BypassMethod? Third { get; set; }
    }
}

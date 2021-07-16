using System;
using System.IO;

namespace VkChecker2
{
    public class ProgramSettings
    {
        public SettingsInfo settingsInfo { get; set; }

        public BypassInfo bypassInfo { get; set; }

        public void SaveSettings()
        {
            settingsInfo.SaveSettings();
            bypassInfo.SaveSettings();
        }

        public ProgramSettings()
        {
            string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\UnoChecker\";
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
            settingsInfo = new SettingsInfo();
            settingsInfo.LoadSettings();

            bypassInfo = new BypassInfo();
            bypassInfo.LoadSettings();
        }
    }
}

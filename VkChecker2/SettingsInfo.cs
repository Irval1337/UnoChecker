using System;
using System.IO;
using Newtonsoft.Json;

namespace VkChecker2
{
    public enum LogsFormat { JSON, XML, Text };
    public enum AuthServiceId
    {
        Random = 1, VkAdmin = 6121396, VkAdmin_iOS = 5776857, VkLive = 5256902, VkLiveAndroid = 5676187, Prisma = 5530956, Instagram = 3698024,
        KateMobile = 2685278, VFeed = 4083558, VkMe = 6146827, VkAPI = 3116505
    };
    public enum AuthService
    {
        Random, VkAdmin, VkAdmin_iOS, VkLive, VkLiveAndroid, Prisma, Instagram, KateMobile, VFeed, VkMe, VkAPI 
    };

    public class SettingsInfo
    {
        public LogsFormat logsFormat { get; set; }
        public AuthService authService { get; set; }

        public OwnAuthService ownAuthService { get; set; }

        public string logsPath { get; set; }
        public bool? filterInvalid { get; set; }

        public AdditionalFields additionalFields { get; set; }

        public void LoadSettings()
        {
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\UnoChecker\Settings.json";
            if (File.Exists(FilePath))
            {
                SettingsInfo settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(File.ReadAllText(FilePath));
                this.Clone(settingsInfo);
            }
            else
            {
                SettingsInfo settingsInfo = new SettingsInfo();
                settingsInfo.logsFormat = LogsFormat.JSON;
                settingsInfo.logsPath = @"C:\UnoChecker\Logs";
                settingsInfo.filterInvalid = false;
                settingsInfo.ownAuthService = new OwnAuthService() { isEnabled = false };
                settingsInfo.authService = AuthService.Random;
                settingsInfo.additionalFields = new AdditionalFields
                {
                    AdminGroups = false,
                    City = false,
                    ErrorType = false,
                    FriendsCount = false,
                    Gender = false,
                    HasPhoto = false,
                    IsBanned = false,
                    Name = false,
                    SubsCount = false,
                    Token = false,
                    Url = false
                };

                this.Clone(settingsInfo);

                File.WriteAllText(FilePath, JsonConvert.SerializeObject(settingsInfo));
            }
        }

        public void SaveSettings()
        {
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\UnoChecker\Settings.json";
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }

        public void Clone(SettingsInfo settingsInfo)
        {
            logsFormat = settingsInfo.logsFormat;
            authService = settingsInfo.authService;
            ownAuthService = settingsInfo.ownAuthService;
            logsPath = settingsInfo.logsPath;
            filterInvalid = settingsInfo.filterInvalid;
            additionalFields = settingsInfo.additionalFields;
        }

        public SettingsInfo()
        {

        }
    }

    public class OwnAuthService
    {
        public bool? isEnabled { get; set; }
        public int? appId { get; set; } 
    }

    public class AdditionalFields
    {
        public bool? Url { get; set; }
        public bool? Name { get; set; }
        public bool? Gender { get; set; }
        public bool? HasPhoto { get; set; }
        public bool? City { get; set; }
        public bool? Token { get; set; }
        public bool? IsBanned { get; set; }
        public bool? FriendsCount { get; set; }
        public bool? SubsCount { get; set; }
        public bool? AdminGroups { get; set; }
        public bool? ErrorType { get; set; }
    }
}

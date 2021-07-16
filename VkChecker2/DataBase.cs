using System;
using System.Collections.Generic;
using System.Globalization;

namespace VkChecker2
{
    [Serializable]
    public class DataBase
    {
        public int totalAccounts { get; set; }
        public int validAccounts { get; set; }
        public int invalidAccounts { get; set; }
        public int validProcent { get; set; }

        public UsingResources usingResources { get; set; }

        public List<Account> accounts { get; set; }

        public void ParseText(ref string data)
        {
            string[] formats = new string[1] { "hh:mm:ss" };

            data = data.Replace("\r", "");
            totalAccounts = Convert.ToInt32(GetNext(ref data, "\n"));
            validAccounts = Convert.ToInt32(GetNext(ref data, "\n"));
            invalidAccounts = Convert.ToInt32(GetNext(ref data, "\n"));
            validProcent = Convert.ToInt32(GetNext(ref data, "%"));

            usingResources = new UsingResources()
            {
                threadsCount = Convert.ToInt32(GetNext(ref data, "\n")),
                accountsInThread = Convert.ToInt32(GetNext(ref data, "\n")),
                checkingTime = DateTime.ParseExact(GetNext(ref data, "\n"), formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal),
                proxiesUsed = Convert.ToInt32(GetNext(ref data, "\n")),
                changeProxyTime = Convert.ToInt32(GetNext(ref data, " сек")),
                captchaSolves = Convert.ToInt32(GetNext(ref data, "\n")),
                badSolves = Convert.ToInt32(GetNext(ref data, "\n")),
                useCapcthaTime = Convert.ToInt32(GetNext(ref data, "\n").Substring(4, data.IndexOf(" аккаунта") - 4))
            };

            data = data.Substring(data.IndexOf("[!] Аккаунты:\n") + 14);
            string[] accounts = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (accounts.Length != 0 && this.accounts == null)
                this.accounts = new List<Account>();
            foreach (var account in accounts)
            {
                var info = account.Split(':');
                this.accounts.Add(new Account() { Login = info[0], Password = info[1], Email = info[2],
                PhoneNumber = info[3], AdditionalInfoStr = info[4]});
            }
        }

        public string GetText()
        {
            string time = $"{(usingResources.checkingTime.Hour < 10 ? "0" : string.Empty)}{usingResources.checkingTime.Hour}:{(usingResources.checkingTime.Minute < 10 ? "0" : string.Empty)}{usingResources.checkingTime.Minute}:{usingResources.checkingTime.Second}";
            string accountsString = "";
            foreach (var account in accounts)
                accountsString += $"{account.Login}:{account.Password}:{account.Email}:{account.PhoneNumber}:{account.AdditionalInfoStr}\n";

            return $"[!] Статистика проверенной базы\nВсего аккаунтов: {totalAccounts}\nВалидных: {validAccounts}\nНевалидных: {invalidAccounts}\nПроцент валидности: {validProcent}%"
                + $"\n\n[!] Используемые ресурсы\nКоличество потоков: {usingResources.threadsCount}\nАккаунтов в потоке: {usingResources.accountsInThread}\nВремя проверки: {time}"
                + $"\n\nИспользовано прокси: {usingResources.proxiesUsed}\nЧастота смены: {usingResources.changeProxyTime} сек."
                + $"\n\nИспользовано токенов RuCaptcha: {usingResources.captchaSolves}\nНеверных разгадываний: {usingResources.badSolves}\nЧастота использования: 1 в {usingResources.useCapcthaTime} аккаунта"
                + $"\n\n[!] Аккаунты:\n{accountsString}";
        }

        private string GetNext(ref string data, string val)
        {
            int index = data.IndexOf(": ");
            data = data.Remove(0, index + 2);
            return data.Substring(0, data.IndexOf(val));
        }
    }

    [Serializable]
    public class UsingResources
    {
        public int threadsCount { get; set; }
        public int accountsInThread { get; set; }
        public int proxiesUsed { get; set; }
        public int changeProxyTime { get; set; }
        public int captchaSolves { get; set; }
        public int badSolves { get; set; }
        public int useCapcthaTime { get; set; }
        public DateTime checkingTime { get; set; }
    }
}

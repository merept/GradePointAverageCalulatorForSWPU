using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using MessageUtil;
using MessageBoxResult = System.Windows.Forms.DialogResult;

namespace GradePointAverageCalulatorForSWPU.src.Settings
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        public About() {
            System.Windows.Forms.Application.EnableVisualStyles();
            InitializeComponent();

            CheckUpdate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            CheckUpdate.FlatAppearance.BorderColor = Color.AliceBlue;

            UpdateProcess.Text = "";

            Ver.Content = $"版本 {MainWindow.Version}";
        }

        private void CheckUpdate_Click(object sender, EventArgs e) {
            try {
                var url = "https://gitee.com/merept/GradePointAverageCalulatorForSWPU/raw/master/update.xml";
                using (var web = new WebClient()) {
                    //web.DownloadProgressChanged += About_DownloadProgressChanged;
                    web.DownloadFileCompleted += CheckUpdate_DownloadFileCompleted;
                    web.DownloadFileAsync(new Uri(url), MainWindow.HistoryFilePath + @"\update.xml");
                }
            } catch (Exception ex) {
                //Log.Log(ex, "检查更新时出错");
                Message.ShowError(ex.Message);
            }
        }

        private void CheckUpdate_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            if (e.Error != null) {
                try {
                    if (e.Error.GetType().Name == "WebException") {
                        UpdateProcess.ForeColor = Color.Red;
                        UpdateProcess.Text = "网络连接错误\n  请检查网络配置。";
                        Sleep10Sec();
                    }
                    else {
                        UpdateProcess.ForeColor = Color.Red;
                        UpdateProcess.Text = "未知错误，请稍后重试。";
                        Sleep10Sec();
                    }
                } catch (Exception) {

                }
            }
            else {
                //UpdateProcess.Text = "";
                Update(false);
            }
        }

        /// <summary>
        /// 下载出错时，UpdateProcess 控件出现提示，十秒后自动清除
        /// </summary>
        private async void Sleep10Sec() {
            await Task.Run(() => {
                Thread.Sleep(10000);
                UpdateProcess.ForeColor = Color.Black; //颜色改回黑色
                UpdateProcess.Text = ""; //清空文字
            });
        }

        public void Update(bool isAuto) {
            try {
                var updateConfigPath = MainWindow.HistoryFilePath + @"\update.xml";
                var updateXml = new XmlDocument();
                updateXml.Load(updateConfigPath);
                XmlElement root = updateXml.DocumentElement;
                MainWindow.UpdateDownloading = MainWindow.HistoryFilePath + $@"\update-{root.SelectSingleNode("version").InnerText}.downloading";
                MainWindow.UpdateExePath = MainWindow.HistoryFilePath + $@"\update-{root.SelectSingleNode("version").InnerText}.exe";
                XmlNode version = root.SelectSingleNode("version");
                if (MainWindow.Version != version.InnerText) {
                    var updateInfo = root.SelectSingleNode("updateinfo")
                                            .InnerText
                                            .Replace("\\n", Environment.NewLine);
                    if (Message.ShowYesNoDialog($"检测到新版本是否更新？\n\n当前版本：v{MainWindow.Version}\n\n最新版本：v{version.InnerText}\n\n{updateInfo}", "应用更新") == MessageBoxResult.Yes) {
                        DownloadExe(root, isAuto);
                    }
                }
                else {
                    if (!isAuto) {
                        //Message.ShowInformation("当前已为最新版本！", "应用更新");
                        UpdateProcess.Text = $"当前已为最新版本";
                        Sleep10Sec();
                    }
                }
            } catch (WebException ex) {
                if (Regex.IsMatch(ex.Message, @"未能解析此远程名称+")) {
                    Message.ShowWarning("网络连接错误，请检查网络配置。", "更新失败");
                }
            } catch (Exception ex) {
                //Log.Log(ex, "检查更新时出错");
                Message.ShowError(ex.Message);
            }
        }

        private void DownloadExe(XmlElement root, bool isAuto) {
            XmlNode download = root.SelectSingleNode("download");
            if (File.Exists(MainWindow.UpdateExePath)) {
                InstallUpdate(MainWindow.UpdateExePath);
                return;
            }
            using (var web = new WebClient()) {
                if (!isAuto) {
                    CheckUpdate.Enabled = false;
                }
                web.DownloadProgressChanged += Update_DownloadProgressChanged;
                web.DownloadFileCompleted += Update_DownloadFileCompleted;
                var s = download.InnerText;
                web.DownloadFileAsync(new Uri(download.InnerText), MainWindow.UpdateDownloading);
            }
        }

        private void Update_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            try {
                if (e.Error != null) {
                    if (e.Error.GetType().Name == "WebException") {
                        UpdateProcess.ForeColor = Color.Red;
                        UpdateProcess.Text = "网络连接错误\n  请检查网络配置。";
                        Sleep10Sec();
                    }
                    else {
                        UpdateProcess.ForeColor = Color.Red;
                        UpdateProcess.Text = "未知错误，请稍后重试。";
                        Sleep10Sec();
                    }
                }
                else {
                    UpdateProcess.Text = "";
                    File.Copy(MainWindow.UpdateDownloading, MainWindow.UpdateExePath, true);
                    File.Delete(MainWindow.UpdateDownloading);
                    InstallUpdate(MainWindow.UpdateExePath);
                }
                CheckUpdate.Enabled = true;
            } catch (Exception) {
                InstallUpdate(MainWindow.UpdateExePath);
                return;
            }
        }

        private void Update_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            try {
                UpdateProcess.Text = $"下载更新中... {e.ProgressPercentage:0} %";
            } catch (Exception) {
                return;
            }
        }

        private void InstallUpdate(string path) {
            if (Message.ShowYesNoDialog("是否立即安装更新？", "应用更新") == MessageBoxResult.No)
                return;
            Process.Start(path);
            Environment.Exit(0);
        }

        private void WebsiteUri_Click(object sender, RoutedEventArgs e) {
            Process.Start(WebsiteUri.NavigateUri.ToString());
        }

        private void GithubUri_Click(object sender, RoutedEventArgs e) {
            Process.Start(GithubUri.NavigateUri.ToString());
        }
    }
}

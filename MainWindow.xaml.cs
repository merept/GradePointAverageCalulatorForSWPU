using GradePointAverageCalulatorForSWPU.src.Settings;
using MessageUtil;
//using MerelyLogTool;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;
using MessageBoxResult = System.Windows.Forms.DialogResult;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public static string Version { get; } = Application.ResourceAssembly.GetName().Version.ToString();
        public static string VersionConfigFile { get; } = @"\version.xml";
        public static string UpdateExePath { get; set; } = "";
        public static string UpdateDownloading { get; set; } = "";
        public static bool IsAutoUpdate { get; set; }
        /// <summary>
        /// 版本配置文件
        /// </summary>
        public static XmlDocument Document { get; } = new XmlDocument();
        public static string HistoryFilePath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + 
            @"\GradePointAverageCalulatorForSWPU";
        public static string HistoryFileName { get; set; } = $@"{HistoryFilePath}\{Environment.UserName}.gpa";
        public readonly string helpText = "欢迎来到SWPU平均学分绩点计算器!\n" +
            "\n" +
            $"版本 {Version} 更新 \n" +
            "修复了备份历史纪录时弹出额外窗口的问题\n" +
            "\n" +
            "请在输入框输入您每科的学分及期末成绩，\n" +
            "可直接将教务系统成绩页的全部内容粘贴进输入框，\n" +
            "然后点击输入框右方 ”查看结果“ 按钮查看您的绩点结果\n" +
            "\n" +
            "输入时请严格遵守一下几点:\n" +
            "(直接复制教务系统成绩时不用管这里)\n" +
            "1.以先输入学分再输入成绩的顺序，否侧结果可能会出错\n" +
            "2.每个数据之间需用任何除数字和小数点外的符号进行间隔\n" +
            "3.可以加上每个科目的名称以方便核对，但是科目名称中请\n" +
            "不要包含数字\n" +
            "以下是输入示例:\n" +
            "高数 5 71\n" +
            "大物 3.5 74\n" +
            "电路 5 73\n" +
            "C语言 3.5 81\n";
        public BindingList<History> Histories { get; set; } = new BindingList<History>();
        //public static MerelyLog Log { get; } = new MerelyLog(HistoryFilePath, "GPAC_log", Version, LogMode.XML);


        public MainWindow() {
            System.Windows.Forms.Application.EnableVisualStyles();

            BeforeWindowLoaded();

            if (Message.ShowOKCancelDialog(helpText, "使用前必读!!!") == MessageBoxResult.Cancel)
                Environment.Exit(0);
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing; 
            InitializeComponent();

            BeginCalculate.Font = new Font(BeginCalculate.Font.FontFamily, 10);
            BeginCalculate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            BeginCalculate.FlatAppearance.BorderColor = Color.AliceBlue;
            BeginCalculate.Focus();

            History.Font = new Font(History.Font.FontFamily, 10);
            History.FlatStyle = System.Windows.Forms.FlatStyle.System;

            Backup.Font = new Font(Backup.Font.FontFamily, 10);
            Backup.FlatStyle = System.Windows.Forms.FlatStyle.System;

            RestoreBackup.Font = new Font(RestoreBackup.Font.FontFamily, 10);
            RestoreBackup.FlatStyle = System.Windows.Forms.FlatStyle.System;

            About.Font = new Font(About.Font.FontFamily, 10);
            About.FlatStyle = System.Windows.Forms.FlatStyle.System;

            UpdateProcess.Font = new Font(UpdateProcess.Font.FontFamily, 10);
            UpdateProcess.FlatStyle = System.Windows.Forms.FlatStyle.System;
            UpdateProcess.TextAlign = ContentAlignment.TopCenter;
            UpdateProcess.Text = "";

            AutoCheck.FlatStyle = System.Windows.Forms.FlatStyle.System;
            AutoCheck.Font = new Font(AutoCheck.Font.FontFamily, 10);

            GradesAndPoints.Font = new Font(GradesAndPoints.Font.FontFamily, 13);

            KeyDown += Esc_Key_Down;

            if (File.Exists(UpdateExePath))
                File.Delete(UpdateExePath);
        }

        private void BeforeWindowLoaded() {
            if (!Directory.Exists(HistoryFilePath))
                Directory.CreateDirectory(HistoryFilePath);
            if (!File.Exists(HistoryFilePath + VersionConfigFile))
                CreateVersionConfig();
            Document.Load(HistoryFilePath + VersionConfigFile);
            XmlElement root = Document.DocumentElement;
            XmlNode isAutoUpdate = root.SelectSingleNode("autoupdate");
            IsAutoUpdate = Convert.ToBoolean(isAutoUpdate.InnerText);
            //var str = root.SelectSingleNode("updateinfo").InnerText;
            //str = str.Replace("\\n", Environment.NewLine);
            //Message.ShowInformation($"检测到新版本是否更新？\n\n最新版本：V{Version}\n\n{str}", "test");
            try {
                if (IsAutoUpdate) { //检查更新
                    var url = "https://gitee.com/merept/GradePointAverageCalulatorForSWPU/raw/master/update.xml";
                    using (var web = new WebClient()) {
                        web.DownloadFile(url, HistoryFilePath + @"\update.xml");
                    }
                    new About().Update(true);
                }
            } catch (WebException) {

            } catch (Exception ex) {
                Message.ShowError(ex.Message, "SWPU学分绩点计算器");
            }
        }

        /// <summary>
        /// 使用 BinaryFormatter 来进行二进制加载历史记录文件
        /// </summary>
        private void LoadHistory() {
            var fs = new FileStream(HistoryFileName, FileMode.Open);
            var formatter = new BinaryFormatter();
            Histories = (BindingList<History>)formatter.Deserialize(fs);
            fs.Close();
            if (Histories.Count != 0 && !string.IsNullOrWhiteSpace(Histories.First().LastTime)) { //打开上次关闭时保留的文本
                GradesAndPoints.Text = Histories.First().LastTime;
            }
        }

        /// <summary>
        /// 使用 BinaryFormatter 来进行二进制保存历史记录文件
        /// </summary>
        private void SaveHistory() {
            if (Histories.Count != 0) { //在有历史记录的情况下保存输入框内的文本
                Histories.First().LastTime = GradesAndPoints.Text;
            }
            var fs = new FileStream(HistoryFileName, FileMode.OpenOrCreate, FileAccess.Write);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, Histories);
            fs.Close();
        }

        /// <summary>
        /// 初次启动时创建版本配置文件
        /// </summary>
        private void CreateVersionConfig() {
            try {
                XmlDeclaration declaration = Document.CreateXmlDeclaration("1.0", "UTF-8", "");
                Document.AppendChild(declaration);
                XmlElement config = Document.CreateElement("config");
                XmlElement autoupdate = Document.CreateElement("autoupdate");
                autoupdate.InnerText = "true";
                XmlElement version = Document.CreateElement("version");
                version.InnerText = Version;
                config.AppendChild(autoupdate);
                config.AppendChild(version);
                Document.AppendChild(config);
                Document.Save(HistoryFilePath + VersionConfigFile);
            } catch (Exception) {
                //Log.Log(ex, "创建XML配置文件时出错");
                Message.ShowError("创建XML配置文件时出错");
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            try {
                var args = Environment.GetCommandLineArgs(); //获取命令行传入参数
                if (args.Length > 1) { //通过文件打开的情况下，历史记录文件加载打开的文件
                    Title = HistoryFileName = args[1]; //将标题设为打开文件的完全路径
                    if (args[1].Length > 70) {
                        var str = args[1].Split('\\');
                        Title = $@"{str[0]}\{str[1]}\...\{str.Last()}";
                    }
                }
                if (!File.Exists(HistoryFileName)) {
                    var f = new FileStream(HistoryFileName, FileMode.Create);
                    var fm = new BinaryFormatter();
                    fm.Serialize(f, Histories);
                    f.Close();
                }
                LoadHistory();
                AutoCheck.Checked = Convert.ToBoolean(IsAutoUpdate);
            } catch (WebException ex) {
                if (!Regex.IsMatch(ex.Message, @"未能解析此远程名称+")) //在网络出错时不做任何提示，直接进入程序
                    Message.ShowError(ex.Message, ex.GetType().Name);
            } catch (Exception ex) {
                //Log.Log(ex, "窗口加载时出错");
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e) {
            try {
                SaveHistory();
                XmlElement root = Document.DocumentElement;
                XmlNode isAutoUpdate = root.SelectSingleNode("autoupdate");
                isAutoUpdate.InnerText = IsAutoUpdate.ToString();
                XmlNode version = root.SelectSingleNode("version");
                version.InnerText = Version;
                Document.Save(HistoryFilePath + VersionConfigFile);
                Environment.Exit(0);
            } catch (Exception ex) {
                //Log.Log(ex, "窗口关闭时出错");
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        /// <summary>
        /// 使用 MessageBox 显示结果弹窗
        /// </summary>
        /// <param name="gpa">学分绩点对象，用于获取结果</param>
        /// <returns>用户点击了弹窗的 OK 即返回true，否则返回false</returns>
        public static bool MessageBoxShow(GradePointAverage gpa) {
            var result = $"您本学期成绩如下\n" +
                         $"总修读学分: {gpa.TotalPoint}\n" +
                         $"已通过学分: {gpa.TotalNotFailedPoint}\n" +
                         $"不及格科目数: {gpa.Fails}\n" +
                         $"平均学分绩点: {gpa.Result:0.00}";
            var messageBoxResult = Message.ShowOKCancelDialog($"{result}", "结果");
            return messageBoxResult == MessageBoxResult.OK;
        }

        /// <summary>
        /// 只显示成绩的结果详情窗口
        /// </summary>
        /// <param name="dataMatches">成绩的匹配项集合</param>
        private void ShowResult(MatchCollection dataMatches) {
            var gpa = new GradePointAverage();
            for (int i = 0; i < dataMatches.Count; i++)
                gpa.Add(Convert.ToDouble(dataMatches[i].ToString()), Convert.ToDouble(dataMatches[++i].ToString()));
            var history = new History(gpa);
            if (!Histories.Contains(history)) //检测历史记录是否已存在
                Histories.Add(history);
            if (MessageBoxShow(gpa)) {
                new ResultWindows(Histories.Last().GradePointAverage, Histories, Histories.IndexOf(Histories.Last())).Show();
            } else return;
        }

        /// <summary>
        /// 显示成绩和学科名字的结果详情窗口
        /// </summary>
        /// <param name="dataMatches">成绩的匹配项集合</param>
        /// <param name="nameMatches">学科名字的匹配项集合</param>
        private void ShowResult(MatchCollection dataMatches, MatchCollection nameMatches) {
            var gpa = new GradePointAverage();
            for (int i = 0, j = 0; i < dataMatches.Count; i++, j++)
                gpa.Add(nameMatches[j].ToString(), Convert.ToDouble(dataMatches[i].ToString()), Convert.ToDouble(dataMatches[++i].ToString()));
            var history = new History(gpa);
            if (!Histories.Contains(history))
                Histories.Add(history);
            if (MessageBoxShow(gpa)) {
                new ResultWindowWithNames(Histories.Last().GradePointAverage, Histories, Histories.IndexOf(Histories.Last())).Show();
            } else return;
        }

        private bool IsNotCount(string first, string name) {
            return Regex.IsMatch(name, @"英语实践+") || //”英语实践“课程不算在内
                   Regex.IsMatch(name, @"全国英语+") || //四六级成绩不算在内
                   first.Substring(0, 1) == "0"; //根据现有经验，课程号开头为0为选修课，不算在内
        }

        /// <summary>
        /// 判断是否为缓考科目
        /// </summary>
        /// <param name="first">下一排第一项，若不是10位课程号即这一排有第12位， 即特殊情况位</param>
        /// <param name="grade">这一排的成绩位，缓考时成绩为0</param>
        /// <returns></returns>
        private bool IsDeferredExam(string first, string grade) {
            if (grade == "0") {
                if (!Regex.IsMatch(first, @"\d{10}")) {
                    if (first == "缓考") {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 纯复制粘贴时的结果显示方法
        /// </summary>
        /// <param name="datas">已经分割好数据的数组</param>
        /// <param name="count">每一科的数据的个数，以便识别从哪里复制</param>
        private void ShowResult(string[] datas, int count) {
            var gpa = new GradePointAverage();
            count = count == 6 ? 7 : count; //从”全部成绩“处复制来的数据计数有可能出现错误
            int gradeIndex = count == 7 ? 6 : 9, //成绩所在位置，不定期检查教务系统的表格，若位置有更改，在此处修改
                pointIndex = 4, //学分所在位置
                nameIndex = 2; //学科名称所在位置
            try {
                for (int i = 0; i < datas.Length - 1; i += count) {
                    if (IsNotCount(datas[i], datas[i + nameIndex])) 
                        continue;
                    if (IsDeferredExam(datas[i + count], datas[i + gradeIndex])) {
                        i++;
                        continue;
                    }
                    if (!Regex.IsMatch(datas[i], @"\d{10}"))
                        i++;
                    gpa.Add(datas[i + nameIndex], Convert.ToDouble(datas[i + pointIndex]), Convert.ToDouble(datas[i + gradeIndex]));
                }
            } catch (Exception ex) {
                //Log.Log(ex, "计算结果时出错");
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
            var history = new History(gpa);
            if (!Histories.Contains(history))
                Histories.Add(history);
            if (MessageBoxShow(gpa)) {
                new ResultWindowWithNames(Histories.Last().GradePointAverage, Histories, Histories.IndexOf(Histories.Last())).Show();
            } else return;
        }

        /// <summary>
        /// 判断是否是纯复制
        /// </summary>
        /// <returns>纯复制，返回 true；否则，返回 false</returns>
        private bool IsJustCopy() {
            var str = GradesAndPoints.Text.Substring(0, 10);
            if (Regex.IsMatch(str, @"\d{10}")) { //如果是纯复制，开头是为是纯数字（课程号），以此判断
                return true;
            } else return false;
        }

        private void BeginCalculate_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(GradesAndPoints.Text)) {
                MessageBox.Show("请输入内容!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsJustCopy()) {
                var s = Regex.Replace(GradesAndPoints.Text, @"\r\n+", ""); //去掉所有的换行
                s = Regex.Replace(s, @"\u0020\u0020\u0020\u0020+", "\u0020\u0020"); //IE下复制的文本有四个/u0020空格，全部替换为两个方便后面分割
                s = Regex.Replace(s, @"成绩明细+", ""); //Edge下复制的文本带有”成绩名细“，需删掉
                var strs = Regex.Split(s, @"\u0020\u0020+"); //将文本按两个空格分割
                var count = 0;
                for (int i = 0; i < strs.Length; i++, count++) { //计第一行涉及的数据个数，以此判断从哪里复制的
                    if (Regex.IsMatch(strs[i], @"\d{10}") && i != 0) break;
                }
                if (count <= 1) { //个数小于1个说明是在Edge下复制的
                    strs = Regex.Split(s, @"	+"); //Edge下复制过来的空格不是两个/u0020，而是一个未查询到的空格，重新分割
                    var listT = strs.ToList(); //Edge下每一行去掉\r\n后剩下的两个数据间隔是/u0020，会导致分割错误，转为List以便重组数组
                    count = 0;
                    for (int i = 0; i < listT.Count; i++) { //重组数组
                        if (Regex.IsMatch(listT[i], @"\d{2}.\d\u0020\d{10}")) { 
                            var str = Regex.Split(listT[i], @"\u0020");
                            listT[i] = str[0];
                            listT.Insert(i + 1, str[1]);
                        }
                    }
                    for (int i = 0; i < listT.Count; i++, count++) //重新计数
                        if (Regex.IsMatch(listT[i], @"\d{10}") && i != 0) break;
                    //Message.ShowInformation(count.ToString(), "");
                    ShowResult(listT.ToArray(), count); //显示结果
                } else {
                    ShowResult(Regex.Split(s, @"\u0020\u0020+"), count);
                }
            } else { //普通输入模式
                var dataMatches = Regex.Matches(GradesAndPoints.Text, @"\d+\.*\d*"); //匹配成绩和学分
                var nameMatches = Regex.Matches(GradesAndPoints.Text, @"[a-zA-z\u4e00-\u9fa5]+"); //匹配学科名字
                if (nameMatches.Count == 0 || nameMatches.Count != dataMatches.Count / 2) { //名字和成绩数量必须完全一致才可以显示名字
                    ShowResult(dataMatches);
                } else ShowResult(dataMatches, nameMatches);
            }
        }

        private void History_Click(object sender, EventArgs e) {
            new HistoryWindow(this).ShowDialog();
        }

        private void Backup_Click(object sender, EventArgs e) {
            //new Backup().Show();
            var dlg = new SaveFileDialog() {
                FileName = Environment.UserName,
                DefaultExt = ".gpa",
                Filter = "GPAC历史文件 | *.gpa"
            };
            var result = dlg.ShowDialog();
            if (result == true) {
                SaveHistory();
                File.Copy(HistoryFileName, dlg.FileName, true);
                Message.ShowInformation("备份成功！", "备份记录");
            }
        }

        private void RestoreBackup_Click(object sender, EventArgs e) {
            var dlg = new OpenFileDialog() {
                DefaultExt = ".gpa",
                Filter = "GPAC历史文件 | *.gpa"
            };
            var result = dlg.ShowDialog();
            if (result == true) {
                if (Message.ShowYesNoCancelDialog("该操作将覆盖原历史记录\n是否继续？", "恢复备份") == MessageBoxResult.Yes) {
                    File.Copy(dlg.FileName, HistoryFileName,  true);
                    Message.ShowInformation("恢复成功！", "恢复记录");
                    LoadHistory();
                }
            }
        }

        private void About_Click(object sender, EventArgs e) {
            new About().Show();
        }

        private void AutoCheck_CheckedChanged(object sender, EventArgs e) {
            IsAutoUpdate = AutoCheck.Checked;
        }
    }
}

using MessageUtil;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxResult = System.Windows.Forms.DialogResult;
using System.Drawing;
using MerelyLogTool;
using System.Collections.Generic;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public static string Version { get; } = "V0.5";
        public static string HistoryFilePath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + 
            @"\GradePointAverageCalulatorForSWPU\";
        public static string HistoryFileName { get; } = $"\\{Environment.UserName}.gpa";
        public readonly string helpText = "欢迎来到SWPU平均学分绩点计算器!\n" +
            "\n" +
            "2022.5.27更新 version 0.5\n" +
            "重大功能更新：\n" +
            "现在可以直接把教务系统成绩页里的全部内容复制过来，\n" +
            "粘贴好后直接点击 “开始计算” 即可获得结果，无需再做更改\n" +
            "以前的方法依然可用\n" +
            "\n" +
            "2022.5.7更新 version 0.4.4\n" +
            "1.新增了异常日志记录（使用 MereyLog 进行记录）\n" +
            "\n" +
            "请在输入框输入您每科的学分及期末成绩，\n" +
            "可直接将教务系统成绩页的全部内容粘贴进输入框，\n" +
            "然后点击输入框下方 ”开始计算“ 按钮进行计算\n";
            //"输入时请严格遵守一下几点:\n" +
            //"1.以先输入学分再输入成绩的顺序，否侧结果可能会出错\n" +
            //"2.每个数据之间需用任何除数字和小数点外的符号进行间隔\n" +
            //"3.可以加上每个科目的名称以方便核对，但是科目名称中请不要包含数字\n" +
            //"以下是输入示例:\n" +
            //"高数 5 71\n" +
            //"大物 3.5 74\n" +
            //"电路 5 73\n" +
            //"C语言 3.5 81\n";
        public BindingList<History> Histories { get; set; } = new BindingList<History>();
        public static MerelyLog Log { get; } = new MerelyLog(HistoryFilePath, "GPAC_log", Version, LogMode.XML);


        public MainWindow() {
            System.Windows.Forms.Application.EnableVisualStyles();
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
            GradesAndPoints.Font = new Font(GradesAndPoints.Font.FontFamily, 13);
            KeyDown += Esc_Key_Down;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            try {
                if (!Directory.Exists(HistoryFilePath))
                    Directory.CreateDirectory(HistoryFilePath);
                if (!File.Exists(HistoryFilePath + HistoryFileName)) {
                    var f = new FileStream(HistoryFilePath + HistoryFileName, FileMode.Create);
                    var fm = new BinaryFormatter();
                    fm.Serialize(f, Histories);
                    f.Close();
                }
                var fs = new FileStream(HistoryFilePath + HistoryFileName, FileMode.Open);
                var formatter = new BinaryFormatter();
                Histories = (BindingList<History>)formatter.Deserialize(fs);
                fs.Close();
                if (Histories.Count != 0 && !string.IsNullOrWhiteSpace(Histories.First().LastTime)) {
                    GradesAndPoints.Text = Histories.First().LastTime;
                }
            } catch (Exception ex) {
                Log.Log(ex, "窗口加载时出错");
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e) {
            try {
                if (Histories.Count != 0) {
                    Histories.First().LastTime = GradesAndPoints.Text;
                }
                var fs = new FileStream(HistoryFilePath + HistoryFileName, FileMode.OpenOrCreate, FileAccess.Write);
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, Histories);
                fs.Close();
            } catch (Exception ex) {
                Log.Log(ex, "窗口关闭时出错");
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        public static bool MessageBoxShow(GradePointAverage gpa) {
            var result = $"您本学期成绩如下\n" +
                         $"总修读学分: {gpa.TotalPoint}\n" +
                         $"已通过学分: {gpa.TotalNotFailedPoint}\n" +
                         $"不及格科目数: {gpa.Fails}\n" +
                         $"平均学分绩点: {gpa.Result:0.00}";
            var messageBoxResult = Message.ShowOKCancelDialog($"{result}", "结果");
            return messageBoxResult == MessageBoxResult.OK;
        }

        private void ShowResult(MatchCollection dataMatches) {
            var gpa = new GradePointAverage();
            for (int i = 0; i < dataMatches.Count; i++)
                gpa.Add(Convert.ToDouble(dataMatches[i].ToString()), Convert.ToDouble(dataMatches[++i].ToString()));
            var history = new History(gpa);
            if (!Histories.Contains(history))
                Histories.Add(history);
            if (MessageBoxShow(gpa)) {
                new ResultWindows(Histories.Last().GradePointAverage, Histories, Histories.IndexOf(Histories.Last())).Show();
            } else return;
        }

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

        private void ShowResult(string[] datas, int count) {
            var gpa = new GradePointAverage();
            int gradeIndex = count == 7 ? 6 : 9, 
                pointIndex = 4, 
                nameIndex = 2;
            try {
                for (int i = 0; i < datas.Length - 1; i += count) {
                    if (Regex.IsMatch(datas[i + nameIndex], @"英语实践+") || 
                        Regex.IsMatch(datas[i + nameIndex], @"全国英语+") || 
                        datas[i].Substring(0, 2) == "00") continue;
                    gpa.Add(datas[i + nameIndex], Convert.ToDouble(datas[i + pointIndex]), Convert.ToDouble(datas[i + gradeIndex]));
                }
            } catch (Exception ex) {
                Log.Log(ex, "计算结果时出错");
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
            var history = new History(gpa);
            if (!Histories.Contains(history))
                Histories.Add(history);
            if (MessageBoxShow(gpa)) {
                new ResultWindowWithNames(Histories.Last().GradePointAverage, Histories, Histories.IndexOf(Histories.Last())).Show();
            } else return;
        }

        private bool IsJustCopy(out int l) {
            var s = GradesAndPoints.Text.Split('\n');
            var a = s[0];
            l = Regex.Split(s[0], @"\u0020\u0020+").Length - 1;
            l = l == 6 ? 7 : l;
            return l == 7 || l == 11 || l == 12;
        }

        private void BeginCalculate_Click(object sender, EventArgs e) {
            int count;
            if (string.IsNullOrWhiteSpace(GradesAndPoints.Text)) {
                MessageBox.Show("请输入内容!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsJustCopy(out count)) {
                var s = Regex.Replace(Regex.Replace(GradesAndPoints.Text, @"\r\n+", ""), @"\u0020\u0020\u0020\u0020+", "\u0020\u0020");
                ShowResult(Regex.Split(s, @"\u0020\u0020+"), count);
            } else {
                var dataMatches = Regex.Matches(GradesAndPoints.Text, @"\d+\.*\d*");
                var nameMatches = Regex.Matches(GradesAndPoints.Text, @"[a-zA-z\u4e00-\u9fa5]+");
                if (nameMatches.Count == 0 || nameMatches.Count != dataMatches.Count / 2) {
                    ShowResult(dataMatches);
                } else ShowResult(dataMatches, nameMatches);
            }
        }

        private void History_Click(object sender, EventArgs e) {
            new HistoryWindow(this).ShowDialog();
        }
    }
}

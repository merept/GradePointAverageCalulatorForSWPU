using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public static string HistoryFilePath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + 
            @"\GradePointAverageCalulatorForSWPU\history";
        public string HistoryReaded { get; set; }
        public readonly string helpText = "欢迎来到SWPU平均学分绩点计算器!\n" +
            "\n" +
            "2022.1.20更新 version 0.3.3\n" +
            "1.增加历史记录功能，可快捷查看历史记录\n" +
            "\n"+
            "请在输入框输入您每科的学分及期末成绩，并点击输入框下方 ”开始计算“ 按钮进行计算\n" +
            "输入时请严格遵守一下几点:\n" +
            "1.以先输入学分再输入成绩的顺序，否侧结果可能会出错\n" +
            "2.每个数据之间需用任何除数字和小数点外的符号进行间隔\n" +
            "3.可以加上每个科目的名称以方便核对，但是科目名称中请不要包含数字\n" +
            "以下是输入示例:\n" +
            "高数 5 71\n" +
            "大物 3.5 74\n" +
            "电路 5 73\n" +
            "C语言 3.5 81\n";

        public MainWindow() {
            if (MessageBox.Show(helpText, "使用前必读!!!", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) == MessageBoxResult.Cancel)
                Environment.Exit(0);
            HistoryReaded = "";
            InitializeComponent();
        }

        private bool MessageBoxShow(GradePointAverage gpa) {
            var result = $"您本学期成绩如下\n" +
                         $"总修读学分: {gpa.TotalPoint}\n" +
                         $"已通过学分: {gpa.TotalNotFailedPoint}\n" +
                         $"不及格科目数: {gpa.Fails}\n" +
                         $"平均学分绩点: {gpa.Result:0.00}";
            var messageBoxResult = MessageBox.Show($"{result}", "结果", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            return messageBoxResult == MessageBoxResult.OK;
        }

        private void ShowResult(MatchCollection dataMatches) {
            var gpa = new GradePointAverage();
            for (int i = 0; i < dataMatches.Count; i++)
                gpa.Add(Convert.ToDouble(dataMatches[i].ToString()), Convert.ToDouble(dataMatches[++i].ToString()));
            if (MessageBoxShow(gpa)) {
                new ResultWindows(gpa).Show();
            } else return;
        }

        private void ShowResult(MatchCollection dataMatches, MatchCollection nameMatches) {
            var gpa = new GradePointAverage();
            for (int i = 0, j = 0; i < dataMatches.Count; i++, j++)
                gpa.Add(nameMatches[j].ToString(), Convert.ToDouble(dataMatches[i].ToString()), Convert.ToDouble(dataMatches[++i].ToString()));
            if (MessageBoxShow(gpa)) {
                new ResultWindowWithNames(gpa).Show();
            } else return;
        }

        private void Write() {
            try {
                var sw = new StreamWriter($"{HistoryFilePath}\\{DateTime.Now:yyyy-MM-dd HH.mm.ss}.txt");
                sw.WriteLine(GradesAndPoints.Text);
                sw.Flush();
                sw.Close();
            } catch (IOException) {
                Directory.CreateDirectory(HistoryFilePath);
                Write();
            }
        }

        private void BeginCalculate_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(GradesAndPoints.Text)) {
                MessageBox.Show("请输入内容!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dataMatches = Regex.Matches(GradesAndPoints.Text, @"\d+\.*\d*");
            var nameMatches = Regex.Matches(GradesAndPoints.Text, @"[a-zA-z\u4e00-\u9fa5]+");
            if (nameMatches.Count == 0 || nameMatches.Count != dataMatches.Count / 2) {
                ShowResult(dataMatches);
            } else ShowResult(dataMatches, nameMatches);
            if (HistoryReaded != GradesAndPoints.Text || string.IsNullOrEmpty(HistoryReaded)) {
                Write();
                HistoryReaded = GradesAndPoints.Text;
            }
        }

        private void History_Click(object sender, RoutedEventArgs e) {
            new History(this).ShowDialog();
        }
    }
}

using MessageUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class History : Window {
        private MainWindow Main { get; }
        private List<string> NoHistory { get; } = new List<string>();

        public History(MainWindow main) {
            InitializeComponent();
            KeyDown += Esc_Key_Down;
            Main = main;
            NoHistory.Add("暂无历史记录");
            if (!Directory.Exists(MainWindow.HistoryFilePath)) {
                Directory.CreateDirectory(MainWindow.HistoryFilePath);
            } else {
                var fileNames = new DirectoryInfo(MainWindow.HistoryFilePath).GetFiles();
                if (fileNames.Length == 0) {
                    Historys.ItemsSource = NoHistory;
                    Clear.IsEnabled = false;
                } else Historys.ItemsSource = fileNames;
            }
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (Historys.SelectedItem == null || Historys.SelectedItem.ToString() == "暂无历史记录")
                return;
            try {
                var sr = new StreamReader($"{MainWindow.HistoryFilePath}\\{Historys.SelectedItem}");
                var sb = new StringBuilder();
                var line = "";
                while ((line = sr.ReadLine()) != null)
                    sb.AppendLine(line);
                Main.GradesAndPoints.Text = sb.ToString();
                Close();
                Main.HistoryReaded = sb.ToString();
                Main.BeginCalculate.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            } catch (Exception ex) {
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            if (Historys.Items[0].ToString() == "暂无历史记录")
                return;
            if (Message.ShowYesNoCancelDialog("是否清空历史记录?", "警告", MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try {
                var fileFullNames = Directory.GetFiles(MainWindow.HistoryFilePath);
                foreach (var file in fileFullNames)
                    File.Delete(file);
                Main.HistoryReaded = "";
                Close();
            } catch (Exception ex) {
                Message.ShowError(ex.Message, ex.GetType().Name);
            }
        }
    }
}

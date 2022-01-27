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
            Main = main;
            NoHistory.Add("暂无历史记录");
            if (!Directory.Exists(MainWindow.HistoryFilePath)) {
                Directory.CreateDirectory(MainWindow.HistoryFilePath);
            } else {
                var fileNames = new DirectoryInfo(MainWindow.HistoryFilePath).GetFiles();
                if (fileNames.Length == 0) Historys.ItemsSource = NoHistory;
                else Historys.ItemsSource = fileNames;
            }
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
                Main.HistoryReaded = sb.ToString();
                Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            if (Historys.Items[0].ToString() == "暂无历史记录")
                return;
            if (MessageBox.Show("是否清空历史记录?", "警告", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try {
                var fileFullNames = Directory.GetFiles(MainWindow.HistoryFilePath);
                foreach (var file in fileFullNames)
                    File.Delete(file);
                Main.HistoryReaded = "";
                Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

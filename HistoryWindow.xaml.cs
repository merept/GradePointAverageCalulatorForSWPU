using MessageUtil;
using System.Windows;
using System.Windows.Input;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryWindow : Window {
        private MainWindow Main { get; }

        public HistoryWindow(MainWindow main) {
            InitializeComponent();
            KeyDown += Esc_Key_Down;
            Main = main;
            if (main.Histories.Count == 0) {
                Clear.IsEnabled = false;
            }
            Historys.ItemsSource = main.Histories;
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (Historys.SelectedItem == null || Historys.SelectedItem.ToString() == "暂无历史记录")
                return;
            Close();
            var gpa = Main.Histories[Historys.SelectedIndex].GradePointAverage;
            if (string.IsNullOrEmpty(gpa.GradesAndPoints[0].Name)) {
                if (MainWindow.MessageBoxShow(gpa)) {
                    new ResultWindows(gpa, Main.Histories[Historys.SelectedIndex]).Show();
                } else return;
            } else {
                if (MainWindow.MessageBoxShow(gpa)) {
                    new ResultWindowWithNames(gpa, Main.Histories[Historys.SelectedIndex]).Show();
                } else return;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            if (Historys.Items.Count == 0)
                return;
            if (Message.ShowYesNoCancelDialog("是否清空历史记录?", "警告", MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            Main.Histories.Clear();
            Clear.IsEnabled = false;
        }

        private void Rename_Click(object sender, RoutedEventArgs e) {
            new HistoryRenameWindow(Main.Histories, Historys.SelectedIndex).ShowDialog();
        }
    }
}

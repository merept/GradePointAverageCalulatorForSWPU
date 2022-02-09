using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// HistoryRenameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryRenameWindow : Window {
        private BindingList<History> Histories { get; }
        private int Index { get; set; }

        public HistoryRenameWindow(BindingList<History> histories, int index) {
            Histories = histories;
            Index = index;
            KeyDown += Esc_Key_Down;
            InitializeComponent();
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Rename_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(Name.Text)) {
                return;
            } else {
                var history = Histories[Index];
                Histories.Remove(history);
                history.HistoryName = Name.Text;
                history.UpdateTime = $"{DateTime.Now}";
                Histories.Insert(Index, history);
            }
            Close();
        }
    }
}

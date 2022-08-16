using System;
using System.ComponentModel;
using System.Drawing;
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
            KeyDown += Enter_Key_Down;

            InitializeComponent();

            Name.Text = Histories[Index].HistoryName;
            Name.Focus();
            Name.SelectAll();

            Rename.Font = new Font(Rename.Font.FontFamily, 8);
            Rename.FlatStyle = System.Windows.Forms.FlatStyle.System;
            Rename.FlatAppearance.BorderColor = Color.AliceBlue;
        }

        private void Enter_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter)
                Rename_Click(null, null);
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Rename_Click(object sender, EventArgs e) {
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

        private void Name_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(Name.Text)) {
                Rename.Enabled = true;
            } else Rename.Enabled = false;
        }
    }
}

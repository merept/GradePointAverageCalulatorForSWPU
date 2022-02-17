using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Message = MessageUtil.Message;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;
using MessageBoxResult = System.Windows.Forms.DialogResult;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryWindow : Window {
        private MainWindow Main { get; }
        private ContextMenuStrip MenuStrip { get; set; } = new ContextMenuStrip();

        public HistoryWindow(MainWindow main) {
            InitializeComponent();
            Historys.Columns.Add("", 0, System.Windows.Forms.HorizontalAlignment.Center);
            Historys.Columns.Add("名称", 200, System.Windows.Forms.HorizontalAlignment.Center);
            Historys.Columns.Add("修改时间", 360, System.Windows.Forms.HorizontalAlignment.Center);
            Clear.Font = new Font(Clear.Font.FontFamily, 8);
            Clear.FlatStyle = FlatStyle.System;
            Clear.FlatAppearance.BorderColor = Color.AliceBlue;
            KeyDown += Esc_Key_Down;
            Main = main;
            var delete = new ToolStripMenuItem("删除");
            delete.Click += Delete_Click;
            var rename = new ToolStripMenuItem("重命名");
            rename.Click += Rename_Click;
            MenuStrip.Items.Add(delete);
            MenuStrip.Items.Add(rename);
            Historys.MouseClick += Historys_MouseClick;
            SetListView();
            if (main.Histories.Count == 0) {
                Clear.Enabled = false;
            }
        }

        private void Historys_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                MenuStrip.Show(Historys, e.Location);
            }
        }

        private void SetListView() {
            Historys.Items.Clear();
            if (Main.Histories.Count != 0) {
                foreach (var history in Main.Histories) {
                    var item = new ListViewItem {
                        Tag = history
                    };
                    item.SubItems.Add(history.HistoryName);
                    item.SubItems.Add(history.UpdateTime);
                    Historys.Items.Add(item);
                }
            }
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ListBox_MouseDoubleClick(object sender, EventArgs e) {
            var index = Historys.SelectedItems[0].Index;
            if (index < 0)
                return;
            Close();
            var gpa = Main.Histories[index].GradePointAverage;
            if (string.IsNullOrEmpty(gpa.GradesAndPoints[0].Name)) {
                if (MainWindow.MessageBoxShow(gpa)) {
                    new ResultWindows(gpa, Main.Histories, index).Show();
                } else return;
            } else {
                if (MainWindow.MessageBoxShow(gpa)) {
                    new ResultWindowWithNames(gpa, Main.Histories, index).Show();
                } else return;
            }
        }

        private void Clear_Click(object sender, EventArgs e) {
            if (Historys.Items.Count == 0)
                return;
            if (Message.ShowYesNoCancelDialog("是否清空历史记录?", "警告", MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            Main.Histories.Clear();
            SetListView();
            Clear.Enabled = false;
        }

        private void Rename_Click(object sender, EventArgs e) {
            var index = Historys.SelectedItems[0].Index;
            if (index < 0)
                return;
            new HistoryRenameWindow(Main.Histories, index).ShowDialog();
            SetListView();
        }

        private void Delete_Click(object sender, EventArgs e) {
            var index = Historys.SelectedItems[0].Index;
            if (index < 0)
                return;
            if (Message.ShowYesNoCancelDialog("是否删除该条历史记录?", "警告", MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            if (Main.Histories.Remove(Main.Histories[index])) {
                SetListView();
                Message.ShowInformation("删除成功", "提示");
            } else Message.ShowError("删除失败\n未知错误");
        }
    }
}

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// ResultWindowWithNames.xaml 的交互逻辑
    /// </summary>
    public partial class ResultWindowWithNames : Window {
        private GradePointAverage GradePointAverage { get; }
        private BindingList<History> Histories { get; }
        private int Index { get; }
        private ContextMenuStrip MenuStrip { get; set; } = new ContextMenuStrip();

        public ResultWindowWithNames(GradePointAverage GPA, BindingList<History> histories, int index) {
            InitializeComponent();

            Results.Columns.Add("", 0, System.Windows.Forms.HorizontalAlignment.Center);
            Results.Columns.Add("学科名称", 180, System.Windows.Forms.HorizontalAlignment.Center);
            Results.Columns.Add("学分", 180, System.Windows.Forms.HorizontalAlignment.Center);
            Results.Columns.Add("成绩", 180, System.Windows.Forms.HorizontalAlignment.Center);

            KeyDown += Esc_Key_Down;

            GradePointAverage = GPA;
            Histories = histories;
            Index = index;

            SetListView();

            var change = new ToolStripMenuItem("修改");
            change.Click += MenuItem_Click;

            MenuStrip.Items.Add(change);

            Results.MouseClick += Results_MouseClick;
            ResultOfGpa.Content = $"总修读学分: {GradePointAverage.TotalPoint} " +
                $"已通过学分: {GradePointAverage.TotalNotFailedPoint} " +
                $"不及格科目数: {GradePointAverage.Fails} " +
                $"平均学分绩点: {GradePointAverage.Result:0.00}";
        }

        private void Results_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                MenuStrip.Show(Results, e.Location);
            }
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void SetListView() {
            Results.Items.Clear();
            foreach (var gradeAndPoint in GradePointAverage.GradesAndPoints) {
                var item = new ListViewItem {
                    Tag = gradeAndPoint
                };
                item.SubItems.Add(gradeAndPoint.Name);
                item.SubItems.Add(gradeAndPoint.Point.ToString());
                item.SubItems.Add(gradeAndPoint.Grade.ToString());
                Results.Items.Add(item);
            }
        }

        private void MenuItem_Click(object sender, EventArgs e) {
            var index = Results.SelectedItems[0].Index;
            if (index < 0)
                return;
            new ChangeData(index, Histories[Index], GradePointAverage).ShowDialog();
            SetListView();
            ResultOfGpa.Content = $"总修读学分: {GradePointAverage.TotalPoint} " +
                $"已通过学分: {GradePointAverage.TotalNotFailedPoint} " +
                $"不及格科目数: {GradePointAverage.Fails} " +
                $"平均学分绩点: {GradePointAverage.Result:0.00}";
        }
    }
}

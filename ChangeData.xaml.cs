using MessageUtil;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// ChangeData.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeData : Window {
        private int Index { get; }
        private BindingList<GradeAndPoint> GradesAndPoints { get; }
        private GradePointAverage GradePointAverage { get; }
        private History History { get; set; }

        public ChangeData(int index, History history, BindingList<GradeAndPoint> gradesAndPoints, GradePointAverage gradePointAverage) {
            InitializeComponent();
            KeyDown += Esc_Key_Down;
            Index = index;
            History = history;
            GradesAndPoints = gradesAndPoints;
            GradePointAverage = gradePointAverage;
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Change_Click(object sender, RoutedEventArgs e) {
            var name = GradesAndPoints[Index].Name;
            var grade = GradesAndPoints[Index].Grade;
            var point = GradesAndPoints[Index].Point;
            try {
                if (!string.IsNullOrEmpty(Name.Text))
                    name = Name.Text;
                if (!string.IsNullOrEmpty(Grade.Text))
                    grade = Convert.ToDouble(Grade.Text);
                if (!string.IsNullOrEmpty(Point.Text))
                    point = Convert.ToDouble(Point.Text);
            } catch (FormatException) {
                Message.ShowError("请输入正确的数值");
                return;
            }
            GradePointAverage.Change(GradesAndPoints[Index], new GradeAndPoint(name, grade, point));
            GradesAndPoints.Remove(GradesAndPoints[Index]);
            GradesAndPoints.Insert(Index, new GradeAndPoint(name, grade, point));
            History.UpdateTime = $"{DateTime.Now}";
            Close();
        }
    }
}

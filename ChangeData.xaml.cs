using System;
using System.ComponentModel;
using System.Windows;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// ChangeData.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeData : Window {
        private int Index { get; }
        private BindingList<GradeAndPoint> GradesAndPoints { get; }
        private GradePointAverage GradePointAverage { get; }

        public ChangeData(int index, BindingList<GradeAndPoint> gradesAndPoints, GradePointAverage gradePointAverage) {
            InitializeComponent();
            Index = index;
            GradesAndPoints = gradesAndPoints;
            GradePointAverage = gradePointAverage;
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
                MessageBox.Show("请输入正确的数值", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            GradePointAverage.Change(GradesAndPoints[Index], new GradeAndPoint(name, grade, point));
            GradesAndPoints.Remove(GradesAndPoints[Index]);
            GradesAndPoints.Insert(Index, new GradeAndPoint(name, grade, point));
            Close();
        }
    }
}

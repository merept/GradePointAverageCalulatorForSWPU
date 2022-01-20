using System.Windows;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// ResultWindowWithNames.xaml 的交互逻辑
    /// </summary>
    public partial class ResultWindowWithNames : Window {
        private GradePointAverage GradePointAverage { get; }

        public ResultWindowWithNames(GradePointAverage GPA) {
            InitializeComponent();
            GradePointAverage = GPA;
            Results.ItemsSource = GradePointAverage.GradesAndPoints;
            ResultOfGpa.Content = $"总修读学分: {GradePointAverage.TotalPoint} " +
                $"已通过学分: {GradePointAverage.TotalNotFailedPoint} " +
                $"不及格科目数: {GradePointAverage.Fails} " +
                $"平均学分绩点: {GradePointAverage.Result:0.00}";
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            new ChangeData(Results.SelectedIndex, GradePointAverage.GradesAndPoints, GradePointAverage).ShowDialog();
            ResultOfGpa.Content = $"总修读学分: {GradePointAverage.TotalPoint} " +
                $"已通过学分: {GradePointAverage.TotalNotFailedPoint} " +
                $"不及格科目数: {GradePointAverage.Fails} " +
                $"平均学分绩点: {GradePointAverage.Result:0.00}";
        }
    }
}

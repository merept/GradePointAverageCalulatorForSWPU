﻿using MessageUtil;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace GradePointAverageCalulatorForSWPU {
    /// <summary>
    /// ChangeData.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeData : Window {
        private int Index { get; }
        private BindingList<GradeAndPoint> GradesAndPoints { get; set; }
        private GradePointAverage GradePointAverage { get; set; }
        private History History { get; set; }
        private bool IsAdding { get; } = false;

        public ChangeData(int index, History history, GradePointAverage gradePointAverage) {
            Init(history, gradePointAverage);

            Index = index;

            Name.Text = GradesAndPoints[Index].Name;
            Grade.Text = GradesAndPoints[Index].Grade.ToString();
            Point.Text = GradesAndPoints[Index].Point.ToString();
        }

        public ChangeData(History history, GradePointAverage gradePointAverage) {
            Init(history, gradePointAverage);

            Title = "添加";
            Change.Text = "添加";

            IsAdding = true;
        }

        private void Init(History history, GradePointAverage gradePointAverage) {
            InitializeComponent();

            Change.Font = new Font(Change.Font.FontFamily, 9);
            Change.Enabled = false;
            Change.FlatStyle = System.Windows.Forms.FlatStyle.System;
            Change.FlatAppearance.BorderColor = Color.AliceBlue;
            Change.Focus();

            KeyDown += Esc_Key_Down;
            KeyDown += Enter_Key_Down;

            History = history;
            GradesAndPoints = gradePointAverage.GradesAndPoints;
            GradePointAverage = gradePointAverage;
        }

        private void Esc_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Enter_Key_Down(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter)
                Change_Click(null, null);
        }

        private void Changeing() {
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
                //MainWindow.Log.Log(ex, "输入了错误的数值");
                Message.ShowError("请输入正确的数值");
                return;
            }
            GradePointAverage.Change(GradesAndPoints[Index], new GradeAndPoint(name, grade, point));
            GradesAndPoints.Remove(GradesAndPoints[Index]);
            GradesAndPoints.Insert(Index, new GradeAndPoint(name, grade, point));
            History.UpdateTime = $"{DateTime.Now}";
            Close();
        }

        private void Adding() {
            string name;
            double grade, point;
            if (string.IsNullOrWhiteSpace(Grade.Text) || string.IsNullOrWhiteSpace(Point.Text)) {
                Message.ShowError("请输入完整");
                return;
            }
            if (string.IsNullOrWhiteSpace(Name.Text)) {
                name = "";
            } else name = Name.Text;
            try {
                grade = Convert.ToDouble(Grade.Text);
                point = Convert.ToDouble(Point.Text);
            } catch (FormatException) {
                Message.ShowError("请输入正确的数值");
                return;
            }
            GradePointAverage.Add(name, point, grade);
            History.UpdateTime = $"{DateTime.Now}";
            Close();
        }

        private void Change_Click(object sender, EventArgs e) {
            if (IsAdding) {
                Adding();
            } else Changeing();
        }

        private void Name_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            TextChanged();
        }

        private void Point_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            TextChanged();
        }

        private void Grade_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            TextChanged();
        }

        private void TextChanged() {
            if (!(string.IsNullOrWhiteSpace(Name.Text) && string.IsNullOrWhiteSpace(Grade.Text) && string.IsNullOrWhiteSpace(Point.Text))) {
                Change.Enabled = true;
            } else Change.Enabled = false;
        }
    }
}

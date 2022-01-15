using System.Collections.Generic;
using System.ComponentModel;

namespace GradePointAverageCalulatorForSWPU {
    public class GradePointAverage : INotifyPropertyChanged {
        public double TotalPoint { get; set; }
        public double TotalNotFailedPoint { get; set; }
        public double TotalGrade { get; set; }
        public BindingList<GradeAndPoint> GradesAndPoints { get; }
        public int Fails { get; set; }
        public double Result {
            get {
                return TotalGrade / TotalPoint;
            }
        }

        public GradePointAverage() {
            TotalGrade = 0;
            TotalPoint = 0;
            GradesAndPoints = new BindingList<GradeAndPoint>();
        }

        private void AddIn(double point, double grade) {
            var gradePoint = 0.0;
            if (grade >= 60) {
                gradePoint = (grade - 60) / 10 + 1;
                TotalNotFailedPoint += point;
            } else Fails++;
            TotalPoint += point;
            TotalGrade += gradePoint * point;
        }

        public void Add(double point, double grade) {
            AddIn(point, grade);
            GradesAndPoints.Add(new GradeAndPoint(grade, point));
        }

        public void Add(string name, double point, double grade) {
            AddIn(point, grade);
            GradesAndPoints.Add(new GradeAndPoint(name, grade, point));
        }

        public void Change(GradeAndPoint before, GradeAndPoint after) {
            var beforeGp = 0.0;
            if (before.Grade >= 60) {
                beforeGp = (before.Grade - 60) / 10 + 1;
                TotalNotFailedPoint -= before.Point;
            } else Fails--;
            TotalPoint -= before.Point;
            TotalGrade -= beforeGp * before.Point;
            AddIn(after.Point, after.Grade);
        }

        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChanged.Invoke(this, e);
        }
        #endregion
    }

    public class GradeAndPoint : INotifyPropertyChanged {
        public string Name { get; set; }
        public double Grade { get; set; }
        public double Point { get; set; }

        public GradeAndPoint(double grade, double point) {
            Grade = grade;
            Point = point;
        }

        public GradeAndPoint(string name, double grade, double point) {
            Name = name;
            Grade = grade;
            Point = point;
        }

        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChanged.Invoke(this, e);
        }

        public override bool Equals(object obj) {
            return obj is GradeAndPoint point &&
                   Name == point.Name &&
                   Grade == point.Grade &&
                   Point == point.Point;
        }

        public override int GetHashCode() {
            int hashCode = 736219787;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Grade.GetHashCode();
            hashCode = hashCode * -1521134295 + Point.GetHashCode();
            return hashCode;
        }
        #endregion
    }
}

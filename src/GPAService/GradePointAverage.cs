using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GradePointAverageCalulatorForSWPU {
    [Serializable]
    public class GradePointAverage : IEquatable<GradePointAverage> {
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
            if (grade >= 60) { //判断是否挂科
                gradePoint = (grade - 60) / 10 + 1; //计算单科学分绩点
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

        public void Delete(GradeAndPoint gradeAndPoint) {
            var gradeAndPointGP = 0.0;
            if (gradeAndPoint.Grade >= 60) {
                gradeAndPointGP = (gradeAndPoint.Grade - 60) / 10 + 1;
                TotalNotFailedPoint -= gradeAndPoint.Point;
            } else Fails--;
            TotalPoint -= gradeAndPoint.Point;
            TotalGrade -= gradeAndPointGP * gradeAndPoint.Point;
        }

        public void Change(GradeAndPoint before, GradeAndPoint after) {
            Delete(before);
            AddIn(after.Point, after.Grade);
        }

        public override bool Equals(object obj) {
            return Equals(obj as GradePointAverage);
        }

        public bool Equals(GradePointAverage other) {
            return other != null &&
                   TotalPoint == other.TotalPoint &&
                   TotalNotFailedPoint == other.TotalNotFailedPoint &&
                   TotalGrade == other.TotalGrade &&
                   GradesAndPoints.SequenceEqual(other.GradesAndPoints) &&
                   Fails == other.Fails &&
                   Result == other.Result;
        }

        public override int GetHashCode() {
            int hashCode = 1009407815;
            hashCode = hashCode * -1521134295 + TotalPoint.GetHashCode();
            hashCode = hashCode * -1521134295 + TotalNotFailedPoint.GetHashCode();
            hashCode = hashCode * -1521134295 + TotalGrade.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<BindingList<GradeAndPoint>>.Default.GetHashCode(GradesAndPoints);
            hashCode = hashCode * -1521134295 + Fails.GetHashCode();
            hashCode = hashCode * -1521134295 + Result.GetHashCode();
            return hashCode;
        }
    }

    [Serializable]
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
        [field: NonSerialized()]
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

    [Serializable]
    public class History : IEquatable<History>, INotifyPropertyChanged {
        public string UpdateTime { get; set; }
        public string HistoryName { get; set; }
        public GradePointAverage GradePointAverage { get; set; }
        public string LastTime { get; set; }

        public History(GradePointAverage gradePointAverage) {
            UpdateTime = $"{DateTime.Now}";
            HistoryName = $"{DateTime.Now.Date:yyyy-MM-dd}";
            GradePointAverage = gradePointAverage;
        }

        public History(string lastTime) {
            LastTime = lastTime;
        }

        public override bool Equals(object obj) {
            return Equals(obj as History);
        }

        public bool Equals(History other) {
            return other != null &&
                   EqualityComparer<GradePointAverage>.Default.Equals(GradePointAverage, other.GradePointAverage);
        }

        public override int GetHashCode() {
            return -1533900655 + EqualityComparer<GradePointAverage>.Default.GetHashCode(GradePointAverage);
        }

        public static bool operator ==(History left, History right) {
            return EqualityComparer<History>.Default.Equals(left, right);
        }

        public static bool operator !=(History left, History right) {
            return !(left == right);
        }

        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChanged.Invoke(this, e);
        }
    }
}

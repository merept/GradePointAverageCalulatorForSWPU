using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GradePointAverageCalulatorForSWPU.src.GPAService
{
    /// <summary>
    /// Backup.xaml 的交互逻辑
    /// </summary>
    public partial class Backup : Window
    {
        public Backup()
        {
            InitializeComponent();
        }

        private void StoreBackup_Click(object sender, EventArgs e)
        {
            //var dlg = new SaveFileDialog()
            //{
            //    FileName = Environment.UserName,
            //    DefaultExt = ".gpa",
            //    Filter = "GPAC历史文件 | *.gpa"
            //};
            //var result = dlg.ShowDialog();
            //if (result == true)
            //{
            //    SaveHistory();
            //    File.Copy(HistoryFileName, dlg.FileName, true);
            //    Message.ShowInformation("备份成功！", "备份记录");
            //}
        }

        private void RestoreBackup_Click(object sender, EventArgs e)
        {
            //var dlg = new OpenFileDialog()
            //{
            //    DefaultExt = ".gpa",
            //    Filter = "GPAC历史文件 | *.gpa"
            //};
            //var result = dlg.ShowDialog();
            //if (result == true)
            //{
            //    if (Message.ShowYesNoCancelDialog("该操作将覆盖原历史记录\n是否继续？", "恢复备份") == MessageBoxResult.Yes)
            //    {
            //        File.Copy(dlg.FileName, HistoryFileName, true);
            //        Message.ShowInformation("恢复成功！", "恢复记录");
            //        LoadHistory();
            //    }
            //}
        }
    }
}

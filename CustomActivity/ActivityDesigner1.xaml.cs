using Microsoft.Win32;
using System.Windows;
using System.IO;
using System.Activities;
namespace CustomActivity
{
    // Interaction logic for ActivityDesigner1.xaml
    public partial class ActivityDesigner1
    {
        public ActivityDesigner1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Logic mở hộp thoại chọn tệp
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Gắn đường dẫn tệp được chọn vào ModelItem 
                ModelItem.Properties["Filename"].SetValue(new InArgument<string>(openFileDialog.FileName));
            }
        }

    }
}

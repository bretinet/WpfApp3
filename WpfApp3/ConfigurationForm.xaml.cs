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

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for ConfigurationForm.xaml
    /// </summary>
    public partial class ConfigurationForm : Window
    {
        private ComparePatternsUserControl comparePatternsUserControl;

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void FilePatternButton_Click(object sender, RoutedEventArgs e)
        {
            //ConfigurationMainFrame.Content = new FilePatternWindow();
            ContentControl1.Content = new FilePatternUserControl();
        }

        private void FolderPatterButton_Click(object sender, RoutedEventArgs e)
        {
            var sss = new FilePatternWindow();
            //ConfigurationMainFrame.Content = sss;
        }

        private void ComparePatternButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (comparePatternsUserControl == null)
            {
                comparePatternsUserControl = new ComparePatternsUserControl();
            }
            ContentControl1.Content = comparePatternsUserControl;
        }
    }
}

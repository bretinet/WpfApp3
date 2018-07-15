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
using WpfApp3.User_Controls;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for ConfigurationForm.xaml
    /// </summary>
    public partial class ConfigurationForm : Window
    {
        private FilePatternUserControl _filePatternUserControl;
        private ComparePatternsUserControl _comparePatternsUserControl;
        private FilterFolderUserControl _filterFolderUserControl;
        private ModificationOptionsUserControl _modificationOptionsUserControl;

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void FilePatternButton_Click(object sender, RoutedEventArgs e)
        {
            if (_filePatternUserControl == null)
            {
                _filePatternUserControl = new FilePatternUserControl();
            }
            ContentControl1.Content = _filePatternUserControl;
        }

        private void FolderPatterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_filterFolderUserControl == null)
            {
                _filterFolderUserControl = new FilterFolderUserControl();
            }
            ContentControl1.Content = _filterFolderUserControl;
        }

        private void ComparePatternButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_comparePatternsUserControl == null)
            {
                _comparePatternsUserControl = new ComparePatternsUserControl();
            }
            ContentControl1.Content = _comparePatternsUserControl;
        }

        private void ModificationOptionsUserControlButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_modificationOptionsUserControl == null)
            {
                _modificationOptionsUserControl = new ModificationOptionsUserControl();
            }
            ContentControl1.Content = _modificationOptionsUserControl;
        }
    }
}

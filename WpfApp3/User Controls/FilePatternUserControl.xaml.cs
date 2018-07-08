using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for FilePatternUserControl.xaml
    /// </summary>
    public partial class FilePatternUserControl : UserControl
    {
        private const string DefaultConfigFile = "DefaultConfig.Config";
        public FilePatternUserControl()
        {
            InitializeComponent();
        }

        private void SaveFilePatternButton_Click(object sender, RoutedEventArgs e)
        {
            var persistence = new Persistence<FilePatternConfiguration>();

            try
            {
                var filePatternConfiguration = new FilePatternConfiguration
                {

                    DefaultFolder = DefaultFolderTextBox.Text,
                    DefaultFilterPattern = DefaulFilterTextBox.Text,
                    DefaultUrl = DefaultUrlTextBox.Text,
                    IncludeSubFolders = IncludeSubFoldersCheckBox?.IsChecked ?? false
                };

                persistence.SetConfigurationValues(DefaultConfigFile, filePatternConfiguration);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(exception.Message);
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var persistence = new Persistence<FilePatternConfiguration>();

            var configuration = persistence.GetConfigurationValues(DefaultConfigFile);
            if (configuration != null)
            {
                DefaultFolderTextBox.Text = configuration.DefaultFolder;
                DefaulFilterTextBox.Text = configuration.DefaultFilterPattern;
                DefaultUrlTextBox.Text = configuration.DefaultUrl;
                IncludeSubFoldersCheckBox.IsChecked = configuration.IncludeSubFolders;

            }

        }

        private void DefaultFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog { IsFolderPicker = true };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DefaultFolderTextBox.Text = folderDialog.FileName;
            }
        }
    }
}

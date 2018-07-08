using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for FileCreationWindow.xaml
    /// </summary>
    public partial class FileCreationWindow : Window
    {
        public FileCreationWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentDirectory = Environment.CurrentDirectory;

            var newDirectory = System.IO.Directory.CreateDirectory("Probe1");

            for (var i = 0; i < int.Parse(PagesNumberTextBox.Text); i++)
            {
                var name = FileNamePatternTextBox.Text.Replace("{num}", i.ToString("00")); // "Newfile" + i + ".asp";
                var fullName = System.IO.Path.Combine(RootFolderTextBox.Text,name) ;
                var streamWriter = new StreamWriter(fullName);
                //ssss.Write("This is a new text - POWERED BY MAC TECH " + i.ToString() );
                streamWriter.Write(TemplateTextBox.Text.Replace("{num}", i.ToString()));
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter = null;

            }
            MessageBox.Show("Done");
        }

        private void RootFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog { IsFolderPicker = true };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                RootFolderTextBox.Text = folderDialog.FileName;
            }
        }
    }
}

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
using WpfApp3.Extensions;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for ModifyFilesWindow.xaml
    /// </summary>
    public partial class ModifyFilesWindow : Window
    {
        private readonly List<string> SelectedFiles;
        public ModifyFilesWindow()
        {
            InitializeComponent();

            SelectedFiles = Configuration.Instance.SelectedFiles;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ModificationFilesList.ItemsSource = SelectedFiles;

        }

        private void ModificationFilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModificationFilesList?.SelectedItem == null)
            {
                return;
            }

            var fileName = ModificationFilesList.SelectedItem.ToString().CleanFileName();
            //MessageBox.Show(FilesListBox.SelectedItem.ToString());
            try
            {
                //if (FilesListBox.SelectedItem.ToString().EndsWith(" (False)"))
                //{
                //    ff = ff.Replace(" (False)", "");
                //}
                //if (FilesListBox.SelectedItem.ToString().EndsWith(" (True)"))
                //{
                //    ff = ff.Replace(" (True)", "");
                //}
                var rootFolder = Configuration.Instance.FileConfiguration.DefaultFolder;

                var path = new FileInfo(Path.Combine(rootFolder, fileName));

                OriginalFileTextBox.Clear();
                ModifiedFileTextBox.Clear();

                var file = path.OpenText().ReadToEndAsync();
                file.ContinueWith(task =>
                {
                    OriginalFileTextBox.Text = file.Result;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                var ss = file.Result;

                ModifiedFileTextBox.Text = $"<!--#include secureFile.asp-->\n{ss}";



            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }
        }

        private void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var rootFolder = Configuration.Instance.FileConfiguration.DefaultFolder;
            
            var writer = new StreamWriter("log.txt");
            for (var i = 0; i < SelectedFiles.Count; i++)
            {
                var fileName = SelectedFiles[i];
                var path = new FileInfo(Path.Combine(rootFolder, fileName));
                
                WriteInformationInLog(writer, path.FullName, "SUCCESS");

                File.Copy(path.FullName, "D:\\Backy\\"+fileName,true);
            }
            writer.Flush();
            writer.Close();
            //writer = null;

        }

        private void WriteInformationInLog(StreamWriter streamWriter, string text, string result)
        {
            

            var ss = new StringBuilder();
            ss.Append(DateTime.Now);
            ss.Append("  -  ");
            ss.Append(text);
            ss.Append("  -  ");
            ss.Append(result);
            ss.Append("Backup:");
            ss.Append("TRUE");
            ss.Append("  -  ");
            ss.Append("Route");

            streamWriter.WriteLine(ss.ToString());
            
        }
    }
}

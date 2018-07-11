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

            ModificationFilesList.ItemsSource = SelectedFiles;
        }

        private void Window_Activated(object sender, EventArgs e)
        {


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

                var fileResult = file.Result;

                ModifiedFileTextBox.Text = $"<!-- #include file=\"permprefixNew.asp\"-->\n{fileResult}";



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
            FileInfo path;
            for (var i = 0; i < SelectedFiles.Count; i++)
            {
                var fileName = SelectedFiles[i];

                var pahtWithoutRoot = fileName.Replace(Configuration.Instance.FileConfiguration.DefaultFolder, "");

                var fff = pahtWithoutRoot.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

                path = new FileInfo(Path.Combine(rootFolder, fileName));

                WriteInformationInLog(writer, path.FullName, "SUCCESS");

                var rootBackup = @"C:\Users\E081138\Documents\TempBackup";
                if (fff.ToList().Count > 1)
                {
                    string currentPath = rootBackup;
                    while (fff.ToList().Count > 1)
                    {
                        System.
                    }
                }
                
                try
                {
                    if (!System.IO.Directory.Exists(rootBackup))
                    {
                        System.IO.Directory.CreateDirectory(rootBackup);
                    }
                    File.Copy(path.FullName, Path.Combine( rootBackup,  fileName), true);
                }
                catch (Exception ex)
                {
                    WriteInformationInLog(writer, path.FullName, "ERROR COPYING");
                    MessageBox.Show("Error copying the file:" + path.FullName + Environment.NewLine + ex.Message);
                }




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

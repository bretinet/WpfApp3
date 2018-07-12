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

                var rootBackup = "D:\\NewProbe";// @"C:\Users\E081138\Documents\TempBackup";
                string currentPath = rootBackup;
                if (fff.ToList().Count > 1)
                {
                    
                    for (var j = 0; j < fff.ToList().Count - 1; j++)
                    {
                        var ddd = System.IO.Path.Combine(currentPath, fff[j]);
                        if (!System.IO.Directory.Exists(ddd))
                        {
                            var directoryInfo = System.IO.Directory.CreateDirectory(ddd);
                            currentPath = directoryInfo.FullName;
                        }
                    }
                }
                
                try
                {
                //    if (!System.IO.Directory.Exists(rootBackup))
                //    {
                //        System.IO.Directory.CreateDirectory(rootBackup);
                //    }
                    var loki = fff[fff.ToList().Count - 1];
                    var lokiwe = Path.Combine(currentPath, loki);
                    File.Copy(path.FullName, lokiwe, true);
                }
                catch (Exception ex)
                {
                    WriteInformationInLog(writer, path.FullName, "ERROR COPYING");
                    MessageBox.Show("Error copying the file:" + path.FullName + Environment.NewLine + ex.Message);
                }


                path = null;

            }
            writer.Flush();
            writer.Close();
            //writer = null;



            /////


            //OriginalFileTextBox.Clear();
            //ModifiedFileTextBox.Clear();

            foreach (var fileName in SelectedFiles)
            {
                var path2 = Path.Combine(rootFolder, fileName);
                //var fileResult = path2.OpenText().ReadToEndAsync().Result;
                var yyy = new StreamReader(path2);
                var fileResult = yyy.ReadToEndAsync().Result;
                yyy.Close();
                yyy = null;
                //file.ContinueWith(task =>
                //{
                //    OriginalFileTextBox.Text = file.Result;
                //}, TaskScheduler.FromCurrentSynchronizationContext());

                //var fileResult = file.Result;

                var finalResult2 = $"<!-- #include file=\"permprefixNew.asp\"-->\n{fileResult}";
                //fileResult = null;
                //path2.OpenWrite().Write(Encoding.ASCII.GetBytes(finalResult),0 , Encoding.ASCII.GetBytes(finalResult).Length );

                var ss = new StreamWriter(path2);
                ss.Write(finalResult2);
                ss.Flush();
                ss.Close();
                ss = null;

            }



            

           // ModifiedFileTextBox.Text = $"<!-- #include file=\"permprefixNew.asp\"-->\n{fileResult}";

        }

        private void WriteInformationInLog(StreamWriter streamWriter, string text, string result)
        {


            var ss = new StringBuilder();
            ss.Append(DateTime.Now);
            ss.Append("  -  ");
            ss.Append(text);
            ss.Append("  -  ");
            ss.Append(result);
            ss.Append("  -  ");
            ss.Append("Backup:");
            ss.Append("TRUE");
            ss.Append("  -  ");
            ss.Append("Route");

            streamWriter.WriteLine(ss.ToString());

        }
    }
}

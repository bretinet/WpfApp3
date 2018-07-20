using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp3.Extensions;
using WpfApp3.Utilities;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for ModifyFilesWindow.xaml
    /// </summary>
    public partial class ModifyFilesWindow : Window
    {
        string injectedCode = "<!--#include file=\"permprefixNew.asp\"-->";
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

            var originalText = IoUtilities.ReadStringFromFile(fileName);
            OriginalFileTextBox.Text = originalText;

            var modifiedText = GetModifiedFile(originalText, injectedCode);
            ModifiedFileTextBox.Text = modifiedText;

            //////MessageBox.Show(FilesListBox.SelectedItem.ToString());
            ////try
            ////{
            ////    //if (FilesListBox.SelectedItem.ToString().EndsWith(" (False)"))
            ////    //{
            ////    //    ff = ff.Replace(" (False)", "");
            ////    //}
            ////    //if (FilesListBox.SelectedItem.ToString().EndsWith(" (True)"))
            ////    //{
            ////    //    ff = ff.Replace(" (True)", "");
            ////    //}
            ////    var rootFolder = Configuration.Instance.FileConfiguration.RootFolder;

            ////    var path = new FileInfo(Path.Combine(rootFolder, fileName));

            ////    OriginalFileTextBox.Clear();
            ////    ModifiedFileTextBox.Clear();

            ////    var file = path.OpenText().ReadToEndAsync();
            ////    file.ContinueWith(task =>
            ////    {
            ////        OriginalFileTextBox.Text = file.Result;
            ////    }, TaskScheduler.FromCurrentSynchronizationContext());

            ////    var fileResult = file.Result;

            ////    ModifiedFileTextBox.Text = $"<!-- #include file=\"permprefixNew.asp\"-->\n{fileResult}";



            ////}
            ////catch (Exception exception)
            ////{
            ////    MessageBox.Show(exception.Message);

            ////}
        }

        private void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
        {


            var injectedCode = "<!--#include file=\"permprefixNew.asp\"-->";


            var rootFolder = Configuration.Instance.FileConfiguration.RootFolder;

            var writer = new StreamWriter("log.txt");
            FileInfo path;
            for (var i = 0; i < SelectedFiles.Count; i++)
            {
                var fileName = SelectedFiles[i];

                var pahtWithoutRoot = fileName.Replace(Configuration.Instance.FileConfiguration.RootFolder, "");

                var fff = pahtWithoutRoot.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

                path = new FileInfo(Path.Combine(rootFolder, fileName));

                WriteInformationInLog(writer, path.FullName, "SUCCESS");

                var rootBackup = @"C:\Users\E081138\Documents\TempBackup"; //"D:\\NewProbe";
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

            var ggg = Configuration.Instance.FileConfiguration.RootFolder;
            var finalResultFolder = Directory.CreateDirectory(Path.Combine(ggg, "FinalResult"));

            //OriginalFileTextBox.Clear();
            //ModifiedFileTextBox.Clear();

            foreach (var fileName in SelectedFiles)
            {
                var path2 = Path.Combine(rootFolder, fileName);
                //var fileResult = path2.OpenText().ReadToEndAsync().Result;
                ////var yyy = new StreamReader(path2);
                ////var fileResult = yyy.ReadToEndAsync().Result;
                ////yyy.Close();
                //yyy = null;
                //file.ContinueWith(task =>
                //{
                //    OriginalFileTextBox.Text = file.Result;
                //}, TaskScheduler.FromCurrentSynchronizationContext());

                //var fileResult = file.Result;


                //////////////////////////////////////////////////
                ////string finalResult2 = string.Empty;

                ////var ttt = Regex.Match(fileResult, @"(?i:(<%@\s*Language=VBScript\s*%>))");

                ////var ttt2 = Regex.Match(fileResult, "((<%)[^<%]*(Option.Explicit)[^(%>)]*(%>))", RegexOptions.Multiline| RegexOptions.IgnoreCase); //"<%.*\r.*Option\sExplicit.*%>"


                ////if (ttt.Success || ttt2.Success)
                ////{
                ////    //MessageBox.Show("Found at position: " + ttt.Index + "Finish at position:" + ttt.Index + ttt.Length);


                ////    string part1, part2;
                ////    if (ttt2.Success)
                ////    {
                ////        //MessageBox.Show("Found at position: " + ttt2.Index + "Finish at position:" + ttt2.Index + ttt2.Length);

                ////        part1 = fileResult.Substring(0, ttt2.Index + ttt2.Length);

                ////        part2 = fileResult.Substring(ttt2.Index + ttt2.Length, fileResult.Length - (ttt2.Index + ttt2.Length));


                ////        //finalResult2 =  $"<!--#include file=\"permprefixNew.asp\"-->\n{fileResult}";

                ////        //finalResult2 = $"{part1}{injectedCode}{part2}";

                ////       // ModifiedFileTextBox.Text = finalResult2;
                ////    }
                ////    else
                ////    {
                ////        part1 = fileResult.Substring(0, ttt.Index + ttt.Length);
                ////        part2 = fileResult.Substring(ttt.Index + ttt.Length, fileResult.Length - (ttt.Index + ttt.Length));
                ////        //finalResult2 = $"{part1}{injectedCode}{part2}";

                ////        //ModifiedFileTextBox.Text = finalResult2;
                ////    }
                ////    finalResult2 = $"{part1}\n{injectedCode}{part2}";


                ////}
                ////else
                ////{
                ////    finalResult2 = $"{injectedCode}\n{fileResult}";
                ////}
                ////ModifiedFileTextBox.Text = finalResult2;


                ///////////////////////////////////////////////////////////////

                //fileResult = null;
                //path2.OpenWrite().Write(Encoding.ASCII.GetBytes(finalResult),0 , Encoding.ASCII.GetBytes(finalResult).Length );
                var originalText = IoUtilities.ReadStringFromFile(path2);

                var modifiedText = GetModifiedFile(originalText, injectedCode);
                if (originalText.Equals(modifiedText))
                {
                    continue;
                }
                IoUtilities.WriteStringToFile(path2,modifiedText);
                
                ////var ss = new StreamWriter(path2);
                ////ss.Write(finalResult2);
                ////ss.Flush();
                ////ss.Close();
                ////ss = null;
                var ggghhh = fileName.Split(new string [] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                var tty = ggghhh[ggghhh.Length - 1];
                var finalCopyFilename = Path.Combine(finalResultFolder.FullName,tty );
                if (File.Exists(finalCopyFilename))
                {
                    finalCopyFilename = finalCopyFilename + DateTime.Now.Ticks.ToString();
                }
                System.IO.File.Copy(path2, finalCopyFilename);
            }



            MessageBox.Show("Done");

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }




        internal string GetModifiedFile(string originalText, string injectedCode)
        {
            string modifiedText;

            if (Regex.Match(originalText, injectedCode).Success)
            {
                return originalText;
            }

            var regexValidation1 = Regex.Match(originalText, @"(?i:(<%@\s*[Ll]anguage\s*=\s*VBScript\s*%>))"); //  @"(?i:(<%@\s*Language=VBScript\s*%>))");

            var temp = originalText.Replace("\r\n", "");
            var regexValidation2 = Regex.Match(originalText, @"(?i:(<%[^<]*\bOption\b\s+\bExplicit\b[^>]*%>))");//                      @":(<%(?!\s+<%).*\bOption\b\s+\bExplicit\b.*(?<!%>.*)%>))"); //    "((<%)[^<%]*(Option.Explicit)[^(%>)]*(%>))", RegexOptions.Singleline | RegexOptions.IgnoreCase); //"<%.*\r.*Option\sExplicit.*%>"


            if (regexValidation1.Success || regexValidation2.Success)
            {
                string part1, part2;
                if (regexValidation2.Success)
                {
                    part1 = originalText.Substring(0, regexValidation2.Index + regexValidation2.Length);
                    part2 = originalText.Substring(regexValidation2.Index + regexValidation2.Length, originalText.Length - (regexValidation2.Index + regexValidation2.Length));
                }
                else
                {
                    part1 = originalText.Substring(0, regexValidation1.Index + regexValidation1.Length);
                    part2 = originalText.Substring(regexValidation1.Index + regexValidation1.Length, originalText.Length - (regexValidation1.Index + regexValidation1.Length));
                }
                modifiedText = $"{part1}\n{injectedCode}{part2}";
            }
            else
            {
                modifiedText = $"{injectedCode}\n{originalText}";
            }
            return modifiedText;
        }

        private void CutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ModifiedFileTextBox.SelectedText);
            ModifiedFileTextBox.SelectedText = string.Empty;
        }

        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ModifiedFileTextBox.SelectedText);
        }

        private void PasteMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ModifiedFileTextBox.Paste();
        }

        private void SaveFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ModificationFilesList.SelectedItems.Count != 1)
            {
                return;
            }

            var fileName = ModificationFilesList.SelectedItem.ToString().CleanFileName();
            var response = MessageBox.Show($"Do you want to modify the file {fileName}?", "Modify file", MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (response != MessageBoxResult.Yes)
            {
                return;
            }

           // var path = new FileInfo(Path.Combine(SearchFolder, fileName));
            var currentText = ModifiedFileTextBox.Text;


            var originalText = IoUtilities.ReadStringFromFile(fileName);
            if (originalText.Equals(currentText))
            {
                MessageBox.Show("Both files are identical", "Modify file", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            IoUtilities.WriteStringToFile(fileName, currentText);
            MessageBox.Show($"The content of the file {fileName} has been changed.", "Modify file", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RestoreFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var response = MessageBox.Show("Do you want to restore this file?", "Restore File", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (response != MessageBoxResult.Yes)
            {
                return;
            }

            var fileName = ModificationFilesList.SelectedItem.ToString().CleanFileName();

            var originalText = IoUtilities.ReadStringFromFile(fileName);

            var modifiedText = GetModifiedFile(originalText, injectedCode);

            ModifiedFileTextBox.Text = modifiedText;
        }
    }
}

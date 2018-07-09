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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Path = System.IO.Path;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable<string> PatternList { get; set; }
        public IEnumerable<string> FolderPatternList { get; set; }

        private SourceCodeFile sourceCodeFile;

        public MainWindow()
        {
            InitializeComponent();
            SearchFolder = String.Empty;
            sourceCodeFile = new SourceCodeFile();
        }

        private string _myVar;

        public string SearchFolder
        {
            get => _myVar;
            set
            {
                _myVar = value;
                BtnStart.IsEnabled = !string.IsNullOrEmpty(value);
            }
        }



        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog { IsFolderPicker = true };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SearchFolder = folderDialog.FileName;
                FolderTextBox.Text = SearchFolder;
            }
        }

        private void GetFilesButton_Click(object sender, RoutedEventArgs e)
        {

            _bretinet.Clear();


            ItemsCounter = default(int);
            IncludeSubFolders = SubFoldersCheckBox.IsChecked ?? true;
            TotalFilesLabel.Content = " - -";

            PatternList = GetPatterns();
            FolderPatternList = GetFolderPatterns();
            GetFoldersAndFiles(string.Empty);

            FilesListBox.ItemsSource = null;
            FilesListBox.ItemsSource = _bretinet;
            TotalFilesLabel.Content = ItemsCounter.ToString();
            CompareTrueFilesLabel.Content = string.Empty;
            CompareFalseFilesLabel.Content = string.Empty;
        }

        public int ItemsCounter { get; set; }
        public bool IncludeSubFolders { get; set; } = true;

        private IEnumerable<string> GetPatterns()
        {
            var patterns = new List<string>();
            if (string.IsNullOrWhiteSpace(PatternTextBox.Text))
            {
                return new List<string> { "*" };
            }

            var patternList = PatternTextBox.Text.Split(new char[] { ',', ';' });

            foreach (var pattern in patternList)
            {
                patterns.Add(pattern.Trim());
            }
            return patterns;
        }

        private IEnumerable<string> GetFolderPatterns()
        {
            var pattenFolderFactory = new PattenFolderFactory();
            var patternFolder = pattenFolderFactory.ReadPatternFolders().ToList();

            if (patternFolder == null || !patternFolder.Any())
            {
                return null;
            }

            var list = new List<string>();
            foreach (var item in patternFolder)
            {
                list.Add(item.Name);
            }
            return list;
        }

        private readonly List<string> _bretinet = new List<string>();
        public void GetFoldersAndFiles(string directory)
        {
            var currentDirectory = Path.Combine(SearchFolder, directory);
            var directoryInfo = new DirectoryInfo(currentDirectory);

            //var pattern = string.IsNullOrEmpty(PatternTextBox.Text) ? "*" : PatternTextBox.Text;
            //var files = directoryInfo.GetFiles(pattern, searchOption: SearchOption.TopDirectoryOnly);

            var sss = new List<string>();

            foreach (var pattern2 in PatternList)
            {
                var sstr = @"(?i:^" + pattern2.Replace(".", "\\.").Replace("?", ".") + "$)";
                var files = directoryInfo.GetFiles(pattern2, searchOption: SearchOption.TopDirectoryOnly);
                foreach (var fileInfo in files)
                {
                    if (!string.IsNullOrEmpty(fileInfo.Name))
                    {
                        var fullPathName = Path.Combine(directory, fileInfo.Name);
                        if (!FilesListBox.Items.Contains(fullPathName))
                        {
                            var pat = new Regex(sstr);
                            var res = pat.Match(fileInfo.Name).Success;
                            if (res)
                                sss.Add(fullPathName);
                        }

                    }
                }
            }
            _bretinet.AddRange(sss.OrderBy(z => z));
            ItemsCounter += sss.Count;


            if (IncludeSubFolders)
            {
                var subdirectories = directoryInfo.GetDirectories();
                foreach (var folder in subdirectories)
                {
                    var match = false;
                    if (FolderPatternList != null)
                    {
                        foreach (var pattern in FolderPatternList)
                        {
                            var reg = new Regex(pattern);
                            match = reg.Match(folder.Name).Success;
                            if (match)
                            {
                                break;
                            }
                        }
                    }

                    if (FolderPatternList == null || match)
                    {
                        var currentFolder = Path.Combine(directory, folder.Name);
                        GetFoldersAndFiles(currentFolder);
                    }
                    else
                    {
                        //MessageBox.Show(Path.Combine(directory, folder.Name));
                    }
                }
            }
        }

        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListBox?.SelectedItem == null)
            {
                return;
            }
            string ff = FilesListBox.SelectedItem.ToString();
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
                //if (FilesListBox.SelectedItem.ToString().EndsWith(" (Warning)"))
                //{
                //    ff = ff.Replace(" (True)", "");
                //}

                ff = ff.Replace(" (True)", "").Replace(" (False)", "").Replace(" (Warning)", "");

                var ppp = new System.IO.FileInfo(System.IO.Path.Combine(SearchFolder, ff));

                var dddd = ppp.OpenText().ReadToEnd();
                FileTextBox.Clear();
                FileTextBox.Text = dddd;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        private void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Probe = _bretinet;
            window1.Show();



        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ComparePatternsButton_Click(object sender, RoutedEventArgs e)
        {


            var CounterFalse = 0;
            var CounterTrue = 0;

            List<PatternCheck> patternItems = new List<PatternCheck>();
            using (Stream stream = File.Open("items.bin", FileMode.OpenOrCreate))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                if (stream.Length > 0)
                {
                    patternItems = (List<PatternCheck>)formatter.Deserialize(stream);
                }
            }

            try
            {
                for (int i = 0; i < _bretinet.Count; i++)
                {
                    var ppp = new System.IO.FileInfo(System.IO.Path.Combine(SearchFolder, _bretinet[i]));

                    var FileText = ppp.OpenText().ReadToEnd();

                    for (int j = 0; j < patternItems.Count; j++)
                    {


                        var MatchValue = Regex.Match(FileText, patternItems[j].Pattern);  //"#include file=\"permprefix.asp\""

                        if (j == 0)
                        {
                            if (MatchValue.Success)
                            {
                                CounterTrue++;
                            }
                            else
                            {
                                CounterFalse++;
                            }
                        }

                        var TextToShow = MatchValue.Success ? patternItems[j].Value == "Warning" ? "Warning" : MatchValue.Success.ToString() : false.ToString();
                        _bretinet[i] = _bretinet[i] + " (" + TextToShow + ")";
                    }
                }

                FilesListBox.ItemsSource = null;
                FilesListBox.ItemsSource = _bretinet;

                CompareTrueFilesLabel.Content = CounterTrue;
                CompareFalseFilesLabel.Content = CounterFalse;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ff = FilesListBox.SelectedItem.ToString();
            //MessageBox.Show(FilesListBox.SelectedItem.ToString());
            try
            {
                if (FilesListBox.SelectedItem.ToString().EndsWith(" (False)"))
                {
                    ff = ff.Replace(" (False)", "");
                }
                if (FilesListBox.SelectedItem.ToString().EndsWith(" (True)"))
                {
                    ff = ff.Replace(" (True)", "");
                }

                var path = new System.IO.FileInfo(System.IO.Path.Combine(SearchFolder, ff));

                System.IO.File.WriteAllText(path.FullName, FileTextBox.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var window2 = new Window2();
            window2.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var preferences = new Preferences
            {
                DefaultFolder = FolderTextBox.Text,
                DefaultPattern = PatternTextBox.Text,
                DefaultUrl = URLBaseTextBox.Text
            };

            using (Stream stream = File.Open("Preferences.bin", FileMode.Create))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, preferences);
            }
        }


        public Preferences preferences;
        private void Window_Activated(object sender, EventArgs e)
        {
            preferences = new Preferences();
            using (Stream stream = File.Open("Preferences.bin", FileMode.OpenOrCreate))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                if (stream.Length > 0)
                {
                    preferences = (Preferences)formatter.Deserialize(stream);
                }
            }

            if (preferences != null)
            {
                SearchFolder = preferences?.DefaultFolder;
                FolderTextBox.Text = SearchFolder;
                PatternTextBox.Text = preferences?.DefaultPattern;
                URLBaseTextBox.Text = preferences?.DefaultUrl;

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FolderPatternWindow window = new FolderPatternWindow();
            window.ShowDialog();
        }
    }

    [Serializable]
    public class Preferences
    {
        public string DefaultFolder { get; set; }

        public string DefaultPattern { get; set; }

        public string DefaultUrl { get; set; }
    }
}

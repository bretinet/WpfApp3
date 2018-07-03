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
        private SourceCodeFile sourceCodeFile;

        public MainWindow()
        {
            InitializeComponent();
            //Probe..Initialize(this);

            //Probe.CanContentScroll = false;


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
                TxtFolder.Text = SearchFolder;


            }
        }

        private void GetFilesButton_Click(object sender, RoutedEventArgs e)
        {
            //FilesListBox.Items.Clear();
            _bretinet.Clear();
            ItemsCounter = default(int);
            IncludeSubFolders = SubFoldersCheckBox.IsChecked ?? true;
            TotalFilesLabel.Content = " - -";
            //var directory = new System.IO.DirectoryInfo(SearchFolder);
            //var fff = sss.GetFiles();

            //foreach (var fileInfo in fff)
            //{
            //    FilesListBox.Items.Add(fileInfo.Name);
            //}

            PatternList = GetPatterns();
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
                            //FilesListBox.Items.Add(fullPathName);
                            //ItemsCounter++;

                            
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
                    var currentFolder = Path.Combine(directory, folder.Name);

                    GetFoldersAndFiles(currentFolder);

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
                if (FilesListBox.SelectedItem.ToString().EndsWith(" (False)"))
                {
                    ff = ff.Replace(" (False)","");
                }
                if (FilesListBox.SelectedItem.ToString().EndsWith(" (True)"))
                {
                    ff = ff.Replace(" (True)", "");
                }

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
            try
            {

                for (int i = 0; i < _bretinet.Count; i++)
                {
                    var ppp = new System.IO.FileInfo(System.IO.Path.Combine(SearchFolder, _bretinet[i]));

                    var dddd = ppp.OpenText().ReadToEnd();

                    var ss = Regex.Match(dddd, "<!--#include file=\"permprefix.asp\" -->");

                    //MatchLabel.Content = ss.Success.ToString();

                    if (ss.Success)
                    {
                        CounterTrue++;
                    }
                    else
                    {
                        CounterFalse++;
                    }

                    _bretinet[i] = _bretinet[i] + " (" + ss.Success + ")";
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
    }
}

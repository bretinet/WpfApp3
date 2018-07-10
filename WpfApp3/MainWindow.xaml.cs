using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using WpfApp3.Extensions;
using Path = System.IO.Path;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DefaultConfigFile = "DefaultConfig.Config";
        private readonly SynchronizationContext synchronizationContext;

        public IEnumerable<string> PatternList { get; set; }
        private SourceCodeFile sourceCodeFile;

        public MainWindow()
        {
            InitializeComponent();
            //Probe..Initialize(this);

            //Probe.CanContentScroll = false;
            synchronizationContext = SynchronizationContext.Current;

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
                RootFolderTextBox.Text = SearchFolder;
            }
        }

        private void GetFilesButton_Click(object sender, RoutedEventArgs e)
        {
            fileNameList.Clear();
            ItemsCounter = default(int);

            IncludeSubFolders = IncludeSubFoldersCheckBox?.IsChecked ?? true;

            TotalFilesLabel.Content = " - -";
            PatternList = GetFilters();

            Task.Factory.StartNew(() =>
            {
                GetFoldersAndFiles(string.Empty);
            }).ContinueWith(ss =>
            {
                FilesListBox.ItemsSource = null;
                FilesListBox.ItemsSource = fileNameList;
                TotalFilesLabel.Content = ItemsCounter.ToString();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            Configuration.Instance.SelectedFiles = fileNameList;
        }

        public int ItemsCounter { get; set; }
        public bool IncludeSubFolders { get; set; } = true;

        private IEnumerable<string> GetFilters()
        {
            var patterns = new List<string>();

            if (string.IsNullOrWhiteSpace(FilterPatternTextBox.Text))
            {
                return new List<string> { "*" };
            }

            var patternList = FilterPatternTextBox.Text.Split(new char[] { ',', ';' });

            foreach (var pattern in patternList)
            {
                patterns.Add(pattern.Trim());
            }
            return patterns;
        }

        private readonly List<string> fileNameList = new List<string>();
        public void GetFoldersAndFiles(string directory)
        {
            var currentDirectory = Path.Combine(SearchFolder, directory);
            var directoryInfo = new DirectoryInfo(currentDirectory);

            //var pattern = string.IsNullOrEmpty(PatternTextBox.Text) ? "*" : PatternTextBox.Text;
            //var files = directoryInfo.GetFiles(pattern, searchOption: SearchOption.TopDirectoryOnly);

            var filesInFolder = new List<string>();

            foreach (var filterPattern in PatternList)
            {
                try
                {
                    var folderInformation = new FolderInformation();
                    //var ttt = folderInformation.IsFolderAccessible(currentDirectory, FileSystemRights.Traverse);
                    //var exist = System.IO.Directory.Exists(currentDirectory);
                    var fffilter = new Regex("^"+ filterPattern +"$");

                    var files = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).Where(x => fffilter.Match(x.Name).Success);
                    foreach (var fileInfo in files)
                    {
                        if (!string.IsNullOrEmpty(fileInfo.Name))
                        {
                            var fullPathName = Path.Combine(directory, fileInfo.Name);
                            if (!FilesListBox.Items.Contains(fullPathName))
                            {
                                filesInFolder.Add(fullPathName);
                            }

                        }
                    }
                }
                catch (System.Security.SecurityException ex)
                {
                    Debug.WriteLine("Error reading the file" + ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine("Error reading the file" + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error reading the file" + ex.Message);
                }


            }

            fileNameList.AddRange(filesInFolder.OrderBy(name => name));
            ItemsCounter += filesInFolder.Count;
            //TotalFilesLabel.Content = ItemsCounter.ToString();

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                TotalFilesLabel.Content = (int)o;
            }), ItemsCounter);

            try
            {
                if (IncludeSubFolders)
                {
                    //var ss = Directory.GetAccessControl(currentDirectory, AccessControlSections.Access);

                    //var ffff = ss.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                    //var sss = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    ////var ttt = ffff.Cast<AuthorizationRule>().ToList().Where(x=>x.IdentityReference.Value.Equals(sss));
                    //var yyy = ffff.Cast<FileSystemAccessRule>().ToList().Where(x => x.IdentityReference.Value.Equals(sss)).ToList();

                    //if (yyy.Any() && (yyy.First().FileSystemRights & FileSystemRights.Read) > 0)
                    //{

                    var subdirectories = directoryInfo.GetDirectories();
                    foreach (var folder in subdirectories)
                    {
                        var currentDirectory2 = Path.Combine(SearchFolder, directory);
                        var currentFolder = Path.Combine(currentDirectory2, folder.Name);
                        var fff = new FolderInformation();
                        //if (fff.IsFolderAccessible(currentFolder, FileSystemRights.Read))
                        {
                            var ff = new Regex("(?i:backup)");
                            var tt = ff.Match(folder.Name).Success;
                            if (!tt)
                            GetFoldersAndFiles(currentFolder);
                        }
                        //else
                        //{
                        //    Debug.WriteLine("You don't have access to" + currentDirectory);
                        //}
                    }
                    // if (ttt.Any() && ttt.First().)
                    //FileIOPermission
                    //var ggg = new FileIOPermission(FileIOPermissionAccess.Read, currentDirectory);


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading the folder: " + ex.Message);
            }



        }

        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListBox?.SelectedItem == null)
            {
                return;
            }
            string fileName = FilesListBox.SelectedItem.ToString().CleanFileName();
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

                var path = new FileInfo(Path.Combine(SearchFolder, fileName));

                FileTextBox.Clear();
                Task<string> file = path.OpenText().ReadToEndAsync();
                file.ContinueWith((s) =>
                {
                    FileTextBox.Text = file.Result;
                }, TaskScheduler.FromCurrentSynchronizationContext());





            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        private void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Probe = fileNameList;
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
            //MessageBox.Show(FilesListBox.SelectedItem.ToString());

            //var ss = Regex.Match(FileTextBox.Text, "html");

            //MatchLabel.Content = ss.Success.ToString();
            //MessageBox.Show(ss.Success.ToString());

            //if (ss.Success)
            //{
            //    var fff= FilesListBox.Items.CurrentItem as ListBoxItem;
            //    var fff2 = FilesListBox.Items.CurrentItem as string;
            //    var ggg = FilesListBox.ItemContainerStyle; 
            //    ;

            //}
            var CompareFilePath = "Compare.bin";


            try
            {

                var persistence = new Persistence<List<ComparePatterns>>();
                var comparerPatterns = persistence.GetConfigurationValues(CompareFilePath);

                if (comparerPatterns == null || !comparerPatterns.Any())
                {
                    return;
                }

                for (var i = 0; i < fileNameList.Count; i++)
                {
                    var fileName = fileNameList[i].CleanFileName();
                    fileNameList[i] = fileName;

                    var path = new FileInfo(Path.Combine(SearchFolder, fileName));

                    var text = path.OpenText().ReadToEndAsync().Result;


                    var validation = new PatternValidation();
                    foreach (var pattern in comparerPatterns)
                    {
                        if (!pattern.Active) continue;

                        string patternValue;
                        if (pattern.Negate)
                        {
                            patternValue = $"^({pattern.Pattern})";
                        }
                        else
                        {
                            patternValue = pattern.Pattern;
                        }
                        RegexOptions options = pattern.Case ? RegexOptions.IgnoreCase : RegexOptions.None;


                        var isMatching = validation.IsMatchingPattern(patternValue, text, options);

                        fileNameList[i] = $"{fileNameList[i]} ({isMatching})";
                    } 
                }

                FilesListBox.ItemsSource = null;
                FilesListBox.ItemsSource = fileNameList;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

   


        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new ConfigurationForm();
            window.Show();
        }

        private void MenuItem_OnChecked(object sender, RoutedEventArgs e)
        {
            //if (FilesListBox != null)
            //{
            //    FilesListBox.Visibility = Visibility.Visible;
            //    FilesGridSplitter.Visibility = Visibility.Visible;
            //}
        }

        private void FileListMenuItem_OnUnchecked(object sender, RoutedEventArgs e)
        {
            //((FilesListBox.Parent as ScrollViewer).Parent as DataGridRow).Visibility = Visibility.Collapsed;
            //(FilesGridSplitter.Parent as DataGridRow).Visibility = Visibility.Collapsed;
            ////(MessageListBox.Parent as Grid).
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Do you want to close the application?", "Close the application", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void FilesCreatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            FileCreationWindow sss = new FileCreationWindow();
            sss.Show();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            var persistence = new Persistence<FilePatternConfiguration>();

            var configuration = persistence.GetConfigurationValues(DefaultConfigFile);
            if (configuration != null)
            {
                SearchFolder = configuration.DefaultFolder;
                RootFolderTextBox.Text = SearchFolder;
                FilterPatternTextBox.Text = configuration.DefaultFilterPattern;
                UrlBaseAddressTextBox.Text = configuration.DefaultUrl;
                IncludeSubFoldersCheckBox.IsChecked = configuration.IncludeSubFolders;

            }
        }

        private void ModificationWindowMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var fileConfiguration = new FilePatternConfiguration
            {
                DefaultFolder = RootFolderTextBox.Text,
                DefaultFilterPattern = FilterPatternTextBox.Text,
                DefaultUrl = UrlBaseAddressTextBox.Text,
                IncludeSubFolders = IncludeSubFoldersCheckBox?.IsChecked ?? false
            };

            Configuration.Instance.FileConfiguration = fileConfiguration;

            var window = new ModifyFilesWindow();
            window.Show();
        }
        bool NewList = false;
        private void FilterListButton_Click(object sender, RoutedEventArgs e)
        {
            var tt = new List<string>();
            if (!NewList)
            {
                foreach (var item in fileNameList)
                {
                    if (item.Contains (" (True)"))
                    {
                        tt.Add(item);
                    }
                }

                FilesListBox.ItemsSource = tt;

            }
            else
            {
                FilesListBox.ItemsSource = null;
                FilesListBox.ItemsSource = fileNameList;
            }
            NewList = !NewList;
        }
    }
}

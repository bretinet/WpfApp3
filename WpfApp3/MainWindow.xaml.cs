using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfApp3.Extensions;
using WpfApp3.Utilities;
using Path = System.IO.Path;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DefaultConfigFile = "DefaultConfig.Config";
        private const string DefaultFolderFile = "DefaultFolder.Config";
        private const string CompareFilePath = "Compare.bin";

        private readonly SynchronizationContext synchronizationContext;

        public IEnumerable<string> PatternList { get; set; }
        private SourceCodeFile sourceCodeFile;

        public int ItemsCounter { get; set; }
        public bool IncludeSubFolders { get; set; } = true;
        private readonly List<string> fileNameList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

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
            //Configuration.Instance.FileConfiguration = new FilePatternConfiguration
            //{
            //    RootFolder = RootFolderTextBox.Text,
            //    FilterPattern = FilterPatternTextBox.Text,
            //    UrlBaseAddresst = UrlBaseAddressTextBox.Text,
            //    IncludeSubFolders = IncludeSubFoldersCheckBox?.IsChecked ?? false
            //};

            UpdateFileConfiguration();

            fileNameList.Clear();
            LoadingMessagesTextBox.Clear();

            ItemsCounter = default(int);

            IncludeSubFolders = IncludeSubFoldersCheckBox?.IsChecked ?? true;

            TotalFilesLabel.Content = " - -";
            PatternList = GetFilters();

            Task.Factory.StartNew(() =>
            {
                GetFoldersAndFiles(SearchFolder);
            }).ContinueWith(ss =>
            {
                FilesListBox.ItemsSource = null;
                FilesListBox.ItemsSource = fileNameList;
                TotalFilesLabel.Content = ItemsCounter.ToString();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            Configuration.Instance.SelectedFiles = fileNameList;
        }



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

      
        public void GetFoldersAndFiles(string directory)
        {
            var currentDirectory = Path.Combine(SearchFolder, directory);
            var directoryInfo = new DirectoryInfo(currentDirectory);


            var filesInFolder = new List<string>();

            foreach (var filterPattern in PatternList)
            {
                try
                {
                    //var folderInformation = new FolderInformation();
                    //var ttt = folderInformation.IsFolderAccessible(currentDirectory, FileSystemRights.Traverse);
                    //var exist = System.IO.Directory.Exists(currentDirectory);
                    var fffilter = new Regex("^" + filterPattern + "$");

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
                            ////var ff = new Regex("(?i:backup)");
                            ////var tt = ff.Match(folder.Name).Success;
                            ////if (!tt & !folder.Name.StartsWith(".") & !folder.Name.StartsWith("FinalResult"))
                            if (IsFolderElegible(folder.Name))
                            {
                                 GetFoldersAndFiles(currentFolder);
                            }
                            else
                            {
                                var vvv = $"Folder {Path.Combine(directory, folder.Name)} not loaded because of the folder filter pattern restrictions\n";
                                synchronizationContext.Post(new SendOrPostCallback(o =>
                                {
                                    //TotalFilesLabel.Content = (int)o;
                                    LoadingMessagesTextBox.Text += o.ToString();
                                }), vvv);
                                
                                    
                            }
                               
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

        private bool IsFolderElegible(string folderName)
        {
            FolderLoadingOptions folderOption = Configuration.Instance.FolderLoadingConfiguration.FolderLoadingOptions;

            if (folderOption == FolderLoadingOptions.AllFolders)
            {
                return true;
            }

            if (folderOption == FolderLoadingOptions.OnlySelectedFolders)
            {
                //var result = false;
                var list = Configuration.Instance.FolderLoadingConfiguration.FolderList;
                foreach (var item in list)
                {
                    var regex = new Regex(item);
                    if (regex.Match(folderName).Success)
                    {
                        return true;
                    }
                }
                return false;
            }

            if (folderOption == FolderLoadingOptions.AllFolderExcept)
            {
                //var result = false;
                var list = Configuration.Instance.FolderLoadingConfiguration.FolderList;
                foreach (var item in list)
                {
                    var regex = new Regex(item);
                    if (regex.Match(folderName).Success)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListBox?.SelectedItem == null)
            {
                return;
            }

            var fileName = FilesListBox.SelectedItem.ToString().CleanFileName();
            var path = new FileInfo(Path.Combine(SearchFolder, fileName));

            var result = IoUtilities.ReadStringFromFile(path.FullName);

            if (result != null)
            {
                FileTextBox.Text = result;
            }
        }

        private void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateFileConfiguration();
            var serverFilesCheckWindow = new ServerFilesCheckWindow {Owner = this};
            serverFilesCheckWindow.Show();
        }

        private void ComparePatternsButton_Click(object sender, RoutedEventArgs e)
        {
            var CompareFilePath = "Compare.bin";


            try
            {

                //var persistence = new Persistence<List<ComparePatterns>>();
                //persistence.GetConfigurationValues(CompareFilePath);
                var comparerPatterns = Configuration.Instance.ComparePatterns;

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
                        if (!pattern.ActiveLocal) continue;

                        string patternValue;
                        //if (pattern.ActiveRemote)
                        //{
                        //    patternValue = $"{pattern.Pattern}";
                        //}
                        //else
                        //{
                        //    patternValue = pattern.Pattern;
                        //}
                        RegexOptions options = pattern.Case ? RegexOptions.IgnoreCase : RegexOptions.None;


                        var isMatching = validation.IsMatchingPattern(pattern.Pattern, text, options);

                        fileNameList[i] = isMatching? $"{fileNameList[i]} ({pattern.Result})": $"{fileNameList[i]} (False)";
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
            UpdateFileConfiguration();
            var window = new ConfigurationForm {Owner = this};
            window.Show();
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

        }

        private void ModificationWindowMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var fileConfiguration = new FilePatternConfiguration
            {
                RootFolder = RootFolderTextBox.Text,
                FilterPattern = FilterPatternTextBox.Text,
                UrlBaseAddresst = UrlBaseAddressTextBox.Text,
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
                    if (item.Contains(" (True)"))
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

        private void SaveFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (FilesListBox.SelectedItems.Count != 1)
            {
                return;
            }

            var fileName = FilesListBox.SelectedItem.ToString().CleanFileName();
            var response = MessageBox.Show($"Do you want to modify the file {fileName}?", "Modify file", MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (response != MessageBoxResult.Yes)
            {
                return;
            }

            var path = new FileInfo(Path.Combine(SearchFolder, fileName));
            var currentText = FileTextBox.Text;


            var originalText = IoUtilities.ReadStringFromFile(path.FullName);
            if (originalText.Equals(currentText))
            {
                MessageBox.Show("Both files are identical", "Modify file", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            IoUtilities.WriteStringToFile(path.FullName, currentText);
            MessageBox.Show($"The content of the file {fileName} has been changed.", "Modify file", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RestoreFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var file = GetListBoxSelectedFile();

            if (file == null)
            {
                return;
            }

            var originalText = IoUtilities.ReadStringFromFile(file.FullName);

            if (originalText.Equals(FileTextBox.Text))
            {
                MessageBox.Show("Both files are identical", "Restore files", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            else
            {
                var response  = MessageBox.Show("Do you want to restore the original file?", "Restore files", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (response == MessageBoxResult.Yes)
                {
                    FileTextBox.Text = originalText;
                }
            }



        }


        private FileInfo GetListBoxSelectedFile()
        {
            if (FilesListBox.SelectedItem == null)
            {
                return null;
            }

            var fileName = FilesListBox.SelectedItem.ToString().CleanFileName();
            return  new FileInfo(Path.Combine(SearchFolder, fileName));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var persistence = new Persistence<FilePatternConfiguration>();

            var configuration = persistence.GetConfigurationValues(DefaultConfigFile);

            if (configuration != null)
            {
                Configuration.Instance.DefaultFileConfiguration = configuration;
                Configuration.Instance.FileConfiguration = Configuration.Instance.DefaultFileConfiguration;
                SearchFolder = Configuration.Instance.FileConfiguration.RootFolder;

            }

            //if (configuration != null)
            //{
            //    SearchFolder = configuration.RootFolder;
            //    RootFolderTextBox.Text = SearchFolder;
            //    FilterPatternTextBox.Text = configuration.FilterPattern;
            //    UrlBaseAddressTextBox.Text = configuration.UrlBaseAddresst;
            //    IncludeSubFoldersCheckBox.IsChecked = configuration.IncludeSubFolders;
            //    Configuration.Instance.FileConfiguration.RootFolder = configuration.RootFolder;
            //    configuration.
            //}

            RootFolderTextBox.Text = Configuration.Instance.FileConfiguration.RootFolder;
            FilterPatternTextBox.Text = Configuration.Instance.FileConfiguration.FilterPattern;
            UrlBaseAddressTextBox.Text = Configuration.Instance.FileConfiguration.UrlBaseAddresst;
            IncludeSubFoldersCheckBox.IsChecked = Configuration.Instance.FileConfiguration.IncludeSubFolders;

            var persistence2 = new Persistence<FolderLoadingConfiguration>();

            var configuration2 = persistence2.GetConfigurationValues(DefaultFolderFile);
            if (configuration != null)
            {
                Configuration.Instance.FolderLoadingConfiguration = configuration2;

            }


            var comparenPatterns = new Persistence<List<ComparePatterns>>();

           Configuration.Instance.ComparePatterns = comparenPatterns.GetConfigurationValues(CompareFilePath);
 
        }

        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
           Clipboard.SetText(FileTextBox.SelectedText);
        }

        private void CutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(FileTextBox.SelectedText);
            FileTextBox.SelectedText = string.Empty;
        }

        private void PasteMenuItem_OnClick(object sender, RoutedEventArgs e)
        {

            FileTextBox.Paste();
        }

        private void UpdateFileConfiguration()
        {
            Configuration.Instance.FileConfiguration.RootFolder = RootFolderTextBox.Text;
            Configuration.Instance.FileConfiguration.FilterPattern = FilterPatternTextBox.Text;
            Configuration.Instance.FileConfiguration.UrlBaseAddresst = UrlBaseAddressTextBox.Text;
            Configuration.Instance.FileConfiguration.IncludeSubFolders = IncludeSubFoldersCheckBox?.IsChecked ?? false;
        }
    }
}

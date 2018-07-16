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

namespace WpfApp3.User_Controls
{
    /// <summary>
    /// Interaction logic for FilterFolderUserControl.xaml
    /// </summary>
    public partial class FilterFolderUserControl : UserControl
    {
        private const string DefaultFolderFile = "DefaultFolder.Config";
        internal List<string> FolderList =new List<string>();

        public FilterFolderUserControl()
        {
            InitializeComponent();


            var persistence = new Persistence<FolderLoadingConfiguration>();

            var configuration = persistence.GetConfigurationValues(DefaultFolderFile);
            if (configuration != null)
            {

                if (configuration.FolderLoadingOptions == FolderLoadingOptions.OnlySelectedFolders)
                {
                    OnlyFoldersRadioButton.IsChecked = true;
                }
                else if (configuration.FolderLoadingOptions == FolderLoadingOptions.AllFolderExcept)
                {
                    AllFoldersExceptRadioButton.IsChecked = true;
                }
                else
                {
                    AllFoldersRadioButton.IsChecked = true;
                }
                FolderList = configuration.FolderList;
                FoldersDataGrid.ItemsSource = FolderList;
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (FoldersDataGrid.SelectedIndex > -1 && FoldersDataGrid.SelectedItems.Count == 1)
            {
                var index = FoldersDataGrid.SelectedIndex;

                FolderList.RemoveAt(index);

                FoldersDataGrid.ItemsSource = null;
                FoldersDataGrid.ItemsSource = FolderList;
            }
        }

        private void ModifyButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (FoldersDataGrid.SelectedIndex > -1 && FoldersDataGrid.SelectedItems.Count == 1)
            {
                var index = FoldersDataGrid.SelectedIndex;

                FolderList[index] = FolderTextBox.Text;


                FoldersDataGrid.ItemsSource = null;
                FoldersDataGrid.ItemsSource = FolderList;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (FolderList == null)
            {
                FolderList = new List<string>();
            }

            FolderList.Add(FolderTextBox.Text);

            FoldersDataGrid.ItemsSource = null;
            FoldersDataGrid.ItemsSource = FolderList.ToList();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var persistence = new Persistence<FolderLoadingConfiguration>();

            try
            {
                FolderLoadingOptions folderOption;

                if (OnlyFoldersRadioButton.IsChecked != null && OnlyFoldersRadioButton.IsChecked.Value)
                {
                    folderOption = FolderLoadingOptions.OnlySelectedFolders;
                }
                else if (AllFoldersExceptRadioButton.IsChecked != null && AllFoldersExceptRadioButton.IsChecked.Value)
                {
                    folderOption = FolderLoadingOptions.AllFolderExcept;
                }
                else
                {
                    folderOption = FolderLoadingOptions.AllFolders;
                }

                var folderLoadingConfiguration = new FolderLoadingConfiguration()
                {
                    FolderLoadingOptions = folderOption,
                    FolderList = FolderList

                };

                persistence.SetConfigurationValues(DefaultFolderFile, folderLoadingConfiguration);
                Configuration.Instance.FolderLoadingConfiguration = folderLoadingConfiguration;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(exception.Message);
            }
        }

        private void ComparePatternsDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FoldersDataGrid.SelectedIndex > -1 && FoldersDataGrid.SelectedItems.Count == 1)
            {
                var index = FoldersDataGrid.SelectedIndex;

                FolderTextBox.Text = FolderList[index];
            }
        }

        private void AllFoldersRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

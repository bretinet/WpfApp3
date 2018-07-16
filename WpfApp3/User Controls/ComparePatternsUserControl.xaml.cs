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

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for ComparePatternsUserControl.xaml
    /// </summary>
    public partial class ComparePatternsUserControl : UserControl
    {
        private const string CompareFilePath = "Compare.bin";
        List<ComparePatterns> comparePatterns = new List<ComparePatterns>();
        public ComparePatternsUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comparePatterns == null)
            {
                comparePatterns = new List<ComparePatterns>();
            }

            comparePatterns.Add(new ComparePatterns
            {
                Pattern = PatternTextBox.Text,
                Result = ResultComboBox.SelectionBoxItem.ToString(),
                ActiveLocal = ActiveLocalCheckBox?.IsChecked ?? false,
                Case = CaseCheckBox?.IsChecked ?? false,
                ActiveRemote = ActiveRemoteCheckBox?.IsChecked ?? false
            });

            ComparePatternsDataGrid.ItemsSource = null;
            ComparePatternsDataGrid.ItemsSource = comparePatterns;
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            if (Configuration.Instance.ComparePatterns == null)
            {
                var persistence = new Persistence<List<ComparePatterns>>();

                comparePatterns = persistence.GetConfigurationValues(CompareFilePath);
                Configuration.Instance.ComparePatterns = comparePatterns;
            }
            else
            {
                comparePatterns = Configuration.Instance.ComparePatterns;
            }


            ComparePatternsDataGrid.ItemsSource = comparePatterns;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var sss = new Persistence<List<ComparePatterns>>();

            sss.SetConfigurationValues(CompareFilePath, comparePatterns);

            Configuration.Instance.ComparePatterns = comparePatterns;
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComparePatternsDataGrid.SelectedIndex > -1 && ComparePatternsDataGrid.SelectedItems.Count == 1)
            {
                var index = ComparePatternsDataGrid.SelectedIndex;

                comparePatterns[index].Pattern = PatternTextBox.Text;
                comparePatterns[index].Result = ResultComboBox.SelectionBoxItem.ToString();
                comparePatterns[index].ActiveLocal = ActiveLocalCheckBox?.IsChecked ?? false;
                comparePatterns[index].ActiveRemote = ActiveRemoteCheckBox?.IsChecked ?? false;
                comparePatterns[index].Case = CaseCheckBox?.IsChecked ?? false;

                ComparePatternsDataGrid.ItemsSource = null;
                ComparePatternsDataGrid.ItemsSource = comparePatterns;
            }
        }

        private void ComparePatternsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ComparePatternsDataGrid.SelectedIndex > -1 && ComparePatternsDataGrid.SelectedItems.Count == 1)
            {
                var index = ComparePatternsDataGrid.SelectedIndex;

                PatternTextBox.Text = comparePatterns[index].Pattern;
                var result = comparePatterns[index].Result;
                if (result == "False") ResultComboBox.SelectedIndex = 0;
                if (result == "True") ResultComboBox.SelectedIndex = 1;
                if (result == "Warning") ResultComboBox.SelectedIndex = 2;
                if (result == "Error") ResultComboBox.SelectedIndex = 3;
                ActiveLocalCheckBox.IsChecked = comparePatterns[index].ActiveLocal;
                ActiveRemoteCheckBox.IsChecked = comparePatterns[index].ActiveRemote;
                CaseCheckBox.IsChecked = comparePatterns[index].Case;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComparePatternsDataGrid.SelectedIndex > -1 && ComparePatternsDataGrid.SelectedItems.Count == 1)
            {
                var index = ComparePatternsDataGrid.SelectedIndex;

                comparePatterns.RemoveAt(index);

                ComparePatternsDataGrid.ItemsSource = null;
                ComparePatternsDataGrid.ItemsSource = comparePatterns;
            }
        }
    }
}

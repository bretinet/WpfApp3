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
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for FolderPatternWindow.xaml
    /// </summary>
    public partial class FolderPatternWindow : Window
    {
        List<PatternFolder> patternFolder;
        public FolderPatternWindow()
        {
            InitializeComponent();
            patternFolder = new List<PatternFolder>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            patternFolder.Add(
                new PatternFolder
                {
                    Name = PatternFolderTextBox.Text
                }
            );

            FolderPatternsDataGrid.ItemsSource = null;
            FolderPatternsDataGrid.ItemsSource = patternFolder;

            var pattenFolderFactory = new PattenFolderFactory();
            pattenFolderFactory.WritePatternFolders(patternFolder);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (FolderPatternsDataGrid.SelectedIndex > -1)
            {
                var index = FolderPatternsDataGrid.SelectedIndex;


                patternFolder.RemoveAt(index);

                FolderPatternsDataGrid.ItemsSource = null;
                FolderPatternsDataGrid.ItemsSource = patternFolder;

                var pattenFolderFactory = new PattenFolderFactory();
                pattenFolderFactory.WritePatternFolders(patternFolder);

            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            var pattenFolderFactory = new PattenFolderFactory();
            patternFolder = pattenFolderFactory.ReadPatternFolders().ToList();

            FolderPatternsDataGrid.ItemsSource = null;
            FolderPatternsDataGrid.ItemsSource = patternFolder;
        }
    }
}

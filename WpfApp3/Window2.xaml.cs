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
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        List<PatternCheck> patternItems = new List<PatternCheck>();
        public Window2()
        {
            InitializeComponent();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            patternItems.Add(new PatternCheck
            {
                Pattern = PatternTextBox.Text,
                Value = GetValue( ValueComboBox.SelectedIndex)
            });
            PatternsDataGrid.ItemsSource = null;
            PatternsDataGrid.ItemsSource = patternItems;

            using (Stream stream = File.Open("items.bin", FileMode.Create))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, patternItems);
            }


        }

        private string GetValue(int value)
        {
            if (value == 1)return "True";
            if (value == 2) return "Warning";
            return "False";
        }

       

        private void Window_Activated(object sender, EventArgs e)
        {

            using (Stream stream = File.Open("items.bin", FileMode.OpenOrCreate))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                if (stream.Length > 0)
                {
                    patternItems = (List<PatternCheck>)formatter.Deserialize(stream);
                }
            }

            PatternsDataGrid.ItemsSource = patternItems;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (PatternsDataGrid.SelectedIndex > -1)
            {
                var fff = PatternsDataGrid.SelectedIndex;

                
                patternItems.RemoveAt(fff);

                PatternsDataGrid.ItemsSource = null;
                PatternsDataGrid.ItemsSource = patternItems;

                using (Stream stream = File.Open("items.bin", FileMode.Create))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, patternItems);
                }
            }
        }
    }
}

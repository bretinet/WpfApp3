using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        internal IList<string> Probe;
        public Window1()
        {
            InitializeComponent();
        }

        private void CheckWebButton_Click(object sender, RoutedEventArgs e)
        {


            GetWebPages();

        }

        private void UpdateUI()
        {
            CheckWebButton.Content = dictionary.Count.ToString();
        }

        Dictionary<string, HttpResponseMessage> dictionary = new Dictionary<string, HttpResponseMessage>();
        private void GetWebPages()
        {            

            for (int i = 0; i < 20; i++)
            {
                try
                {
                    var httpClient = new HttpClient();
                    //var selectedItem = ItemsListBox.SelectedItem.ToString().Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
                    var selectedItem = ItemsListBox.Items[i].ToString().Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");

                    var url = "https://cpt-icedev.datacash.co.za/" + selectedItem;
                    var httpClientResponse = httpClient.GetAsync(url).ContinueWith((response) =>
                    {
                        dictionary.Add(selectedItem, response.Result);

                    }).ContinueWith(re =>
                   {
                       CheckWebButton.Content = dictionary.Count.ToString();
                        //if (re.IsCompleted)
                        //{
                        //    UpdateUI();
                        //}
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    //dictionary.Add(selectedItem, httpClientResponse);
                    // var header = httpClientResponse.Headers.ToString();
                    //var returnedValue = httpClientResponse.Content.ReadAsStringAsync().Result;
                    //var contentHeader = httpClientResponse.Content.Headers.ToString();

                    //HeaderTextBox.Text = header;
                    //HeaderTextBox.Text += contentHeader;
                    //return await;
                    //HtmlTextBox.Text = returnedValue;
                    //var temp 

                    //if (!string.IsNullOrWhiteSpace(returnedValue))
                    //{
                    //    WebBrowserHTML.NavigateToString(returnedValue);
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }



        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Probe = Probe.ToList();
            for (int i = 0; i < Probe.Count(); i++)
            {
                Probe[i] = Probe[i].Replace(@"\", "/");

            }

            ItemsListBox.ItemsSource = Probe;
        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ItemsListBox.SelectedItem.ToString().Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
            if (dictionary.ContainsKey(selectedItem))
            {
                HttpResponseMessage ssss;
                var isHttpMessage = dictionary.TryGetValue(selectedItem, out ssss);
                if (isHttpMessage)
                {
                    var header = ssss.Headers.ToString();
                    var returnedValue = ssss.Content.ReadAsStringAsync().Result;
                    var contentHeader = ssss.Content.Headers.ToString();

                    HeaderTextBox.Text = header;
                    HeaderTextBox.Text += contentHeader;
                    //return await;
                    HtmlTextBox.Text = returnedValue;

                    if (!string.IsNullOrWhiteSpace(returnedValue))
                    {
                        WebBrowserHTML.NavigateToString(returnedValue);
                    }
                }


            }
            else
            {
                MessageBox.Show("Item not loaded yet!");
            }
        }
    }
}

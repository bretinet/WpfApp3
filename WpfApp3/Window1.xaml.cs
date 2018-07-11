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
        internal IList<string> UrlList;
        public Window1()
        {
            InitializeComponent();



            var defaultFolder = Configuration.Instance.FileConfiguration.DefaultFolder;//.Replace("\\", "/");
            //Probe = Probe.ToList();
            //for (int i = 0; i < Probe.Count(); i++)
            //{
            //    Probe[i] = Probe[i].Replace(@"\", "/");

            //}

            //ItemsListBox.ItemsSource = Probe;
            var urlList = new List<string>();
            var fileList = Configuration.Instance.SelectedFiles;
            for (var i = 0; i < fileList.Count; i++)
            {
                var urlItem = fileList[i].Replace(defaultFolder, "").Replace("\\ASPWebsite", "").Replace("\\ICE_Local", "").Replace("\\", "/").Replace(" (True)", "").Replace(" (False)", "");
                if (urlItem.StartsWith("/")) urlItem.Remove(0, 1);
                urlList.Add(urlItem);
            }

            Configuration.Instance.UrlFiles = urlList;
            ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
        }

        private void CheckWebButton_Click(object sender, RoutedEventArgs e)
        {


            GetWebPages();

        }

        private void UpdateUI()
        {
            CheckWebButton.Content = dictionary.Count.ToString();
        }

        Dictionary<string, Container> dictionary = new Dictionary<string, Container>();
        private void GetWebPages()
        {
            var defaultFolder = Configuration.Instance.FileConfiguration.DefaultFolder.Replace("\\", "/");

            var ssss = new System.Text.RegularExpressions.Regex("id=\"frmLogon\"");
            var items = Configuration.Instance.UrlFiles;

            for (int i = 0; i < items.Count; i++) //
            {
                try
                {

                    var httpClient = new HttpClient();
                    //var selectedItem = ItemsListBox.SelectedItem.ToString().Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
                    var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");

                    var url = Configuration.Instance.FileConfiguration.DefaultUrl + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
                    var httpClientResponse = httpClient.GetAsync(url).ContinueWith((response) =>
                    {

                        var returnedValue = response.Result.Content.ReadAsStringAsync().Result;

                        dictionary.Add(selectedItem, new Container()
                        {
                            message = response.Result,
                            result = ssss.Match(returnedValue).Success
                        });
                            

                        CheckWebButton.Content = dictionary.Count.ToString();

                        
                        //if (ssss.Match(returnedValue).Success)
                        //{
                        //    items[i] = items[i] + " (True)";
                        //}
                        //else
                        //{
                        //    items[i] = items[i] + " (False)";
                        //}


                    }, TaskScheduler.FromCurrentSynchronizationContext());

                    System.Threading.Tasks.Task.Delay(200);
                    // .ContinueWith(re =>
                    //{
                    //    dictionary.Add(selectedItem, re.);




                    //    System.Threading.Tasks.Task.Delay(100);
                    //});
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

            
        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var defaultFolder = Configuration.Instance.FileConfiguration.DefaultFolder.Replace("\\", "/");
            var selectedItem = ItemsListBox.SelectedItem.ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
            if (dictionary.ContainsKey(selectedItem))
            {
                Container container;
                var isHttpMessage = dictionary.TryGetValue(selectedItem, out container);
                if (isHttpMessage)
                {
                    var header = container.message.Headers.ToString();
                    var returnedValue = container.message.Content.ReadAsStringAsync().Result;
                    var contentHeader = container.message.Content.Headers.ToString();

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

        private void CheckContent_Click(object sender, RoutedEventArgs e)
        {

            //Configuration.Instance.UrlFiles = items;
            //ItemsListBox.ItemsSource = null;
            //ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;

            for (var i = 0; i < Configuration.Instance.UrlFiles.Count; i++)
            {
                var t = Configuration.Instance.UrlFiles[i];

                if (dictionary.ContainsKey(t))
                {
                    var v = dictionary[t].result;

                    if (v)
                    {
                        Configuration.Instance.UrlFiles[i] = Configuration.Instance.UrlFiles[i] + " (True)";
                    }
                    else
                    {
                        Configuration.Instance.UrlFiles[i] = Configuration.Instance.UrlFiles[i] + " (False)";
                    }
                }
            }

            ItemsListBox.ItemsSource = null;
            ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
        }
    }

    internal class Container
    {
        internal HttpResponseMessage message;
        internal bool result;
    }
}

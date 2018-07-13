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
            TotalFilesLabel.Content = Configuration.Instance.UrlFiles.Count;
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
            dictionary.Clear();

            var defaultFolder = Configuration.Instance.FileConfiguration.DefaultFolder.Replace("\\", "/");

            var ssss = new System.Text.RegularExpressions.Regex("id=\"frmLogon\"");
            var items = Configuration.Instance.UrlFiles;

            if (items == null || !items.Any())
            {
                return;
            }
            int itemsInARow = 200;

            //var calculation = items.Count % itemsInARow == 0 ? items.Count / itemsInARow : (items.Count / itemsInARow) + 1;

            //for (var z = 0; z < calculation; z++)
            {
                //i < System.Math.Max(itemsInARow,  items.Count - (z*itemsInARow))    ///itemsInARow * z

                for (var i = 0; i < items.Count; i++) //
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

                            ProcesedFilesLabel.Content = dictionary.Count.ToString();
                            //CheckWebButton.Content = dictionary.Count.ToString();


                            //if (ssss.Match(returnedValue).Success)
                            //{
                            //    items[i] = items[i] + " (True)";
                            //}
                            //else
                            //{
                            //    items[i] = items[i] + " (False)";
                            //}
                            response.Dispose();
                            //httpClient = null;
                            //httpClient.Dispose();

                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        if (i % 10 == 0)
                        {
                            System.Threading.Tasks.Task.Delay(400);
                        }
                        else
                        {
                            System.Threading.Tasks.Task.Delay(200);
                        }

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
                System.Threading.Tasks.Task.Delay(3000);
            }


        }

        private void Window_Activated(object sender, EventArgs e)
        {


        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var defaultFolder = Configuration.Instance.FileConfiguration.DefaultFolder.Replace("\\", "/");
            var selectedItem2 = ItemsListBox.SelectedItem;
            if (selectedItem2 == null) return;

            var selectedItem = selectedItem2?.ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
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
            var trueValueCounter = 0;
            var falseValueCounter = 0;


            var tempList = Configuration.Instance.UrlFiles;

            for (var i = 0; i < tempList.Count; i++)
            {
                var item = tempList[i];

                if (dictionary.ContainsKey(item))
                {
                    var v = dictionary[item].result;

                    if (v)
                    {
                        tempList[i] = tempList[i] + " (True)";
                        trueValueCounter++;
                        //var vv = dictionary[item].message;

                        //if (vv.Content.ReadAsStringAsync().Result.Contains("id=\"frmLogon\""))
                        //{

                        //    tempList[i] = tempList[i] + " (True)";

                        //}
                        //else
                        //{
                        //    tempList[i] = tempList[i] + " (False)";
                        //}
                    }
                    else
                    {
                        tempList[i] = tempList[i] + " (False)";
                        falseValueCounter++;
                    }
                }
            }

            Configuration.Instance.UrlFiles = tempList;
            ItemsListBox.ItemsSource = null;
            ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;

            TrueFilesLabel.Content = trueValueCounter;
            FalseFilesLabel.Content = falseValueCounter;
        }
        int filterRound = 0;
        private void FilterContent_Click(object sender, RoutedEventArgs e)
        {
            filterRound = ++filterRound % 3;
            if (filterRound == 1)
            {
                ItemsListBox.ItemsSource = null;
                ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles.Where(x => x.Contains(" (True)"));
            }
            else if (filterRound == 2)
            {
                ItemsListBox.ItemsSource = null;
                ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles.Where(x => x.Contains(" (False)"));
            }
            else
            {
                ItemsListBox.ItemsSource = null;
                ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
            }

            //TrueFilesLabel.Content = ItemsListBox.Items.Count;
            //FalseFilesLabel.Content = id ItemsListBox.Items.Count;
        }
    }

    internal class Container
    {
        internal HttpResponseMessage message;
        internal bool result;
    }
}

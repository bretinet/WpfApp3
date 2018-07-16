using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp3.Extensions;

namespace WpfApp3
{
    public partial class ServerFilesCheckWindow : Window
    {
        private readonly SynchronizationContext synchronizationContext;
        internal IList<string> UrlList;
        public ServerFilesCheckWindow()
        {
            InitializeComponent();



            var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder;//.Replace("\\", "/");
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
                if (urlItem.StartsWith("/"))
                {
                    urlItem = urlItem.Remove(0, 1);
                }
                urlList.Add(urlItem);
            }

            Configuration.Instance.UrlFiles = urlList;
            ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;

            synchronizationContext = SynchronizationContext.Current;
        }

        private void CheckWebButton_Click(object sender, RoutedEventArgs e)
        {


            GetWebPages();

        }
       
        readonly ConcurrentDictionary<string, Container> _dictionary = new ConcurrentDictionary<string, Container>();
        private void GetWebPages()
        {
            _dictionary.Clear();

            var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");

            var regex = new System.Text.RegularExpressions.Regex("id=\"frmLogon\"");
            var items = Configuration.Instance.UrlFiles;

            Func<string, Container, Container> f1;
            f1 = (s, container) => new Container()
            {
                result = false,
                message = container.message

            };
            SemaphoreSlim maxThread = new SemaphoreSlim(2);

            for (int i = 0; i < items.Count; i++) //
            {

                var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");

                var url = Configuration.Instance.FileConfiguration.UrlBaseAddresst + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
 var httpClient = new HttpClient { Timeout = new TimeSpan(0, 0, 0, 20) };
                maxThread.Wait();
                Task.Factory.StartNew(() =>
                {
                   

                    var response = httpClient.GetAsync(url).Result;
                    //var returnedValue = response.Content.ReadAsStringAsync().Result;


                    _dictionary.AddOrUpdate(selectedItem, new Container()
                    {
                        message = response,
                        //result = regex.Match(returnedValue).Success
                    }, f1);


                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {
                        CheckWebButton.Content = o.ToString();
                    }), _dictionary.Count);

                    //    CheckWebButton.Content = o.ToString();
                    //}), dictionary.Count.ToString());

                })

                .ContinueWith((task) => maxThread.Release())




                ;


                /*
                try
                {

                    var httpClient = new HttpClient();
                    //var selectedItem = ItemsListBox.SelectedItem.ToString().Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
                    var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");

                    var url = Configuration.Instance.FileConfiguration.UrlBaseAddresst + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
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
                }*/
            }
        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");
            var selectedItem2 = ItemsListBox.SelectedItem;
            if (selectedItem2 == null) return;

            var selectedItem = selectedItem2.ToString().CleanFileName().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "");
            if (_dictionary.ContainsKey(selectedItem))
            {
                Container container;
                var isHttpMessage = _dictionary.TryGetValue(selectedItem, out container);
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

            var patterns = Configuration.Instance.ComparePatterns.Where(x=>x.ActiveRemote);

            //Configuration.Instance.UrlFiles = items;
            //ItemsListBox.ItemsSource = null;
            //ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
            var tempList = Configuration.Instance.UrlFiles;

            for (var i = 0; i < tempList.Count; i++)
            {
                var item = tempList[i];

                foreach (var pattern  in patterns)
                {
                    var xxx = new Regex(pattern.Pattern);
                    if (_dictionary.ContainsKey(item))
                    {
                        var g = _dictionary[item];

                        var ttt = ((Container) g).message.Content.ReadAsStringAsync().Result;

                        if (xxx.Match(ttt).Success)
                        {
                            tempList[i] = tempList[i] +  $" ({pattern.Result})";
                        }
                        else
                        {
                            tempList[i] = tempList[i] + " (False)";
                        }
                    }
                }

                //if (_dictionary.ContainsKey(item))
                //{
                //    var v = _dictionary[item].result;

                //    if (v)
                //    {
                //        tempList[i] = tempList[i] + " (True)";

                //        //var vv = dictionary[item].message;

                //        //if (vv.Content.ReadAsStringAsync().Result.Contains("id=\"frmLogon\""))
                //        //{

                //        //    tempList[i] = tempList[i] + " (True)";

                //        //}
                //        //else
                //        //{
                //        //    tempList[i] = tempList[i] + " (False)";
                //        //}
                //    }
                //    else
                //    {
                //        tempList[i] = tempList[i] + " (False)";
                //    }
                //}
            }

            Configuration.Instance.UrlFiles = tempList;
            ItemsListBox.ItemsSource = null;
            ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
        }

        private void FilterContent_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    internal class Container
    {
        internal HttpResponseMessage message;
        internal bool result;
    }
}

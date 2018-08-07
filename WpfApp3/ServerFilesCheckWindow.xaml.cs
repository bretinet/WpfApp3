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
            TotalFilesLabel.Content = Configuration.Instance.UrlFiles.Count.ToString();
        }

        private void CheckWebButton_Click(object sender, RoutedEventArgs e)
        {


            GetWebPages();

        }
        private object dictionaryLock = new object();
        readonly Dictionary<string, Container> _dictionary = new Dictionary<string, Container>();
        //readonly ConcurrentDictionary<string, Container> _dictionary = new ConcurrentDictionary<string, Container>();
        private void GetWebPages()
        {
            _dictionary.Clear();

            var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");

            var regex = new System.Text.RegularExpressions.Regex("id=\"frmLogon\"");
            var items = Configuration.Instance.UrlFiles;

            //Func<string, Container, Container> f1;
            //f1 = (s, container) => new Container()
            //{
            //    result = false,
            //    message = container.message

            //};
            //SemaphoreSlim maxThread = new SemaphoreSlim(2);

            ////for (int i = 0; i < items.Count; i++) //
            ////{

            ////    var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");

            ////    var url = Configuration.Instance.FileConfiguration.UrlBaseAddresst + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
            ////    var httpClient = new HttpClient { Timeout = new TimeSpan(0, 0, 0, 20) };
            ////    maxThread.Wait();
            ////    Task.Factory.StartNew(() =>
            ////    {


            ////        var response = httpClient.GetAsync(url).Result;
            ////        //var returnedValue = response.Content.ReadAsStringAsync().Result;


            ////        _dictionary.AddOrUpdate(selectedItem, new Container()
            ////        {
            ////            message = response,
            ////            //result = regex.Match(returnedValue).Success
            ////        }, f1);


            ////        synchronizationContext.Post(new SendOrPostCallback(o =>
            ////        {
            ////            CheckWebButton.Content = o.ToString();
            ////        }), _dictionary.Count);

            ////        //    CheckWebButton.Content = o.ToString();
            ////        //}), dictionary.Count.ToString());

            ////    })

            ////    .ContinueWith((task) => maxThread.Release());


            ////}


            var httpClient = new HttpClient();

            for (int i = 0; i < items.Count; i++)
            {
                var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
                var url = Configuration.Instance.FileConfiguration.UrlBaseAddress + selectedItem; //"https://cpt-icedev.datacash.co.za/" 

                GetUrlResponse(url);
                //try
                //{

                //    var httpClient = new HttpClient();
                //    //var selectedItem = ItemsListBox.SelectedItem.ToString().Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
                //    var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");

                //    var url = Configuration.Instance.FileConfiguration.UrlBaseAddresst + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
                //    var httpClientResponse = httpClient.GetAsync(url).ContinueWith((response) =>
                //    {

                //        var returnedValue = response.Result.Content.ReadAsStringAsync().Result;

                //        lock (dictionaryLock)
                //        {


                //            _dictionary.Add(selectedItem, new Container()
                //            {
                //                message = response.Result,
                //                result = regex.Match(returnedValue).Success
                //            });
                //        }



                //        //CheckWebButton.Content = _dictionary.Count.ToString();
                //        ProcesedFilesLabel.Content = _dictionary.Count.ToString();

                //        //if (ssss.Match(returnedValue).Success)
                //        //{
                //        //    items[i] = items[i] + " (True)";
                //        //}
                //        //else
                //        //{
                //        //    items[i] = items[i] + " (False)";
                //        //}


                //    }, TaskScheduler.FromCurrentSynchronizationContext());

                //    System.Threading.Tasks.Task.Delay(200);
                //    // .ContinueWith(re =>
                //    //{
                //    //    dictionary.Add(selectedItem, re.);




                //    //    System.Threading.Tasks.Task.Delay(100);
                //    //});
                //    //dictionary.Add(selectedItem, httpClientResponse);
                //    // var header = httpClientResponse.Headers.ToString();
                //    //var returnedValue = httpClientResponse.Content.ReadAsStringAsync().Result;
                //    //var contentHeader = httpClientResponse.Content.Headers.ToString();

                //    //HeaderTextBox.Text = header;
                //    //HeaderTextBox.Text += contentHeader;
                //    //return await;
                //    //HtmlTextBox.Text = returnedValue;
                //    //var temp 

                //    //if (!string.IsNullOrWhiteSpace(returnedValue))
                //    //{
                //    //    WebBrowserHTML.NavigateToString(returnedValue);
                //    //}
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
            }
        }


        private void GetUrlResponse(string Url, HttpClient httpClient = null)
        {
            try
            {
                if (httpClient == null)
                {
                    httpClient = new HttpClient();
                }
                ////var selectedItem = ItemsListBox.SelectedItem.ToString().Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");
                //var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "");

                //var url = Configuration.Instance.FileConfiguration.UrlBaseAddresst + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
                var httpClientResponse = httpClient.GetAsync(Url).ContinueWith((response) =>
                {
                    //var regex = new System.Text.RegularExpressions.Regex("id=\"frmLogon\"");
                    //var returnedValue = response.Result.Content.ReadAsStringAsync().Result;

                    lock (dictionaryLock)
                    {


                        _dictionary.Add(Url, new Container()
                        {
                            message = response.Result,
                            //result = regex.Match(returnedValue).Success
                        });
                    }



                    //CheckWebButton.Content = _dictionary.Count.ToString();
                    ProcesedFilesLabel.Content = _dictionary.Count.ToString();


                }, TaskScheduler.FromCurrentSynchronizationContext());

                //System.Threading.Tasks.Task.Delay(200);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");
            //var selectedItem2 = ItemsListBox.SelectedItem;
            //if (selectedItem2 == null) return;

            //var selectedItem = selectedItem2.ToString().CleanFileName().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "");
            //if (_dictionary.ContainsKey(selectedItem))
            //{
            //    Container container;
            //    var isHttpMessage = _dictionary.TryGetValue(selectedItem, out container);
            //    if (isHttpMessage)
            //    {
            //        var header = container.message.Headers.ToString();
            //        var returnedValue = container.message.Content.ReadAsStringAsync().Result;
            //        var contentHeader = container.message.Content.Headers.ToString();

            //        HeaderTextBox.Text = header;
            //        HeaderTextBox.Text += contentHeader;
            //        //return await;
            //        HtmlTextBox.Text = returnedValue;

            //        if (!string.IsNullOrWhiteSpace(returnedValue))
            //        {
            //            WebBrowserHTML.NavigateToString(returnedValue);
            //        }
            //    }


            //}
            //else
            //{
            //    MessageBox.Show("Item not loaded yet!");
            //}
        }

        private void CheckContent_Click(object sender, RoutedEventArgs e)
        {

            var patterns = Configuration.Instance.ComparePatterns.Where(x => x.ActiveRemote);

            //Configuration.Instance.UrlFiles = items;
            //ItemsListBox.ItemsSource = null;
            //ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
            var tempList = Configuration.Instance.UrlFiles;

            for (var i = 0; i < tempList.Count; i++)
            {
                var item = tempList[i];

                foreach (var pattern in patterns)
                {
                    var url = Configuration.Instance.FileConfiguration.UrlBaseAddress + item;
                    var regex = new Regex(pattern.Pattern);
                    if (_dictionary.ContainsKey(url))
                    {
                        var g = _dictionary[url];

                        var contentResult = ((Container)g).message.Content.ReadAsStringAsync().Result;

                        if (regex.Match(contentResult).Success)
                        {
                            tempList[i] = tempList[i] + $" ({((Container)g).message.StatusCode}) ({pattern.Result})";
                            _dictionary[url].result = true;
                        }
                        else
                        {
                            tempList[i] = tempList[i] + $" ({((Container)g).message.StatusCode}) (False)";
                            _dictionary[url].result = true;
                        }
                    }
                }

                TrueFilesLabel.Content = Configuration.Instance.UrlFiles.Where(x => x.Contains(" (True)")).ToList().Count.ToString();
                FalseFilesLabel.Content = Configuration.Instance.UrlFiles.Where(x => x.Contains(" (False)")).ToList().Count.ToString();
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
        int mode = 0;
        private void FilterContent_Click(object sender, RoutedEventArgs e)
        {
            mode++;
            ItemsListBox.ItemsSource = null;
            mode = mode % 3;
            if (mode == 1)
            {
                ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles.Where(x => x.Contains(" (True)"));
            }
            else if (mode == 2)
            {
                ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles.Where(x => x.Contains(" (False)")); ;
            }
            else
            {
                ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
            }


        }

        private void CheckStatus(string url)
        {
            //var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");
            //var selectedItem = ItemsListBox.SelectedItem.ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "").Replace(" (OK)", "").Replace(" (InternalServerError)", "").Replace(" (NotFound)", "");
            //var url = Configuration.Instance.FileConfiguration.UrlBaseAddresst + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
            //GetUrlResponse(url);

            var tempList = Configuration.Instance.UrlFiles;
            var currentItem = ItemsListBox.SelectedItem.ToString();
            //Replace(" (True)", "").Replace(" (False)", "").Replace(" (OK)", "").Replace(" (InternalServerError)", "").Replace(" (NotFound)", "");

            var index = tempList.IndexOf(currentItem);

            if (index < 0) return;

            var patterns = Configuration.Instance.ComparePatterns.Where(x => x.ActiveRemote);

            //var temporalValue = tempList[index].CleanFileName();
            tempList[index] = tempList[index].CleanFileName();

            if (!_dictionary.ContainsKey(url)) return;

            var responseMessage = _dictionary[url];

            var AddStatusResult = true;
            if (AddStatusResult)
            {
                tempList[index] =  $"{tempList[index]} ({((Container)responseMessage).message.StatusCode})";
            }



            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern.Pattern);
                if (_dictionary.ContainsKey(url))
                {
                   
                    var contentResult = ((Container)responseMessage).message.Content.ReadAsStringAsync().Result;

                    if (regex.Match(contentResult).Success)
                    {

                        tempList[index] = $"{tempList[index]} ({pattern.Result})";
                        _dictionary[url].result = true;
                    }
                    else
                    {
                        tempList[index] = $"{tempList[index]} (False)";
                        _dictionary[url].result = true;
                    }
                }
            }
        }


        private void CheckUrlMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");
            var selectedItem = ItemsListBox.SelectedItem.ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "").Replace(" (OK)", "").Replace(" (InternalServerError)", "").Replace(" (NotFound)", "");
            var url = Configuration.Instance.FileConfiguration.UrlBaseAddress + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
            GetUrlResponse(url);
        }

        private void ItemsListBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {


                if (ItemsListBox.SelectedItem == null) return;

                var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");
                var selectedItem = ItemsListBox.SelectedItem.ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").Replace(" (True)", "").Replace(" (False)", "").Replace(" (OK)", "").Replace(" (InternalServerError)", "").Replace(" (NotFound)", "");
                var selectedItem2 = Configuration.Instance.FileConfiguration.UrlBaseAddress + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
                                                                                                            //var selectedItem2 = ItemsListBox.SelectedItem;
                if (selectedItem2 == null) return;

                //var selectedItem = selectedItem2.ToString().CleanFileName().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "");
                if (_dictionary.ContainsKey(selectedItem2))
                {
                    Container container;
                    var isHttpMessage = _dictionary.TryGetValue(selectedItem2, out container);
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
                        else
                        {
                            WebBrowserHTML.Navigate("about:blank");
                        }
                    }


                }
                else
                {
                    MessageBox.Show("Item not loaded yet!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckContentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");
            var selectedItem = ItemsListBox.SelectedItem.ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").CleanFileName();
            var url = Configuration.Instance.FileConfiguration.UrlBaseAddress + selectedItem; //"https://cpt-icedev.datacash.co.za/" 
            CheckStatus(url);

            ItemsListBox.ItemsSource = null;
            ItemsListBox.ItemsSource = Configuration.Instance.UrlFiles;
        }

        private void CheckUrlContentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CheckUrlMenuItem_Click(sender, e);
            CheckContentMenuItem_Click(sender, e);
        }

        private void CheckUrlReturnedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var defaultFolder = Configuration.Instance.FileConfiguration.RootFolder.Replace("\\", "/");
            var items = Configuration.Instance.UrlFiles.Where(s => !s.Contains("(OK)")).ToList();

            var httpClient = new HttpClient();

            for (int i = 0; i < items.Count; i++)
            {
                var selectedItem = ItemsListBox.Items[i].ToString().Replace(defaultFolder, "").Replace("ASPWebsite/", "").Replace("ICE_Local/", "").CleanFileName();
                var url = Configuration.Instance.FileConfiguration.UrlBaseAddress + selectedItem; //"https://cpt-icedev.datacash.co.za/" 

                GetUrlResponse(url);

            }
        }

        private void CopyListInClipboard_Click(object sender, RoutedEventArgs e)
        {
            var text = string.Empty;
            foreach (var item in Configuration.Instance.UrlFiles)
            {
                if (_dictionary.ContainsKey(item))
                {
                    text += item.CleanFileName() + Environment.NewLine;
                }
            }

            Clipboard.SetText(text);
        }
    }

    internal class Container
    {
        internal HttpResponseMessage message;
        internal bool result;
    }
}

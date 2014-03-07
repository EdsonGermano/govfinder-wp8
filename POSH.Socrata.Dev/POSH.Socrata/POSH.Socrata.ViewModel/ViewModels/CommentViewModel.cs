using POSH.Socrata.Entity.Models;
using POSH.Socrata.ViewModel.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace POSH.Socrata.ViewModel.ViewModels
{
    public class CommentViewModel : INotifyPropertyChanged
    {
        private HttpClient client = new HttpClient();

        private CityData _cityCategoryItemData = null;

        /// <summary>
        /// Gets or sets DataSetItem
        /// </summary>
        public CityData CityCategoryItemData
        {
            get
            {
                return _cityCategoryItemData;
            }
            set
            {
                _cityCategoryItemData = value;
                NotifyPropertyChanged("CityCategoryItemData");
            }
        }

        private ObservableCollection<CommentData> _commentList = null;

        /// <summary>
        /// Gets or sets comments collection
        /// </summary>
        public ObservableCollection<CommentData> CommentList
        {
            get
            {
                return _commentList;
            }
            set
            {
                _commentList = value;
                NotifyPropertyChanged("CommentList");
            }
        }

        private string _messageDialog = null;

        /// <summary>
        /// Gets or sets messages generated from this class
        /// </summary>
        public string MessageDialog
        {
            get
            {
                return _messageDialog;
            }
            set
            {
                _messageDialog = value;
                NotifyPropertyChanged("MessageDialog");
            }
        }

        private bool _isDataLoading = true;

        /// <summary>
        /// Gets or sets whether data loaded or not
        /// </summary>
        public bool IsDataLoading
        {
            get
            {
                return _isDataLoading;
            }
            set
            {
                if (value != _isDataLoading)
                {
                    _isDataLoading = value;
                    NotifyPropertyChanged("IsDataLoading");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public CommentViewModel()
        {
            client.BaseAddress = new Uri(Constant.CommentsBaseAddress);
            this.CityCategoryItemData = new CityData();
            this.CommentList = new ObservableCollection<CommentData>();
        }

        /// <summary>
        /// Adds comment.
        /// </summary>
        async public void AddComment(CommentData requestData)
        {
            try
            {
                this.IsDataLoading = true;
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    var results = JSON_Helper.Serialize<CommentData>(requestData);
                    HttpContent content = new StringContent(results);

                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.PostAsync(string.Format(Constant.AzureServiceURL, Constant.CommentsAzureServiceKey), content);
                    this.IsDataLoading = false;
                }
                else
                {
                    this.MessageDialog = "Internet connection required. Please check your internet connection and try again.";
                    this.IsDataLoading = false;
                }
            }
            catch (Exception)
            {
                this.MessageDialog = "There is some problem on connecting to server. Please check your internet connection and try again.";
                this.IsDataLoading = false;
            }
        }

        /// <summary>
        /// Gets comments
        /// </summary>
        public async Task GetComment(string id, string city, string reportName)
        {
            try
            {
                this.IsDataLoading = true;
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(string.Format(Constant.GetCommentsUrl, Constant.CommentsAzureServiceKey, id, city, reportName));
                    if (response.IsSuccessStatusCode)
                    {
                        var products = response.Content.ReadAsStringAsync().Result;
                        this.CommentList = JSON_Helper.Deserialize<List<CommentData>>(products).ToObservableCollection();
                    }
                    this.IsDataLoading = false;
                }
                else
                {
                    this.MessageDialog = "Internet connection required. Please check your internet connection and try again.";
                    this.IsDataLoading = false;
                }
            }
            catch (Exception)
            {
                this.MessageDialog = "There is some problem on connecting to server. Please check your internet connection and try again.";
                this.IsDataLoading = false;
            }
        }

        /// <summary>
        /// Gets the comments count for dataset item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="city"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        async public Task<string> GetCommentCount(string id, string city, string reportName)
        {
            string result = "0 comments";
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(string.Format(Constant.GetCommentsCountUrl, Constant.CommentsAzureServiceKey, id, city, reportName));
                    if (response.IsSuccessStatusCode)
                    {
                        var commentCount = response.Content.ReadAsStringAsync().Result;
                        int outValue = 0;
                        int.TryParse(commentCount, out outValue);
                        if (outValue != 0)
                        {
                            result = commentCount.ToString() + " comments";
                        }
                        System.Diagnostics.Debug.WriteLine(result);
                    }
                }
            }
            catch (Exception)
            {
                result = "0 comments";
            }
            return result;
        }
    }
}
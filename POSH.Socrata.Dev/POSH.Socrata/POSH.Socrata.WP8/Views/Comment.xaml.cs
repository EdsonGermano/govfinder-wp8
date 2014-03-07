using Microsoft.Live;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using POSH.Socrata.Entity.Models;
using POSH.Socrata.WP8.Resources;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace POSH.Socrata.WP8.Views
{
    public partial class Comment : PhoneApplicationPage
    {
        #region "========Local variables ============"

        private string compositeKey = string.Empty;
        private string city = string.Empty;
        private string dataSetName = string.Empty;
        private NetworkCredential socrataLoginCredentials = null;
        private SocrataUserDetail socrataUserDetails = null;

        private static string socrataUserName = string.Empty;
        private static string socrataPassword = string.Empty;
        private static bool IsSocrataLoginRemember = false;
        
        #endregion "========Local variables ============"

        /// <summary>
        /// Page Constructor
        /// </summary>
        public Comment()
        {
            InitializeComponent();
        }

        #region "========Page Events ============"

        private void CommentViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (e.PropertyName == "MessageDialog")
                {
                    MessageBox.Show(App.ViewModel.CommentViewModel.MessageDialog, AppResources.NoInternetMessageTitle, MessageBoxButton.OK);
                }
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.ViewModel.CommentViewModel.PropertyChanged -= CommentViewModel_PropertyChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ContentPanel.DataContext = App.ViewModel.CommentViewModel.CityCategoryItemData;
            this.compositeKey = App.ViewModel.CommentViewModel.CityCategoryItemData.CompositeKey;
            this.dataSetName = App.ViewModel.CommentViewModel.CityCategoryItemData.Category;
            this.city = GlobalVariables.CityName;
            this.DataContext = App.ViewModel.CommentViewModel;
            App.ViewModel.CommentViewModel.PropertyChanged -= CommentViewModel_PropertyChanged;
            App.ViewModel.CommentViewModel.PropertyChanged += CommentViewModel_PropertyChanged;
            GetComments();
        }

        /// <summary>
        /// add comment into comment list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void appbar_addComment_Click(object sender, EventArgs e)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    MessageBox.Show(AppResources.NoInternetConnectionMessage, AppResources.NoInternetMessageTitle, MessageBoxButton.OK);
                    this.Focus();
                    return;
                }
                App.ViewModel.CommentViewModel.IsDataLoading = true;
                if (!string.IsNullOrEmpty(txtComment.Text.Trim()))
                {
                    this.Focus();
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                    (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                    txtComment.IsEnabled = false;

                    IAsyncResult result = Microsoft.Xna.Framework.GamerServices.Guide.BeginShowMessageBox(AppResources.LoginText, AppResources.ChooseLoginOptionText, new string[] { AppResources.MicrosoftLoginText, AppResources.SocrataLoginText }, 0, Microsoft.Xna.Framework.GamerServices.MessageBoxIcon.None, null, null);
                    result.AsyncWaitHandle.WaitOne();
                    int? choice = Microsoft.Xna.Framework.GamerServices.Guide.EndShowMessageBox(result);
                    if (choice.HasValue)
                    {
                        if (choice.Value == 0)
                        {
                            MicrosoftLogin();
                        }
                        else if (choice.Value == 1)
                        {
                            if (IsSocrataLoginRemember)
                            {
                                this.ApplicationBar.IsVisible = true;
                                SocrataLogin(socrataUserName,socrataPassword);
                            }
                            else
                            {
                                OpenSocrataLogin();
                            }                            
                        }
                    }
                    else
                    {
                        this.popUpSocrataLogin.IsOpen = false;
                        this.ApplicationBar.IsVisible = true;
                        App.ViewModel.CommentViewModel.IsDataLoading = false;
                        (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                        (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                        txtComment.IsEnabled = true;
                    }
                }
                else
                {
                    MessageBox.Show(AppResources.CommentEmptyMessage);
                    App.ViewModel.CommentViewModel.IsDataLoading = false;
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                    (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                    txtComment.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                App.ViewModel.CommentViewModel.IsDataLoading = false;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                txtComment.IsEnabled = true;
            }
        }

        /// <summary>
        /// clear text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbar_cancelComment_Click(object sender, EventArgs e)
        {
            this.Focus();
            txtComment.Text = "";
        }

        private void btnLogin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var userId = (this.popUpSocrataLogin.Child as SocrataLoginUserControl).txtEmail.Text.Trim();
            var password = (this.popUpSocrataLogin.Child as SocrataLoginUserControl).pbPassword.Password.Trim();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(AppResources.UsernameAndPassEmptyMessage, AppResources.InvalidCredentialsMessageTitle, MessageBoxButton.OK);
            }
            else
            {
                this.popUpSocrataLogin.IsOpen = false;
                this.ApplicationBar.IsVisible = true;
                SocrataLogin(userId, password);
            }
        }

        private void btnCancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.popUpSocrataLogin.IsOpen = false;
            this.ApplicationBar.IsVisible = true;
            App.ViewModel.CommentViewModel.IsDataLoading = false;
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
            txtComment.IsEnabled = true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (this.popUpSocrataLogin.IsOpen)
            {
                this.popUpSocrataLogin.IsOpen = false;
                this.ApplicationBar.IsVisible = true;
                App.ViewModel.CommentViewModel.IsDataLoading = false;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                txtComment.IsEnabled = true;
                e.Cancel = true;
            }
            else
            {
                NavigationService.GoBack();
            }            
        }

        #endregion "========Page Events ============"

        #region "========Page Events ============"

        /// <summary>
        /// Opens Microsoft Login page using live sdk
        /// </summary>
        private void MicrosoftLogin()
        {
            try
            {
                var auth = new LiveAuthClient(Entity.Models.Constant.LiveSdkClientIdWP);
                this.Dispatcher.BeginInvoke(async () =>
                {
                    LiveLoginResult result = await auth.InitializeAsync(new string[] { "wl.basic", "wl.signin" });
                    if (result.Status != LiveConnectSessionStatus.Connected)
                    {
                        try
                        {
                            result = await auth.LoginAsync(new string[] { "wl.basic", "wl.signin" });
                        }
                        catch (Exception)
                        {
                            App.ViewModel.CommentViewModel.IsDataLoading = false;
                            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                            txtComment.IsEnabled = true;
                        }
                    }
                    if (result.Status == LiveConnectSessionStatus.Connected)
                    {
                        LiveConnectClient liveClient = new LiveConnectClient(result.Session);
                        var userDetails = await liveClient.GetAsync("me");
                        var userName = userDetails.Result["name"];
                        //auth.Logout();
                        CommentData commentData = new CommentData();
                        commentData.Author = userName.ToString();
                        commentData.CityName = this.city;
                        commentData.ReportName = this.dataSetName;
                        commentData.Id = this.compositeKey;
                        commentData.CommentPublishAt = DateTime.Now.ToUniversalTime();
                        commentData.CommentTitle = "";
                        commentData.CommentMessage = this.txtComment.Text.Trim();
                        App.ViewModel.CommentViewModel.AddComment(commentData);

                        App.ViewModel.CommentViewModel.CommentList.Add(commentData);
                        this.tbkNoResult.Visibility = Visibility.Collapsed;
                        IncreaseCommentsCount();
                        txtComment.Text = "";
                    }
                    App.ViewModel.CommentViewModel.IsDataLoading = false;
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                    (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                    txtComment.IsEnabled = true;
                });
            }
            catch (LiveAuthException)
            {
                App.ViewModel.CommentViewModel.IsDataLoading = false;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                txtComment.IsEnabled = true;
            }
        }

        /// <summary>
        /// Opens socrata Login page
        /// </summary>
        private void OpenSocrataLogin()
        {
            this.ApplicationBar.IsVisible = false;
            var socrataLoginPage = new SocrataLoginUserControl();
            if (this.Orientation == PageOrientation.Landscape || this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
            {
                socrataLoginPage.Height = Application.Current.Host.Content.ActualWidth;
                socrataLoginPage.Width = Application.Current.Host.Content.ActualHeight;
            }
            else
            {
                socrataLoginPage.Width = Application.Current.Host.Content.ActualWidth;
                socrataLoginPage.Height = Application.Current.Host.Content.ActualHeight;
            }
           
            this.popUpSocrataLogin.Child = socrataLoginPage;
            socrataLoginPage.btnCancel.Tap -= btnCancel_Tap;
            socrataLoginPage.btnLogin.Tap -= btnLogin_Tap;
            socrataLoginPage.btnCancel.Tap += btnCancel_Tap;
            socrataLoginPage.btnLogin.Tap += btnLogin_Tap;
            this.popUpSocrataLogin.IsOpen = true;
        }


        private void SocrataLogin(string username, string password)
        {
            try
            {
                socrataLoginCredentials = new NetworkCredential(username, password);
                string httpBase = Constant.SocrataLoginHttpBase;
                string appToken = Constant.SocrataLoginAppToken;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpBase + Constant.SocrataUserDetailUrl);
                //request.PreAuthenticate = true;
                request.Credentials = socrataLoginCredentials;
                request.Headers["X-App-Token"] = appToken;
                request.Headers["Authorization"] = getAuthorization();
                                
                string temp = null;
                int count = 0;
                byte[] buffer;
                StringBuilder sb = new StringBuilder();
                request.BeginGetResponse(delegate(IAsyncResult responseData)
                {
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(responseData);
                        using (var stream = response.GetResponseStream())
                        {
                            buffer = new byte[stream.Length];
                            // Read to the end of the response
                            do
                            {
                                count = stream.Read(buffer, 0, buffer.Length);
                                if (count != 0)
                                {
                                    temp = Encoding.UTF8.GetString(buffer, 0, count);
                                    sb.Append(temp);
                                }
                            } while (count > 0);
                        }
                        socrataUserDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<SocrataUserDetail>(sb.ToString());

                        if (socrataUserDetails != null)
                        {
                            this.Dispatcher.BeginInvoke(() =>
                            {
                                CommentData commentData = new CommentData();
                                commentData.Author = socrataUserDetails.displayName;
                                commentData.CityName = this.city;
                                commentData.ReportName = this.dataSetName;
                                commentData.Id = this.compositeKey;
                                commentData.CommentPublishAt = DateTime.Now.ToUniversalTime();
                                commentData.CommentTitle = "";
                                commentData.CommentMessage = this.txtComment.Text.Trim();

                                App.ViewModel.CommentViewModel.AddComment(commentData);

                                App.ViewModel.CommentViewModel.CommentList.Add(commentData);
                                this.tbkNoResult.Visibility = Visibility.Collapsed;
                                IncreaseCommentsCount();
                                txtComment.Text = "";

                                this.popUpSocrataLogin.IsOpen = false;
                                this.ApplicationBar.IsVisible = true;
                                App.ViewModel.CommentViewModel.IsDataLoading = false;
                                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                                txtComment.IsEnabled = true;
                                IsSocrataLoginRemember = true;
                                socrataUserName = username;
                                socrataPassword = password;
                            });
                        }
                        else
                        {
                            MessageBox.Show(AppResources.SocrataLoginErrorMessageText, AppResources.LoginErrorMessageTitle, MessageBoxButton.OK);
                            App.ViewModel.CommentViewModel.IsDataLoading = false;
                            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                            txtComment.IsEnabled = true;
                        }
                    }
                    catch (Exception)
                    {
                        this.Dispatcher.BeginInvoke(() =>
                            {
                                MessageBox.Show(AppResources.SocrataLoginErrorMessageText, AppResources.LoginErrorMessageTitle, MessageBoxButton.OK);
                                App.ViewModel.CommentViewModel.IsDataLoading = false;
                                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                                txtComment.IsEnabled = true;
                            });
                    }
                }, null);

            }
            catch (Exception)
            {
                MessageBox.Show(AppResources.SocrataLoginErrorMessageText, AppResources.LoginErrorMessageTitle, MessageBoxButton.OK);
                App.ViewModel.CommentViewModel.IsDataLoading = false;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                txtComment.IsEnabled = true;
            }
        }

        /// <summary>
        /// Increases comment counts locally on fetched data
        /// </summary>
        private void IncreaseCommentsCount()
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    if (App.ViewModel.CityDetailsViewModel.AllCityCategoryItems.ContainsKey(this.dataSetName))
                    {
                        var dataList = App.ViewModel.CityDetailsViewModel.AllCityCategoryItems[this.dataSetName];
                        if (dataList.Count > 0)
                        {
                            var cityData = dataList.Where(item => item.CompositeKey.ToLower() == this.compositeKey.ToLower()).FirstOrDefault();
                            cityData.Comments = App.ViewModel.CommentViewModel.CommentList.Count().ToString() + " comments";
                            App.ViewModel.CityCategoryDetailViewModel.CityCategoryItem.Comments = cityData.Comments;
                        }
                    }
                });
        }

        /// <summary>
        /// Gets comments from service
        /// </summary>
        /// <returns></returns>
        private async Task GetComments()
        {
            await App.ViewModel.CommentViewModel.GetComment(this.compositeKey, this.city, this.dataSetName);
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
            txtComment.IsEnabled = true;
            if (App.ViewModel.CommentViewModel.CommentList.Count == 0)
            {
                this.tbkNoResult.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbkNoResult.Visibility = Visibility.Collapsed;
            }
        }

        private string getAuthorization()
        {
            string creds = String.Format("{0}:{1}", socrataLoginCredentials.UserName, socrataLoginCredentials.Password);
            byte[] bytes = Encoding.UTF8.GetBytes(creds);
            string base64 = Convert.ToBase64String(bytes);
            return "Basic " + base64;
        }

        #endregion "========Page Events ============"

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (this.popUpSocrataLogin.IsOpen)
            {
                if (e.Orientation == PageOrientation.Landscape || e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
                {
                    (this.popUpSocrataLogin.Child as SocrataLoginUserControl).Height = Application.Current.Host.Content.ActualWidth;
                    (this.popUpSocrataLogin.Child as SocrataLoginUserControl).Width = Application.Current.Host.Content.ActualHeight;
                }
                else
                {
                    (this.popUpSocrataLogin.Child as SocrataLoginUserControl).Width = Application.Current.Host.Content.ActualWidth;
                    (this.popUpSocrataLogin.Child as SocrataLoginUserControl).Height = Application.Current.Host.Content.ActualHeight;
                }
            }
        }
    }
}
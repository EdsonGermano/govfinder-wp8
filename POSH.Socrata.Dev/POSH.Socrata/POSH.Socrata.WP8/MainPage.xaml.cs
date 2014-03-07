#define DEBUG_AGENT

using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Scheduler;
using POSH.Socrata.Entity.Models;
using POSH.Socrata.WP8.Resources;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;
using System.Linq;

namespace POSH.Socrata.WP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region "===========Local variables ==========="

        private PeriodicTask smartPeriodicTask;
        private string socrataPeriodicTaskName = AppResources.PeriodicTaskName;
        private string socrataPeriodicTaskDescription = AppResources.PeriodicTaskDescription;

        #endregion "===========Local variables ==========="

        /// <summary>
        /// Page Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            App.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.Loaded += MainPage_Loaded;
        }

        #region "==========Page Events==========="

        /// <summary>
        /// Main page loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App.ViewModel;
            if (App.ViewModel.CityDataSetList.Count == 0)
            {
                LoadData();
            }
            else
            {
                FilterCityData();
            }
            if (App.setting.Contains("allowGps"))
            {
                var isGpsEnabled = App.setting["allowGps"] as Nullable<bool>;
                if (isGpsEnabled.Value)
                {
                    GetCurrentLocation();
                }
            }
            StartPeriodicAgent();
        }

        /// <summary>
        /// ViewModels property changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ErrorMessage")
            {
                MessageBox.Show(App.ViewModel.ErrorMessage.MessageText);
            }
        }

        /// <summary>
        /// Calls whenever navigates to this page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.lpkCityList.SelectionChanged -= lpkCityList_SelectionChanged;
        }

        /// <summary>
        /// get selected selected city's place and navigate to details page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cityList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                var selectedItem = ((POSH.Socrata.Entity.Models.GovFinderData)((e.OriginalSource as FrameworkElement).DataContext)).DataSetName;
                if (App.ViewModel.CityDetailsViewModel.AllCityCategoryItems.ContainsKey(selectedItem))
                {
                    App.ViewModel.CityDetailsViewModel.AllCityCategoryItems.Remove(selectedItem);
                }
                NavigationService.Navigate(new Uri("/Views/CityDetails.xaml?parameter=" + selectedItem + Constant.Seprator + this.lpkCityList.SelectedItem as string, UriKind.Relative));
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// navigation to about us page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbar_about_us_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AboutUs.xaml", UriKind.Relative));
        }

        /// <summary>
        /// navigation to privacy policy page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbar_privacy_policy_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/PrivacyPolicy.xaml", UriKind.Relative));
        }

        /// <summary>
        /// navigation to setting page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbar_settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
        }

        private void imgSearch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Search.xaml", UriKind.Relative));
        }

        private void appbar_search_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Search.xaml", UriKind.Relative));
        }

        #endregion "==========Page Events==========="

        #region "========== Page Functions==========="

        /// <summary>
        /// Filter city list on the basis of city name.
        /// </summary>
        private void FilterCityData()
        {
            string currentCity = App.setting["cityName"].ToString();
            App.ViewModel.GetCityBackground(currentCity);
            GlobalVariables.CityName = currentCity;
            App.ViewModel.GetCities();
            this.lpkCityList.SelectedItem = App.ViewModel.CitiesList.Where(obj => obj.ToLower() == currentCity.ToLower()).FirstOrDefault();
            this.lpkCityList.SelectionChanged += lpkCityList_SelectionChanged;
           // this.tblCity.Text = currentCity;
            App.ViewModel.GetSelectedCityDataSets(currentCity);
        }

        private void lpkCityList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lpkCityList.SelectedItem != null)
            {
                if ((lpkCityList.SelectedItem as string).ToLower() != GlobalVariables.CityName.ToLower())
                {
                    App.setting["cityName"] = (lpkCityList.SelectedItem as string);
                    App.setting.Save();
                    GlobalVariables.CityName = (lpkCityList.SelectedItem as string);
                    App.ViewModel.GetCityBackground(GlobalVariables.CityName);
                    App.ViewModel.GetSelectedCityDataSets((lpkCityList.SelectedItem as string));
                }
            }
        }

        /// summary>
        /// Loads datasets
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            await App.ViewModel.LoadXML();
            FilterCityData();
        }

        /// <summary>
        /// Runs schedule agent for notification
        /// </summary>
        private void StartPeriodicAgent()
        {
            // is old task running, remove it
            smartPeriodicTask = ScheduledActionService.Find(socrataPeriodicTaskName) as PeriodicTask;
            if (smartPeriodicTask != null)
            {
                try
                {
                    ScheduledActionService.Remove(socrataPeriodicTaskName);
                }
                catch (Exception)
                {
                }
            }
            // create a new task
            smartPeriodicTask = new PeriodicTask(socrataPeriodicTaskName);
            // load description from localized strings
            smartPeriodicTask.Description = socrataPeriodicTaskDescription;
            // set expiration days
            smartPeriodicTask.ExpirationTime = DateTime.Now.AddDays(14);
            try
            {
                // add this to scheduled action service
                ScheduledActionService.Add(smartPeriodicTask);
                // debug, so run in every 30 secs
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(socrataPeriodicTaskName, TimeSpan.FromSeconds(30));
                System.Diagnostics.Debug.WriteLine("Periodic task is started: " + socrataPeriodicTaskName);
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    // load error text from localized strings
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
        }

        /// <summary>
        /// Gets current location
        /// </summary>
        /// <returns></returns>
        private async Task GetCurrentLocation()
        {
            this.Dispatcher.BeginInvoke(async () =>
                       {
                           try
                           {
                               if (NetworkInterface.GetIsNetworkAvailable())
                               {
                                   Geolocator geolocator = new Geolocator();
                                   Geoposition geoposition = await geolocator.GetGeopositionAsync();
                                   Geocoordinate geocoordinate = geoposition.Coordinate;

                                   App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates = new Altitude() { Latitude = geocoordinate.Latitude, Longitude = geocoordinate.Longitude };
                               }
                               else
                               {
                                   MessageBox.Show(AppResources.NoInternetConnectionMessage, AppResources.LocationServiceText, MessageBoxButton.OK);
                               }
                           }
                           catch (UnauthorizedAccessException)
                           {
                               var messageResult = MessageBox.Show(AppResources.LocationServiceOffMessage, AppResources.LocationServiceText, MessageBoxButton.OKCancel);
                               if (messageResult == MessageBoxResult.OK)
                               {
                                   var op = Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                               }
                           }
                           catch (Exception)
                           {
                               var messageResult = MessageBox.Show(AppResources.LocationServiceOffMessage, AppResources.LocationServiceText, MessageBoxButton.OKCancel);
                               if (messageResult == MessageBoxResult.OK)
                               {
                                   var op = Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                               }
                           }
                       });
        }

        #endregion "========== Page Functions==========="

        private async void appbar_refresh_app_Click(object sender, EventArgs e)
        {
            App.ViewModel.CityDataSetList.Clear();
            pbProgressBar.DataContext = App.ViewModel;
            App.ViewModel.IsCityDataLoading = true;
            await App.ViewModel.RefreshXML();
            FilterCityData();
             System.Threading.Thread.Sleep(2000);
            App.ViewModel.IsCityDataLoading = false;
        }
    }
}
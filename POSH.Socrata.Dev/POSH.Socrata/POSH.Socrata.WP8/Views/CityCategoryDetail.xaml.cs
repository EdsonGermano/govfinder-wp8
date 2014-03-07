using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using POSH.Socrata.Entity.Models;
using POSH.Socrata.WP8.Resources;
using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;

namespace POSH.Socrata.WP8.Views
{
    public partial class CityCategoryDetail : PhoneApplicationPage
    {
        #region "======Local variables============"

        private string apiUrl = string.Empty;
        private string category = string.Empty;
        private string tileParameter = string.Empty;
        private string compositeKey = string.Empty;

        #endregion "======Local variables============"

        /// <summary>
        /// page constructor
        /// </summary>
        public CityCategoryDetail()
        {
            InitializeComponent();
        }

        #region "=======Page Events==========="

        private void CityCategoryDetail_Loaded(object sender, RoutedEventArgs e)
        {            
            if (App.setting.Contains("allowGps"))
            {
                var isGpsEnabled = App.setting["allowGps"] as Nullable<bool>;
                if (isGpsEnabled.Value)
                {
                    GetCurrentLocation();
                }
                else
                {
                    LoadTileData(App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates);
                }
            }
        }

        private void CityCategoryDetailViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    if (e.PropertyName == "ErrorMessage")
                    {
                        MessageBox.Show(App.ViewModel.CityCategoryDetailViewModel.ErrorMessage);
                    }
                });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.ViewModel.CityCategoryDetailViewModel.PropertyChanged -= CityCategoryDetailViewModel_PropertyChanged;           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("parameter"))
            {
                apiUrl = this.NavigationContext.QueryString["parameter"];
                this.category = App.ViewModel.CityCategoryDetailViewModel.CityCategoryItem.Category;
                this.compositeKey = App.ViewModel.CityCategoryDetailViewModel.CityCategoryItem.CompositeKey;
                SetDataContext();
            }
            else if (this.NavigationContext.QueryString.ContainsKey("tileParameter"))
            {
                tileParameter = this.NavigationContext.QueryString["tileParameter"];
                string[] splitParam = new string[] { "splitfromhere" };
                var splittedString = tileParameter.Split(splitParam, 3, StringSplitOptions.RemoveEmptyEntries);
                this.apiUrl = splittedString[0].Trim();
                this.category = splittedString[1].Trim();
                this.compositeKey = splittedString[2].Trim();
                GlobalVariables.CityName = App.setting["cityName"].ToString();
                this.Loaded -= CityCategoryDetail_Loaded;
                this.Loaded += CityCategoryDetail_Loaded;
            }
            App.ViewModel.CityCategoryDetailViewModel.PropertyChanged -= CityCategoryDetailViewModel_PropertyChanged;
            App.ViewModel.CityCategoryDetailViewModel.PropertyChanged += CityCategoryDetailViewModel_PropertyChanged;
            CheckForPin();
        }

        /// <summary>
        /// make a phone call on mobile no.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbkPhone_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                PhoneCallTask callTask = new PhoneCallTask();
                var phoneNo = (e.OriginalSource as TextBlock).Text;
                if (phoneNo.ToLower() != AppResources.NoPhoneNumber)
                {
                    callTask.PhoneNumber = phoneNo;
                    callTask.Show();
                }
                else
                {
                    MessageBox.Show(AppResources.InValidNumberMessage);
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// pin item to main screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbar_pin_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (this.DataContext as CityData);
                var tileParameterData = apiUrl + " splitfromhere " + this.category + " splitfromhere " + data.CompositeKey;
                ShellTile tile = CheckIfTileExist(tileParameterData);
                if (tile == null)
                {
                    StandardTileData secondaryTile = new StandardTileData
                    {
                        Title = data.Name,
                        BackgroundImage = new Uri(data.BackgroundImage, UriKind.Relative),
                        BackContent = data.Name
                    };

                    string tileUri = string.Concat("/Views/CityCategoryDetail.xaml?tileParameter=", tileParameterData);
                    ShellTile.Create(new Uri(tileUri, UriKind.Relative), secondaryTile);
                }
                else
                {
                    ApplicationBarIconButton appButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                    tile.Delete();
                    MessageBox.Show(AppResources.SecondaryTileDeletedMessage);
                    appButton.IconUri = new Uri("/Assets/Pin.png", UriKind.Relative);
                    appButton.Text = AppResources.AppBarButtonPinText;
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// on double tap, navigate a page to view full map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nokiaMap_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                App.ViewModel.FullMapViewModel.CurrentLocation = App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates;
                App.ViewModel.FullMapViewModel.GetCityItemDetails(App.ViewModel.CityCategoryDetailViewModel.CityCategoryItemsList);
                NavigationService.Navigate(new Uri("/Views/FullMap.xaml", UriKind.Relative));
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Navigate a page to show and add new comments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddComents_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedCategory = (this.DataContext as CityData);
            App.ViewModel.CommentViewModel.CityCategoryItemData = selectedCategory;

            App.ViewModel.CommentViewModel.CommentList.Clear();
            NavigationService.Navigate(new Uri("/Views/Comment.xaml", UriKind.Relative));
        }

        #endregion "=======Page Events==========="

        #region "======Page functions ==========="

        /// <summary>
        /// To check already pined or not.
        /// </summary>
        /// <param name="tileUri"></param>
        /// <returns></returns>
        private ShellTile CheckIfTileExist(string tileUri)
        {
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(
                    tile => tile.NavigationUri.ToString().Contains(tileUri));
            return shellTile;
        }

        /// <summary>
        /// Sets datacontext of this page
        /// </summary>
        private void SetDataContext()
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    this.DataContext = null;
                    this.DataContext = App.ViewModel.CityCategoryDetailViewModel.CityCategoryItem;
                    LoadCategoriesData();
                    this.progressBar.IsIndeterminate = false;
                });
        }

        /// <summary>
        /// Checks for current location when navigated from secondary tile
        /// </summary>
        /// <returns></returns>
        private async void GetCurrentLocation()
        {
            this.Dispatcher.BeginInvoke(async () =>
                       {
                           try
                           {
                               if (NetworkInterface.GetIsNetworkAvailable())
                               {

                                   Geolocator geolocator = new Geolocator();
                                   Geoposition geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));
                                   Geocoordinate geocoordinate = geoposition.Coordinate;

                                   App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates = new Altitude() { Latitude = geocoordinate.Latitude, Longitude = geocoordinate.Longitude };
                                   LoadTileData(App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates);
                               }
                               else
                               {
                                   MessageBox.Show(AppResources.NoInternetConnectionMessage, AppResources.NoInternetMessageTitle, MessageBoxButton.OK);
                               }
                           }
                           catch (Exception)
                           {
                               var messageResult = MessageBox.Show(AppResources.LocationServiceOffMessage, AppResources.LocationServiceText, MessageBoxButton.OKCancel);
                               if (messageResult == MessageBoxResult.OK)
                               {
                                   var op = Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                                   LoadTileData(App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates);
                               }
                               else
                               {
                                   LoadTileData(App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates);
                               }
                           }
                       });
        }

        /// <summary>
        /// Loads data from secondary tile
        /// </summary>
        /// <param name="altitude"></param>
        /// <returns></returns>
        private void LoadTileData(Altitude altitude)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show(AppResources.NoInternetConnectionMessage, AppResources.NoInternetMessageTitle, MessageBoxButton.OK);
                return;
            }
            Task<string> task = Task<string>.Run<string>(async() =>
                {
                    await App.ViewModel.LoadXML();
                    var data = App.ViewModel.CityDataSetList.Where(obj => obj.ApiUrl.ToLower() == this.apiUrl.ToLower()).FirstOrDefault();
                    if (data != null)
                    {
                        App.ViewModel.CityCategoryDetailViewModel.GetTileCityItemDetails(this.apiUrl, data, altitude, this.compositeKey);
                    }                    
                    return "";
                });
            task.ContinueWith((Task<string> value) =>
                {
                    SetDataContext();
                });
        }

        /// <summary>
        /// Checks for pin / unpin at page load
        /// </summary>
        private void CheckForPin()
        {
            ApplicationBarIconButton appButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            var isPin = CheckIfTileExist(apiUrl + " splitfromhere " + this.category + " splitfromhere " + this.compositeKey);
            if (isPin != null)
            {
                appButton.IconUri = new Uri("/Assets/Unpin.png", UriKind.Relative);
                appButton.Text = AppResources.AppBarButtonUnpinText;
            }
            else
            {
                appButton.IconUri = new Uri("/Assets/Pin.png", UriKind.Relative);
                appButton.Text = AppResources.AppBarButtonPinText;
            }
        }

        /// <summary>
        /// load city category details.
        /// </summary>
        public void LoadCategoriesData()
        {
            try
            {
                this.Dispatcher.BeginInvoke(() =>
                    {
                        ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(nokiaMap);
                        var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
                        nokiaMap.Center = new GeoCoordinate(App.ViewModel.CityCategoryDetailViewModel.MapCenterPoint.Latitude, App.ViewModel.CityCategoryDetailViewModel.MapCenterPoint.Longitude);
                        if (obj.ItemsSource == null)
                        {
                            obj.ItemsSource = App.ViewModel.CityCategoryDetailViewModel.CityCategoryPushpinsList;
                        }
                    });
            }
            catch (Exception)
            {
            }
        }

        #endregion "======Page functions ==========="

        /// <summary>
        /// to display distance b/w selected location from current location on map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbkDistance_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedCategory = (this.DataContext as CityData);
            BingMapsDirectionsTask bingMapsDirectionsTask = new BingMapsDirectionsTask();
            LabeledMapLocation start = new LabeledMapLocation();
            LabeledMapLocation end = new LabeledMapLocation(selectedCategory.Address, new GeoCoordinate(selectedCategory.Coordinate.Latitude, selectedCategory.Coordinate.Longitude));
            bingMapsDirectionsTask.Start = start;
            bingMapsDirectionsTask.End = end;
            bingMapsDirectionsTask.Show();
        }
    }
}
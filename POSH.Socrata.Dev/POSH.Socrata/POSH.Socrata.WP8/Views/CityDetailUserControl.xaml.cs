using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Tasks;
using POSH.Socrata.Entity.Models;
using POSH.Socrata.ViewModel.HelperClasses;
using POSH.Socrata.WP8.Resources;
using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace POSH.Socrata.WP8.Views
{
    public partial class CityDetailUserControl : UserControl
    {
        #region "======Local Variables============="

        public CancellationTokenSource cts;

        #endregion "======Local Variables============="

        /// <summary>
        /// Page constructor
        /// </summary>
        public CityDetailUserControl()
        {
            InitializeComponent();
          
            this.pbProgressBar.IsIndeterminate = true;
            App.ViewModel.CityDetailsViewModel.PropertyChanged -= CityDetailsViewModel_PropertyChanged;
            App.ViewModel.CityDetailsViewModel.PropertyChanged += CityDetailsViewModel_PropertyChanged;
            this.Loaded += CityDetailUserControl_Loaded;
        }

        void CityDetailUserControl_Loaded(object sender, RoutedEventArgs e)
        {
      
        }

        #region "=========Page Events==========="

        /// <summary>
        /// Property Changed event for get IsDataLoaded value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CityDetailsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                string message = ((POSH.Socrata.ViewModel.ViewModels.CityDetailsViewModel)(sender)).MessageDialog;
                this.Dispatcher.BeginInvoke(() =>
                                        {
                                            if (e.PropertyName == "IsDataLoading")
                                            {
                                                this.pbProgressBar.IsIndeterminate = App.ViewModel.CityDetailsViewModel.IsDataLoading;
                                            }
                                            //if (e.PropertyName == "MapCenterPoint")
                                            //{
                                            //    nokiaMap.Center = new GeoCoordinate(App.ViewModel.CityDetailsViewModel.MapCenterPoint.Latitude, App.ViewModel.CityDetailsViewModel.MapCenterPoint.Longitude);
                                            //}
                                            //if (e.PropertyName == "CityCategoryItemsList")
                                            //{
                                            //    this.cityItemsList.ItemsSource = App.ViewModel.CityDetailsViewModel.CityCategoryItemsList;
                                            //}
                                        });
            }
            catch (Exception)
            { }
        }

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

        private void cityItemsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var data = (this.DataContext as GovFinderData);
            CityData selectedCategory = (sender as ListBox).SelectedItem as CityData;
            if (selectedCategory != null)
            {
                App.ViewModel.CityCategoryDetailViewModel.CityCategoryItem = new CityData();
                App.ViewModel.CityCategoryDetailViewModel.GetCityItemDetails(selectedCategory);
                var frame = App.Current.RootVisual as PhoneApplicationFrame;
                frame.Navigate(new Uri("/Views/CityCategoryDetail.xaml?parameter=" + data.ApiUrl, UriKind.Relative));
            }
        }

        private void btnAddComents_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedCategory = ((sender as FrameworkElement).DataContext as CityData);
            App.ViewModel.CommentViewModel.CityCategoryItemData = selectedCategory;

            App.ViewModel.CommentViewModel.CommentList.Clear();
            var frame = App.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri("/Views/Comment.xaml", UriKind.Relative));
        }

        /// <summary>
        /// to display distance between selected location and current location on map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbkDistance_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedCategory = ((sender as FrameworkElement).DataContext as CityData);
            BingMapsDirectionsTask bingMapsDirectionsTask = new BingMapsDirectionsTask();
            LabeledMapLocation start = new LabeledMapLocation();
            LabeledMapLocation end = new LabeledMapLocation(selectedCategory.Address, new GeoCoordinate(selectedCategory.Coordinate.Latitude, selectedCategory.Coordinate.Longitude));
            bingMapsDirectionsTask.Start = start;
            bingMapsDirectionsTask.End = end;
            bingMapsDirectionsTask.Show();
        }

        #endregion "=========Page Events==========="

        #region "==========Page Functions============"

        /// <summary>
        /// Loads all data of a dataset
        /// </summary>
        /// <returns></returns>
        public async Task LoadCategoriesData()
        {
            try
            {
                ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(nokiaMap);
                var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
                nokiaMap.Center = new GeoCoordinate(App.ViewModel.CityDetailsViewModel.MapCenterPoint.Latitude, App.ViewModel.CityDetailsViewModel.MapCenterPoint.Longitude);

                var data = (this.DataContext as GovFinderData);
                App.ViewModel.CityDetailsViewModel.CurrentCategoryName = data.DataSetName;
                //check if ApiUrl not null
                if (!string.IsNullOrEmpty(data.ApiUrl))
                {
                    this.Dispatcher.BeginInvoke(() =>
                                   {
                                       App.ViewModel.CityDetailsViewModel.CityCategoryItemsList.Clear();
                                       App.ViewModel.CityDetailsViewModel.CityCategoryPushpinsList.Clear();
                                   });
                    //Check if AllCityCategoryItems contains key for this category
                    //if (App.ViewModel.CityDetailsViewModel.AllCityCategoryItems.ContainsKey(data.DataSetName))
                    //{
                    //    this.Dispatcher.BeginInvoke(() =>
                    //             {
                    //                 this.pbProgressBar.IsIndeterminate = true;
                    //                 App.ViewModel.CityDetailsViewModel.CityCategoryItemsList = App.ViewModel.CityDetailsViewModel.AllCityCategoryItems[data.DataSetName].ToObservableCollection();
                    //                 this.pbProgressBar.IsIndeterminate = false;
                    //                 var firstData = App.ViewModel.CityDetailsViewModel.CityCategoryItemsList.FirstOrDefault();
                    //                 if (firstData != null)
                    //                 {
                    //                     App.ViewModel.CityDetailsViewModel.MapCenterPoint = firstData.Coordinate;
                    //                     nokiaMap.Center = new GeoCoordinate(App.ViewModel.CityDetailsViewModel.MapCenterPoint.Latitude, App.ViewModel.CityDetailsViewModel.MapCenterPoint.Longitude);
                    //                     if (App.ViewModel.CityDetailsViewModel.MapCenterPoint.Latitude == 0 && App.ViewModel.CityDetailsViewModel.MapCenterPoint.Longitude == 0)
                    //                     {
                    //                         nokiaMap.Center = new GeoCoordinate(App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates.Latitude, App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates.Longitude);
                    //                     }
                                         
                    //                 }
                    //                 foreach (var pushpin in App.ViewModel.CityDetailsViewModel.CityCategoryItemsList.Take(40).ToObservableCollection())
                    //                 {
                    //                     if (pushpin.Coordinate.Latitude != 0 && pushpin.Coordinate.Longitude != 0)
                    //                     {
                    //                         App.ViewModel.CityDetailsViewModel.CityCategoryPushpinsList.Add(pushpin);
                    //                     }
                    //                 }

                    //                 this.cityItemsList.ItemsSource = App.ViewModel.CityDetailsViewModel.CityCategoryItemsList;
                    //                 if (obj.ItemsSource == null)
                    //                 {
                    //                     obj.ItemsSource = App.ViewModel.CityDetailsViewModel.CityCategoryPushpinsList;
                    //                 }
                    //                 this.pbProgressBar.IsIndeterminate = false;
                    //             });
                    //}
                    //else
                    //{
                        cts = new CancellationTokenSource();
                        Task<string> taskGetCityItemDetails = Task<string>.Run<string>(() =>
                            {
                                App.ViewModel.CityDetailsViewModel.GetCityItemDetails(data.ApiUrl, data);
                                return "";
                            }, cts.Token);

                        taskGetCityItemDetails.ContinueWith((Task<string> Value) =>
                            {
                                this.Dispatcher.BeginInvoke(() =>
                                {
                                        //this.cityItemsList.DataContext = App.ViewModel;
                                        this.cityItemsList.ItemsSource = App.ViewModel.CityDetailsViewModel.CityCategoryItemsList;
                                        if (obj.ItemsSource == null)
                                        {
                                            obj.ItemsSource = App.ViewModel.CityDetailsViewModel.CityCategoryPushpinsList;
                                        }
                                        nokiaMap.Center = new GeoCoordinate(App.ViewModel.CityDetailsViewModel.MapCenterPoint.Latitude, App.ViewModel.CityDetailsViewModel.MapCenterPoint.Longitude);
                                        this.pbProgressBar.IsIndeterminate = App.ViewModel.CityDetailsViewModel.IsDataLoading;
                                    });
                            }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                //}
            }
            catch (Exception)
            {
                MessageBox.Show(App.ViewModel.CityDetailsViewModel.MessageDialog);
            }
        }

        #endregion "==========Page Functions============"
    }
}
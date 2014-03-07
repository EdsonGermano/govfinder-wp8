using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using POSH.Socrata.Entity.Models;
using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace POSH.Socrata.WP8.Views
{
    public partial class FullMap : PhoneApplicationPage
    {
        #region "=========Local Variables ==========="

        private GeoCoordinate currentLocation = null;
        private MapLayer locationLayer = null;
        private ObservableCollection<CityData> lstCityPushpin = new ObservableCollection<CityData>();
        private int fetchedItems = 0;
        private int prevFetchedItem = 0;
        private DispatcherTimer timer;

        #endregion "=========Local Variables ==========="

        /// <summary>
        /// Page Constructor
        /// </summary>
        public FullMap()
        {
            InitializeComponent();
        }

        #region "========= Page Events ==========="

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GetPushpins();
            this.DataContext = App.ViewModel.FullMapViewModel;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            AddPushpins();
        }

        #endregion "========= Page Events ==========="

        #region "===========Page Functions============"

        /// <summary>
        ///  Give item source to map's pushpins.
        /// </summary>
        public void GetPushpins()
        {
            try
            {
                ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(nokiaMap);
                var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;

                //current positions
                if (App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates.Latitude != 0 && App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates.Longitude != 0)
                {
                    currentLocation = new GeoCoordinate(App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates.Latitude, App.ViewModel.CityDetailsViewModel.CurrentLocationCoordinates.Longitude);
                    ShowLocation();
                }

                nokiaMap.Center = new GeoCoordinate(App.ViewModel.FullMapViewModel.MapCenterPoint.Latitude, App.ViewModel.FullMapViewModel.MapCenterPoint.Longitude);
                if (obj.ItemsSource == null)
                {
                    obj.ItemsSource = this.lstCityPushpin;// App.ViewModel.FullMapViewModel.CityCategoryPushpinsList;
                }
                else
                {
                    obj.ItemsSource = this.lstCityPushpin;// App.ViewModel.FullMapViewModel.CityCategoryPushpinsList;
                }
                AddPushpins();
            }
            catch (Exception)
            {
                MessageBox.Show(App.ViewModel.FullMapViewModel.MessageDialog);
            }
        }

        /// <summary>
        /// Display Current Location Pushpin
        /// </summary>
        private void ShowLocation()
        {
            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Assets/currentpin.png", UriKind.Relative)) };
            myCircle.Height = 40;
            myCircle.Width = 40;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = currentLocation;

            // Create a MapLayer to contain the MapOverlay.
            locationLayer = new MapLayer();
            locationLayer.Add(myLocationOverlay);

            // Add the MapLayer to the Map.
            nokiaMap.Layers.Add(locationLayer);
        }

        /// <summary>
        /// Adds pushpin in the local collection after every 2 seconds
        /// </summary>
        private void AddPushpins()
        {
            prevFetchedItem = fetchedItems;
            fetchedItems = fetchedItems + 50;
            for (int i = prevFetchedItem; i < fetchedItems; i++)
            {
                if (i < App.ViewModel.FullMapViewModel.CityCategoryPushpinsList.Count)
                {
                    this.lstCityPushpin.Add(App.ViewModel.FullMapViewModel.CityCategoryPushpinsList[i]);
                }
                else
                {
                    if (timer != null)
                    {
                        timer.Stop();
                    }
                }
            }
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 2);
                timer.Tick += timer_Tick;
                timer.Start();
            }
        }

        #endregion "===========Page Functions============"
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Media;
using System.Windows.Shapes;

namespace POSH.Socrata.WP8.Views
{
    public partial class FullMapView : PhoneApplicationPage
    {
        public FullMapView()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MapLayer layer0 = new MapLayer();
            Pushpin pushpin1 = new Pushpin();
            pushpin1.GeoCoordinate = new GeoCoordinate(47.6097, -122.3331);
        
            MapOverlay overlay1 = new MapOverlay();
            pushpin1.Content = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Red),
                Width = 40,
                Height = 40
            };
            layer0.Add(overlay1);
            myMap.Layers.Add(layer0);
        }
    }
}
using POSH.Socrata.Entity.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace POSH.Socrata.ViewModel.ViewModels
{
    public class FullMapViewModel : INotifyPropertyChanged
    {
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

        private Altitude _mapCenterPoint = null;

        /// <summary>
        /// Gets or sets center co-ordinates of the map
        /// </summary>
        public Altitude MapCenterPoint
        {
            get
            {
                return _mapCenterPoint;
            }
            set
            {
                if (value != _mapCenterPoint)
                {
                    _mapCenterPoint = value;
                    NotifyPropertyChanged("MapCenterPoint");
                }
            }
        }

        private ObservableCollection<CityData> _cityCategoryPushpinsList = null;

        /// <summary>
        /// Gets or sets DataSetItems pushpin list
        /// </summary>
        public ObservableCollection<CityData> CityCategoryPushpinsList
        {
            get
            {
                return _cityCategoryPushpinsList;
            }
            set
            {
                _cityCategoryPushpinsList = value;
                NotifyPropertyChanged("CityCategoryPushpinsList");
            }
        }

        public Altitude CurrentLocation { get; set; }

        private ObservableCollection<CityData> _cityCategoryItemsList = null;

        /// <summary>
        /// Gets or sets DataSetItems list
        /// </summary>
        public ObservableCollection<CityData> CityCategoryItemsList
        {
            get
            {
                return _cityCategoryItemsList;
            }
            set
            {
                _cityCategoryItemsList = value;
                NotifyPropertyChanged("CityCategoryItemsList");
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FullMapViewModel()
        {
            this.MapCenterPoint = new Altitude();
            this.CityCategoryPushpinsList = new ObservableCollection<CityData>();
            this.CityCategoryItemsList = new ObservableCollection<CityData>();
            this.CurrentLocation = new Altitude();
        }

        /// <summary>
        /// Parsing json data from url and add into a list.
        /// </summary>
        /// <param name="parameterValue"></param>
        public void GetCityItemDetails(ObservableCollection<CityData> lstCityItemList)
        {
            try
            {
                this.IsDataLoading = true;

                this.CityCategoryPushpinsList.Clear();
                this.CityCategoryItemsList.Clear();
                foreach (var item in lstCityItemList)
                {
                    this.CityCategoryItemsList.Add(item);
                    if (item.Coordinate.Latitude != 0 && item.Coordinate.Longitude != 0)
                    {
                        this.CityCategoryPushpinsList.Add(item);
                    }
                }
                if (this.CityCategoryItemsList.FirstOrDefault() != null)
                {
                    var item = this.CityCategoryItemsList.FirstOrDefault();
                    if (item.Coordinate.Latitude != 0 && item.Coordinate.Longitude != 0)
                    {
                        this.MapCenterPoint = item.Coordinate;
                    }
                    else
                    {
                        this.MapCenterPoint = this.CurrentLocation;
                    }
                }
                this.IsDataLoading = false;
            }
            catch (Exception)
            {
                this.MessageDialog = "There is some problem on fetching data from server. Please check your internet connection and try again.";
                this.IsDataLoading = false;
            }
        }
    }
}
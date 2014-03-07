using POSH.Socrata.Entity.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace POSH.Socrata.ViewModel.ViewModels
{
    public class CityDetailsViewModel : INotifyPropertyChanged
    {
        #region declarations

        private bool _isDataLoading = true;

        /// <summary>
        /// Gets or Sets value whether data loaded or not
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
        /// Gets or Sets center co-ordinates of the map
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

        private string _messageDialog = null;

        /// <summary>
        /// Gets or Sets message generated from this class to UI
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

        /// <summary>
        /// Gets or Sets all fetched items list for search
        /// </summary>
        public List<CityData> AllCityItems { get; set; }

        private ObservableCollection<CityData> _cityCategoryPushpinsList = null;

        /// <summary>
        /// Gets or sets DataSetPushpin list
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

        private Dictionary<string, List<CityData>> _allCityCategoryItems;

        /// <summary>
        /// Gets or Sets fetched items of selected city
        /// </summary>
        public Dictionary<string, List<CityData>> AllCityCategoryItems
        {
            get { return _allCityCategoryItems; }
            set
            {
                _allCityCategoryItems = value;
                NotifyPropertyChanged("CityCategoryList");
            }
        }

        /// <summary>
        /// Gets or sets currently selected DataSetName
        /// </summary>
        public string CurrentCategoryName { get; set; }

        /// <summary>
        /// Gets or sets current location co-ordinates
        /// </summary>
        public Altitude CurrentLocationCoordinates { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion declarations

        /// <summary>
        /// constructor
        /// </summary>
        public CityDetailsViewModel()
        {
            this.AllCityCategoryItems = new Dictionary<string, List<CityData>>();
            this.CityCategoryItemsList = new ObservableCollection<Entity.Models.CityData>();
            this.MapCenterPoint = new Altitude();
            this.CityCategoryPushpinsList = new ObservableCollection<CityData>();
            this.CurrentLocationCoordinates = new Altitude();
            this.AllCityItems = new List<CityData>();
        }

        /// <summary>
        /// Parsing json data from url and add into a list.
        /// </summary>
        /// <param name="parameterValue"></param>
        public  void GetCityItemDetails(string parameterValue, GovFinderData cityDetail)
        {
            try
            {
                this.IsDataLoading = true;
                var responseText = string.Empty;
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response =  client.GetAsync(parameterValue).Result;
                    List<CityData> tempCityDataList = new List<CityData>();
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        responseText =  response.Content.ReadAsStringAsync().Result;

                      

                        XElement xmlData = XElement.Parse(responseText, LoadOptions.PreserveWhitespace);
                        foreach (XElement curElement in xmlData.Descendants("row").Descendants("row"))
                        {
                           
                            try
                            {
                                POSH.Socrata.Entity.Models.CityData cityData = new POSH.Socrata.Entity.Models.CityData();
                                cityData.Coordinate = new Altitude();

                                cityData.Name = GetContents(curElement, cityDetail.Name);
                                cityData.SubTitle = GetContents(curElement, cityDetail.SubTitle);
                                cityData.Phone = GetContents(curElement, cityDetail.Phone);
                                cityData.BackgroundImage = "/Assets/CommunityCenters.png";

                                if (cityDetail.Latitude != "NA")
                                {   
                                    if (cityDetail.Latitude == "location.latitude")
                                    {
                                        cityData.Coordinate.Latitude = Convert.ToDouble(curElement.Element("location").Attribute("latitude").Value);
                                    }
                                    else
                                    {
                                        cityData.Coordinate.Latitude = Convert.ToDouble(GetContents(curElement, cityDetail.Latitude));
                                    }
                                }
                                else
                                {
                                    cityData.Coordinate.Latitude = 0; 
                                }

                                if (cityDetail.Longitude != "NA")
                                {
                                    if (cityDetail.Longitude == "location.longitude")
                                    {
                                        cityData.Coordinate.Longitude = Convert.ToDouble(curElement.Element("location").Attribute("longitude").Value);
                                    }
                                    else
                                    {
                                        cityData.Coordinate.Longitude = Convert.ToDouble(GetContents(curElement, cityDetail.Longitude));
                                    }
                                }
                                else
                                {
                                    cityData.Coordinate.Longitude = 0;
                                }


                                if (cityData.Coordinate.Latitude == 0 && cityData.Coordinate.Longitude == 0)
                                {
                                    cityData.Distance = "No distance";
                                }
                                else
                                {
                                    var distance = CalculateDistance(this.CurrentLocationCoordinates.Latitude, this.CurrentLocationCoordinates.Longitude, cityData.Coordinate.Latitude, cityData.Coordinate.Longitude);
                                    cityData.Distance = distance.ToString() + " miles";
                                }

                            

                                cityData.Category = cityDetail.DataSetName;
                                cityData.CompositeKey = GenerateCompositeKey(cityData);
                                cityData.Comments = GetCommentCount(cityData).Result;
                                tempCityDataList.Add(cityData);
                                this.AllCityItems.Add(cityData);

                                if (this.CurrentLocationCoordinates.Latitude != 0 && this.CurrentLocationCoordinates.Longitude != 0 && this.MapCenterPoint.Latitude == 0 && this.MapCenterPoint.Longitude == 0)
                                {
                                    this.MapCenterPoint = this.CurrentLocationCoordinates;
                                }
                                else
                                {
                                    if (this.MapCenterPoint.Latitude == 0 && this.MapCenterPoint.Longitude == 0)
                                    {
                                        this.MapCenterPoint = cityData.Coordinate;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                    var sortList = tempCityDataList.OrderBy(item => item.Distance).ThenBy(item => item.Name).ToList();
                    this.CityCategoryItemsList = new ObservableCollection<CityData>();
                    this.CityCategoryPushpinsList = new ObservableCollection<CityData>();
                    foreach (var item in sortList)
                    {
                        item.IndexNo = this.CityCategoryItemsList.Count + 1;
                        this.CityCategoryItemsList.Add(item);
                        if (this.CityCategoryPushpinsList.Count < 40)
                        {
                            if (item.Coordinate.Latitude != 0 && item.Coordinate.Longitude != 0)
                            {
                                this.CityCategoryPushpinsList.Add(item);
                            }
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
                            this.MapCenterPoint = this.CurrentLocationCoordinates;
                        }
                    }
                    if (!this.AllCityCategoryItems.ContainsKey(cityDetail.DataSetName))
                    {
                        this.AllCityCategoryItems.Add(cityDetail.DataSetName, this.CityCategoryItemsList.ToList());
                    }

                    this.IsDataLoading = false;
                }
                else
                {
                    this.IsDataLoading = false;
                    this.MessageDialog = "No internet connection found. Please check your connection and try again.";
                }
            }
            catch (Exception)
            {
                if (this.CurrentCategoryName.ToLower() == cityDetail.DataSetName.ToLower())
                {
                    this.MessageDialog = "There is some problem on fetching data from server. Please check your internet connection and try again.";
                    this.IsDataLoading = false;
                }
            }
        }

        /// <summary>
        /// method for trimming string value.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetContents(XElement parent, string name)
        {
            try
            {
                if (parent == null)
                {
                    return string.Empty;
                }
                return StringUtils.trim((string)parent.Element(name));
            }
            catch (Exception)
            {
                return string.Empty; ;
            }
        }

        /// <summary>
        /// Gets comment count
        /// </summary>
        /// <param name="cityData"></param>
        public async Task<string> GetCommentCount(CityData cityData)
        {
            CommentViewModel commentViewModel = new CommentViewModel();
            return commentViewModel.GetCommentCount(cityData.CompositeKey, GlobalVariables.CityName, cityData.Category).Result;
        }

        /// <summary>
        /// Generates a composite key for city item
        /// </summary>
        /// <param name="cityItem"></param>
        /// <returns></returns>
        public static string GenerateCompositeKey(CityData cityItem)
        {
            try
            {
                string compositeKey = string.Empty;
                compositeKey = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", cityItem.Name, cityItem.Category, cityItem.Address, cityItem.Coordinate.Latitude.ToString(), cityItem.Coordinate.Longitude.ToString(), cityItem.Phone, cityItem.SubTitle);

                return compositeKey.Replace(" ", "");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Calculates distance between two co-ordinates
        /// </summary>
        /// <param name="currentLat"></param>
        /// <param name="currentLong"></param>
        /// <param name="itemLat"></param>
        /// <param name="itemLong"></param>
        /// <returns></returns>
        public static double CalculateDistance(double currentLat, double currentLong, double itemLat, double itemLong)
        {
            try
            {
                const double degreesToRadians = (Math.PI / 180.0);
                const double earthRadius = 6371; // kilometers

                // convert latitude and longitude values to radians
                var prevRadLat = currentLat * degreesToRadians;
                var prevRadLong = currentLong * degreesToRadians;
                var currRadLat = itemLat * degreesToRadians;
                var currRadLong = itemLong * degreesToRadians;

                // calculate radian delta between each position.
                var radDeltaLat = currRadLat - prevRadLat;
                var radDeltaLong = currRadLong - prevRadLong;

                // calculate distance
                var expr1 = (Math.Sin(radDeltaLat / 2.0) *
                             Math.Sin(radDeltaLat / 2.0)) +

                            (Math.Cos(prevRadLat) *
                             Math.Cos(currRadLat) *
                             Math.Sin(radDeltaLong / 2.0) *
                             Math.Sin(radDeltaLong / 2.0));

                var expr2 = 2.0 * Math.Atan2(Math.Sqrt(expr1),
                                             Math.Sqrt(1 - expr1));

                var distance = (earthRadius * expr2);
                return Math.Round(distance * 0.621371, 0);  // return results as meters
            }
            catch (Exception)
            {
                throw;
            }
        }

    
    }
}
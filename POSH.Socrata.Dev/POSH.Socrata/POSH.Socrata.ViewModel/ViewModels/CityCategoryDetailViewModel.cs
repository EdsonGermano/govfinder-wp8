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
    public class CityCategoryDetailViewModel : INotifyPropertyChanged
    {
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

        private string _errorMessage = string.Empty;
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            {
                _errorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        private ObservableCollection<CityData> _cityCategoryPushpinsList = null;

        /// <summary>
        /// Gets or Sets the DataSetItems pushpin list
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

        private ObservableCollection<CityData> _cityCategoryItemsList = null;

        /// <summary>
        /// Gets or Sets the DataSetItems list
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

        private CityData _cityCategoryItem = null;

        /// <summary>
        /// Gets or Sets the currently selected DataSetItem
        /// </summary>
        public CityData CityCategoryItem
        {
            get
            {
                return _cityCategoryItem;
            }
            set
            {
                _cityCategoryItem = value;
                NotifyPropertyChanged("CityCategoryItem");
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
        public CityCategoryDetailViewModel()
        {
            this.MapCenterPoint = new Altitude();
            this.CityCategoryPushpinsList = new ObservableCollection<CityData>();
            this.CityCategoryItemsList = new ObservableCollection<CityData>();
            this.CityCategoryItem = new CityData();
        }

        /// <summary>
        /// Parsing json data from url and add into a list.
        /// </summary>
        /// <param name="parameterValue"></param>
        public object GetCityItemDetails(CityData selectedCategory)
        {
            object result = null;
            try
            {
                this.CityCategoryPushpinsList.Clear();

                POSH.Socrata.Entity.Models.CityData cityData = new POSH.Socrata.Entity.Models.CityData();
                cityData.Coordinate = new Altitude();

                cityData.Address = selectedCategory.Address;
                cityData.Coordinate.Latitude = Convert.ToDouble(selectedCategory.Coordinate.Latitude);
                cityData.Coordinate.Longitude = Convert.ToDouble(selectedCategory.Coordinate.Longitude);
                cityData.Phone = selectedCategory.Phone;
                cityData.Name = selectedCategory.Name;
                cityData.Distance = selectedCategory.Distance;
                cityData.Comments = selectedCategory.Comments;
                cityData.IndexNo = selectedCategory.IndexNo;
                cityData.SubTitle = selectedCategory.SubTitle;
                cityData.Category = selectedCategory.Category;
                cityData.BackgroundImage = selectedCategory.BackgroundImage;
                cityData.CompositeKey = selectedCategory.CompositeKey;
                this.CityCategoryItem = cityData;
                if (cityData.Coordinate.Latitude != 0 && cityData.Coordinate.Longitude != 0)
                {
                    this.CityCategoryPushpinsList.Add(cityData);
                }

                this.MapCenterPoint = this.CityCategoryItem.Coordinate;
            }

            catch (Exception)
            {
                result = "There is an error on loading report data.";
            }
            return result;
        }

        /// <summary>
        /// Gets city item details for secondary tile
        /// </summary>
        /// <param name="parameterValue"></param>
        /// <param name="categoryName"></param>
        /// <param name="currentLocation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public void GetTileCityItemDetails(string parameterValue, GovFinderData cityDetail, Altitude currentLocation, string compositeKey)
        {
            try
            {
                var responseText = string.Empty;
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    //HttpClient client = new HttpClient();
                    //HttpResponseMessage response = client.GetAsync(parameterValue).Result;

                    //response.EnsureSuccessStatusCode();
                    //string responseBody = response.Content.ReadAsStringAsync().Result;

                    //List<CityData> tempCityDataList = new List<CityData>();
                    //var dataSetItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CityAllData>>(responseBody);
                    //if (dataSetItems != null)
                    //{
                    //    foreach (var item in dataSetItems)
                    //    {
                    //        try
                    //        {
                    //            POSH.Socrata.Entity.Models.CityData cityData = new POSH.Socrata.Entity.Models.CityData();
                    //            cityData.Coordinate = new Altitude();

                    //            if (!string.IsNullOrEmpty(item.address))
                    //            {
                    //                cityData.Address = item.address.ToString();
                    //            }
                    //            else if (!string.IsNullOrEmpty(item.human_address))
                    //            {
                    //                cityData.Address = item.human_address.ToString();
                    //            }
                    //            else if (!string.IsNullOrEmpty(item.street_address))
                    //            {
                    //                cityData.Address = item.street_address.ToString();
                    //            }
                    //            else
                    //            {
                    //                cityData.Address = "No address";
                    //            }

                    //            if (!string.IsNullOrEmpty(item.latitude))
                    //            {
                    //                cityData.Coordinate.Latitude = Convert.ToDouble(item.latitude.ToString());
                    //            }
                    //            else if (item.location != null && !string.IsNullOrEmpty(item.location.latitude))
                    //            {
                    //                cityData.Coordinate.Latitude = Convert.ToDouble(item.location.latitude.ToString());
                    //            }
                    //            else
                    //            {
                    //                cityData.Coordinate.Latitude = 0;
                    //            }

                    //            if (!string.IsNullOrEmpty(item.longitude))
                    //            {
                    //                cityData.Coordinate.Longitude = Convert.ToDouble(item.longitude.ToString());
                    //            }
                    //            else if (item.location != null && !string.IsNullOrEmpty(item.location.longitude))
                    //            {
                    //                cityData.Coordinate.Longitude = Convert.ToDouble(item.location.longitude.ToString());
                    //            }
                    //            else
                    //            {
                    //                cityData.Coordinate.Longitude = 0;
                    //            }

                    //            if (cityData.Coordinate.Latitude == 0 && cityData.Coordinate.Longitude == 0)
                    //            {
                    //                cityData.Distance = "No distance";
                    //            }
                    //            else
                    //            {
                    //                var distance = CityDetailsViewModel.CalculateDistance(currentLocation.Latitude, currentLocation.Longitude, cityData.Coordinate.Latitude, cityData.Coordinate.Longitude);
                    //                cityData.Distance = distance.ToString() + " miles";
                    //            }

                    //            if (!string.IsNullOrEmpty(item.phone))
                    //            {
                    //                cityData.Phone = item.phone.ToString();
                    //            }
                    //            else
                    //            {
                    //                cityData.Phone = "No phone number";
                    //            }

                    //            if (!string.IsNullOrEmpty(item.name))
                    //            {
                    //                cityData.Name = item.name.ToString();
                    //            }
                    //            else if (!string.IsNullOrEmpty(item.station_name))
                    //            {
                    //                cityData.Name = item.station_name.ToString();
                    //            }
                    //            else if (!string.IsNullOrEmpty(item.architect))
                    //            {
                    //                cityData.Name = item.architect.ToString();
                    //            }
                    //            else if (!string.IsNullOrEmpty(item.Event))
                    //            {
                    //                cityData.Name = item.Event.ToString();
                    //            }
                    //            else
                    //            {
                    //                cityData.Name = "No name";
                    //            }

                    //            if (!string.IsNullOrEmpty(item.sub_category))
                    //            {
                    //                cityData.SubTitle = item.sub_category;
                    //            }
                    //            else if (!string.IsNullOrEmpty(item.fuel_type_code))
                    //            {
                    //                cityData.SubTitle = item.fuel_type_code;
                    //            }
                    //            else if (!string.IsNullOrEmpty(item.building_code))
                    //            {
                    //                cityData.SubTitle = item.building_code;
                    //            }
                    //            else
                    //            {
                    //                cityData.SubTitle = "No subtitle";
                    //            }

                    //            if (!string.IsNullOrEmpty(item.image))
                    //            {
                    //                cityData.BackgroundImage = item.image;
                    //            }
                    //            else
                    //            {
                    //                cityData.BackgroundImage = "/Assets/CommunityCenters.png";
                    //            }

                    //            cityData.Category = categoryName;

                    //            cityData.CompositeKey = CityDetailsViewModel.GenerateCompositeKey(cityData);
                    //            cityData.Comments = this.GetCommentCount(cityData).Result;

                    //            tempCityDataList.Add(cityData);

                    //            if (currentLocation.Latitude != 0 && currentLocation.Longitude != 0 && this.MapCenterPoint.Latitude == 0 && this.MapCenterPoint.Longitude == 0)
                    //            {
                    //                this.MapCenterPoint = currentLocation;
                    //            }
                    //            else
                    //            {
                    //                if (this.MapCenterPoint.Latitude == 0 && this.MapCenterPoint.Longitude == 0)
                    //                {
                    //                    this.MapCenterPoint = cityData.Coordinate;
                    //                }
                    //            }
                    //        }
                    //        catch (Exception)
                    //        {
                    //            continue;
                    //        }
                    //    }
                    //}
                    //var sortList = tempCityDataList.OrderBy(item => item.Distance).ThenBy(item => item.Name).ToList();

                    //this.CityCategoryItemsList = new ObservableCollection<CityData>();
                    //this.CityCategoryPushpinsList = new ObservableCollection<CityData>();
                    //foreach (var item in sortList)
                    //{
                    //    item.IndexNo = this.CityCategoryItemsList.Count + 1;
                    //    this.CityCategoryItemsList.Add(item);
                    //    if (this.CityCategoryPushpinsList.Count < 40)
                    //    {
                    //        if (item.Coordinate.Latitude != 0 && item.Coordinate.Longitude != 0)
                    //        {
                    //            this.CityCategoryPushpinsList.Add(item);
                    //        }
                    //    }
                    //}

                    //if (this.CityCategoryItemsList.FirstOrDefault() != null)
                    //{
                    //    var pinnedItem = this.CityCategoryItemsList.Where(item => item.CompositeKey.ToLower() == compositeKey.ToLower()).FirstOrDefault();
                    //    if (pinnedItem != null)
                    //    {
                    //        if (pinnedItem.Coordinate.Latitude != 0 && pinnedItem.Coordinate.Longitude != 0)
                    //        {
                    //            this.MapCenterPoint = pinnedItem.Coordinate;
                    //        }
                    //        else
                    //        {
                    //            this.MapCenterPoint = currentLocation;
                    //        }

                    //        GetCityItemDetails(pinnedItem);
                    //    }
                    //}
                    
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = client.GetAsync(parameterValue).Result;
                    List<CityData> tempCityDataList = new List<CityData>();
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        responseText = response.Content.ReadAsStringAsync().Result;



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
                                    var distance = CityDetailsViewModel.CalculateDistance(currentLocation.Latitude, currentLocation.Longitude, cityData.Coordinate.Latitude, cityData.Coordinate.Longitude);
                                    cityData.Distance = distance.ToString() + " miles";
                                }



                                cityData.Category = cityDetail.DataSetName;
                                cityData.CompositeKey = CityDetailsViewModel.GenerateCompositeKey(cityData);
                                cityData.Comments = GetCommentCount(cityData).Result;
                                tempCityDataList.Add(cityData);
                               
                                if (currentLocation.Latitude != 0 && currentLocation.Longitude != 0 && this.MapCenterPoint.Latitude == 0 && this.MapCenterPoint.Longitude == 0)
                                {
                                    this.MapCenterPoint = currentLocation;
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
                        var pinnedItem = this.CityCategoryItemsList.Where(item => item.CompositeKey.ToLower() == compositeKey.ToLower()).FirstOrDefault();
                        if (pinnedItem != null)
                        {
                            if (pinnedItem.Coordinate.Latitude != 0 && pinnedItem.Coordinate.Longitude != 0)
                            {
                                this.MapCenterPoint = pinnedItem.Coordinate;
                            }
                            else
                            {
                                this.MapCenterPoint = currentLocation;
                            }

                            GetCityItemDetails(pinnedItem);
                        }
                    }
                }
            }
            catch (Exception)
            {
                this.ErrorMessage = "There is an error on fetching data from server. Please try again...";
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
    }
}
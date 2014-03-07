using POSH.Socrata.Entity.Models;
using POSH.Socrata.ViewModel.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace POSH.Socrata.ViewModel.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region declarations

        private POSH.Socrata.PclStorage.GovXMLData fileStorage;

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

        private ErrorMessage _errorMessage = null;

        /// <summary>
        /// Gets or sets ErrorMessage object
        /// </summary>
        public ErrorMessage ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        private bool _isCityDataLoading = true;

        /// <summary>
        /// Gets or sets whether data loaded or not
        /// </summary>
        public bool IsCityDataLoading
        {
            get
            {
                return _isCityDataLoading;
            }
            set
            {
                if (value != _isCityDataLoading)
                {
                    _isCityDataLoading = value;
                    NotifyPropertyChanged("IsCityDataLoading");
                }
            }
        }

        private string _pageBackground = "/Assets/BackgroundImage.png";

        /// <summary>
        /// Gets or sets the page background.
        /// </summary>
        /// <value>
        /// The page background.
        /// </value>
        public string PageBackground
        {
            get
            {
                return _pageBackground;
            }
            set
            {
                if (value != _pageBackground)
                {
                    _pageBackground = value;
                    NotifyPropertyChanged("PageBackground");
                }
            }
        }

        private ObservableCollection<GovFinderData> _cityDataSetList = null;

        /// <summary>
        /// Gets or sets all city with datasets list
        /// </summary>
        public ObservableCollection<GovFinderData> CityDataSetList
        {
            get
            {
                return _cityDataSetList;
            }
            set
            {
                if (value != _cityDataSetList)
                {
                    _cityDataSetList = value;
                    NotifyPropertyChanged("CityDataSetList");
                }
            }
        }

        private ObservableCollection<GovFinderData> _filteredCityDataSetList = null;

        /// <summary>
        /// Gets or sets selected city with datasets
        /// </summary>
        public ObservableCollection<GovFinderData> FilteredCityDataSetList
        {
            get
            {
                return _filteredCityDataSetList;
            }
            set
            {
                _filteredCityDataSetList = value;
                NotifyPropertyChanged("FilteredCityDataSetList");
            }
        }

        private ObservableCollection<string> _citiesList = null;

        /// <summary>
        /// Gets or sets selected city with datasets
        /// </summary>
        public ObservableCollection<string> CitiesList
        {
            get
            {
                return _citiesList;
            }
            set
            {
                _citiesList = value;
                NotifyPropertyChanged("CitiesList");
            }
        }

        /// <summary>
        /// Gets CityDetailsViewModel object
        /// </summary>
        public CityDetailsViewModel CityDetailsViewModel { get; set; }

        /// <summary>
        /// Gets CityCategoryDetailViewModel object
        /// </summary>
        public CityCategoryDetailViewModel CityCategoryDetailViewModel { get; set; }

        /// <summary>
        /// Gets FullMapViewModel object
        /// </summary>
        public FullMapViewModel FullMapViewModel { get; set; }

        /// <summary>
        /// Gets SearchViewModel object
        /// </summary>
        public SearchViewModel SearchViewModel { get; set; }

        /// <summary>
        /// Gets SearchViewModel object
        /// </summary>
        public CommentViewModel CommentViewModel { get; set; }

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
        public MainViewModel()
        {
            this.CityDetailsViewModel = new CityDetailsViewModel();
            this.CityCategoryDetailViewModel = new CityCategoryDetailViewModel();
            this.CityDataSetList = new ObservableCollection<GovFinderData>();
            this.FilteredCityDataSetList = new ObservableCollection<GovFinderData>();
            this.FullMapViewModel = new FullMapViewModel();
            this.ErrorMessage = new ErrorMessage();
            this.SearchViewModel = new SearchViewModel();
            this.CommentViewModel = new CommentViewModel();
            this.CitiesList = new ObservableCollection<string>();
        }

        /// <summary>
        /// Method to load xml from url and add into collection
        /// </summary>
      public async Task LoadXML()
        {
            try
            {
                fileStorage = new POSH.Socrata.PclStorage.GovXMLData();
                bool hasError = await fileStorage.GetData();
                if (hasError == false)
                {
                    string response = fileStorage.responseText;
                    XElement xmlData = XElement.Parse(response, LoadOptions.PreserveWhitespace);
                    foreach (XElement curElement in xmlData.Descendants("row").Descendants("row"))
                    {
                        try
                        {
                            var hasApiUrl = GetContents(curElement, "api_url");
                            var isValid = GetContents(curElement, "isvalid");
                            if (hasApiUrl != "" && isValid.ToLower()=="1")
                            {
                                CityDataSetList.Add(new GovFinderData
                                {
                                    City = GetContents(curElement, "city"),
                                    DataSetName = GetContents(curElement, "dataset_name"),
                                    Url = GetContents(curElement, "url"),
                                    ApiUrl = hasApiUrl,
                                    ImageUrl = GetDatasetImagePath(GetContents(curElement, "dataset_name")),
                                    Name = GetContents(curElement, "name"),
                                    Phone = GetContents(curElement, "phone"),
                                    SubTitle = GetContents(curElement, "subtitle"),
                                    Latitude = GetContents(curElement, "latitude"),
                                    Longitude = GetContents(curElement, "longitude"),
                                });
                            }                          
                        }
                        catch (Exception ex)
                        {
                            this.MessageDialog = ex.ToString();
                        }
                    }
                    this.GetSelectedCityDataSets(GlobalVariables.CityName);
                    this.IsCityDataLoading = false;
                }
                else
                {
                    this.ErrorMessage = new ErrorMessage() { MessageText = fileStorage.errorMessage };
                    this.IsCityDataLoading = false;
                }
            }
            catch (Exception)
            {
                this.ErrorMessage = new ErrorMessage() { MessageText = "Internet connection required. Please check your connection and try again." };
                this.IsCityDataLoading = false;
            }
        }

      public async Task RefreshXML()
      {
          try
          {
              fileStorage = new POSH.Socrata.PclStorage.GovXMLData();
              bool hasError = await fileStorage.DownloadFileAndSave();
              if (hasError == false)
              {
                  string response = fileStorage.responseText;
                  XElement xmlData = XElement.Parse(response, LoadOptions.PreserveWhitespace);
                  foreach (XElement curElement in xmlData.Descendants("row").Descendants("row"))
                  {
                      try
                      {
                          var hasApiUrl = GetContents(curElement, "api_url");
                          var isValid = GetContents(curElement, "isvalid");
                          if (hasApiUrl != "" && isValid.ToLower() == "1")
                          {
                              CityDataSetList.Add(new GovFinderData
                              {
                                  City = GetContents(curElement, "city"),
                                  DataSetName = GetContents(curElement, "dataset_name"),
                                  Url = GetContents(curElement, "url"),
                                  ApiUrl = hasApiUrl,
                                  ImageUrl = GetDatasetImagePath(GetContents(curElement, "dataset_name")),
                                  Name = GetContents(curElement, "name"),
                                  Phone = GetContents(curElement, "phone"),
                                  SubTitle = GetContents(curElement, "subtitle"),
                                  Latitude = GetContents(curElement, "latitude"),
                                  Longitude = GetContents(curElement, "longitude"),
                              });
                          }
                      }
                      catch (Exception ex)
                      {
                          this.MessageDialog = ex.ToString();
                      }
                  }
                  this.GetSelectedCityDataSets(GlobalVariables.CityName);
                
              }
              else
              {
                  this.ErrorMessage = new ErrorMessage() { MessageText = fileStorage.errorMessage };
                  this.IsCityDataLoading = false;
              }
          }
          catch (Exception)
          {
              this.ErrorMessage = new ErrorMessage() { MessageText = "Internet connection required. Please check your connection and try again." };
              this.IsCityDataLoading = false;
          }
      }

        /// <summary>
        /// Gets the dataset images url from local
        /// </summary>
        /// <param name="dataSetName"></param>
        /// <returns></returns>
        public string GetDatasetImagePath(string dataSetName)
        {
            try
            {
                if (dataSetName.ToLower().Contains("community"))
                {
                    return "/Assets/DatasetImages/Community Service Centers.png";
                }
                else if (dataSetName.ToLower().Contains("fire"))
                {
                    return "/Assets/DatasetImages/Fire Houses.png";
                }
                else if (dataSetName.ToLower().Contains("libraries"))
                {
                    return "/Assets/DatasetImages/Libraries.png";
                }
                else if (dataSetName.ToLower().Contains("health"))
                {
                    return "/Assets/DatasetImages/Health Clinic.png";
                }
                else if (dataSetName.ToLower().Contains("police"))
                {
                    return "/Assets/DatasetImages/Police Stations.png";
                }
                else if (dataSetName.ToLower().Contains("recreation"))
                {
                    return "/Assets/DatasetImages/Recreation Centres.png";
                }
                else if (dataSetName.ToLower().Contains("office"))
                {
                    return "/Assets/DatasetImages/RepresentativeOffice.png";
                }
                else if (dataSetName.ToLower().Contains("school"))
                {
                    return "/Assets/DatasetImages/School.png";
                }
                else if (dataSetName.ToLower().Contains("technology"))
                {
                    return "/Assets/DatasetImages/Technology Center.png";
                }
                else if (dataSetName.ToLower().Contains("workforce"))
                {
                    return "/Assets/DatasetImages/Workforce Centers.png";
                }
                else
                {
                    return "/Assets/DatasetImages/Community Service Centers.png";
                }
            }
            catch (Exception ex)
            {
                this.MessageDialog = ex.ToString();
                return "/Assets/DatasetImages/Community Service Centers.png";
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
        /// Gets the city background.
        /// </summary>
         public void GetCityBackground(string cityName)
        {
            try
            {
                switch (cityName)
                {
                    case "Chicago":
                        this.PageBackground = "/Assets/BackgroundImage.png";
                        break;

                    case "New York":
                        this.PageBackground = "/Assets/nyBackground.png";
                        break;

                    case "Edmonton":
                        this.PageBackground = "/Assets/edBackground.png";
                        break;

                    default:
                        this.PageBackground = "/Assets/BackgroundImage.png";
                        break;
                }
            }
            catch (Exception)
            {
                this.PageBackground = "/Assets/BackgroundImage.png";
            }
        }

        /// <summary>
        /// Filters DataSetItems for selected city
        /// </summary>
        public void GetSelectedCityDataSets(string cityName)
        {
            try
            {
                if (this.FilteredCityDataSetList.Count != 0)
                {
                    var lastCity = this.FilteredCityDataSetList.FirstOrDefault().City;
                    if (lastCity.ToLower() != cityName.ToLower())
                    {
                        this.CityDetailsViewModel.AllCityCategoryItems.Clear();
                        this.CityDetailsViewModel.AllCityItems.Clear();
                    }
                }
                this.FilteredCityDataSetList.Clear();
                this.FilteredCityDataSetList = (from item in this.CityDataSetList where item.City == cityName select item).ToObservableCollection();
            }
            catch (Exception)
            {
                this.MessageDialog = "Data not found. Please try again.";
            }
        }

        /// <summary>
        /// Gets cities list
        /// </summary>
        public void GetCities()
        {
            try
            {
                if (this.CitiesList.Count == 0)
                {
                    var cities = this.CityDataSetList.GroupBy(item => item.City).ToList();
                    List<string> tempList = new List<string>();
                    foreach (var city in cities)
                    {
                        tempList.Add(city.Key);                        
                    }
                     this.CitiesList=tempList.OrderBy(item=>item).ToObservableCollection();
                }
            }
            catch (Exception)
            {
                this.MessageDialog = "There is some error on loading cities.";
            }
        }
    }
}
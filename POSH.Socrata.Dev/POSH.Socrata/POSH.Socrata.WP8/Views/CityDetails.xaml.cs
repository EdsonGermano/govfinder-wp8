using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using POSH.Socrata.Entity.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace POSH.Socrata.WP8.Views
{
    public partial class CityPlacesDetails : PhoneApplicationPage
    {
        #region "========Local variables ============"

        private CityDetailUserControl ucCityDetailsControl;
        private List<PivotItem> categoriesPivotList;
        private ObservableCollection<CityDetailUserControl> userControlList;

        #endregion "========Local variables ============"

        /// <summary>
        ///Page constructor
        /// </summary>
        public CityPlacesDetails()
        {
            InitializeComponent();
            userControlList = new ObservableCollection<CityDetailUserControl>();
            App.ViewModel.CityDetailsViewModel.PropertyChanged -= CityDetailsViewModel_PropertyChanged;
            App.ViewModel.CityDetailsViewModel.PropertyChanged += CityDetailsViewModel_PropertyChanged;
        }

        #region "========Page Events ============"

        private void CityDetailsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string message = ((POSH.Socrata.ViewModel.ViewModels.CityDetailsViewModel)(sender)).MessageDialog;
            this.Dispatcher.BeginInvoke(() =>
                                    {
                                        if (e.PropertyName == "MessageDialog")
                                        {
                                            MessageBox.Show(message);
                                        }
                                    });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                App.ViewModel.CityCategoryDetailViewModel.CityCategoryItemsList = App.ViewModel.CityDetailsViewModel.CityCategoryItemsList;
            }
            else
            {
                App.ViewModel.CityCategoryDetailViewModel.CityCategoryItemsList.Clear();
                App.ViewModel.CityDetailsViewModel.CityCategoryItemsList.Clear();
            }
            App.ViewModel.CityDetailsViewModel.PropertyChanged -= CityDetailsViewModel_PropertyChanged;
        }

        /// <summary>
        /// Add pivot dynamically on the bases of  filter city list count.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                this.DataContext = App.ViewModel.CityDetailsViewModel;
                string parameterValue = NavigationContext.QueryString["parameter"];
                string[] parameters =parameterValue.Split(new string[]{ Constant.Seprator }, StringSplitOptions.None) ;
                if (e.NavigationMode == NavigationMode.New)
                {              
                    var localSettings = IsolatedStorageSettings.ApplicationSettings;
                    int count = 0;
                    if (localSettings.Contains("TotalUpdatedItems"))
                     {
                     count = Convert.ToInt32(localSettings["TotalUpdatedItems"]);
                     }
                    if (count != 0)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            if (localSettings.Contains("DatasetName" + i))
                            {
                                if (localSettings["DatasetName" + i].ToString().Equals(parameters[0]) && localSettings["City" + i].ToString().Equals(parameters[1]))
                                {
                                    localSettings["UpdatedItems"] = Convert.ToInt32(localSettings["UpdatedItems"]) - 1;

                                    localSettings.Remove("DatasetName" + i);
                                    localSettings.Remove("City" + i);

                                    localSettings.Save();
                                    if (Convert.ToInt32(localSettings["UpdatedItems"]) > 0)
                                    {
                                        UpdatePrimaryTile(localSettings["UpdatedItems"].ToString() + " " + Constant.NotificationMsg);
                                        ShellToast toast = new ShellToast();
                                        toast.Title = localSettings["UpdatedItems"].ToString();
                                        toast.Content = Constant.NotificationMsg;
                                        toast.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
                                        toast.Show();
                                    }
                                    else
                                    {
                                        UpdatePrimaryTile(string.Empty);
                                    }
                                }
                            }
                        }                         
                    }
                    LoadPivotItems(parameters[0]);
                }
                else if (e.NavigationMode == NavigationMode.Back)
                {
                    ucCityDetailsControl.LoadCategoriesData();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void cityItemsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var data = (ucCityDetailsControl.DataContext as GovFinderData);
            CityData selectedCategory = (sender as ListBox).SelectedItem as CityData;
            if (selectedCategory != null)
            {
                App.ViewModel.CityCategoryDetailViewModel.CityCategoryItem = new CityData();
                var errorMessage = App.ViewModel.CityCategoryDetailViewModel.GetCityItemDetails(selectedCategory);
                if (errorMessage == null)
                {
                    NavigationService.Navigate(new Uri("/Views/CityCategoryDetail.xaml?parameter=" + data.ApiUrl, UriKind.Relative));
                }
                else
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(errorMessage as string);
                    });
                }               
            }
        }

        /// <summary>
        /// Load user control in to selected pivot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void pvCities_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            try
            {
                ucCityDetailsControl = new CityDetailUserControl();
                ucCityDetailsControl.DataContext = (pvCities.SelectedItem as PivotItem).DataContext;
                e.Item.Content = ucCityDetailsControl;
                await ucCityDetailsControl.LoadCategoriesData();
                ucCityDetailsControl.cityItemsList.Tap += cityItemsList_Tap;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// unload the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pvCities_UnloadedPivotItem(object sender, PivotItemEventArgs e)
        {
            try
            {
                if ((e.Item.Content as CityDetailUserControl).cts != null)
                {
                    (e.Item.Content as CityDetailUserControl).cts.Cancel();
                    (e.Item.Content as CityDetailUserControl).cityItemsList.Tap -= cityItemsList_Tap;
                }
                e.Item.Content = null;
                //App.ViewModel.CityDetailsViewModel.CityCategoryItemsList.Clear();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        #endregion "========Page Events ============"

        #region "========Page Functions ============"

        /// <summary>
        /// Loads pivot items for all categories of a city
        /// </summary>
        /// <param name="selectedCategory"></param>
        public void LoadPivotItems(string selectedCategory)
        {
            try
            {
                categoriesPivotList = new List<PivotItem>();
                foreach (var item in App.ViewModel.FilteredCityDataSetList)
                {
                    PivotItem pvItem = new PivotItem();
                    pvItem.DataContext = item;
                    pvItem.Header = item.DataSetName;
                    categoriesPivotList.Add(pvItem);
                }
                this.pvCities.ItemsSource = categoriesPivotList;
                var selectedPivotItem = categoriesPivotList.Where(item => item.Header.ToString().ToLower() == selectedCategory.ToLower()).FirstOrDefault();
                if (selectedPivotItem != null)
                {
                    this.pvCities.SelectedItem = selectedPivotItem;
                }
                LoadFirstPivotItem();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Loads first pivot item content at page load
        /// </summary>
        private async void LoadFirstPivotItem()
        {
            ucCityDetailsControl = new CityDetailUserControl();
            ucCityDetailsControl.DataContext = (pvCities.SelectedItem as PivotItem).DataContext;
            (pvCities.SelectedItem as PivotItem).Content = ucCityDetailsControl;
            await ucCityDetailsControl.LoadCategoriesData();
            ucCityDetailsControl.cityItemsList.Tap += cityItemsList_Tap;
            pvCities.LoadedPivotItem += pvCities_LoadedPivotItem;
            pvCities.UnloadedPivotItem += pvCities_UnloadedPivotItem;
        }

        #endregion "========Page Functions ============"


        #region"===========UpdateTile========"
        /// <summary>
        /// Updates primary tile data
        /// </summary>
        private void UpdatePrimaryTile(string tileText)
        {
            FlipTileData TileData = new FlipTileData()
            {
                // BackTitle = content,

                Title = "Gov Finder",
                BackContent = tileText,
                WideBackContent = tileText,
                BackgroundImage = new Uri("Assets/336-336.png", UriKind.Relative),
                SmallBackgroundImage = new Uri("Assets/159-159.png", UriKind.Relative),
                WideBackgroundImage = new Uri("Assets/691-336.png", UriKind.Relative),
                BackBackgroundImage = new Uri("Assets/CommunityCenters.png", UriKind.Relative),
                WideBackBackgroundImage = new Uri("Assets/CommunityCenters.png", UriKind.Relative),
            };
            ShellTile primaryTile = ShellTile.ActiveTiles.First();
            primaryTile.Update(TileData);
        }
        #endregion
    }
}
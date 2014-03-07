using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using POSH.Socrata.Entity.Models;
using POSH.Socrata.ViewModel.HelperClasses;
using POSH.Socrata.WP8.Resources;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace POSH.Socrata.WP8.Views
{
    public partial class Search : PhoneApplicationPage
    {
        /// <summary>
        /// Page constructor
        /// </summary>
        public Search()
        {
            InitializeComponent();
            this.txtSearchText.KeyDown += txtSearchText_KeyDown;
        }

        #region "==========Page Events============="

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.DataContext = App.ViewModel.SearchViewModel;
            this.lbxSearchItems.SelectedItem = null;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                App.ViewModel.SearchViewModel.SearchList.Clear();
            }
        }

        private async void txtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            await App.ViewModel.SearchViewModel.GetSearchResult(this.txtSearchText.Text.ToLower().Trim(), App.ViewModel.CityDetailsViewModel.AllCityItems);
            if (App.ViewModel.SearchViewModel.SearchList.Count == 0)
            {
                this.tbkNoResult.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbkNoResult.Visibility = Visibility.Collapsed;
            }
        }

        private async void txtSearchText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await App.ViewModel.SearchViewModel.GetSearchResult(this.txtSearchText.Text.ToLower().Trim(), App.ViewModel.CityDetailsViewModel.AllCityItems);
                if (App.ViewModel.SearchViewModel.SearchList.Count == 0)
                {
                    this.tbkNoResult.Visibility = Visibility.Visible;
                }
                else
                {
                    this.tbkNoResult.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void lbxSearchItems_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CityData selectedCategory = (sender as ListBox).SelectedItem as CityData;
            if (selectedCategory != null)
            {
                var data = App.ViewModel.FilteredCityDataSetList.Where(item => item.DataSetName.ToLower() == selectedCategory.Category.ToLower()).FirstOrDefault();

                if (App.ViewModel.CityDetailsViewModel.AllCityCategoryItems.ContainsKey(data.DataSetName))
                {
                    App.ViewModel.CityCategoryDetailViewModel.CityCategoryItemsList = App.ViewModel.CityDetailsViewModel.AllCityCategoryItems[data.DataSetName].ToObservableCollection();
                }

                App.ViewModel.CityCategoryDetailViewModel.CityCategoryItem = new CityData();
                App.ViewModel.CityCategoryDetailViewModel.GetCityItemDetails(selectedCategory);
                var frame = App.Current.RootVisual as PhoneApplicationFrame;
                frame.Navigate(new Uri("/Views/CityCategoryDetail.xaml?parameter=" + data.ApiUrl, UriKind.Relative));
            }
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

        #endregion "==========Page Events============="
    }
}
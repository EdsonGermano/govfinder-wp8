using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Linq;
using System.Windows.Navigation;

namespace POSH.Socrata.WP8.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        /// <summary>
        /// Page constructor
        /// </summary>
        public Settings()
        {
            InitializeComponent();
        }

        #region "========== Page Events============="

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.DataContext = App.ViewModel;
            App.ViewModel.GetCities();
            var cityName = App.setting["cityName"].ToString();
            this.lpkCityName.SelectedItem = App.ViewModel.CitiesList.Where(obj => obj.ToLower() == cityName.ToLower()).FirstOrDefault();// lpkCityName.Items.Where(obj => (string)(obj as ListPickerItem).Content == cityName).FirstOrDefault();
            this.chkLocation.IsChecked = (bool)App.setting["allowGps"];
            this.tsNotification.IsChecked = (bool)App.setting["notification"];
        }

        /// <summary>
        /// Code to save the settings to isolated storage on navigating from the page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.setting["allowGps"] = (bool)this.chkLocation.IsChecked;
            App.setting["notification"] = (bool)this.tsNotification.IsChecked;
            App.setting["cityName"] = (lpkCityName.SelectedItem as string);
            App.setting.Save();
        }

        /// <summary>
        /// Click event for rate this app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appbar_rate_app_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketPlaceReviewTask = new MarketplaceReviewTask();
            marketPlaceReviewTask.Show();
        }

        #endregion "========== Page Events============="
    }
}
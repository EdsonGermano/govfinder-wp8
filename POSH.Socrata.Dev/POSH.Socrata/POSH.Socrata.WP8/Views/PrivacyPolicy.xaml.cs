using Microsoft.Phone.Controls;
using POSH.Socrata.WP8.Resources;

namespace POSH.Socrata.WP8.Views
{
    public partial class PrivacyPolicy : PhoneApplicationPage
    {
        public PrivacyPolicy()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);           
        }

        private void tbkPrivacyPolicy_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.tbkPrivacyPolicy.Text = AppResources.PrivacyPolicyText;
            this.tbkPrivacyPolicy.SizeChanged += tbkPrivacyPolicy_SizeChanged;
        }

        private void tbkPrivacyPolicy_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.tbkLoadingData.Visibility = System.Windows.Visibility.Collapsed;            
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        private string URI;
        public WebViewPage(string link)
        {
            InitializeComponent();
            URI = link;
            Browser.Source = URI;
        }

        private void backClicked(object sender, EventArgs e)
        {
            // Check to see if there is anywhere to go back to
            if (Browser.CanGoBack)
            {
                Browser.GoBack();
            }
            else
            { // If not, leave the view
                Navigation.PopAsync();
            }
        }

        private void forwardClicked(object sender, EventArgs e)
        {
            if (Browser.CanGoForward)
            {
                Browser.GoForward();
            }
        }

        private void OpenInBrowser(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(URI));
        }

        void webOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            LoadingLabel.IsVisible = true;
        }

        void webOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            LoadingLabel.IsVisible = false;
        }
    }
}
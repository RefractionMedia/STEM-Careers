using STEM_Careers.Data;
using STEM_Careers.Helpers;
using STEM_Careers.ViewModels;
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
    public partial class LoadingPage : ContentPage
    {
        private double progressDouble { get; set; }

        public double ProgressDouble
        {
            get { return progressDouble; }
            set
            {
                progressDouble = value;
                UpdateBar();
            }
        }

        public LoadingPage()
        {
            InitializeComponent();
            RetryButton.IsVisible = false;
            RetryButton.IsEnabled = false;
            ProgressDouble = 0.0;

            MessagingCenter.Subscribe<App>(this, "ConnectivityChanged", (sender) =>
            {
                if (App.Database.IsInitialized())
                {
                    return;
                }
                else
                {
                    if (App.HasInternetConnexion())
                        App.Database.RetryInitAsync();
                    else
                    {
                        ProgressDouble = 0.0;
                        commentLabel.Text = "Loading error, please enable internet";
                        commentLabel.TextColor = Color.Red;
                        RetryButton.IsVisible = true;
                        RetryButton.IsEnabled = true;
                    }
                }
            });

            
            MessagingCenter.Subscribe<Database, string>(this, "DatabaseInfo", (sender, str) =>
            {
                if (!App.HasInternetConnexion() && !App.Database.IsInitialized())
                {
                    ProgressDouble = 0.0;
                    commentLabel.Text = "Loading error, please enable internet";
                    commentLabel.TextColor = Color.Red;
                    RetryButton.IsVisible = true;
                    RetryButton.IsEnabled = true;
                    return;
                }
                switch (str)
                {
                    case "Initializing":
                        ProgressDouble = 0.15;
                        commentLabel.Text = "Heating up";
                        commentLabel.TextColor = Color.Default;
                        RetryButton.IsEnabled = false;
                        RetryButton.IsVisible = false;
                        break;
                    
                    case "Tables Dropped":
                        commentLabel.Text = "Dumping space waste";
                        ProgressDouble = 0.20;
                        break;
                    case "Tables created":
                        commentLabel.Text = "Downloading...";
                        ProgressDouble = 0.66;
                        break;
                    case "Initialized":
                        ProgressDouble = 1.0;
                        commentLabel.Text = "Ready";
                        AnimateToHomeScreen();
                        break;
                    case "Initialization error":
                        RetryButton.IsVisible = true;
                        RetryButton.IsEnabled = true;
                        commentLabel.Text = "Something went wrong.. Please retry";
                        commentLabel.TextColor = Color.Red;
                        break;
                }
            });

            if (App.Database.IsInitialized())
            {
                ProgressDouble = 1.0;
                commentLabel.Text = "Let us begin";
                AnimateToHomeScreen();
            }
        }



        private async Task UpdateBar()
        {
            await progressBar.ProgressTo(ProgressDouble, 4000, Easing.CubicInOut);
        }

        private async Task AnimateToHomeScreen()
        {
            await Task.Delay(1000);
            App.Current.MainPage = new MainPage.MainPage();
        }

        private void RetryButton_Clicked(object sender, EventArgs e)
        {
            if (App.HasInternetConnexion())
            {
                if (App.Database.RetryInitAsync())
                {
                    RetryButton.IsEnabled = false;
                    RetryButton.IsVisible = false;
                }
            }
        }
    }
}
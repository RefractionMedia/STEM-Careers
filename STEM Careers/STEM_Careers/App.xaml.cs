using Plugin.Connectivity;
using STEM_Careers.Data;
using STEM_Careers.Helpers;
using STEM_Careers.Views;
using System;

using Xamarin.Forms;

namespace STEM_Careers
{
    public partial class App : Application
    {
        public static bool UseMock
            
            = false;
        public static string AzureMobileAppUrl = "https://[CONFIGURE-THIS-URL].azurewebsites.net";

        //Database 
        static Database database;
        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(DependencyService.Get<IFileHelper>().GetLocalFilePath("Database.db"));
                }
                return database;
            }
            set { }
        }

        static CwSTEMapi api;
        public static CwSTEMapi Api
        {
            get
            {
                if (api == null)
                {
                    api = new CwSTEMapi();
                }
                return api;
            }
            set { }
        }

        static internal PeopleHelper webHelper = new PeopleHelper();

        #region PickerIndexes Functions and variables
        //These properties are to use the Properties Dictionnary in the app (saves values of selected picker when navigating through pages)
        const string regionPickerIndex = "regionPickerIndex";
        const string fieldPickerIndex = "fieldPickerIndex";
        const string xPickerIndex = "XPickerIndex";

        public int RegionPickerIndex { get; set; }
        public int FieldPickerIndex { get; set; }
        public int XPickerIndex { get; set; }

        private void InitializePickers()
        {
            try
            {
                string tmp = Properties.ContainsKey(regionPickerIndex) ? (string)Properties[regionPickerIndex] : "-1";
                RegionPickerIndex = Int32.Parse(tmp);
                tmp = Properties.ContainsKey(xPickerIndex) ? (string)Properties[xPickerIndex] : "-1";
                XPickerIndex = Int32.Parse(tmp);
                tmp = Properties.ContainsKey(fieldPickerIndex) ? (string)Properties[fieldPickerIndex] : "-1";
                FieldPickerIndex = Int32.Parse(tmp);
            }
            catch (Exception ex)
            {
                ex.ToString();
                FieldPickerIndex = -1;
                RegionPickerIndex = -1;
                XPickerIndex = -1;
            }
        }

        private void SavePickers()
        {
            Properties[regionPickerIndex] = RegionPickerIndex;
            Properties[xPickerIndex] = XPickerIndex;
            Properties[fieldPickerIndex] = FieldPickerIndex;
        }
        #endregion

        #region Internet Connection 
        public static bool HasInternetConnexion()
        {
            if (!CrossConnectivity.IsSupported)
                return true;
            return CrossConnectivity.Current.IsConnected;
        }

        public class ConnectivityChangedEventArgs : EventArgs
        {
            public bool IsConnected { get; set; }
        }

        public delegate void ConnectivityChangedEventHandler(object sender, ConnectivityChangedEventArgs e);
        #endregion

        public App()
        {
            InitializeComponent();
            Api.GetCategories();
            MainPage = new LoadingPage();
            InitializePickers();
            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            {
                MessagingCenter.Send<App>(this, "ConnectivityChanged");
            };
        }


        protected override void OnResume()
        {
            MessagingCenter.Send<App>(this, "ConnectivityChanged");
            base.OnResume();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            SavePickers();
            base.OnSleep();
        }
    }
}
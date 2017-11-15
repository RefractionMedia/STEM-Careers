using STEM_Careers.Converters;
using STEM_Careers.Models;
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
    public partial class DegreeDetailsPage : ContentPage
    {
        DegreeDetailsViewModel vm;

        public DegreeDetailsPage()
        {
            InitializeComponent();
        }

        public DegreeDetailsPage(DegreeDetailsViewModel vim)
        {
            InitializeComponent();
            BindingContext = this.vm = vim;
            if(vm.Degree.LinkToWebsite == "")
            {
                linkButton.Text = "No link, google search ?";
            }
        }

        protected async override void OnAppearing()
        {
            await vm.Initialize();
            BindingContext = vm;
            base.OnAppearing();
        }

        private void OpenWebLink(object sender, EventArgs e)
        {
            var item = sender as Button;
            if(item.Text == "No link, google search ?")
            {
                string query = vm.Degree.Name + "+"+  vm.Degree.University;
                string searchQuery = new SearchQueryCreator().Create(query);

                query = "https://www.google.com.au/search?q=" + searchQuery;
                //format example of query q=Psychology+australian+national+university&oq=Psychology+australian+national+university
                Device.OpenUri(new Uri(query));
                return;
            }
            Device.OpenUri(new Uri(vm.Degree.LinkToWebsite));
        }
    }
}
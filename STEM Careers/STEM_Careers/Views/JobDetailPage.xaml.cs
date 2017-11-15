using STEM_Careers.Converters;
using STEM_Careers.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class JobDetailPage : ContentPage
	{
        JobsViewModel vm;
		public JobDetailPage (JobsViewModel vm)
		{
			InitializeComponent ();
            this.vm = vm;
            BindingContext = vm;

        }
        private void OpenWebLink(object sender, EventArgs e)
        {
            var item = sender as Button;
            if (item.Text == jobsInfo.Text)
            {
                string query = vm.selectedJob.Name + "  information";
                string searchQuery = new SearchQueryCreator().Create(query);

                query = "https://www.google.com.au/search?q=" + searchQuery;
                //format example of query q=Psychology+australian+national+university&oq=Psychology+australian+national+university
                Device.OpenUri(new Uri(query));
                return;
            }
            else if(item.Text == jobsSearch.Text)
            {
                string query = vm.selectedJob.Name + "  in australia";
                string searchQuery = new SearchQueryCreator().Create(query);

                query = "https://www.google.com.au/search?q=" + searchQuery;
                //format example of query q=Psychology+australian+national+university&oq=Psychology+australian+national+university
                Device.OpenUri(new Uri(query));
                return;
            }
        }
    }
}
using STEM_Careers.Converters;
using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using System;
using System.Threading.Tasks;
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

        public JobDetailPage(Job job)
        {
            InitializeComponent();
            vm = new JobsViewModel();
            vm.selectedJob = job;
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

        private async Task StarTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            var bindinContext = image.BindingContext;

            var jobVM = bindinContext as JobsViewModel;
            var job = jobVM.selectedJob as Job;
            if (job.IsFavorite == false)
            {
                job.IsFavorite = true;
                image.Source = "gold_star_full";
            }
            else
            {
                job.IsFavorite = false;
                image.Source = "gold_star_empty";
            }
            await App.Database.UpdateJobAsync(job);
        }
    }
}
using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobsPage : ContentPage
    {
        JobsViewModel vm;

        public JobsPage(string field = "", string X = "")
        {
            InitializeComponent();

            BindingContext = vm = new JobsViewModel(field, X);
        }

       
        protected async override void OnAppearing()
        {
            if (vm == null)
                vm = new JobsViewModel();
            await vm.Initialize();
            BindingContext = vm;
            base.OnAppearing();
        }

        private async Task JobListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as Job;
            if (item == null)
                return;
            if (sender is ListView listView)
                listView.SelectedItem = null;
            vm.selectedJob = item;
            await Navigation.PushAsync(new JobDetailPage(this.vm));
            return;
        }
    }
}
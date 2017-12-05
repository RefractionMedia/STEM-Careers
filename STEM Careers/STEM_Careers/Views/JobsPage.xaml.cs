using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobsPage : ContentPage
    {
        JobsViewModel vm;
        STEMPickerViewModel PickerVM;

        public JobsPage()
        {
            InitializeComponent();
            BindingContext = vm = new JobsViewModel();
            Title = "Jobs";
            PickerVM = new STEMPickerViewModel();
        }
        public JobsPage(string field = "", string X = "")
        {
            InitializeComponent();
            BindingContext = vm = new JobsViewModel(field, X);
            Title = "Jobs";
            PickerVM = new STEMPickerViewModel();
        }

        protected async override void OnAppearing()
        {
            if (vm == null)
                vm = new JobsViewModel();
            await vm.Initialize();
            InitializeComponent();
            BindingContext = vm;
            ToolbarItem stemPickers = new ToolbarItem
            {
                Text = "Filter",
                Order = ToolbarItemOrder.Primary,
                Command = new Command(async () =>
                {
                    await Navigation.PushModalAsync(new JobPickPage(PickerVM), true);
                })
            };
            if (ToolbarItems.Count > 0)
            {
                ToolbarItems.RemoveAt(0);
            }
            ToolbarItems.Add(stemPickers);
            base.OnAppearing();
        }

        private void Goback_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        private async Task OnItemSelected(object sender, EventArgs e)
        {
            var vc = sender as ViewCell;
            var parent = vc.Parent;
            while (!(parent is ListView))
            {
                parent = parent.Parent;
            }
            var listView = parent as ListView;
            var job = listView.SelectedItem as Job;
            listView.SelectedItem = null;
            if (job == null)
                return;
            vm.selectedJob = job;
            await Navigation.PushAsync(new JobDetailPage(this.vm));
            return;
        }

        private async Task StarTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            var bindinContext = image.BindingContext;

            var job = bindinContext as Job;
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
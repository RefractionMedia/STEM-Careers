using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System;
using System.Windows.Input;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DegreePage : ContentPage
    {
        DegreePageViewModel vm;

        public DegreePage(string field = "", string X = "", string state = "")
        {
            InitializeComponent();
            BindingContext = vm = new DegreePageViewModel(field, X, state);
        }

        private void Goback_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected async override void OnAppearing()
        {
            if (vm == null)
                vm = new DegreePageViewModel();
            BindingContext = vm;
            InitializeComponent();
            await vm.Initialize();
            base.OnAppearing();
        }


        async Task OnItemSelected(object sender, EventArgs args)
        {
            var vc = sender as ViewCell;
            var parent = vc.Parent;
            while (!(parent is ListView))
            {
                parent = parent.Parent;
            }
            var listView = parent as ListView;
            var degree = listView.SelectedItem as Degree;
            listView.SelectedItem = null;
            if (degree == null)
                return;
            await Navigation.PushAsync(new DegreeDetailsPage(new DegreeDetailsViewModel(degree)));
            return;
        }

        private async Task StarTapped(object sender, EventArgs args)
        {
            var image = sender as Image;
            var bindinContext = image.BindingContext;

            var degree = bindinContext as Degree;
            if (degree.IsFavorite == false)
            {
                degree.IsFavorite = true;
                image.Source = "gold_star_full";
            }
            else
            {
                degree.IsFavorite = false;
                image.Source = "gold_star_empty";
            }
            await App.Database.UpdateDegreeAsync(degree);
        }
    }
}

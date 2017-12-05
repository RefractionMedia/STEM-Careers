using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DegreePage : ContentPage
    {
        DegreePageViewModel vm;
        DegreeSearchViewModel PickerVM;

        //Default ctor for 
        public DegreePage()
        {
            InitializeComponent();
            BindingContext = vm = new DegreePageViewModel();
            PickerVM = new DegreeSearchViewModel();
        }

        public DegreePage(string field = "", string X = "", string state = "")
        {
            InitializeComponent();
            BindingContext = vm = new DegreePageViewModel(field, X, state);
            PickerVM = new DegreeSearchViewModel();
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
            ToolbarItem stemPickers = new ToolbarItem
            {
                Text = "Filter",
                Order = ToolbarItemOrder.Primary,
                Command = new Command(async () =>
                {
                     await Navigation.PushModalAsync(new DegreeSearchPage(PickerVM), true);
                })
            };
            if (ToolbarItems.Count > 0)
            {
                ToolbarItems.RemoveAt(0);
            }
            ToolbarItems.Add(stemPickers);
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

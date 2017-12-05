using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using STEM_Careers.Views.Controls;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeoplePage : ContentPage
    {
        STEMPickerViewModel PickerVM;
        PeoplePageViewModel vm;
        public PeoplePage()
        {
            InitializeComponent();
            if (vm == null)
            {
                vm = new PeoplePageViewModel();
            }
            BindingContext = vm;
            vm.Initialize();
            Title = "People";
            PickerVM = new STEMPickerViewModel();
        }
        public PeoplePage(string field = "", string X = "")
        {
            InitializeComponent();
            if (vm == null)
            {
                vm = new PeoplePageViewModel(field, X);
            }
            BindingContext = vm;
            vm.Initialize();
            Title = "People";
            PickerVM = new STEMPickerViewModel();
        }
        protected override void OnAppearing()
        {
            ToolbarItem stemPickers = new ToolbarItem
            {
                Text = "Filter",
                Order = ToolbarItemOrder.Primary,
                Command = new Command(async () =>
                {
                    await Navigation.PushModalAsync(new PeoplePickPage(PickerVM), true);
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

        private async Task PeopleListView_Refreshing(object sender, EventArgs e)
        {
            if (!vm.IsBusy)
            {
                vm.Update();
            }
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
            var person = listView.SelectedItem as People;
            listView.SelectedItem = null;
            if (person == null)
                return;
            await Navigation.PushAsync(new PeopleDetailPage(person));
            return;
        }

        private async Task StarTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            var bindinContext = image.BindingContext;

            var person = bindinContext as People;
            if (person.IsFavorite == false)
            {
                person.IsFavorite = true;
                image.Source = "gold_star_full";
            }
            else
            {
                person.IsFavorite = false;
                image.Source = "gold_star_empty";
            }
            await App.Database.UpdatePersonAsync(person);
        }
    }
}



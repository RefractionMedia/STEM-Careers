using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeoplePage : ContentPage
    {
        PeoplePageViewModel vm;

        public PeoplePage(string field = "", string X = "")
        {
            InitializeComponent();
            if (vm == null)
            {
                vm = new PeoplePageViewModel(field, X);
            }
            BindingContext = vm;
            vm.Initialize();

            string title = "People: ";
            title = field == "" ? "Any" : field;
            title += "\n+\n";
            title += X == "" ? "Any" : X;
            Title = title;
        }

        private async Task PeopleListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender is ListView)
            {
                ListView listView = sender as ListView;
                People person = listView.SelectedItem as People;
                listView.SelectedItem = null;
                //if (person != null)
                //    await Navigation.PushAsync(new WebViewPage(person.Href));
                //for later on if time allows it
                await Navigation.PushAsync(new PeopleDetailPage(person));
            }
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
    }
}



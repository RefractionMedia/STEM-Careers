using STEM_Careers.Data;
using STEM_Careers.Helpers;
using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritePage : ContentPage
    {

        public ObservableRangeCollection<Grouping<string, object>> Favorites = new ObservableRangeCollection<Grouping<string, object>>();
        //public ObservableRangeCollection<object> Favorites = new ObservableRangeCollection<object>();
        public FavoritePage()
        {
            InitializeComponent();
            BindingContext = Favorites;
        }

        private async Task InitFaves()
        {
            var items = await App.Database.GetFavorites();
            var sorted =
                    from d in items
                    orderby d.GetType().Name
                    group d by d.GetType().Name
                    into grouped
                    select new Grouping<string, object>(grouped.Key, grouped);
            Favorites.ReplaceRange(sorted);
            if(Favorites.Count == 0)
            {
                NoFavesLabel.IsEnabled = true;
                NoFavesLabel.IsVisible = true;
            }
            else
            {
                NoFavesLabel.IsEnabled = false;
                NoFavesLabel.IsVisible = false;
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
            switch (listView.SelectedItem.GetType().Name)
            {
                case "Degree":
                    await Navigation.PushAsync(new DegreeDetailsPage(new DegreeDetailsViewModel(listView.SelectedItem as Degree)));
                    break;
                case "People":
                    await Navigation.PushAsync(new PeopleDetailPage(listView.SelectedItem as People));
                    break;
                case "Job":
                    await Navigation.PushAsync(new JobDetailPage(listView.SelectedItem as Job));
                    break;
                default:
                    break;
            }
            listView.SelectedItem = null;
        }

        private void StarTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            var bindinContext = image.BindingContext;
            switch (bindinContext.GetType().Name)
            {
                case "Degree":
                    var degree = bindinContext as Degree;
                    degree.IsFavorite = false;
                    App.Database.UpdateDegreeAsync(degree);
                    foreach (var item in Favorites)
                    {
                        if (item.Remove(degree as object))
                            return;
                    }
                    break;
                case "People":
                    var person = bindinContext as People;
                    person.IsFavorite = false;
                    App.Database.UpdatePersonAsync(person);
                    foreach (var item in Favorites)
                    {
                        if (item.Remove(person as object))
                            return;
                    }
                    break;
                case "Job":
                    var job = bindinContext as Job;
                    job.IsFavorite = false;
                    App.Database.UpdateJobAsync(job);
                    foreach (var item in Favorites)
                    {
                        if (item.Remove(job as object))
                            return;
                    }
                    break;
                default:
                    break;
            }
        }
        protected override void OnAppearing()
        {
            InitFaves();
            base.OnAppearing();
        }
    }
}
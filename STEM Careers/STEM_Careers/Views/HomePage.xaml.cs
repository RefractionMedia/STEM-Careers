using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {

        public HomePage()
        {
            Title = "Home";
            InitializeComponent();
        }

        private void DegreePicker(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new DegreeSearchPage());
        }

        private void CareerInspo(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new JobPickPage());
        }

        private void Featured(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new FeaturedStoriesPickerPage());
        }

        private void People(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new PeoplePickPage());
        }

        private void Favorites(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new FavoritePage());
        }

        private void AboutUs(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AboutPage());
        }
    }
}
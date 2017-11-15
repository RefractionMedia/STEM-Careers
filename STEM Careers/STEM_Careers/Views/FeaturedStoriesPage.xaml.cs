using STEM_Careers.Models;
using STEM_Careers.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeaturedStoriesPage : ContentPage
    {
        FeaturedStoriesViewModel vm;
        public FeaturedStoriesPage(string X = "", string Field = "")
        {
            InitializeComponent();
            if (vm == null)
                vm = new FeaturedStoriesViewModel(X, Field);

            BindingContext = vm;
            vm.Initalize();
        }

        private void FeaturedListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as Story;
            if (item == null)
                return;
            if (sender is ListView listView)
                listView.SelectedItem = null;
            Navigation.PushAsync(new WebViewPage(item.Link));
            return;
        }
    }
}
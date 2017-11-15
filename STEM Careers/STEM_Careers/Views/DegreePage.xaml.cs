using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

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

        async Task OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Degree;
            if (item == null)
                return;
            if (sender is ListView listView)
                listView.SelectedItem = null;
            await Navigation.PushAsync(new DegreeDetailsPage(new DegreeDetailsViewModel(item)));
            return;
        }

        protected async override void OnAppearing()
        {
            if (vm == null)
                vm = new DegreePageViewModel();
            await vm.Initialize();
            BindingContext = vm;
            base.OnAppearing();
        }
    }
}

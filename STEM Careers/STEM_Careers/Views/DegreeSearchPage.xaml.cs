using STEM_Careers.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DegreeSearchPage : ContentPage
    {
        private DegreeSearchViewModel vm;

        public DegreeSearchPage()
        {
            InitializeComponent();
            if (vm == null)
                vm = new DegreeSearchViewModel();
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            vm.Initialize();
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            BindingContext = vm;
            

            // Leaving the selected pickers as they were previously
            App app = Application.Current as App;
            regionPicker.SelectedIndex = app.RegionPickerIndex;
            XPicker.SelectedIndex = app.XPickerIndex;
            subjectPicker.SelectedIndex = app.FieldPickerIndex;
        }

        private void FindPath(object sender, System.EventArgs e)
        {
            //A tedious triple if/else, lovely. Also if it's -1, it means not selected
            string field = subjectPicker.SelectedIndex == -1 ? "Any" : subjectPicker.SelectedItem.ToString();
            string X = XPicker.SelectedIndex == -1 ? "Any" : XPicker.SelectedItem.ToString();
            string state = regionPicker.SelectedIndex == -1 ? "Any" : regionPicker.SelectedItem.ToString();
            Navigation.PushAsync(new DegreePage(field, X, state));
        }

        private void IndexChanged(object sender, System.EventArgs e)
        {
            App app = Application.Current as App;
            if(sender is Picker)
            {
                Picker picker = sender as Picker;
                if(picker.SelectedIndex != -1)
                {
                    app.RegionPickerIndex = regionPicker.SelectedIndex;
                    app.XPickerIndex = XPicker.SelectedIndex;
                    app.FieldPickerIndex = subjectPicker.SelectedIndex;
                }
            }
        }

        //protected async override void OnAppearing()
        //{
        //    if (vm == null)
        //        vm = new DegreeSearchViewModel();
        //    BindingContext = vm;
        //    base.OnAppearing();
        //}
    }
}
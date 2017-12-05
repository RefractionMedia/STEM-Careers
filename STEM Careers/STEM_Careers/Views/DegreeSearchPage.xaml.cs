using STEM_Careers.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DegreeSearchPage : ContentPage
    {
        private DegreeSearchViewModel vm;

        public DegreeSearchPage(DegreeSearchViewModel vm)
        {
            InitializeComponent();
            if (vm == null)
                this.vm = new DegreeSearchViewModel();
            else
                this.vm = vm;
            BindingContext = vm;
            vm.Initialize();
            Task.Delay(800);
            regionPicker.SelectedIndex = vm.RegionPickerIndex;
            XPicker.SelectedIndex = vm.XPickerIndex;
            subjectPicker.SelectedIndex = vm.FieldPickerIndex;
        }

        private async Task FindPath(object sender, System.EventArgs e)
        {
            //Get data ready for messagingcenter
            string field = subjectPicker.SelectedIndex == -1 ? "Any" : subjectPicker.SelectedItem.ToString();
            string X = XPicker.SelectedIndex == -1 ? "Any" : XPicker.SelectedItem.ToString();
            string state = regionPicker.SelectedIndex == -1 ? "Any" : regionPicker.SelectedItem.ToString();
            string concat = string.Concat(field, ",", X, ",", state);

            //save picker positions
            vm.RegionPickerIndex = regionPicker.SelectedIndex;
            vm.XPickerIndex = XPicker.SelectedIndex;
            vm.FieldPickerIndex = subjectPicker.SelectedIndex;

            await Navigation.PopModalAsync();
            MessagingCenter.Send<DegreeSearchPage, string>(this, "STEMPickers", concat);
        }

    }
}
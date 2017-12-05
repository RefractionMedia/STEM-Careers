using STEM_Careers.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class JobPickPage : ContentPage
	{
        private STEMPickerViewModel vm;
        public JobPickPage (STEMPickerViewModel vm)
		{
            InitializeComponent();
            if (vm == null)
                this.vm = new STEMPickerViewModel();
            else
                this.vm = vm;
            vm.Initialize();
            BindingContext = vm;

            // Leaving the selected pickers as they were previously
            XPicker.SelectedIndex = vm.XPickerIndex;
            subjectPicker.SelectedIndex = vm.FieldPickerIndex;
        }

        private async Task FindPath(object sender, System.EventArgs e)
        {
            string field = subjectPicker.SelectedIndex == -1 ? "Any" : subjectPicker.SelectedItem.ToString();
            string X = XPicker.SelectedIndex == -1 ? "Any" : XPicker.SelectedItem.ToString();

            string concat = string.Concat(field, ",", X);

            //save picker positions
            vm.XPickerIndex = XPicker.SelectedIndex;
            vm.FieldPickerIndex = subjectPicker.SelectedIndex;

            await Navigation.PopModalAsync();
            MessagingCenter.Send<JobPickPage, string>(this, "STEMPickers", concat);
        }
    }
}
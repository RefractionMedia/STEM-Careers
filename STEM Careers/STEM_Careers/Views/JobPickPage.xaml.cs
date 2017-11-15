﻿using STEM_Careers.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class JobPickPage : ContentPage
	{
        private STEMPickerViewModel vm;
        public JobPickPage ()
		{
            InitializeComponent();
            if (vm == null)
                vm = new STEMPickerViewModel();
            vm.Initialize();
            BindingContext = vm;

            // Leaving the selected pickers as they were previously
            App app = Application.Current as App;
            XPicker.SelectedIndex = app.XPickerIndex;
            subjectPicker.SelectedIndex = app.FieldPickerIndex;
        }

    


        private void FindPath(object sender, System.EventArgs e)
        {
            //A tedious double if/else, lovely. Also if it's -1, it means not selected
            string field = subjectPicker.SelectedIndex == -1 ? "Any" : subjectPicker.SelectedItem.ToString();
            string X = XPicker.SelectedIndex == -1 ? "Any" : XPicker.SelectedItem.ToString();

            Navigation.PushAsync(new JobsPage(field == "Any" ? "" : field, X == "Any" ? "" : X));
        }

        private void IndexChanged(object sender, System.EventArgs e)
        {
            App app = Application.Current as App;
            if (sender is Picker)
            {
                Picker picker = sender as Picker;
                if (picker.SelectedIndex != -1)
                {
                    app.XPickerIndex = XPicker.SelectedIndex;
                    app.FieldPickerIndex = subjectPicker.SelectedIndex;
                }
            }
        }
    }
}
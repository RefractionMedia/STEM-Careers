using STEM_Careers.Models;
using STEM_Careers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeopleDetailPage : ContentPage
    {
        PeopleDetailViewModel vm;

        public PeopleDetailPage(People person)
        {
            InitializeComponent();
            if (person == null)
                Navigation.PopAsync();

            if (vm == null)
                vm = new PeopleDetailViewModel();
            BindingContext = vm;
            vm.Initialize(person);
        }
    }
}
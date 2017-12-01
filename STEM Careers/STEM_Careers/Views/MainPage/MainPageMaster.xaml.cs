using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace STEM_Careers.Views.MainPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageMaster : ContentPage
    {
        public ListView ListView;

        public MainPageMaster()
        {
            InitializeComponent();

            BindingContext = new MainPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MainPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainPageMenuItem> MenuItems { get; set; }

            public MainPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainPageMenuItem>(new[]
                {
                    new MainPageMenuItem { Id = 0, Title = "Home" , TargetType=typeof(HomePage)},
                    new MainPageMenuItem { Id = 1, Title = "Degrees" , TargetType=typeof(DegreeSearchPage)},
                    new MainPageMenuItem { Id = 2, Title = "People" , TargetType=typeof(PeoplePickPage)},
                    new MainPageMenuItem { Id = 3, Title = "Jobs" , TargetType=typeof(JobPickPage)},
                    new MainPageMenuItem { Id = 4, Title = "Favorites", TargetType=typeof(FavoritePage) },
                    new MainPageMenuItem { Id = 5, Title = "About", TargetType=typeof(AboutPage) },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
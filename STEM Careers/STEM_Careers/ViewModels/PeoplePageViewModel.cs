using STEM_Careers.Helpers;
using STEM_Careers.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace STEM_Careers.ViewModels
{
    class PeoplePageViewModel : BaseViewModel
    {

        private bool noResults = false;
        public bool NoResults
        {
            get { return noResults; }
            set { SetProperty(ref noResults, value); }
        }


        public ObservableRangeCollection<People> Peeps { get; set; }

        public Command LoadItemsCommand { get; set; }

        private string field;
        private string X;

        private PeopleHelper Helper;

        public PeoplePageViewModel(string field = "", string X = "")
        {
            this.field = field;
            this.X = X;
            Title = "People";
            NoResults = false;
            Peeps = new ObservableRangeCollection<People>();
            AddPersonSubscribe();
            Helper = new PeopleHelper();
        }

        private void AddPersonSubscribe()
        {
            MessagingCenter.Subscribe<PeopleHelper, People>(this, "AddPerson", (helper, person) =>
            {
                if (!Peeps.Contains(person))
                {
                    Peeps.Reverse();
                    Peeps.Add(person);
                    Peeps.Reverse();
                }
            });
        }

        public async Task Initialize()
        {
            IsBusy = true;
            PeopleHelper helper = new PeopleHelper();
            Peeps.AddRange(await helper.GetPeople(this.field, this.X).ContinueWith(t =>
            {
                IsBusy = false;
                return t.Result;
            }));
            IsBusy = false;
            if (Peeps.Count == 0)
                NoResults = true;
        }

        internal async Task Update()
        {
            IsBusy = true;
            if (App.HasInternetConnexion())
                await Helper.UpdatePeople(field, X);
            IsBusy = false;
            if (Peeps.Count == 0)
                NoResults = true;
        }
    }
}

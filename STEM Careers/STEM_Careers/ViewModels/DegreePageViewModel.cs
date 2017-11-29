using STEM_Careers.Models;
using System;
using System.Threading.Tasks;
using STEM_Careers.Helpers;
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using STEM_Careers.Data;
using System.Windows.Input;

namespace STEM_Careers.ViewModels
{
    class DegreePageViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Grouping<string, Degree>> Degrees { get; set; }
        public Command LoadItemsCommand { get; set; }

        private bool noResults = false;
        public bool NoResults
        {
            get { return noResults; }
            set { SetProperty(ref noResults, value); }
        }

        private string field;
        private string X;
        private string state;

        public DegreePageViewModel(string field = "", string X = "", string state = "")
        {
            this.field = field;
            this.state = state;
            this.X = X;
            string title = "Degrees: ";
            title += field == "" ? "Any" : field;
            title += " + ";
            title += X == "" ? "Any" : X;
            title += " + ";
            title += state == "" ? "Any" : state;
            Title = title;
            Degrees = new ObservableRangeCollection<Grouping<string, Degree>>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand(field, X, state));
        }
        
        public async Task Initialize()
        {
            if(Degrees.Count< 2)
                await ExecuteLoadItemsCommand(field, X, state);
        }

        async Task ExecuteLoadItemsCommand(string field = "", string X = "", string state = "")
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Degrees.Clear();
                var items = await App.Database.GetDegreesFieldXStateAsync(field, X, state);

                var sorted =
                    from d in items
                    orderby d.University
                    group d by d.University
                    into grouped
                    select new Grouping<string, Degree>(grouped.Key, grouped);

                if(items.Count == 0)
                {
                    NoResults = true;
                }

                Degrees.ReplaceRange(sorted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessagingCenter.Send(new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Unable to load items.",
                    Cancel = "OK"
                }, "message");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

using STEM_Careers.Models;
using System;
using System.Threading.Tasks;
using STEM_Careers.Helpers;
using Xamarin.Forms;
using System.Diagnostics;

namespace STEM_Careers.ViewModels
{
    class DegreePageViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Degree> Degrees { get; set; }
        public Command LoadItemsCommand { get; set; }

        private string field;
        private string X;
        private string state;

        public DegreePageViewModel(string field = "", string X = "", string state = "")
        {
            this.field = field;
            this.state = state;
            this.X = X;
            Title = "Results";
            Degrees = new ObservableRangeCollection<Degree>();
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
                
                if(items.Count == 0)
                {
                    items.Add(new Degree()
                    {
                        Name = "Sorry, no results",
                        University=""
                    });

                }
                Degrees.ReplaceRange(items);
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

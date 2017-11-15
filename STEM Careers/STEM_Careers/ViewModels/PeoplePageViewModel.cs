using STEM_Careers.Helpers;
using STEM_Careers.Models;
using System;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;

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

        public PeoplePageViewModel(string field = "", string X = "")
        {
            this.field = field;
            this.X = X;
            Title = "People";
            NoResults = false;
            Peeps = new ObservableRangeCollection<People>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }


        public async Task Initialize()
        {
            IsBusy = true;
            MessagingCenter.Subscribe<WebContentHelper>(this, "All pages retrieved", (sender) =>
            {
                IsBusy = false;
                if (Peeps.Count == 0)
                    NoResults = true;
            });
            MessagingCenter.Subscribe<WebContentHelper, People>(this, "AddPerson",  (helper, person) =>
            {
                if(!Peeps.Contains(person))
                    Peeps.Add(person);
            });
            WebContentHelper webContentHelper = new WebContentHelper();
            await webContentHelper.FetchPeopleArticles(this.field, this.X);

        }

        async Task ExecuteLoadItemsCommand(People person = null)
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                if (!Peeps.Contains(person))
                {
                    List<People> items = new List<People>();
                    foreach (var item in Peeps)
                    {
                        items.Add(item);
                    }
                    Peeps.ReplaceRange(items);
                }

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

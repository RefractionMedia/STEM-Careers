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
        }


        public async Task Initialize()
        {
            IsBusy = true;
            MessagingCenter.Subscribe<PeopleHelper>(this, "All pages retrieved", (sender) =>
            {
                IsBusy = false;
                if (Peeps.Count == 0)
                    NoResults = true;
            });
            //MessagingCenter.Subscribe<PeopleHelper, People>(this, "AddPerson", (helper, person) =>
            //{
            //    if (!Peeps.Contains(person))
            //        Peeps.Add(person);
            //});
            PeopleHelper helper = new PeopleHelper();
            Peeps.AddRange(await helper.GetPeople(this.field, this.X).ContinueWith(t=>
            {
                IsBusy = false;
                return t.Result;
            }));
            IsBusy = false;
        }
    }
}

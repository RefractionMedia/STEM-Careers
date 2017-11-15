using STEM_Careers.Helpers;
using STEM_Careers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STEM_Careers.ViewModels
{
    public class PeopleDetailViewModel : BaseViewModel
    {
        People person;
        public void Initialize(People person)
        {
            this.person = person;
            IsBusy = true;
            MessagingCenter.Subscribe<WebContentHelper, People>(this, "PersonDetails", (sender, p) =>
            {
                IsBusy = false;
                this.person.Description = p.Description;
            });
        }
    }
}

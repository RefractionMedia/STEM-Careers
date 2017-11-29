using STEM_Careers.Helpers;
using STEM_Careers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STEM_Careers.ViewModels
{
    public class DegreeDetailsViewModel : BaseViewModel
    {
        public Command LoadItemsCommand { get; set; }
        public Degree Degree { get; set; }
        private University university;

        public string ImagePath
        {
            get
            {
                if (Degree.IsFavorite)
                    return "gold_star_full.png";
                else
                    return "gold_star_empty.png";
            }
            set { }
        }
        public University University
        {
            get { return university; }
            set { SetProperty(ref university, value); }
        }

        public DegreeDetailsViewModel(Degree degree = null)
        {
            Title = degree.Name;
            Degree = degree;

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand(Degree.Name));
        }

        public async Task Initialize()
        {
            await ExecuteLoadItemsCommand(Degree.University);
        }

        async Task ExecuteLoadItemsCommand(string name = "")
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                University = await App.Database.GetUniversityWithName(name);

                if (University == null)
                {
                    University = new University
                    {
                        ID = -21,
                        Name = "",
                        Rank = "ND",
                        Description = "ND"
                    };

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

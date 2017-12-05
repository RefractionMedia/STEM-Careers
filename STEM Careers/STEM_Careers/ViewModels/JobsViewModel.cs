using STEM_Careers.Models;
using System;
using System.Threading.Tasks;
using STEM_Careers.Helpers;
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using STEM_Careers.Views;

namespace STEM_Careers.ViewModels
{
    public class JobsViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Job> Jobs { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Job selectedJob { get; set; }

        public string ImagePath { get { return selectedJob.IsFavorite ? "gold_star_full.png" : "gold_star_empty.png"; } }
        private bool noResults = false;
        public bool NoResults
        {
            get { return noResults; }
            set { SetProperty(ref noResults, value); }
        }
        private string field;
        private string X;

        public JobsViewModel(string field = "", string X = "")
        {
            this.field = field;
            this.X = X;
            Title = "Jobs";
            Jobs = new ObservableRangeCollection<Job>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand(field, X));
            MessagingCenter.Subscribe<JobPickPage, string>(this, "STEMPickers", async (obj, concat) =>
            {
                string[] args = concat.Split(',');
                await ExecuteLoadItemsCommand(args[0], args[1]);
            });
        }


        public async Task Initialize()
        {
            if (Jobs.Count < 2)
                await ExecuteLoadItemsCommand(field, X);
        }

        async Task ExecuteLoadItemsCommand(string field = "", string X = "", string orderBy = "Name")
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Jobs.Clear();
                var items = await App.Database.GetJobsAsync(field, X);

                if (items.Count == 0)
                {
                    NoResults = true;
                }
                //TODO: Implement sorting and grouping
                IEnumerable<Job> jobEnum = items.DistinctBy(x => x.Name);
                items = jobEnum.ToList();
                switch (orderBy)
                {
                    case "Salary":
                        items = items.OrderBy(job => job.MedianSalary, comparer: StringComparer.OrdinalIgnoreCase).ToList();
                        break;
                    case "Name":
                        items = items.OrderBy(job => job.Name, comparer: StringComparer.OrdinalIgnoreCase).ToList();

                        break;
                    default:
                        items = items.OrderBy(job => job.Name, comparer: StringComparer.OrdinalIgnoreCase).ToList();
                        break;
                }
                Jobs.ReplaceRange(items);
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

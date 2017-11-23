using STEM_Careers.Models;
using System;
using System.Threading.Tasks;
using STEM_Careers.Helpers;
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace STEM_Careers.ViewModels
{
    public class JobsViewModel : BaseViewModel
    {

        public ObservableRangeCollection<Job> Jobs { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Job selectedJob { get; set; }

        private string field;
        private string X;

        public JobsViewModel(string field = "", string X = "")
        {
            this.field = field;
            this.X = X;
            Title = "Jobs";
            Jobs = new ObservableRangeCollection<Job>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand(field, X));
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
                    items.Add(new Job()
                    {
                        Name = "Sorry, no results",
                        MedianSalary = ""
                    });
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

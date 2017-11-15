using STEM_Careers.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STEM_Careers.ViewModels
{
    class DegreeSearchViewModel : BaseViewModel
    {
        public Command LoadItemsCommand { get; set; }

        private List<string> fields = new List<string>();
        public List<string> Fields
        {
            get { return fields; }
            set { SetProperty(ref fields, value); }
        }

        private List<string> yourX = new List<string>();
        public List<string> YourX
        {
            get { return yourX; }
            set { SetProperty(ref yourX, value); }
        }

        private List<string> states = new List<string>();
        public List<string> States
        {
            get { return states; }
            set { SetProperty(ref states, value); }
        }

        public DegreeSearchViewModel()
        {
            Title = "Find your path";
            Fields = new List<string>();
            YourX = new List<string>();
            States = new List<string>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        public async Task Initialize()
        {
            await ExecuteLoadItemsCommand();
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Stack<List<string>> res = await App.Database.GetPickerDataAsync();
                Fields = res.Pop();
                Fields.Sort();
                Fields.Insert(0, "Any");
                YourX = res.Pop();
                YourX.Sort();
                YourX.Insert(0, "Any");
                States = res.Pop();
                States.Sort();
                States.Insert(0, "Any");
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


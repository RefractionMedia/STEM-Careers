﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using STEM_Careers.Helpers;
using System.Diagnostics;
using Xamarin.Forms;

namespace STEM_Careers.ViewModels
{
    public class STEMPickerViewModel : BaseViewModel
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

        public int XPickerIndex { get; internal set; }
        public int FieldPickerIndex { get; internal set; }
        public bool Initialized { get; private set; }

        public STEMPickerViewModel()
        {
            Title = "Find your path";
            Fields = new List<string>();
            YourX = new List<string>();
            XPickerIndex = -1;
            FieldPickerIndex = -1;
            Initialized = false;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        public async Task Initialize()
        {
            if (Initialized)
                return;
            else
                await ExecuteLoadItemsCommand();
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Stack<List<string>> res = await App.Database.GetSTEMPickerDataAsync();
                Fields = res.Pop();
                Fields.Sort();
                Fields.Insert(0, "Any");
                YourX = res.Pop();
                YourX.Sort();
                YourX.Insert(0, "Any");

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
                Initialized = true;
                IsBusy = false;
            }
        }
    }
}

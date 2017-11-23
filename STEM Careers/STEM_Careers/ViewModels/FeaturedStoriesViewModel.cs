using STEM_Careers.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Diagnostics;
using STEM_Careers.Helpers;

namespace STEM_Careers.ViewModels
{
    public class FeaturedStoriesViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Story> Stories { get; set; }

        private string X;
        private string Field;

        public FeaturedStoriesViewModel(string X = "", string Field = "")
        {
            this.X = X;
            this.Field = Field;
            Stories = new ObservableRangeCollection<Story>();
        }

        internal async Task Initalize()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                IEnumerable<Category> Xcats = from cat in App.Api.Categories where cat.Name.Equals(X, StringComparison.CurrentCultureIgnoreCase) select cat;
                var list = Xcats.ToList();
                Category Xcat = list.FirstOrDefault();
                List<Story> Xstories = await App.Api.GetStories(Xcat);
                IEnumerable<Category> FieldCats = from cat2 in App.Api.Categories where cat2.Name.Equals(Field, StringComparison.CurrentCultureIgnoreCase) select cat2;
                list = FieldCats.ToList();
                Category FieldCat = list.FirstOrDefault();
                List<Story> FieldStories = await App.Api.GetStories(FieldCat);
                var test = FieldStories.Intersect(Xstories, StoryEqualityComparer.Instance);
                Stories.ReplaceRange(test);
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

using STEM_Careers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STEM_Careers.Views.Selectors
{
    public class JobDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultNoFavTemplate { get; set; }
        public DataTemplate DefaultFavTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((Job)item).IsFavorite ? DefaultFavTemplate : DefaultNoFavTemplate;
        }
    }
}

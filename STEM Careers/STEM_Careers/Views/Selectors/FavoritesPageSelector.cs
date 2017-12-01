using STEM_Careers.Models;
using System;
using Xamarin.Forms;

namespace STEM_Careers.Views.Selectors
{
    public class FavoritesPageSelector : DataTemplateSelector
    {
        public DataTemplate DegreeTemplate { get; set; }
        public DataTemplate PeopleTemplate { get; set; }

        public DataTemplate JobTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item.GetType() == typeof(Degree))
                return DegreeTemplate;
            if (item.GetType() == typeof(Job))
                return JobTemplate;
            if (item.GetType() == typeof(People))
                return PeopleTemplate;
            else throw new ArgumentException("Has to be a degree, people or job"); 
        }
    }
}

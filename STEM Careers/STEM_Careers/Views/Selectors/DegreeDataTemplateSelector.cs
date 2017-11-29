using STEM_Careers.Models;
using Xamarin.Forms;

namespace STEM_Careers.Views.Selectors
{
    public class DegreeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DegreeRegularTemplate { get; set; }
        public DataTemplate FavoriteTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((Degree)item).IsFavorite ? FavoriteTemplate : DegreeRegularTemplate;
        }
    }
}

using System.Collections.Generic;

namespace STEM_Careers.MobileAppService.Models
{
    public class People
    {
        public string ArticleID { get; set; }
        public string Href { get; set; }
        public string ImgReference { get; set; }
        public List<string> ProfileCategories { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //Set in GetPeople
        public string Quote { get; set; }
        public string Content { get; set; }
        public List<string> Path { get; set; }
    }
}

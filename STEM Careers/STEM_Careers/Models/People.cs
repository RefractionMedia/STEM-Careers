using SQLite;
using System.Collections.Generic;

namespace STEM_Careers.Models
{
    public class People
    {
        [PrimaryKey]
        public string ArticleID { get; set; }
        public string Href { get; set; }
        public string ImgReference { get; set; }
        public List<string> ProfileCategories { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

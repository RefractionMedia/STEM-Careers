using SQLite;
using System.Collections.Generic;

namespace STEM_Careers.Models
{
    public class People
    {
        [PrimaryKey]
        public int ArticleID { get; set; }
        public string Href { get; set; }
        public string ImgReference { get; set; }
        public string ProfileCategories { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; internal set; }
    }
}

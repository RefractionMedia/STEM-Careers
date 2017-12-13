using SQLite;
using System;
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
        public string Content { get; set; }

        public bool IsFavorite { get; set; } = false;
    }

    class PeopleComparer : Comparer<People>
    {
        private static readonly PeopleComparer _instance = new PeopleComparer();
        public static PeopleComparer Instance { get { return _instance; } }
        public override int Compare(People x, People y)
        {
            return x.ArticleID.CompareTo(y.ArticleID);
        }
    }
}

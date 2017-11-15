using SQLite;
using System;
using System.Collections.Generic;

namespace STEM_Careers.Models
{
    public class Story : IEquatable<Story>
    {
        [PrimaryKey]
        public int Id { get; set; }
        public Title Title { get; set; }
        public string Link { get; set; }
        public List<int> Categories { get; set; }
        public Content Content { get; set; }
        public Excerpt Excerpt { get; set; }
        
        //not JSON queryed
        public string Image { get; set; }
        #region IEquatable implementation

        public bool Equals(Story other)
        {
            if (other.Id == Id)
            {
                return true;
            }
            else return false;
        }

        public int GetHashCode(object obj)
        {
            return Id.GetHashCode() ^ Link.GetHashCode();
        }

        #endregion
    }

    class StoryEqualityComparer : EqualityComparer<Story>
    {
        private static readonly StoryEqualityComparer _instance = new StoryEqualityComparer();
        public static StoryEqualityComparer Instance { get { return _instance; } }

        private StoryEqualityComparer() { }

        public override bool Equals(Story x, Story y)
        {
            return x.Id == y.Id;
        }

        public override int GetHashCode(Story obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    #region Classes used for JSON compatibility: Title, Content and Excerpt

    public class Title
    {
        public string Rendered { get; set; }
    }

    public class Content
    {
        public string Rendered { get; set; }
    }
    public class Excerpt
    {
        public string Rendered { get; set; }
    }
    #endregion
}

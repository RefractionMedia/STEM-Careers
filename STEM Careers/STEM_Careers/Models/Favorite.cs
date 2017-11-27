using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STEM_Careers.Models
{
    public enum FavGroup { People, Job, University }
    public class Favorite
    {
        public int Id { get; set; }
        public FavGroup Group {get; set;}
    }
}

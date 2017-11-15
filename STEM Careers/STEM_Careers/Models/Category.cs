using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STEM_Careers.Models
{
    public struct Category
    {
        public int ID { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public List<string> Slugs { get; set; }

        public override string ToString()
        {
            return string.Format("ID={0} \nCount={1} \nName={2} \nLink={3}", ID, Count, Name, Link);
        }
    }
}

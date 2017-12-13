using SQLite;

namespace STEM_Careers.Models
{
    public class Degree
    {
        [PrimaryKey]
        public int ID { get; set; }

        public string Name { get; set; }
        public string State { get;set; }
        public string Field { get; set; }
        public string YourX { get;set; }
        public string University { get; set; }
        public string LinkToWebsite { get; set; }

        public bool IsFavorite { get; set; } = false;
    }
}

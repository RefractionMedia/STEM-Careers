using SQLite;

namespace STEM_Careers.Models
{
    public class University
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Rank { get; set; }
    }
}

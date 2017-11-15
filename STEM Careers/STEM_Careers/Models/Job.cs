using SQLite;
using System;

namespace STEM_Careers.Models
{
    public class Job : IComparable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
        public string MedianSalary { get; set; }
        public string YourX { get; set; }
        public string Field { get; set; }
        public string Description { get; set; }

        public int CompareTo(object obj)
        {
            Job job = obj as Job;
            if (job == null)
            {
                throw new ArgumentException("Object is not a Job");
            }
            return Name.CompareTo(job.Name);
        }
    }
}

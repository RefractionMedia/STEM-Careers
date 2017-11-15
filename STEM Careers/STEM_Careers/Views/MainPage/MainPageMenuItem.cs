using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STEM_Careers.Views.MainPage
{

    public class MainPageMenuItem
    {
        public MainPageMenuItem()
        {
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}
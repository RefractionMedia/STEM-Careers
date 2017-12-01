using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STEM_Careers.Converters
{
    public class SearchQueryCreator
    {
        public string Create(string text)
        {
            string query = "";
            char[] textArray = text.ToCharArray();
            foreach (char c in textArray)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    query += '+';
                }
                else
                {
                    query += c;
                }
            }
            return query;
        }
    }
}

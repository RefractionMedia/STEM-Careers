using System;

namespace STEM_Careers.Converters
{
    class ClearTextFormater
    {
        public ClearTextFormater()
        {
        }

        public string MakeClearer(string text)
        {
            if(text.Equals("queensland-university-of-technology", StringComparison.CurrentCultureIgnoreCase)){
                return "QUT (Queensland Uni of Technology)";
            }
            if (text.Contains("unsw"))
            {
                return "UNSW Australia";
            }
            char[] textArray = text.ToCharArray();
            string clearerText = "";
            for(int i= 0; i<textArray.Length; i++)
            {
                
                if (i == 0 && char.IsLetterOrDigit(textArray[i]))    //First letter --> uppercase
                    clearerText += char.ToUpper(textArray[i]);
                else if (i == 0 && !char.IsLetterOrDigit(textArray[i]))    //First is a symbol --> ignore
                    continue;
                else if (!char.IsLetterOrDigit(textArray[i]))  //not a letter or number --> space
                {
                    clearerText += ' ';
                    if (i!=textArray.Length && !char.IsLetterOrDigit(textArray[i + 1]))   //deletes double spaces or symbols from final result
                        i++;
                }
                else if (char.IsLetterOrDigit(textArray[i]) && !char.IsLetterOrDigit(textArray[i - 1]))
                    clearerText += char.ToUpper(textArray[i]);
                else
                    clearerText += textArray[i];
            }

            return clearerText;
        }
    }
}

using STEM_Careers.Droid;
using STEM_Careers.Data;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace STEM_Careers.Droid
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}
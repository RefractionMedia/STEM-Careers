using STEM_Careers;
using STEM_Careers.Data;
using System.IO;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace STEM_Careers
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
        }
    }
}
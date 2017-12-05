namespace STEM_Careers.Data
{
    /// <summary>
    /// Interface for accessing the filesystem
    /// </summary>
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}

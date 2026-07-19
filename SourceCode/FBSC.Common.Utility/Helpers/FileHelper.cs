namespace FBSC.Common.Utility.Helpers
{
    public static class FileHelper
    {
        public static void CreateFolderIfNotExists(string folderPath)
        {
            bool folderPathExists = Directory.Exists(folderPath);
            if (!folderPathExists)
                Directory.CreateDirectory(folderPath);
        }
    }
}

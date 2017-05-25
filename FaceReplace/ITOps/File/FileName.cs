using System.IO;

namespace FaceReplace.ITOps.File
{
    public class FileName
    {
        public FileName(string fileNameNoDirectory) : this(null, fileNameNoDirectory) { }

        public FileName(string directory, string fileNameNoDirectory)
        {
            if (directory != null)
            {
                Directory = RemoveTrailingSlash(directory);
            }
            Name = CleanseFileName(fileNameNoDirectory);
        }

        protected static string RemoveTrailingSlash(string directoryPath)
        {
            if (directoryPath.EndsWith("/"))
            {
                directoryPath = directoryPath.Remove(directoryPath.Length - 1);
            }
            return directoryPath;
        }

        protected static string CleanseFileName(string fileName)
        {
            return fileName.Replace("\"", "");
        }

        public bool HasDirectory => Directory != null;

        public string Directory { get; }

        public string Name { get; }

        public string Path
        {
            get
            {
                if(HasDirectory)
                    return Directory + "/" + Name;
                return Name;
            }
        }

        public string Extension => new FileInfo(Name).Extension;
    }
}

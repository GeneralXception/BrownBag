using System.IO;

namespace FaceReplace.ITOps.File
{
    public class UseProvidedNameStrategy : IFileNameStrategy
    {
        public FileName CalcululateFileName(Stream fileStream, FileName sourceFileName)
        {
            return sourceFileName;
        }
    }
}

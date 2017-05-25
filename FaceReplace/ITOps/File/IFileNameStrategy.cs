using System.IO;

namespace FaceReplace.ITOps.File
{
    public interface IFileNameStrategy
    {
        FileName CalcululateFileName(Stream fileStream, FileName sourceFileName);
    }
}
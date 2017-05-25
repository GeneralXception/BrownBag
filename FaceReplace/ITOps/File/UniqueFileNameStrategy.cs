using System;
using System.IO;

namespace FaceReplace.ITOps.File
{
    public class UniqueFileNameStrategy : IFileNameStrategy
    {
        public FileName CalcululateFileName(Stream fileStream, FileName sourceFileName)
        {
            return new FileName(sourceFileName.Directory, Guid.NewGuid() + sourceFileName.Name);
        }
    }
}

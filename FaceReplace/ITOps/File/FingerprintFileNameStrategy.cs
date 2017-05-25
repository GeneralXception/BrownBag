using System;
using System.IO;
using System.Security.Cryptography;

namespace FaceReplace.ITOps.File
{
    public class FingerprintFileNameStrategy : IFileNameStrategy
    {
        public FileName CalcululateFileName(Stream fileStream, FileName sourceFileName)
        {
            byte[] hashBytes;
            using (var sha = new SHA256Managed())
            {
                sha.Initialize();
                hashBytes = sha.ComputeHash(fileStream);
                fileStream.Position = 0;
            }

            var fileName = BitConverter.ToString(hashBytes).Replace("-", "") + sourceFileName.Extension;

            return new FileName(sourceFileName.Directory, fileName);
        }
    }
}

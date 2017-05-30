using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FaceReplace.ITOps.File;

namespace FaceReplace.ITOps
{
    public interface IBlobFileClient
    {
        Task<bool> FileExists(FileName fileName);
        Task Delete(FileName fileName);
        Task<UploadResult> Upload(Stream file, FileName fileName);
        Task<MoveResult> Move(FileName sourceFileName, string destinationDirectory);
        Task<string> RandomDownload(Stream outputStream);
        Task<string> DownloadAtIndexPosition(int indexPosition, Stream outputStream);
        Task Download(FileName fileName, Stream outputStream);
    }
}
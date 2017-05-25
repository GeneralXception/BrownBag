using System.Collections.Generic;
using System.Threading.Tasks;
using FaceReplace.Cognitive;

namespace FaceReplace.ForegroundImage
{
    public interface IProcessedImageDataStorageClient
    {
        Task StoreFaceData(string fileName, List<FaceResult> face);
    }
}
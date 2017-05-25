using System.Collections.Generic;
using System.Threading.Tasks;
using FaceReplace.Cognitive;

namespace FaceReplace.BackgroundImage
{
    public interface IBackgroundImageFaceDataStorageClient
    {
        Task StoreFaceData(string fileName, List<FaceResult> face);
        Task<List<FaceResult>> RetrieveFaceData(string backgroundImageFileName);
    }
}
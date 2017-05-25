using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FaceReplace.Cognitive
{
    public interface IFaceDetectApiClient
    {
        Task<List<FaceResult>> DetectFaces(Stream imageStream);
    }
}
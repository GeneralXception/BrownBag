using System;
using System.IO;
using System.Threading.Tasks;
using FaceReplace.Cognitive;
using FaceReplace.ITOps;
using FaceReplace.ITOps.File;

namespace FaceReplace.BackgroundImage
{
    public class BackgroundImageProcessor
    {
        private readonly IBlobFileClient storageBlobFileClient;
        private readonly IFaceDetectApiClient faceApiApiClient;
        private readonly IBackgroundImageFaceDataStorageClient faceDataBackgroundImageFaceDataStorageClient;

        public BackgroundImageProcessor(IBlobFileClient storageBlobFileClient, IFaceDetectApiClient faceApiApiClient, IBackgroundImageFaceDataStorageClient faceDataBackgroundImageFaceDataStorageClient)
        {
            if (storageBlobFileClient == null)
            {
                throw new ArgumentNullException(nameof(storageBlobFileClient));
            }

            if (faceApiApiClient == null)
            {
                throw new ArgumentNullException(nameof(faceApiApiClient));
            }

            if (faceDataBackgroundImageFaceDataStorageClient == null)
            {
                throw new ArgumentNullException(nameof(faceDataBackgroundImageFaceDataStorageClient));
            }

            this.storageBlobFileClient = storageBlobFileClient;
            this.faceApiApiClient = faceApiApiClient;
            this.faceDataBackgroundImageFaceDataStorageClient = faceDataBackgroundImageFaceDataStorageClient;
        }

        public async Task<ProcessResult> ProcessImage(Stream fileStream, string sourceDirectory, string fileName)
        {
            var result = await faceApiApiClient.DetectFaces(fileStream);

            var filePath = new FileName(sourceDirectory, fileName);
            if (result.Count == 0)
            {
                await storageBlobFileClient.Delete(filePath);
                return ProcessResult.Failure("Image did not contain any faces.");
            }
            await faceDataBackgroundImageFaceDataStorageClient.StoreFaceData(fileName, result);
            await storageBlobFileClient.Move(filePath, Shared.BackgroundImageBlobDirectoryName);
            return ProcessResult.Success();
        }

    }
}
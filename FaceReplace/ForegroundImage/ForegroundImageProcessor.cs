using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using FaceReplace.BackgroundImage;
using FaceReplace.Cognitive;
using FaceReplace.ITOps;
using FaceReplace.ITOps.File;
using ImageProcessor;
using ImageProcessor.Imaging;

namespace FaceReplace.ForegroundImage
{
    public class ForegroundImageProcessor
    {
        private readonly IBlobFileClient foregroundBlobFileClient;
        private readonly IFaceDetectApiClient faceDetectApiClient;
        private readonly IBlobFileClient backgroundBlobFileClient;
        private readonly IBackgroundImageFaceDataStorageClient backgroundImageFaceDataStorageClient;

        public ForegroundImageProcessor(IBlobFileClient foregroundBlobFileClient, IFaceDetectApiClient faceDetectApiClient, IBlobFileClient backgroundBlobFileClient, IBackgroundImageFaceDataStorageClient backgroundImageFaceDataStorageClient)
        {
            if (foregroundBlobFileClient == null)
            {
                throw new ArgumentNullException(nameof(foregroundBlobFileClient));
            }

            if (faceDetectApiClient == null)
            {
                throw new ArgumentNullException(nameof(faceDetectApiClient));
            }

            if (backgroundBlobFileClient == null)
            {
                throw new ArgumentNullException(nameof(backgroundBlobFileClient));
            }
            if (backgroundImageFaceDataStorageClient == null)
            {
                throw new ArgumentNullException(nameof(backgroundImageFaceDataStorageClient));
            }

            this.foregroundBlobFileClient = foregroundBlobFileClient;
            this.faceDetectApiClient = faceDetectApiClient;
            this.backgroundBlobFileClient = backgroundBlobFileClient;
            this.backgroundImageFaceDataStorageClient = backgroundImageFaceDataStorageClient;
        }

        public async Task<ProcessResult> ProcessImage(Stream blobImage, FileName fileName)
        {
            var foregroundImage = new MemoryStream();
            await blobImage.CopyToAsync(foregroundImage);
            foregroundImage.Position = 0;

            var foregroundImageBytes = foregroundImage.ToArray();
            var foregroundFaceData = await faceDetectApiClient.DetectFaces(foregroundImage);
            
            if (foregroundFaceData.Count == 0)
            {
                await foregroundBlobFileClient.Delete(fileName);
                return ProcessResult.Failure("Image did not contain any faces.");
            }

            var backgroundImage = new MemoryStream();
            var backgroundImageFileName = await backgroundBlobFileClient.RandomDownload(backgroundImage);
            backgroundImage.Position = 0;

            var outStream = new MemoryStream();
            try
            {
                var backgroundFaceData = await backgroundImageFaceDataStorageClient.RetrieveFaceData(backgroundImageFileName);
                foregroundImage = new MemoryStream(foregroundImageBytes);
                ReplaceTheFace(foregroundImage, RandomFaceResult(foregroundFaceData), backgroundImage, RandomFaceResult(backgroundFaceData), outStream);

                await foregroundBlobFileClient.Upload(outStream, new FileName(Shared.ProcessedImageBlobDirectoryName, fileName.Name));
                await foregroundBlobFileClient.Delete(fileName);
            }
            catch(Exception exception)
            {
                return ProcessResult.Failure(exception.Message);
            }
            finally
            {
                outStream.Dispose();
            }

            return ProcessResult.Success();
        }

        private FaceResult RandomFaceResult(List<FaceResult> faceData)
        {
            var index = new Random().Next(faceData.Count);
            return faceData[index];
        }

        private void ReplaceTheFace(Stream foregroundImage, FaceResult foregroundFaceData, Stream backgroundImageStream, FaceResult backgroundFaceData, Stream outStream)
        {
            var foregroundImageFaceOnly = new MemoryStream();
            using (var imageFactory = new ImageFactory())
            {
                imageFactory.Load(foregroundImage)
                    .AutoRotate()
                    .Crop(new Rectangle(foregroundFaceData.faceRectangle.left, foregroundFaceData.faceRectangle.top, foregroundFaceData.faceRectangle.width, foregroundFaceData.faceRectangle.height))
                    .Resize(new Size(backgroundFaceData.faceRectangle.width, backgroundFaceData.faceRectangle.height))
                    .Save(foregroundImageFaceOnly);
            }
            foregroundImageFaceOnly.Position = 0;

            using (var imageFactory = new ImageFactory())
            {
                var imageLayer = new ImageLayer { Image = Image.FromStream(foregroundImageFaceOnly), Position = new Point(backgroundFaceData.faceRectangle.left, backgroundFaceData.faceRectangle.top) };
                var backgroundImage = Image.FromStream(backgroundImageStream);

                imageFactory.Load(backgroundImage)
                    .Overlay(imageLayer)
                    .Watermark(new TextLayer()
                    {
                        Text = "Produced by FaceReplace on " + DateTime.UtcNow.ToShortDateString(),
                        Position = new Point(10, backgroundImage.Height - 60)
                    })
                    .Save(outStream);
            }
        }
    }
}
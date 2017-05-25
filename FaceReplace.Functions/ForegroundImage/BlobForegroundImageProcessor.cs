using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using FaceReplace.BackgroundImage;
using FaceReplace.Cognitive;
using FaceReplace.ForegroundImage;
using FaceReplace.ITOps;
using FaceReplace.ITOps.File;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace FaceReplace.Functions.ForegroundImage
{
    public static class BlobForegroundImageProcessor
    {
        [FunctionName("BlobForegroundImageProcessor")]        
        public static async Task Run([BlobTrigger(Shared.BlobContainerName + "/" + Shared.ForegroundUnprocessedImageBlobDirectoryName + "/{name}", Connection = "StorageConnectionString")]Stream myBlob, string name, TraceWriter log)
        {
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var cloudTableClient = storageAccount.CreateCloudTableClient();

            var foregroundBlobFileClient = new AzureBlobStorageClient(cloudBlobClient, Shared.ForegroundUnprocessedImageBlobDirectoryName, new UseProvidedNameStrategy());
            var backgroundBlobFileClient = new AzureBlobStorageClient(cloudBlobClient, Shared.BackgroundImageBlobDirectoryName, new FingerprintFileNameStrategy());

            var processor = 
                new ForegroundImageProcessor(
                    foregroundBlobFileClient, 
                    new FaceDetectApiClient(ConfigurationManager.AppSettings["FaceDetectApiSubscriptionKey"]),
                    backgroundBlobFileClient, 
                    new BackgroundImageFaceDataStorageClient(cloudTableClient));

            var result = await processor.ProcessImage(myBlob, new FileName(Shared.ForegroundUnprocessedImageBlobDirectoryName, name));

            log.Info(result.IsSuccessful
                ? $"Successfuly processed the image file {name}"
                : $"Failed to process the image file {name}, {result.Message}");
        }
    }
}
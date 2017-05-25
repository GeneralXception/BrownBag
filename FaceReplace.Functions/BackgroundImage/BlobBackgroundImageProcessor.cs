using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using FaceReplace.BackgroundImage;
using FaceReplace.Cognitive;
using FaceReplace.ITOps;
using FaceReplace.ITOps.File;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace FaceReplace.Functions.BackgroundImage
{
    public static class BlobBackgroundImageProcessor
    {
        [FunctionName("BlobBackgroundImageProcessor")]        
        public static async Task Run([BlobTrigger(Shared.BlobContainerName + "/" + Shared.BackgroundUnprocessedImageBlobDirectoryName + "/{name}", Connection = "StorageConnectionString")]Stream myBlob, string name, TraceWriter log)
        {
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();

            var storageFileClient = new AzureBlobStorageClient(cloudBlobClient, Shared.BackgroundUnprocessedImageBlobDirectoryName, new FingerprintFileNameStrategy());
            var processor
                = new BackgroundImageProcessor(
                    storageFileClient,
                    new FaceDetectApiClient(ConfigurationManager.AppSettings["FaceDetectApiSubscriptionKey"]),
                    new BackgroundImageFaceDataStorageClient(storageAccount.CreateCloudTableClient()));
            var result = await processor.ProcessImage(myBlob, Shared.BackgroundUnprocessedImageBlobDirectoryName, name);

            if (result.IsSuccessful)
            {
                log.Info($"Successfuly processed the image file {name}");
            }
            else
            {
                log.Error($"Failed to process the image file {name}, {result.Message}");
            }
        }
    }
}
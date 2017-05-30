using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FaceReplace.ITOps;
using FaceReplace.ITOps.File;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace FaceReplace.Functions.ForegroundImage
{
    public class HttpForegroundImageUpload
    {
        [FunctionName("HttpForegroundImageUpload")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Request to upload image received");

            if (!req.Content.IsMimeMultipartContent())
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Where's the file?");
            }

            try
            {
                var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var cloudBlobClient = storageAccount.CreateCloudBlobClient();

                var storageFileClient = new AzureBlobStorageClient(cloudBlobClient, Shared.ForegroundUnprocessedImageBlobDirectoryName, new UniqueFileNameStrategy());
                var uploader = new Uploader(storageFileClient);

                var filesUploaded = await PerformUpload(req, uploader);

                var httpResponseMessage = req.CreateResponse(HttpStatusCode.OK, filesUploaded);
                httpResponseMessage.Headers.Add("Access-Control-Allow-Origin", "*");
                return httpResponseMessage;
            }
            catch (Exception exception)
            {
                log.Error("Error whilst trying to upload a foreground image", exception);
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }

        private static async Task<UploadResult> PerformUpload(HttpRequestMessage req, Uploader uploader)
        {
            var multiContent = await req.Content.ReadAsMultipartAsync();
            var fileContentStream = multiContent.Contents.FirstOrDefault(content => content.Headers.ContentType != null);

            if (fileContentStream == null)
            {
                return UploadResult.Failure("No files found.");
            }

            var memoryStream = await CopyToMemoryStream(fileContentStream);
            var filePath = new FileName(GetFileNameOnly(fileContentStream.Headers.ContentDisposition.FileName));
            var uploadResult = await uploader.Upload(memoryStream, filePath);

            return uploadResult;
        }

        private static string GetFileNameOnly(string path)
        {
            if (path.Contains('\\'))
            {
                return path.Split('\\').Last();
            }

            if (path.Contains('/'))
            {
                return path.Split('/').Last();
            }

            return path;
        }

        private static async Task<MemoryStream> CopyToMemoryStream(HttpContent content)
        {
            var memoryStream = new MemoryStream();
            await content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

    }
}
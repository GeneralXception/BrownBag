using System;
using System.Collections.Generic;
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

namespace FaceReplace.Functions.BackgroundImage
{
    public class HttpBackgroundImageUpload
    {
        [FunctionName("HttpBackgroundImageUpload")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            if (!req.Content.IsMimeMultipartContent())
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Where's the file?");
            }

            try
            {
                var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var cloudBlobClient = storageAccount.CreateCloudBlobClient();

                var storageFileClient = new AzureBlobStorageClient(cloudBlobClient, Shared.BackgroundUnprocessedImageBlobDirectoryName, new FingerprintFileNameStrategy());
                var uploader = new Uploader(storageFileClient);

                var filesUploaded = await PerformUpload(req, uploader);

                return req.CreateResponse(HttpStatusCode.OK, filesUploaded);
            }
            catch (Exception exception)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }

        private static async Task<List<UploadResult>> PerformUpload(HttpRequestMessage req, Uploader uploader)
        {
            var multiContent = await req.Content.ReadAsMultipartAsync();
            var fileContentStreams = multiContent.Contents.Where(content => content.Headers.ContentType != null).ToList();

            var uploadResults = new List<UploadResult>();
            foreach (var content in fileContentStreams)
            {
                var memoryStream = await CopyToMemoryStream(content);
                var fileName = new FileInfo(content.Headers.ContentDisposition.FileName.Replace("\"", "")).Name;
                uploadResults.Add(await uploader.Upload(memoryStream, new FileName(fileName)));
            }
            return uploadResults;
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
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FaceReplace.ITOps;
using FaceReplace.ITOps.File;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace FaceReplace.Functions.BackgroundImage
{
    public class HttpBackgroundImageRetrieve
    {
        [FunctionName("HttpBackgroundImageRetrieve")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "HttpBackgroundImageRetrieve/{index}")]HttpRequestMessage req, int index, TraceWriter log)
        {
            try
            {
                var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var cloudBlobClient = storageAccount.CreateCloudBlobClient();

                var storageFileClient = new AzureBlobStorageClient(cloudBlobClient, Shared.BackgroundImageBlobDirectoryName, new UseProvidedNameStrategy());
                var stream = new MemoryStream();
                await storageFileClient.DownloadAtIndexPosition(index, stream);
                stream.Position = 0;
                var response = req.CreateResponse(HttpStatusCode.OK);

                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                return response;
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
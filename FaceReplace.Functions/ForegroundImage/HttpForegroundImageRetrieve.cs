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

namespace FaceReplace.Functions.ForegroundImage
{
    public class HttpForegroundImageRetrieve
    {
        [FunctionName("HttpForegroundImageRetrieve")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "HttpForegroundImageRetrieve/{fileName}")]HttpRequestMessage req, string fileName, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            try
            {
                var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var cloudBlobClient = storageAccount.CreateCloudBlobClient();

                var storageFileClient = new AzureBlobStorageClient(cloudBlobClient, Shared.ProcessedImageBlobDirectoryName, new UseProvidedNameStrategy());
                var filePath = new FileName(fileName);
                var exists = await storageFileClient.FileExists(filePath);

                if (!exists)
                {
                    var httpResponseMessage = req.CreateResponse(HttpStatusCode.NotFound);
                    httpResponseMessage.Headers.Add("Access-Control-Allow-Origin", "*");
                    return httpResponseMessage;
                }

                var stream = new MemoryStream();
                await storageFileClient.Download(filePath, stream);
                stream.Position = 0;
                var response = req.CreateResponse(HttpStatusCode.OK);

                response.Content = new StreamContent(stream);

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                {
                    FileName = fileName
                };

                response.Headers.Add("Access-Control-Allow-Origin", "*");

                return response;
            }
            catch (Exception exception)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }
    }
}
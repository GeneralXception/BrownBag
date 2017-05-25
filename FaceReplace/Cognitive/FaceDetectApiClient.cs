using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FaceReplace.Cognitive
{
    public class FaceDetectApiClient : IFaceDetectApiClient
    {
        private readonly string subscriptionKey;

        public FaceDetectApiClient(string subscriptionKey)
        {
            if (string.IsNullOrEmpty(subscriptionKey))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(subscriptionKey));
            }
            this.subscriptionKey = subscriptionKey;
        }

        public async Task<List<FaceResult>> DetectFaces(Stream imageStream)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            const string queryString = "returnFaceId=false&returnFaceLandmarks=true&returnFaceAttributes=age,gender";
            const string uri = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;

            var content = new StreamContent(imageStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(uri, content);
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FaceResult>>(jsonString);
        }
    }
}
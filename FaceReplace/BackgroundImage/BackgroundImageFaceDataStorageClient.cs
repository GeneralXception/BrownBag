using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaceReplace.Cognitive;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace FaceReplace.BackgroundImage
{
    public class BackgroundImageFaceDataStorageClient : IBackgroundImageFaceDataStorageClient
    {
        private readonly CloudTableClient cloudTableClient;
        private readonly CloudTable table;

        public BackgroundImageFaceDataStorageClient(CloudTableClient cloudTableClient)
        {
            if (cloudTableClient == null)
            {
                throw new ArgumentNullException(nameof(cloudTableClient));
            }

            this.cloudTableClient = cloudTableClient;
            table = this.cloudTableClient.GetTableReference(Shared.BackgroundImageTableStoreTableName);
        }

        public async Task StoreFaceData(string fileName, List<FaceResult> face)
        {
            await table.CreateIfNotExistsAsync();
            var entity = new DynamicTableEntity("background", fileName);
            entity.Properties.Add("face", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(face)));
            await table.ExecuteAsync(TableOperation.Insert(entity));
        }

        public async Task<List<FaceResult>> RetrieveFaceData(string backgroundImageFileName)
        {
            await table.CreateIfNotExistsAsync();
            var result = await table.ExecuteAsync(TableOperation.Retrieve("background", backgroundImageFileName));
            var dynamicResult = (DynamicTableEntity) result.Result;
            return JsonConvert.DeserializeObject<List<FaceResult>>(dynamicResult.Properties["face"].StringValue);
        }
    }
}

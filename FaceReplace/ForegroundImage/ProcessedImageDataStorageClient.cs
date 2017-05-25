using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaceReplace.Cognitive;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace FaceReplace.ForegroundImage
{
    public class ProcessedImageDataStorageClient : IProcessedImageDataStorageClient
    {
        private readonly CloudTableClient cloudTableClient;
        private readonly CloudTable table;

        public ProcessedImageDataStorageClient(CloudTableClient cloudTableClient)
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

    }
}

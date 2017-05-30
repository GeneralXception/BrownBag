using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceReplace.ITOps.File;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FaceReplace.ITOps
{
    public class AzureBlobStorageClient : IBlobFileClient
    {
        private readonly IFileNameStrategy fileNameStrategy;
        private readonly CloudBlobDirectory directory;
        private readonly CloudBlobContainer container;

        public AzureBlobStorageClient(CloudBlobClient cloudBlobClient, string directoryName, IFileNameStrategy fileNameStrategy)
        {
            this.fileNameStrategy = fileNameStrategy;

            if (cloudBlobClient == null)
            {
                throw new ArgumentNullException(nameof(cloudBlobClient));
            }

            if (fileNameStrategy == null)
            {
                throw new ArgumentNullException(nameof(fileNameStrategy));
            }

            container = cloudBlobClient.GetContainerReference(Shared.BlobContainerName);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);
            directory = container.GetDirectoryReference(directoryName);
        }

        public async Task<bool> FileExists(FileName fileName)
        {
            var file = GetCloudFileReference(fileName);
            return await file.ExistsAsync();
        }

        public async Task Delete(FileName fileName)
        {
            var file = GetCloudFileReference(fileName);
            await file.DeleteAsync();
        }

        public async Task<UploadResult> Upload(Stream fileStream, FileName fileName)
        {
            var calculatedFileName = fileNameStrategy.CalcululateFileName(fileStream, fileName);
            var file = GetCloudFileReference(calculatedFileName);

            if (await file.ExistsAsync())
            {
                return UploadResult.Failure("File already exists.");
            }

            await file.UploadFromStreamAsync(fileStream);
            return UploadResult.Success(calculatedFileName);
        }

        public async Task<MoveResult> Move(FileName sourceFileName, string destinationDirectory)
        {
            var sourceFile = GetCloudFileReference(sourceFileName);
            var destinationFile = container.GetDirectoryReference(destinationDirectory).GetBlockBlobReference(sourceFileName.Name);

            await destinationFile.StartCopyAsync(sourceFile);
            await sourceFile.DeleteAsync();

            return MoveResult.Success();
        }

        public async Task<string> RandomDownload(Stream outputStream)
        {
            var blobs = directory.ListBlobs(useFlatBlobListing: true).Cast<CloudBlockBlob>().ToList();
            var blobIndex = new Random().Next(blobs.Count);

            var blob = blobs[blobIndex];
            await blob.DownloadToStreamAsync(outputStream);

            return blob.Name.Split('/').Last();
        }

        public async Task<string> DownloadAtIndexPosition(int indexPosition, Stream outputStream)
        {
            var blobs = directory.ListBlobs(useFlatBlobListing: true).Cast<CloudBlockBlob>().ToList();
            var blob = blobs[indexPosition];
            await blob.DownloadToStreamAsync(outputStream);

            return blob.Name.Split('/').Last();
        }

        public async Task Download(FileName fileName, Stream outputStream)
        {
            var blob = GetCloudFileReference(fileName);
            await blob.DownloadToStreamAsync(outputStream);
        }

        private CloudBlockBlob GetCloudFileReference(FileName fileName)
        {
            return fileName.HasDirectory
                ? container.GetBlockBlobReference(fileName.Path)
                : directory.GetBlockBlobReference(fileName.Name);
        }
    }
}
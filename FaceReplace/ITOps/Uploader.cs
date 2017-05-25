using System;
using System.IO;
using System.Threading.Tasks;
using FaceReplace.ITOps.File;

namespace FaceReplace.ITOps
{
    public class Uploader
    {
        private readonly IBlobFileClient client;

        public Uploader(IBlobFileClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            this.client = client;
        }

        public async Task<UploadResult> Upload(Stream file, FileName fileName)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (await client.FileExists(fileName))
            {
                return UploadResult.Failure("File already exists.");
            }

            return await client.Upload(file, fileName);
        }

    }
}
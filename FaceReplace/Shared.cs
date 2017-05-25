namespace FaceReplace
{
    public class Shared
    {
        public const string BlobContainerName = "facereplace";

        public const string BackgroundUnprocessedImageBlobDirectoryName = "background-unprocessed";
        public const string BackgroundProcessedWithErrorImageBlobDirectoryName = "background-error";
        public const string BackgroundImageBlobDirectoryName = "background";
        public const string BackgroundImageTableStoreTableName = "backgroundfacedata";

        public const string ForegroundUnprocessedImageBlobDirectoryName = "foreground-unprocessed";

        public const string ProcessedImageBlobDirectoryName = "processed";
    }
}
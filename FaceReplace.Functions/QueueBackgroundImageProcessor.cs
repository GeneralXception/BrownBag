using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FootFace.Functions
{
    public static class QueueBackgroundImageProcessor
    {
        [FunctionName("QueueBackgroundImageProcessor")]        
        static void Run([QueueTrigger("backgroundimage", Connection = "backgroundimage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
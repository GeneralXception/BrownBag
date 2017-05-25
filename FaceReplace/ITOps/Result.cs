using FaceReplace.ITOps.File;

namespace FaceReplace.ITOps
{
    public class Result
    {
        public bool IsSuccessful { get; protected set; }

        public string Message { get; protected set; }
    }

    public class MoveResult : Result
    {
        private MoveResult() { }

        public static MoveResult Success()
        {
            return new MoveResult { IsSuccessful = true };
        }

        public static MoveResult Failure(string reason)
        {
            return new MoveResult { IsSuccessful = false, Message = reason };
        }
    }

    public class ProcessResult : Result
    {
        private ProcessResult() { }

        public static ProcessResult Success()
        {
            return new ProcessResult { IsSuccessful = true };
        }

        public static ProcessResult Failure(string reason)
        {
            return new ProcessResult { IsSuccessful = false, Message = reason };
        }
    }

    public class UploadResult : Result
    {
        private UploadResult() { }

        public static UploadResult Success(FileName fileName)
        {
            return new UploadResult { IsSuccessful = true, FileName = fileName };
        }

        public static UploadResult Failure(string reason)
        {
            return new UploadResult { IsSuccessful = false, Message = reason };
        }

        public FileName FileName { get; set; }
    }

}

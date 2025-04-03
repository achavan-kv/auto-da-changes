namespace Unicomer.Cosacs.Model.Exceptions
{
    public class WorkflowError
    {
        public int Code { get; set; }
        public string Detail { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class WorkflowErrorResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public WorkflowError Errors { get; set; }
        public int ErrorCode { get; set; }
    }

    public class WorkflowErrorResponseModel
    {
        public WorkflowErrorResponse GeneralResponse { get; set; }
    }
}

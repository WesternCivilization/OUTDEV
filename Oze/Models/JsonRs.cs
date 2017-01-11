namespace Oze.Models
{
    public class JsonRs
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public static JsonRs create(int status,string message)
        {
            return new JsonRs(){Status=status.ToString(),Message=message};
        }
    }
    public class JsonData
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

    public class JsonRsSaleDto
    {
        public string Type { get; set; }
        public int Index { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class ResultService
    {
        public ResultService()
        {
        }

        public ResultService(bool status, string message)
        {
            Status = status;
            Message = message;
        }

        public ResultService(bool status, object data)
        {
            Status = status;
            Data = data;
        }

        public ResultService(bool status, string message, object data)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class DepositDescription
    {
        public int Index { get; set; }
        public string Content { get; set; }
    }
    
    public class HistoryModel
    {
        public int Index { get; set; }
        public string Content { get; set; }
    }

    
}

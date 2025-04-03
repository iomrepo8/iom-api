namespace IOM.Models
{
    public class TransactionResult
    {
        public object Data { get; set; }
        public object Error { get; set; }
        public bool IsSuccessful { get; set; }

        public TransactionResult(object error)
        {
            Error = error;
            IsSuccessful = false;
        }

        public TransactionResult()
        {
            IsSuccessful = true;
        }
    }
}
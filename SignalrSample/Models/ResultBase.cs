namespace SignalrSample.Models
{
    public class ResultBase
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int TotailRecords { get; set; }
        //public object Aggregates { get; set; }
    }
}
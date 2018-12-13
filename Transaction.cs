namespace Blockchain
{
    public class Transaction
    {
        public string FromAddress { get; set; }
        public string Data { get; set; }

        public Transaction(string from, string data)
        {
            FromAddress = from;
            Data = data;
        }
    }
}

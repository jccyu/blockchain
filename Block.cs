using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain
{
    public class Block
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public IList<Transaction> Transactions { get; set; }
        public int Nonce { get; set; }

        public Block(int index, DateTime timestamp, string previousHash, IList<Transaction> transactions)
        {
            Index = index;
            Timestamp = timestamp;
            PreviousHash = previousHash;
            Transactions = transactions;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(
                $"{Timestamp}-{PreviousHash ?? ""}-{JsonConvert.SerializeObject(Transactions)}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);
            return Convert.ToBase64String(outputBytes);
        }

        public void Mine(string lead)
        {
            while (Hash.Substring(0, lead.Length) != lead)
            {
                Nonce += 1;
                Hash = CalculateHash();
            }
        }
    }
}

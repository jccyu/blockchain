using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain
{
    public class BlockChain
    {  
        public IList<Block> Chain { set; get; }
        public IList<Transaction> PendingTransactions { get; set; }

        private string _lead = "EZ";

        public void InitializeChain()
        {
            PendingTransactions = new List<Transaction>();
            Chain = new List<Block>();  
            Block block = new Block(0, DateTime.UtcNow, null, PendingTransactions);
            block.Mine(_lead);
            Chain.Add(block);
            PendingTransactions = new List<Transaction>();
        }

        public Block GetLatestBlock() => Chain.Last();
  
        public Block AddBlock(DateTime timestamp)
        {  
            Block latestBlock = GetLatestBlock();
            Block block = new Block(latestBlock.Index + 1, timestamp, latestBlock.Hash, PendingTransactions);
            block.Mine(_lead);
            Chain.Add(block);
            PendingTransactions = new List<Transaction>();
            return block;
        }

        public void AddTransaction(Transaction transaction) => PendingTransactions.Add(transaction);

        public bool IsValid()
        {
            Block previousBlock = Chain.First();
            if (previousBlock.Hash.Substring(0, _lead.Length) != _lead)
                return false;
            if (previousBlock.Hash != previousBlock.CalculateHash())
                return false;

            foreach (var block in Chain.Skip(1))
            {
                if (block.Hash.Substring(0, _lead.Length) != _lead)
                    return false;
                if (block.Hash != block.CalculateHash())
                    return false;
                if (block.PreviousHash != previousBlock.Hash)
                    return false;
                previousBlock = block;
            }

            return true;  
        }

        public void PrintExtends(BlockChain other)
        {
            using (IEnumerator<Block> ce = Chain.GetEnumerator())
            using (IEnumerator<Block> oe = other.Chain.GetEnumerator())
            {
                while (oe.MoveNext())
                {
                    ce.MoveNext();
                    Block o = oe.Current;
                    Block c = ce.Current;
                    if (o.Hash != c.Hash)
                    {
                        Chat.PrintLog("Blockchain is updated with the longest chat log:");
                        foreach (var block in Chain)
                            foreach (var trans in block.Transactions)
                                Chat.PrintTransaction(trans);
                        return;
                    }
                }

                while (ce.MoveNext())
                    foreach (var trans in ce.Current.Transactions)
                        Chat.PrintTransaction(trans);
            }
        }
    }  
}

using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Blockchain
{
    class Program
    {
        public static string IP = "";
        public static int Port = 0;
        public static P2PNode Node = null;
        public static BlockChain Blockchain = new BlockChain();
        public static string Name = "Unknown";

        static void Main(string[] args)
        {
            Blockchain.InitializeChain();

            //if (args.Length >= 1)
            //    Port = int.Parse(args[0]);

            //if (args.Length >= 2)
            //    Name = args[1];

            Port = Int32.Parse(Console.ReadLine());
            Name = Console.ReadLine();

            if (Port > 0)
            {
                Node = new P2PNode();
                Node.Start();
            }

            Chat.PrintLog($"Current user is {Name} on port {Port}.");
            Chat.PrintLog("Use '!connect <IP>:<PORT>' to add a node to broadcasting list.");
            Chat.PrintLog("Use '!printchain' to print the chain.");
            Chat.PrintLog("Use '!quit' to quit.");
            Chat.PrintLog("--------------------------------------");

            while (true)
            {
                string action = Console.ReadLine();
                if (action == "!quit")
                {
                    Node.Close();
                    Environment.Exit(0);
                }
                else if (action == "!printchain")
                {
                    Chat.PrintLog("Blockchain:");
                    Chat.PrintChain();
                }
                else if (action.Length >= 10 && action.Substring(0, 9) == "!connect ")
                {
                    string[] conn = action.Substring(9).Split(':');
                    if (conn[0] == "localhost")
                        conn[0] = "127.0.0.1";
                    Task.Run(() => Node.InitiateHandshake(conn[0], Int32.Parse(conn[1])));
                }
                else
                {
                    Blockchain.AddTransaction(new Transaction(Name, action));
                    Blockchain.AddBlock(DateTime.UtcNow);
                    Node.Broadcast();
                }
            }
        }

        //public static string IP = "";
        //public static int Port = 0;
        //public static P2PServer Server = null;
        //public static P2PClient Client = new P2PClient();
        //public static BlockChain Blockchain = new BlockChain();
        //public static string Name = "Unknown";

        //static void Main(string[] args)
        //{
        //    Blockchain.InitializeChain();

        //    if (args.Length >= 1)
        //        Port = int.Parse(args[0]);

        //    if (args.Length >= 2)
        //        Name = args[1];

        //    if (Port > 0)
        //    {
        //        Server = new P2PServer();
        //        Server.Start();
        //    }

        //    Chat.PrintLog($"Current user is {Name} on port {Port}.");
        //    Chat.PrintLog("Use '!connect <IP>:<PORT>' to add a node to broadcasting list.");
        //    Chat.PrintLog("Use '!printchain' to print the chain.");
        //    Chat.PrintLog("Use '!quit' to quit.");
        //    Chat.PrintLog("--------------------------------------");

        //    while (true)
        //    {
        //        string action = Console.ReadLine();
        //        if (action == "!quit")
        //        {
        //            Client.Close();
        //            Environment.Exit(0);
        //        }
        //        else if (action == "!printchain")
        //        {
        //            Chat.PrintLog("Blockchain:");
        //            Chat.PrintChain();
        //        }
        //        else if (action.Length >= 10 && action.Substring(0, 9) == "!connect ")
        //        {
        //            string[] conn = action.Substring(9).Split(':');
        //            if (conn[0] == "localhost")
        //                conn[0] = "127.0.0.1";
        //            Task.Run(() => Client.Start(new Connection(conn[0], int.Parse(conn[1]))));
        //        }
        //        else
        //        {
        //            Blockchain.AddTransaction(new Transaction(Name, action));
        //            Blockchain.AddBlock(DateTime.UtcNow);
        //            Client.Broadcast(JsonConvert.SerializeObject(Blockchain));
        //        }
        //    }
        //}
    }
}

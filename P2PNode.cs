using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain
{

    class P2PNode
    {
        private readonly IDictionary<string, TcpClient> _wsDict = new Dictionary<string, TcpClient>();
        bool listening = false;

        private string ToString(EndPoint endpoint)
        {
            IPEndPoint ipendpoint = (IPEndPoint) endpoint;
            return ipendpoint.ToString();
        }

        public void InitiateHandshake(string address, int port)
        {
            if (!_wsDict.ContainsKey(address + port))
            {
                Chat.PrintLog("Connecting to node...");
                TcpClient client = new TcpClient(address, port);

                Chat.PrintLog("Sending handshake to node...");
                Chat.Send(client, "HISERVER");
                if (Chat.Recieve(client) != "HICLIENT")
                {
                    Chat.PrintLog("Unable to recieve handshake from node.");
                    return;
                }
                Chat.PrintLog("Recieved handshake from node!");

                Chat.Send(client, JsonConvert.SerializeObject(Program.Blockchain));
                string retval = Chat.Recieve(client);
                if (retval != "GOODCHAIN")
                {
                    BlockChain newChain = JsonConvert.DeserializeObject<BlockChain>(retval);
                    if (newChain.IsValid() && newChain.Chain.Count > Program.Blockchain.Chain.Count)
                    {
                        List<Transaction> newTransactions = new List<Transaction>();
                        newTransactions.AddRange(newChain.PendingTransactions);
                        newTransactions.AddRange(Program.Blockchain.PendingTransactions);

                        newChain.PendingTransactions = newTransactions;
                        Program.Blockchain = newChain;
                    }
                }

                _wsDict.Add(ToString(client.Client.RemoteEndPoint), client);
                Task.Run(() => Receive(client));
            }
        }

        public void FinishHandshake(TcpClient client)
        {
            string key = ToString(client.Client.RemoteEndPoint);
            if (!_wsDict.ContainsKey(key))
            {
                string recieved = Chat.Recieve(client);

                if (recieved == "HISERVER")
                {
                    Chat.PrintLog("Recieved handshake request!");
                    Chat.Send(client, "HICLIENT");
                }

                recieved = Chat.Recieve(client);
                BlockChain newChain = JsonConvert.DeserializeObject<BlockChain>(recieved);

                if (newChain.IsValid() && newChain.Chain.Count > Program.Blockchain.Chain.Count)
                {
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(newChain.PendingTransactions);
                    newTransactions.AddRange(Program.Blockchain.PendingTransactions);

                    newChain.PendingTransactions = newTransactions;
                    newChain.PrintExtends(Program.Blockchain);
                    Program.Blockchain = newChain;

                    Chat.Send(client, "GOODCHAIN");
                }
                else
                    Chat.Send(client, JsonConvert.SerializeObject(Program.Blockchain));

                _wsDict.Add(key, client);
                Task.Run(() => Receive(client));
            }
        }

        public void Receive(TcpClient client)
        {
            while (listening)
            {
                BlockChain newChain = JsonConvert.DeserializeObject<BlockChain>(Chat.Recieve(client));

                if (newChain.IsValid() && newChain.Chain.Count > Program.Blockchain.Chain.Count)
                {
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(newChain.PendingTransactions);
                    newTransactions.AddRange(Program.Blockchain.PendingTransactions);

                    newChain.PendingTransactions = newTransactions;
                    newChain.PrintExtends(Program.Blockchain);
                    Program.Blockchain = newChain;
                }
            }
        }

        public void Broadcast()
        {
            foreach (var item in _wsDict)
            {
                if (item.Value.Connected)
                    Chat.Send(item.Value, JsonConvert.SerializeObject(Program.Blockchain));
                else
                    Chat.PrintLog($"Unable to broadcast to {item.Key}");
            }

        }

        public void Close()
        {
            listening = false;
            foreach (var item in _wsDict)
                item.Value.Close();
        }

        public async void Start()
        {
            listening = true;

            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Program.Port);
            listener.Start();
            while (listening)
            {
                TcpClient newClient = await listener.AcceptTcpClientAsync();
                FinishHandshake(newClient);
            }
            listener.Stop();
        }
    }
}

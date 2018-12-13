using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Blockchain
{
    public static class Chat
    {
        public static void PrintTransaction(Transaction trans) =>
            Console.WriteLine($"{trans.FromAddress}: {trans.Data}");
        public static void PrintLog(string message) =>
            Console.WriteLine($"[LOG]: {message}");

        public static void PrintChain() =>
            Console.WriteLine(JsonConvert.SerializeObject(Program.Blockchain, Formatting.Indented));

        public static void Send(TcpClient client, string message)
        {
            if (!client.Connected)
                return;

            NetworkStream stream = client.GetStream();
            byte[] bytes;
            bytes = BitConverter.GetBytes(message.Length);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();

            bytes = Encoding.ASCII.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }

        public static string Recieve(TcpClient client)
        {
            if (!client.Connected)
                return null;

            Socket socket = client.Client;
            byte[] bytes;
            bytes = new byte[4];
            socket.Receive(bytes, 0, 4, 0);
            int length = BitConverter.ToInt32(bytes, 0);

            bytes = new byte[length];
            socket.Receive(bytes, 0, bytes.Length, 0);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}

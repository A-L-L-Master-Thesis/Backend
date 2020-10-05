using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using P9_Backend.Models;

namespace P9_Backend.Services
{
    public class SocketService : ISocketService
    {
        private Dictionary<string, TcpClient> _clientDict = new Dictionary<string, TcpClient>();

        public SocketService()
        {
            Start();
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 44310);
            listener.Start(10);

            while (true)
            {
                IAsyncResult res = listener.BeginAcceptTcpClient(HandleIncommingConnection, listener);
            }
        }

        private async void HandleIncommingConnection(IAsyncResult res)
        {
            TcpListener connectionListener = (TcpListener)res.AsyncState;
            TcpClient connectionClient = connectionListener.EndAcceptTcpClient(res);

            var data = new byte[connectionClient.ReceiveBufferSize];

            using (NetworkStream ns = connectionClient.GetStream())
            {
                await ns.ReadAsync(data, 0, data.Length);
            }

            string msg = Encoding.UTF8.GetString(data);

            try
            {
                Models.Message message = (Models.Message)JsonConvert.DeserializeObject(msg);
                if (!_clientDict.ContainsKey(message.sender))
                {
                    _clientDict.Add(message.sender, connectionClient);
                }

                Console.WriteLine(message.message);
            }
            catch
            {
                return;
            }
        }
    }
}

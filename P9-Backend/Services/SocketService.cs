using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using P9_Backend.Models;
using System.Threading;

namespace P9_Backend.Services
{
    public class SocketService : ISocketService
    {
        private TcpListener _listener;
        private Dictionary<string, TcpClient> _clientDict = new Dictionary<string, TcpClient>();
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private Task _listeningTask;
        private List<Task> _activeClientTasks = new List<Task>();

        public SocketService()
        {
            Start();
        }

        public void Start()
        {
            _listeningTask = Task.Run(() => StartListening());
        }

        public void Stop()
        {
            _listeningTask.Dispose();

            foreach (var pair in _clientDict)
            {
                pair.Value.Close();
            }

            _clientDict.Clear();

            foreach (Task task in _activeClientTasks)
            {
                task.Dispose();
            }

            _activeClientTasks.Clear();
        }

        public void StartListening()
        {
            _listener = new TcpListener(IPAddress.Any, 44444);
            System.Diagnostics.Debug.WriteLine("Listening on port: " + 44444);
            _listener.Start(10);

            while (true)
            {
                allDone.Reset();
                _listener.BeginAcceptTcpClient(HandleIncommingConnection, _listener);
                allDone.WaitOne();
            }
        }

        public void SendOne(string uuid, Message msg)
        {
            if (!_clientDict.ContainsKey(uuid))
            {
                return;
            }

            NetworkStream ns = _clientDict[uuid].GetStream();
            ns.Write(msg.toBytes());
        }

        public void SendAll(Message msg)
        {
            foreach (var pair in _clientDict)
            {
                NetworkStream ns = pair.Value.GetStream();
                ns.Write(msg.toBytes());
            }
        }

        private void HandleIncommingConnection(IAsyncResult res)
        {
            TcpListener connectionListener = (TcpListener)res.AsyncState;
            TcpClient connectionClient = connectionListener.EndAcceptTcpClient(res);
            connectionClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Message message = WaitForNextMessage(connectionClient).Result;

            if (message == null)
                return;

            if (!_clientDict.ContainsKey(message.sender))
            {
                _clientDict.Add(message.sender, connectionClient);
                System.Diagnostics.Debug.WriteLine("Just before task Connected: " + connectionClient.Client.Connected);
                _activeClientTasks.Add(Task.Run(() => ContinueListening(message.sender)));
            }

            //HandleMessage(message);
            System.Diagnostics.Debug.WriteLine("Handle Incoming msg: " + message.message);
            allDone.Set();
        }

        private async Task<Message> WaitForNextMessage(TcpClient connectionClient)
        {
            var data = new byte[connectionClient.ReceiveBufferSize];
            Message message;

            System.Diagnostics.Debug.WriteLine("Connected: " + connectionClient.Client.Connected);

            using NetworkStream ns = connectionClient.GetStream();
            await ns.ReadAsync(data, 0, data.Length);
            
            System.Diagnostics.Debug.WriteLine("After Connected: " + connectionClient.Client.Connected);

            string msg = Encoding.UTF8.GetString(data);
            System.Diagnostics.Debug.WriteLine("Raw msg: " + msg);

            try
            {
                message = JsonConvert.DeserializeObject<Message>(msg);
            }
            catch
            {
                message = null;
            }

            System.Diagnostics.Debug.WriteLine("Before Return Connected: " + connectionClient.Client.Connected);
            return message;
        }

        private void ContinueListening(string index)
        {
            TcpClient client = _clientDict[index];
            while (true)
            {
                if (!client.Connected)
                    break;

                Message message = WaitForNextMessage(client).Result;

                if (message == null)
                    continue;

                //HandleMessage(message);
                System.Diagnostics.Debug.WriteLine("Continue msg: " + message.message);
            }
        }

        private void HandleMessage(Message msg)
        {
            throw new NotImplementedException();
        }
    }
}

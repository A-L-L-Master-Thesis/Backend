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
using P9_Backend.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace P9_Backend.Services
{
    public class SocketService : ISocketService
    {
        private readonly int PORT = 44444; 
        private TcpListener _listener;
        private Dictionary<string, TcpClient> _clientDict = new Dictionary<string, TcpClient>();
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private Task _listeningTask;
        private List<Task> _activeClientTasks = new List<Task>();
        private readonly IDroneService _droneService;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private bool isDead;

        public SocketService(IDroneService droneService)
        {
            _droneService = droneService;
            Start();
        }

        public void Start()
        {
            _listeningTask = Task.Run(() => { StartListening(); }, _cts.Token);
        }

        public void Stop()
        {
            isDead = true;
            _cts.Cancel();
            _listener.Stop();
            //_listeningTask.Dispose();

            /*foreach (var pair in _clientDict)
            {
                //pair.Value.GetStream().Close();
                pair.Value.Close();
            }*/

            _clientDict.Clear();

            /*foreach (Task task in _activeClientTasks)
            {
                task.Dispose();
            }*/

            _activeClientTasks.Clear();
        }

        private void StartListening()
        {
            _listener = new TcpListener(IPAddress.Any, PORT);
            System.Diagnostics.Debug.WriteLine("Listening on port: " + PORT);
            _listener.Start(10);

            while (true)
            {
                if (_cts.Token.IsCancellationRequested)
                    break;

                allDone.Reset();
                _listener.BeginAcceptTcpClient(HandleIncommingConnection, _listener);
                allDone.WaitOne();
            }
        }

        public void SendOne(string uuid, string command, string data)
        {
            if (!_clientDict.ContainsKey(uuid))
            {
                return;
            }

            Commando cmd = new Commando { command = command, data = data };
            Message msgSend = new Message(uuid, cmd, "API");


            NetworkStream ns = _clientDict[uuid].GetStream();
            ns.Write(msgSend.toBytes());
        }

        public void SendAll(string command, string data)
        {
            Commando cmd = new Commando { command = command, data = data };

            foreach (var pair in _clientDict)
            {
                Message msgSend = new Message(pair.Key, cmd, "API");

                NetworkStream ns = pair.Value.GetStream();
                ns.Write(msgSend.toBytes());
            }
        }

        private void HandleIncommingConnection(IAsyncResult res)
        {
            TcpListener connectionListener = (TcpListener)res.AsyncState;
            if (isDead)
                return;
            TcpClient connectionClient = connectionListener.EndAcceptTcpClient(res);

            Message message = null;

            try
            {
                message = WaitForNextMessage(connectionClient).Result;
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debug.WriteLine("Socket closed unexpectedly..");
                connectionClient.Close();
            }
            catch (AggregateException e)
            {
                if (e.InnerException is System.IO.IOException)
                {
                    System.Diagnostics.Debug.WriteLine("Client closed connection..");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unexpected exception: " + e.Message);
                }
                connectionClient.Close();
            }

            if (message == null)
                return;

            if (!_clientDict.ContainsKey(message.sender))
            {
                _clientDict.Add(message.sender, connectionClient);
                _activeClientTasks.Add(Task.Run(() => ContinueListening(message.sender)));
            }

            HandleMessage(message, connectionClient);
            allDone.Set();
        }

        private async Task<Message> WaitForNextMessage(TcpClient connectionClient)
        {
            var data = new byte[connectionClient.ReceiveBufferSize];
            Message message;

            NetworkStream ns = connectionClient.GetStream();
            int counter = 0;
            do
            {
                try
                {
                    await ns.ReadAsync(data, counter, 1, _cts.Token);

                    if (_cts.Token.IsCancellationRequested)
                        _cts.Token.ThrowIfCancellationRequested();

                    counter++;
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            } while (data[counter-1] != 0xA);
            
            string msg = Encoding.UTF8.GetString(data);

            try
            {
                message = JsonConvert.DeserializeObject<Message>(msg);
            }
            catch
            {
                message = null;
            }

            return message;
        }

        private void ContinueListening(string index)
        {
            TcpClient client = _clientDict[index];

            while (true)
            {
                if (_cts.Token.IsCancellationRequested)
                    _cts.Token.ThrowIfCancellationRequested();

                try
                {
                    Message message = WaitForNextMessage(client).Result;

                    if (message == null)
                        continue;

                    HandleMessage(message, client);
                    
                }
                catch (ObjectDisposedException)
                {
                    System.Diagnostics.Debug.WriteLine("Socket closed unexpectedly..");
                    _clientDict[index].Close();
                    _clientDict.Remove(index);
                    break;
                }
                catch (AggregateException e)
                {
                    if(e.InnerException is System.IO.IOException)
                    {
                        System.Diagnostics.Debug.WriteLine("Client closed connection..");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Unexpected exception: " + e.Message);
                    }
                    _clientDict[index].Close();
                    _clientDict.Remove(index);
                    break;
                } 
            }
        }

        private void HandleMessage(Message msg, TcpClient client)
        {
            switch (msg.message.command)
            {
                case "register":
                    Drone regDrone = JsonConvert.DeserializeObject<Drone>(msg.message.data.ToString());
                    regDrone.IP = client.Client.RemoteEndPoint.ToString();
                    _droneService.RegisterDrone(regDrone);
                    break;
                case "update":
                    Drone upDrone = JsonConvert.DeserializeObject<Drone>(msg.message.data.ToString());
                    upDrone.IP = client.Client.RemoteEndPoint.ToString();
                    _droneService.UpdateDrone(upDrone.UUID, upDrone);
                    break;
                case "ping":
                    SendOne(msg.sender, "pong", "");
                    break;
                default:
                    break;
            }
        }
    }
}

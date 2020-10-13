using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using P9_Backend.Models;
using P9_Backend.Services;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace P9_Backend_Tests.Services
{
    class SocketServiceTests
    {
        private Mock<IDroneService> _mock = new Mock<IDroneService>();

        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public void Test_StartSocket()
        {
            // Arrange
            SocketService service = new SocketService(_mock.Object);
            SocketClient client = new SocketClient();

            // Act
            client.Connect();
            client.Send("ping", "");
            Message ReceivedMessage = client.Receive();

            // Assert
            Assert.AreEqual("pong", ReceivedMessage.message.command);

            service.Stop();
        }

        [Test]
        public void Test_StopSocket()
        {
            // Arrange
            SocketService service = new SocketService(_mock.Object);
            SocketClient client = new SocketClient();

            // Ensures that the server socket is started
            Thread.Sleep(20);

            // Act
            client.Connect();
            Thread.Sleep(20);
            service.Stop();

            // Assert
            var exception = Assert.Catch(() => client.Connect());
            Assert.IsInstanceOf<SocketException>(exception);
        }

        [Test]
        public void Test_SendMsg()
        {
            // Arrange
            SocketService service = new SocketService(_mock.Object);
            SocketClient client = new SocketClient();

            Commando cmd = new Commando { command = "command", data = "data" };
            Message msgSend = new Message("test", cmd, "API");

            // Act
            client.Connect();
            client.Send(msgSend.message.command, msgSend.message.data);
            Thread.Sleep(20);
            service.SendOne(msgSend.sender, msgSend.message.command, msgSend.message.data);
            Message ReceivedMessage = client.Receive();

            // Assert
            Assert.AreEqual(msgSend, ReceivedMessage);

            service.Stop();
        }

        [Test]
        public void Test_SendAllMsg()
        {
            // Arrange
            SocketService service = new SocketService(_mock.Object);
            SocketClient client = new SocketClient();

            Commando cmd = new Commando { command = "command", data = "data" };
            Message msgSend = new Message("test", cmd, "API");

            // Act
            client.Connect();
            client.Send(msgSend.message.command, msgSend.message.data);
            Thread.Sleep(20);
            service.SendAll(msgSend.message.command, msgSend.message.data);
            Message ReceivedMessage = client.Receive();

            // Assert
            Assert.AreEqual(msgSend, ReceivedMessage);

            service.Stop();
        }
    }

    class SocketClient
    {
        const int PORT_NO = 44444;
        const string SERVER_IP = "127.0.0.1";
        TcpClient _client;
        NetworkStream _nwStream;

        public SocketClient() { }

        public void Connect()
        {
            _client = new TcpClient(SERVER_IP, PORT_NO);
            _nwStream = _client.GetStream();
        }

        public void Send(string command, string data)
        {
            Commando cmd = new Commando { command = command, data= data };
            Message msgSend = new Message("test", cmd, "API");

            _nwStream.Write(msgSend.toBytes());
        }

        public Message Receive()
        {
            byte[] bytesToRead = new byte[_client.ReceiveBufferSize];
            int bytesRead =  _nwStream.ReadAsync(bytesToRead, 0, _client.ReceiveBufferSize).Result;
            string msg = Encoding.UTF8.GetString(bytesToRead, 0, bytesRead);
            return JsonConvert.DeserializeObject<Message>(msg);
        }

        public void Close()
        {
            _client.Close();
        }
    }
}

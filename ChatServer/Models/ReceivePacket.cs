using Newtonsoft.Json;
using SocketMessageData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace ChatServer.Models
{
    public class ReceivePacket
    {
        private readonly int _clientId;
        private byte[] _buffer;
        private readonly Socket _receiveSocket;

        public ReceivePacket(Socket receiveSocket, int id)
        {
            _receiveSocket = receiveSocket;
            _clientId = id;
        }

        public void StartReceiving()
        {
            try
            {
                _buffer = new byte[4];
                _receiveSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (_receiveSocket.EndReceive(ar) > 1)
                {
                    _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                    _receiveSocket.Receive(_buffer, _buffer.Length, SocketFlags.None);

                    var data = (Message)FromByteArray(_buffer);

                    switch (data.Command)
                    {
                        case Command.Login:
                            Console.WriteLine($"Socket with id = {_clientId} logged in as {data.Content}");
                            ClientController.Clients.First(x => x.Id == _clientId).Username = data.Content;
                            ClientController.Broadcast(new Message { Command = Command.Message, Content = $"<server> {data.Content} joined the room" }, _clientId);
                            break;
                        case Command.Logout:
                            Console.WriteLine($"Socket with id = {_clientId} logged out");
                            Disconnect();
                            return;
                        case Command.Message:
                            Console.WriteLine($"Message from socket {_clientId} ({ClientController.Clients.First(x => x.Id == _clientId).Username}): {data.Content}\n\tNumber of files: {data.Files.Length}");
                            data.Content = $"{ClientController.Clients.First(x => x.Id == _clientId).Username}: " + data.Content;
                            ClientController.Broadcast(data, _clientId);
                            break;
                        case Command.List:
                            Console.WriteLine($"Message from socket {_clientId} ({ClientController.Clients.First(x => x.Id == _clientId).Username}): List requested");
                            var json = JsonConvert.SerializeObject(ClientController.Clients.Select(x => new { x.Id, x.Username, x.JoinTime }));
                            ClientController.Clients.First(x => x.Id == _clientId).Send.Send(new Message { Command = Command.List, Content = json });
                            break;
                    }

                    StartReceiving();
                }
                else
                {
                    Disconnect();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine($"Client {_clientId} closed connection without logging off");
                Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                if (!_receiveSocket.Connected)
                {
                    Disconnect();
                }
                else
                {
                    StartReceiving();
                }
            }
        }

        private void Disconnect()
        {
            ClientController.Broadcast(new Message { Command = Command.Message, Content = $"<server> {ClientController.Clients.First(x => x.Id == _clientId).Username} left the room" }, _clientId);
            _receiveSocket.Disconnect(true);
            ClientController.RemoveClient(_clientId);
        }

        private static object FromByteArray(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            stream.Write(bytes);
            stream.Position = 0;
            return formatter.Deserialize(stream);
        }
    }
}

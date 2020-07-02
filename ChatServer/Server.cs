using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChatServer
{
    public class Server
    {
        private readonly int _port;
        private readonly IPAddress _address;
        private readonly Socket _socket;

        public Server(IPAddress address, int port = 8000)
        {
            _address = address;
            _port = port;

            //We are using TCP sockets
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool StartServer()
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(_address, _port);

                //Bind and listen on the given address
                _socket.Bind(ipEndPoint);
                _socket.Listen(10);

                //Accept the incoming clients
                _socket.BeginAccept(AcceptCallback, _socket);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return false;
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine($"Accept CallBack port:{_port} protocol type: {ProtocolType.Tcp}");
            Socket acceptedSocket = _socket.EndAccept(out byte[] messageBytes, ar);
            try
            {
               ClientController.AddClient(acceptedSocket);
                _socket.BeginAccept(AcceptCallback, _socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting: {ex}");
                acceptedSocket.Close();
            }
        }
    }
}

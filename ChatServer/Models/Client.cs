using System;
using System.Data;
using System.Net.Sockets;

namespace ChatServer.Models
{
    public class Client
    {
        public Socket Socket { get; set; }
        public ReceivePacket Receive { get; set; }
        public SendPacket Send { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime JoinTime { get; set; }

        public Client(Socket socket, int id)
        {
            Receive = new ReceivePacket(socket, id);
            Send = new SendPacket(socket);
            JoinTime = DateTime.UtcNow;
            Receive.StartReceiving();
            Socket = socket;
            Id = id;
        }
    }
}

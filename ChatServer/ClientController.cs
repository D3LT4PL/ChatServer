using ChatServer.Models;
using SocketMessageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    static class ClientController
    {
        public static List<Client> Clients = new List<Client>();

        public static void AddClient(Socket socket)
        {
            Clients.Add(new Client(socket, Clients.Count));
        }

        public static void RemoveClient(int id)
        {
            Clients.RemoveAt(Clients.FindIndex(x => x.Id == id));
        }

        public static void Broadcast(Message message, int sourceId)
        {
            foreach (var client in Clients)
            {
                if (client.Id != sourceId)
                {
                    client.Send.Send(message);
                }
            }
        }
    }
}

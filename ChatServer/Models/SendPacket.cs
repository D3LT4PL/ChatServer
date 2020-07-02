using SocketMessageData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChatServer.Models
{
    public class SendPacket
    {
        private readonly Socket _sendSocket;

        public SendPacket(Socket sendSocket)
        {
            _sendSocket = sendSocket;
        }

        public void Send(Message message)
        {
            try
            {
                var data = ToByteArray(message);
                var fullPacket = new List<byte>();
                fullPacket.AddRange(BitConverter.GetBytes(data.Length));
                fullPacket.AddRange(data);

                _sendSocket.Send(fullPacket.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception sending message: {ex}");
            }
        }

        private static byte[] ToByteArray(object source)
        {
            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            formatter.Serialize(stream, source);
            return stream.ToArray();
        }
    }
}
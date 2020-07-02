using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(IPAddress.Any);
            if (server.StartServer())
            {
                Console.WriteLine("Server started at 0.0.0.0:8000");
            }
            else
            {
                Console.WriteLine("Server could not be started");
            }
            Task.Delay(-1).Wait();
        }
    }
}

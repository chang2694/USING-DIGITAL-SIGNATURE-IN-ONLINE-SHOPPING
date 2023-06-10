using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
namespace Screen
{
    class Client
    {
        public Socket _client;  // client's socket
        public string desIP;    // destination IP to send file to
        public Client(Socket client)
        {
            this._client = client;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
namespace Screen
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            Connect();
        }

        private void Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Close();
        }
        IPEndPoint IP;
        Socket sever;
        List<Socket> clientList;
        // Connect to sever
        void Connect()
        {
            //IP: sever ip address
            clientList = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Any, 2803); // 2803 is port number
            sever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            sever.Bind(IP);

            Thread Listen = new Thread(() => {

                try
                {
                    while (true)
                    {
                        sever.Listen(100);
                        Socket client = sever.Accept();
                        clientList.Add(client);


                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 2803); // 2803 is port number
                    sever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }

            });
            Listen.IsBackground = true;
            Listen.Start();
        }

        // Close the present connection
        void _Close()
        {
            sever.Close();
        }

        // Receive message
        void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5001];
                    client.Receive(data);

                    foreach (Socket item in clientList)
                    {
                        if (client != null && item != client)
                        {
                            item.Send(data);
                        }
                    }
                    string log = $"Receiving data from {client.RemoteEndPoint}";
                    AddMessage(log);
                }
            }
            catch
            {
                clientList.Remove(client);
                client.Close();
            }
        }


        //add message vao listview
        void AddMessage(string m)
        {
            listView1.Items.Add(new ListViewItem() { Text = m });
        }

        //Phan manh
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);

            return stream.ToArray();
        }

        //Gom lai
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

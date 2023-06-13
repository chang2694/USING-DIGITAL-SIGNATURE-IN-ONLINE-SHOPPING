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
    public partial class ServerForm : Form
    {
        enum DataFormat
        {
            PDF = 1,
            PublicKey = 2,
            DesIP = 3
        }

        public ServerForm() 
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
        List<Client> clientList;
        // Connect to sever
        void Connect()
        {
            //IP: sever ip address
            clientList = new List<Client>();
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
                        Client user = new Client(client);
                        clientList.Add(user);
                        AddMessage($"Incomming connection from {client.RemoteEndPoint}");

                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(user);
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
            Client user = obj as Client;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    user._client.Receive(data);
                    if (data[0]==(byte)DataFormat.PublicKey)    // Send to all client connected to server
                    {
                        foreach (Client item in clientList)
                        {
                            if (user != null && item != user)
                            {
                                item._client.Send(data);
                            }
                        }
                    }
                    if (data[0] == (byte)DataFormat.PDF)    // Send to client has IP END POINT == desIP
                    {
                        foreach (Client item in clientList)
                        {
                            if (user != null && item._client.RemoteEndPoint.ToString() == user.desIP)
                            {
                                item._client.Send(data);
                                break;
                            }
                        }
                    }
                    if (data[0]==(byte)DataFormat.DesIP)
                    {
                        foreach (Client item in clientList)
                        {
                            if (user != null && item == user)
                            {
                                byte[] temp = new byte[50];
                                Array.Copy(data, 1, temp, 0, 50);
                                data = null;
                                item.desIP = Deserialize(temp).ToString();
                                break;
                            }    
                                
                        }
                    }
                    string log = $"Receiving data from {user._client.RemoteEndPoint}";
                    AddMessage(log);
                }
            }
            catch
            {
                clientList.Remove(user);
                user._client.Close();
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

    }
}

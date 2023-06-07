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
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        IPEndPoint IP;
        Socket client;
        string _ipAddress; // server's IP address
        private void btnSendFile_Click(object sender, EventArgs e)
        {
            //to do: gui file pdf
            byte[] data = new byte[1024 * 5000];
            ReadDataFromFile(data, "./NT219.N22.ATCL-Session2_Group12.pdf");    // ten file
            byte[] temp = new byte[1024 * 5001];
            temp[0] = 1;
            Array.Copy(data,0,temp,1,data.Length);
            Send(temp);
        }

        private void btnSendKey_Click(object sender, EventArgs e)
        {
            //to do: gui file chua public key
            byte[] data = new byte[1024 * 5000];
            ReadDataFromFile(data, "./test.txt");    // ten file
            byte[] temp = new byte[1024 * 5001];
            temp[0] = 2;
            Array.Copy(data, 0, temp, 1, data.Length);
            Send(temp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
                button1.Enabled = false;
            }
            catch 
            {
                button1.Enabled = true;
            }
            
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Close();
        }


        // Connect to sever
        void Connect()
        {
            //IP: sever ip address
            _ipAddress = textBox1.Text.Trim();
            IP = new IPEndPoint(IPAddress.Parse(_ipAddress), 2803); // 2803 is port number
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Không thể kết nối sever!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        // Close the present connection
        void _Close()
        {
            client.Close();
        }

        // Send message
        void Send(byte[] data)
        {
            if (data != null)
            {
                client.Send(data);
            }
        }

        // Receive message
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5001];
                    client.Receive(data);

                    if (data[0]==1)
                    {
                        // to do: xu ly du lieu nhan tach ra signature voi data
                        SaveDataToFile(data, "./result.pdf");
                    }
                    if (data[0]==2)
                    {
                        Console.WriteLine("Nhan file chua public key");
                        SaveDataToFile(data, "./result.txt");
                    }
                }
            }
            catch
            {
                Close();
            }
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

        public void ReadDataFromFile(byte[] data, string _fileName)
        {
            System.IO.FileStream stream = new System.IO.FileStream(_fileName, System.IO.FileMode.Open);
            stream.Read(data, 0, data.Length);
            stream.Close();
        }

        public void SaveDataToFile(byte[] data, string _fileName)
        {
            System.IO.FileStream stream = new System.IO.FileStream(_fileName, System.IO.FileMode.Create);
            stream.Write(data, 1, data.Length-1);
            stream.Close();
        }
    }
}

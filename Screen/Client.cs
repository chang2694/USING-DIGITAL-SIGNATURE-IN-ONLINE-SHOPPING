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
            string message = ReadStringFromFile("");    // ten file 
            Send(message);
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
        void Send(string message)
        {
            if (message != string.Empty)
            {
                client.Send(Serialize(message));
            }
        }

        // Receive message
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);

                    string message = (string)Deserialize(data);
                    // to do: xu ly du lieu nhan tach ra public key voi data
                    // to do: luu file xuong thanh pdf
                    SaveStringToFile(message, "");  // Ten file de luu
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

        public string ReadStringFromFile(string _fileName)
        {
            FileStream fs = new FileStream(_fileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string content = sr.ReadToEnd();
            sr.Close();
            return content;
        }

        public void SaveStringToFile(string _str, string _fileName)
        {
            FileStream fs = new FileStream(_fileName, FileMode.Create);
            string content = _str;
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(content);
            sw.Close();
        }
    }
}

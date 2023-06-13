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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;

namespace Screen
{
    public partial class ClientForm : Form
    {
        enum DataFormat 
        {
            PDF = 1,
            PublicKey = 2,
            DesIP=3
        }
        public ClientForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        IPEndPoint IP;
        Socket client;
        string _ipAddress; // server's IP address
        private void btnSendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            //to do: lay dia chi cua nguoi gui toi va gui toi server
            string desIP = textBox3.Text;

            byte[] temp = new byte[50];
            temp[0] = (byte)DataFormat.DesIP;
            Array.Copy(Serialize(desIP), 0, temp, 1, Serialize(desIP).Length);
            Send(temp);
            //to do: gui file pdf
            byte[] data = new byte[1024 * 5000];
            ReadDataFromFile(data, ofd.FileName, 1);    // ten file
            data[0] = (byte)DataFormat.PDF; // Them 1 byte vao de phan biet day la du lieu gi
            Send(data);
        }

        private void btnSendKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            //to do: gui file chua public key
            byte[] data = new byte[1024 * 5000];
            ReadDataFromFile(data, ofd.FileName,1);    // ten file
            data[0] = (byte)DataFormat.PublicKey; // Them 1 byte vao de phan biet day la du lieu gi
            Send(data);
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
                textBox2.Text = client.LocalEndPoint.ToString();    // Print out client local End Point IP
                
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
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);

                    if (data[0]==(byte)DataFormat.PDF)
                    {
                        byte[] signature = new byte[3294];
                        byte[] message = new byte[32388];
                        Array.Copy(data, signature, 3294);
                        Array.Copy(data,1+3293, message,0,32388);
                        // to do: xu ly du lieu nhan tach ra signature voi data
                        SaveDataToFile(signature, "./1234_signature.pdf",1);
                        SaveDataToFile(message, "./1234.pdf",0);
                        //SaveDataToFile(data, "result.pdf", 1);
                    }
                    if (data[0]== (byte)DataFormat.PublicKey)
                    {
                        byte[] temp = new byte[1952];
                        Array.Copy(data, 1, temp, 0, 1952);
                        SaveDataToFile(temp, "./publickey.key", 0);
                    }
                }
            }
            catch
            {
                _Close();
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

        public void ReadDataFromFile(byte[] data, string _fileName, int offset=0)
        {
            System.IO.FileStream stream = new System.IO.FileStream(_fileName, System.IO.FileMode.Open);
            stream.Read(data, offset, data.Length-offset);
            stream.Close();
        }

        public void SaveDataToFile(byte[] data, string _fileName, int offset = 0)
        {
            System.IO.FileStream stream = new System.IO.FileStream(_fileName, System.IO.FileMode.Create);
            stream.Write(data, offset, data.Length-offset);
            stream.Close();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public static void EmbedDataInImage(byte[] imageData, byte[] data)
        {

        }

        public static void EmbedImageInPDF(byte[] pdfData, byte[] imageData, string outputFilePath)
        {
            // Tạo tệp PDF mới
            PdfDocument pdfDocument = new PdfDocument();

            // Đọc dữ liệu từ tệp PDF ban đầu
            using (MemoryStream memoryStream = new MemoryStream(pdfData))
            {
                pdfDocument = PdfReader.Open(memoryStream);
            }

            // Tạo trang mới chứa hình ảnh
            PdfPage page = pdfDocument.AddPage();

            // Chèn hình ảnh vào trang PDF
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XImage image = XImage.FromStream(new MemoryStream(imageData));
            gfx.DrawImage(image, 0, 0, page.Width, page.Height);

            // Lưu tệp PDF đầu ra
            pdfDocument.Save(outputFilePath);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string filename = "Sign.exe";
            Process.Start(filename);
            /*string pdfFilePath = "D:\\DigitalSignature\\Dilithium\\Screen\\1234_signed.pdf";
            string imageFilePath = "D:\\DigitalSignature\\Dilithium\\DiditalSignature\\signed.png";
            string outputFilePath = "D:\\DigitalSignature\\Dilithium\\DiditalSignature\\FilePDF\\1234.pdf";

            byte[] pdfData = File.ReadAllBytes(pdfFilePath);

            byte[] imageData = File.ReadAllBytes(imageFilePath);

            EmbedDataInImage(imageData, pdfData);

            EmbedImageInPDF(pdfData, imageData, outputFilePath);*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filename = "Verify.exe";
            Process.Start(filename);
        }
    }
}

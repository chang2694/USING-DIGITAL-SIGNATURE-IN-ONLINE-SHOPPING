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
using PdfSharp.Pdf.Annotations;

using System.Diagnostics;

namespace Screen
{
    public partial class ClientForm : Form
    {
        Invoice invoice;
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
                        byte[] message = new byte[1024 * 5000];
                        Array.Copy(data, 1, message, 0, data.Length - 1);
                        SaveDataToFile(message, "1234.pdf");
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
        public void getFile(string filenName)
        {
            using (PdfDocument document = PdfReader.Open(filenName, PdfDocumentOpenMode.Import))
            {
                PdfPage page = document.Pages[0];
                string annotationContents = "";

                PdfAnnotations annotations = page.Annotations;

                foreach (PdfAnnotation annotation in annotations)
                {
                    annotationContents = annotation.Contents;
                }

                byte[] content = Encoding.UTF8.GetBytes(annotationContents);

                SaveDataToFile(content, "phases.json");
                Process.Start("phases.json");
            }
        }

        private void button2_Click(object sender, EventArgs e)  // sign button
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string fileName =ofd.FileName;
            getFile(fileName);

            invoice.ReadData();
            
            string message = "";
            byte[] signature = new byte[3293];

            if (invoice.phases.phase == 1)
            {
                goto Phase1;
            }
            else if (invoice.phases.phase == 2)
            {
                goto Phase2;

            }
            else if (invoice.phases.phase == 3)
            {
                goto Phase3;
            }
            else
            {
                MessageBox.Show("Error");
                return;
            }
                
        Phase3:
            Phase3Data p3 = invoice.phases.phase3;
            message += p3.bank + p3.accountNumber;
        Phase2:
            Phase2Data p2 = invoice.phases.phase2;
            message += p2.seller + p2.bank + p2.accountNumber + p2.sellerAddress;
        Phase1:
            Phase1Data p1 = invoice.phases.phase1;
            message += p1.buyer + p1.phoneNumber + p1.email + p1.paymentMethod + p1.buyerAddress + p1.no + p1.date + p1.shippingCost + p1.discount;
            foreach (Product product in p1.Products)
            {
                message += product.description + product.quantity + product.unitPrice + product.amount;
            }

            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            SaveDataToFile(byteMessage, "sign.txt");
            string filename = "Sign.exe";
            Process p = Process.Start(filename);
            p.WaitForExit();
            if(p.ExitCode != 0)
            {
                MessageBox.Show("Signing failed");
                return;
            } else
            {
                ReadDataFromFile(signature, "signature.txt", 0);
                if(invoice.phases.phase == 1)
                {
                    invoice.phases.phase1.signature = signature;
                } else if (invoice.phases.phase == 2)
                {
                    invoice.phases.phase2.signature = signature;
                }
                else if (invoice.phases.phase == 3)
                {
                    invoice.phases.phase3.signature = signature;
                }
                invoice.phases.phase++;
                invoice.WriteData();
                invoice.CreateFile();
                addSignatureStamp(fileName);
                invoice.attachFile(fileName, "phases.json", fileName);
                File.Delete("sign.txt");
                File.Delete("signature.txt");
                File.Delete("phases.json");
                MessageBox.Show("Succeeded");
            }
        }
        private void addSignatureStamp(string fileName)
        {
            PdfDocument invoiceDocument = PdfReader.Open(fileName, PdfDocumentOpenMode.Modify);
            PdfPage page = invoiceDocument.Pages[0];
            XGraphics gfx = XGraphics.FromPdfPage(page);
            double y = page.Height - 20;
            XFont font = new XFont("Microsoft Sans Serif", 10);
            gfx.DrawString("Signer", font, XBrushes.Blue, new XPoint(50, y));
            string signer = "";
            if (invoice.phases.phase == 2)
            {
                signer = "San giao dich";
            } else if (invoice.phases.phase == 3)
            {
                signer = invoice.phases.phase2.seller;
            }
            else if (invoice.phases.phase == 4)
            {
                signer = invoice.phases.phase1.buyer;
            }
            else
            {
                return;
            }
            gfx.DrawString(": " + signer, font, XBrushes.Blue, new XPoint(80, y));
            y += 10;
            gfx.DrawString("Date", font, XBrushes.Blue, new XPoint(50, y));
            DateTime date = DateTime.Now;
            string formattedTime = date.ToString("hh:mm tt d/M/yyyy");
            gfx.DrawString(": " + formattedTime, font, XBrushes.Blue, new XPoint(80, y));
            invoiceDocument.Save(fileName);

        }
        private void button3_Click(object sender, EventArgs e) // verify button
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string fileName = ofd.FileName;
            getFile(fileName);

            invoice.ReadData();
            
            string message = "";
            byte[] signature = new byte[3293];
            byte[] publicKey = new byte[1952];
            if (invoice.phases.phase == 2)
            {
                signature = invoice.phases.phase1.signature;
                publicKey = invoice.phases.phase1.publicKey;
                goto Phase1;
            }
            else if (invoice.phases.phase == 3)
            {
                signature = invoice.phases.phase2.signature;
                publicKey = invoice.phases.phase1.publicKey;
                goto Phase2;
            }
            else if (invoice.phases.phase == 4)
            {
                signature = invoice.phases.phase3.signature;
                publicKey = invoice.phases.phase1.publicKey;
                goto Phase3;
            }
            else
            {
                MessageBox.Show("Error");
                return;
            }

        Phase3:
            Phase3Data p3 = invoice.phases.phase3;
            message += p3.bank + p3.accountNumber;
        Phase2:
            Phase2Data p2 = invoice.phases.phase2;
            message += p2.seller + p2.bank + p2.accountNumber + p2.sellerAddress;
        Phase1:
            Phase1Data p1 = invoice.phases.phase1;
            message += p1.buyer + p1.phoneNumber + p1.email + p1.paymentMethod + p1.buyerAddress + p1.no + p1.date + p1.shippingCost + p1.discount;
            foreach (Product product in p1.Products)
            {
                message += product.description + product.quantity + product.unitPrice + product.amount;
            }

            SaveDataToFile(signature, "signature.txt");
            SaveDataToFile(publicKey, "publickey.key");
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            SaveDataToFile(byteMessage, "sign.txt");
            string filename = "Verify.exe";
            Process p = Process.Start(filename);
            p.WaitForExit();
            if(p.ExitCode != 0)
            {
                MessageBox.Show("Verification failed");
                return;
            }
            else
            {
                File.Delete("sign.txt");
                File.Delete("signature.txt");
                File.Delete("publickey.key");
                MessageBox.Show("Succeeded");
            }
            File.Delete("phases.json");
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            invoice = new Invoice();
        }
    }
}

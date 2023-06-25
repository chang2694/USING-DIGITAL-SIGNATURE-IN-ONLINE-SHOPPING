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
    public partial class ServerForm : Form
    {
        Invoice invoice;

        enum DataFormat
        {
            PDF = 1,
            PublicKey = 2,
            DesIP = 3,
            Cert=4
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
            AddMessage($"Server's IP: {IP}");
            textBox2.Text = IP.Address.ToString().Trim();
            byte[] data = new byte[4096];
            data[0] = (byte)DataFormat.Cert;
            ReadDataFromFile(data,"ServerCert.crt",1);
            Thread Listen = new Thread(() => {

                try
                {
                    while (true)
                    {
                        sever.Listen(100);
                        Socket client = sever.Accept();
                        Client user = new Client(client);
                        clientList.Add(user);
                        //Sending server's cert
                        user._client.Send(data);
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
                                AddMessage("Sending data (public key)...");
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
                                AddMessage("Sending data (pdf file)...");
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

        public void ReadDataFromFile(byte[] data, string _fileName, int offset = 0)
        {
            System.IO.FileStream stream = new System.IO.FileStream(_fileName, System.IO.FileMode.Open);
            stream.Read(data, offset, data.Length - offset);
            stream.Close();
        }

        public void SaveDataToFile(byte[] data, string _fileName, int offset = 0)
        {
            System.IO.FileStream stream = new System.IO.FileStream(_fileName, System.IO.FileMode.Create);
            stream.Write(data, offset, data.Length - offset);
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

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            //to do: lay dia chi cua nguoi gui toi
            string desIP = textBox3.Text;

            //to do: gui file pdf
            byte[] data = new byte[1024 * 5000];
            ReadDataFromFile(data, ofd.FileName, 1);    // ten file
            data[0] = (byte)DataFormat.PDF; // Them 1 byte vao de phan biet day la du lieu gi

            foreach (Client item in clientList)
            {
                if (item != null && item._client.RemoteEndPoint.ToString() == desIP)
                {
                    AddMessage("Sending data (pdf file)...");
                    item._client.Send(data);
                    break;
                }
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
            }
            else if (invoice.phases.phase == 3)
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

        private void button2_Click(object sender, EventArgs e)  // sign button
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string fileName = ofd.FileName;
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
            if (p.ExitCode != 0)
            {
                MessageBox.Show("Signing failed");
                return;
            }
            else
            {
                ReadDataFromFile(signature, "signature.txt", 0);
                if (invoice.phases.phase == 1)
                {
                    invoice.phases.phase1.signature = signature;
                }
                else if (invoice.phases.phase == 2)
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

        private void button3_Click(object sender, EventArgs e)  // verify button
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string fileName = ofd.FileName;
            getFile(fileName);

            invoice.ReadData();

            string message = "";
            byte[] signature = new byte[3293];
            if (invoice.phases.phase == 2)
            {
                signature = invoice.phases.phase1.signature;
                goto Phase1;
            }
            else if (invoice.phases.phase == 3)
            {
                signature = invoice.phases.phase2.signature;
                goto Phase2;
            }
            else if (invoice.phases.phase == 4)
            {
                signature = invoice.phases.phase3.signature;
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
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            SaveDataToFile(byteMessage, "sign.txt");
            string filename = "Verify.exe";
            Process p = Process.Start(filename);
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                MessageBox.Show("Verification failed");
                return;
            }
            else
            {
                File.Delete("sign.txt");
                File.Delete("signature.txt");
                MessageBox.Show("Succeeded");
            }
            File.Delete("phases.json");
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            invoice = new Invoice();
        }
    }
}

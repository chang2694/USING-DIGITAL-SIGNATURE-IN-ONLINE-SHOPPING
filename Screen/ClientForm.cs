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
using System.Diagnostics;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using Aspose.Pdf.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;


namespace Screen
{
    public partial class ClientForm : System.Windows.Forms.Form
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
                        //SaveDataToFile(signature, "./1234_signature.pdf",1);
                        //SaveDataToFile(message, "./1234.pdf",0);
                        SaveDataToFile(data, "1234.pdf", 1);
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

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = "Sign.exe";
            Process p = Process.Start(filename);
            p.WaitForExit();

            AddSignature();
        }

        private void AddSignature()
        {
            string path = "./1234.pdf";

            Document pdfDocument = new Document(path);
            //PdfDocument document = PdfReader.Open(path, PdfDocumentOpenMode.Modify);
            
            byte[] signature = File.ReadAllBytes("./1234_signature.pdf");
            FileSpecification fileSpecification = new FileSpecification("./1234_signature.pdf", "Signature");
            pdfDocument.EmbeddedFiles.Add(fileSpecification);
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //PdfPage page = document.Pages[0];
            //XGraphics gfx = XGraphics.FromPdfPage(page);

            //gfx.DrawString(Convert.ToBase64String(signature), new XFont("Microsoft Sans Serif", 0, XFontStyle.Bold), XBrushes.Blue, new XRect(0, 0, 0, 0));

            pdfDocument.Save(path);
            //System.IO.File.Delete("./1234_signature.pdf");
            //MessageBox.Show("Deleted");
        }

        private void ReadSignature()
        {
            string path = "./1234.pdf";
            Document pdfDocument = new Document(path);
            foreach (FileSpecification fileSpecification in pdfDocument.EmbeddedFiles)
            {
                if(fileSpecification.Description == "Signature")
                {
                    string savePath = "./1234_signature.pdf";

                    using (Stream fileStream = fileSpecification.StreamContents)
                    {
                        using (FileStream outputFileStream = File.Create(savePath))
                        {
                            fileStream.CopyTo(outputFileStream);
                        }
                    }

                }
            }
            pdfDocument.EmbeddedFiles.Delete();
            pdfDocument.Save(path);

            //byte[] pdfBytes = File.ReadAllBytes(filePath);

            //using (MemoryStream stream = new MemoryStream(pdfBytes))
            //{
            //    PdfDocument document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

            //    int pageCount = document.PageCount;

            //    int objectStartIndex = pdfBytes.Length - 3293;

            //    byte[] objectBytes = new byte[3293];
            //    Array.Copy(pdfBytes, objectStartIndex, objectBytes, 0, 3293);

            //    document.Close();

            //    SaveDataToFile(objectBytes, "./1234_signature.pdf");
            //}
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ReadSignature();
            string filename = "Verify.exe";
            Process p = Process.Start(filename);
            p.WaitForExit();
            System.IO.File.Delete("./1234_signature.pdf");
        }

        public static void SignWithTimeStampServer()
        {
            using (Document document = new Document("./1234.pdf"))
            {
                using (PdfFileSignature signature = new PdfFileSignature(document))
                {
                    PKCS7 pkcs = new PKCS7(@"C:\Keys\test.pfx", "nhom11");
                    TimestampSettings timestampSettings = new TimestampSettings("https://freetsa.org/tsr", string.Empty); // User/Password can be omitted
                    pkcs.TimestampSettings = timestampSettings;
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(100, 100, 200, 100);
                    // Create any of the three signature types
                    signature.Sign(1, "Test", "Chang", "UIT", true, rect, pkcs);
                    // Save output PDF file
                    signature.Save("./1234.pdf");
                }
            }
        }

        public void ExtractSignature()
        {
            string input = "./1234.pdf";
            using (Document pdfDocument = new Document(input))
            {
                foreach (Field field in pdfDocument.Form)
                {
                    SignatureField sf = field as SignatureField;
                    if (sf != null)
                    {
                        Stream cerStream = sf.ExtractCertificate();
                        if (cerStream != null)
                        {
                            using (cerStream)
                            {
                                byte[] bytes = new byte[cerStream.Length];
                                using (FileStream fs = new FileStream( @"./1234.cer", FileMode.CreateNew))
                                {
                                    cerStream.Read(bytes, 0, bytes.Length);
                                    fs.Write(bytes, 0, bytes.Length);
                                }
                            }
                        }
                    }
                }
                removeSignature();
            }
        }

        public void removeSignature()
        {
            try
            {
                PdfFileSignature pdfSign = new PdfFileSignature();
                // Open PDF document
                pdfSign.BindPdf("./1234.pdf");
                // Get list of signature names
                IList<string> names = pdfSign.GetSignNames();
                // Remove all the signatures from the PDF file
                for (int index = 0; index < names.Count; index++)
                {
                    pdfSign.RemoveSignature((string)names[index]);
                }
                // Save updated PDF file
                pdfSign.Save("./1234.pdf");
                // ExEnd:RemoveSignature
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

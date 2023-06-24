using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace Screen
{
    public partial class Invoice : Form
    {
        public InvoiceData phases;
        public Invoice()
        {
            InitializeComponent();
            phases = new InvoiceData();
            ReadData();
        }
        public void CreateFile()
        {
            if ((phases.phase == 1 && phases.phase1.signature.Length == 0 )|| ( phases.phase == 2 && phases.phase1.signature.Length!=0))
            {
                phase1();
            } else if ((phases.phase == 2 && phases.phase2.signature.Length == 0) || (phases.phase == 3 && phases.phase2.signature.Length != 0))
            {
                phase2();
            } else if ((phases.phase == 3 && phases.phase3.signature.Length == 0) || (phases.phase == 4 && phases.phase3.signature.Length != 0))
            {
                phase3();
            }
        }
        public void attachFile(string invoiceFilePath, string phasesFilePath, string outputFilePath)
        {
            PdfDocument invoiceDocument = PdfReader.Open(invoiceFilePath, PdfDocumentOpenMode.Import);

            string phasesJson = File.ReadAllText(phasesFilePath);

            PdfDocument outputDocument = new PdfDocument();

            foreach (PdfPage page in invoiceDocument.Pages)
            {
                outputDocument.AddPage(page);
            }

            PdfPage phasesPage = outputDocument.Pages[0];
            phasesPage.Annotations.Clear();
            phasesPage.Annotations.Add(new PdfSharp.Pdf.Annotations.PdfTextAnnotation()
            {
                Contents = phasesJson,
                Rectangle = new PdfSharp.Pdf.PdfRectangle(new XRect(0, 0, 0, 0)) 
            });

            outputDocument.Save(outputFilePath);
        }

        private void phase1()
        {
            string no;
            string date;
            string buyer;
            string phoneNumber;
            string email;
            string payment;
            string buyerAddress;

            getData(out no, out date, out buyer, out phoneNumber, out email, out payment, out buyerAddress);


            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Microsoft Sans Serif", 13);
            XFont fontHeader = new XFont("Microsoft Sans Serif", 13, XFontStyle.Bold);

            gfx.DrawString("INVOICE", new XFont("Microsoft Sans Serif", 40, XFontStyle.Bold), XBrushes.Blue,
                new XPoint(50, 70));

            gfx.DrawString("No.", font, XBrushes.Black,
                new XPoint(400, 50));
            gfx.DrawString(": " + no, font, XBrushes.Black,
                new XPoint(430, 50));
            gfx.DrawString("Date", font, XBrushes.Black,
                new XPoint(400, 65));
            gfx.DrawString(date, font, XBrushes.Black,
                new XPoint(430, 65));

            gfx.DrawString("Buyer", font, XBrushes.Black,
                new XPoint(50, 130));
            gfx.DrawString(buyer, font, XBrushes.Black,
                new XPoint(150, 130));
            gfx.DrawString("Phone number", font, XBrushes.Black,
                new XPoint(50, 145));
            gfx.DrawString(phoneNumber, font, XBrushes.Black,
                new XPoint(150, 145));
            gfx.DrawString("Email", font, XBrushes.Black,
                new XPoint(50, 160));
            gfx.DrawString(email, font, XBrushes.Black,
                new XPoint(150, 160));
            gfx.DrawString("Payment method", font, XBrushes.Black,
                new XPoint(50, 175));
            gfx.DrawString(payment, font, XBrushes.Black,
                new XPoint(150, 175));
            gfx.DrawString("Adress", font, XBrushes.Black,
                new XPoint(50, 190));
            gfx.DrawString(buyerAddress, font, XBrushes.Black,
                new XPoint(150, 190));

            gfx.DrawString("NO.", fontHeader, XBrushes.Blue,
                new XPoint(50, 250));
            gfx.DrawString("DESCRIPTION", fontHeader, XBrushes.Blue,
                new XPoint(100, 250));
            gfx.DrawString("QUANTITY", fontHeader, XBrushes.Blue,
                new XPoint(300, 250));
            gfx.DrawString("UNIT PRICE", fontHeader, XBrushes.Blue,
                new XPoint(400, 250));
            gfx.DrawString("AMOUNT", fontHeader, XBrushes.Blue,
                new XPoint(500, 250));

            Point currentPosition = new Point(50, 265);

            // add list of product

            if (currentPosition.Y >= page.Height - 110)
            {
                page = document.AddPage();
                currentPosition.X = 50;
                currentPosition.Y = 50;
            }

            gfx.DrawString("Sub total", font, XBrushes.Black,
                new XPoint(page.Width / 2 + 10, currentPosition.Y));
            currentPosition.Y += 15;
            gfx.DrawString("Shipping cost", font, XBrushes.Black,
                new XPoint(page.Width / 2 + 10, currentPosition.Y));
            currentPosition.Y += 15;
            gfx.DrawString("Discount", font, XBrushes.Black,
                new XPoint(page.Width / 2 + 10, currentPosition.Y));
            currentPosition.Y += 15;
            gfx.DrawString("Total", fontHeader, XBrushes.Blue,
                new XPoint(page.Width / 2 + 10, currentPosition.Y));


            string path =  no + ".pdf";
            document.Save(path);
            
        }
        private void phase2()
        {
            string bank = phases.phase2.bank;
            string accountNumber = phases.phase2.accountNumber;
            int no = phases.phase1.no;
            string seller = phases.phase2.seller;
            string sellerAddress = phases.phase2.sellerAddress;
            string date = phases.phase1.date;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Microsoft Sans Serif", 13);
            XFont fontHeader = new XFont("Microsoft Sans Serif", 13, XFontStyle.Bold);

            gfx.DrawString("INVOICE", new XFont("Microsoft Sans Serif", 40, XFontStyle.Bold), XBrushes.Blue,
                new XPoint(200, 100));

            gfx.DrawString("No.", font, XBrushes.Black,
                new XPoint(70, 150));
            gfx.DrawString(": " + no, font, XBrushes.Black,
                new XPoint(170, 150));
            gfx.DrawString("Date", font, XBrushes.Black,
               new XPoint(70, 165));
            gfx.DrawString(": " + date, font, XBrushes.Black,
                new XPoint(170, 165));

            gfx.DrawString("Seller", font, XBrushes.Black,
                new XPoint(70, 180));
            gfx.DrawString(": " + seller, font, XBrushes.Black,
                new XPoint(170, 180));
            gfx.DrawString("Bank", font, XBrushes.Black,
                new XPoint(70, 195));
            gfx.DrawString(": " + bank, font, XBrushes.Black,
                new XPoint(170, 195));
            gfx.DrawString("Account number", font, XBrushes.Black,
                new XPoint(70, 210));
            gfx.DrawString(": " + accountNumber, font, XBrushes.Black,
                new XPoint(170, 210));
            gfx.DrawString("Address", font, XBrushes.Black,
                new XPoint(70, 225));
            gfx.DrawString(": " + sellerAddress, font, XBrushes.Black,
                new XPoint(170, 225));

            string path = no + ".pdf";
            document.Save(path);
        }
        private void phase3()
        {
            string bank = phases.phase3.bank;
            string accountNumber = phases.phase3.accountNumber;
            int no = phases.phase1.no;
            string buyer = phases.phase1.buyer;
            string buyerAddress = phases.phase1.buyerAddress;
            string date = phases.phase1.date;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Microsoft Sans Serif", 13);
            XFont fontHeader = new XFont("Microsoft Sans Serif", 13, XFontStyle.Bold);

            gfx.DrawString("INVOICE", new XFont("Microsoft Sans Serif", 40, XFontStyle.Bold), XBrushes.Blue,
                new XPoint(200, 100));

            gfx.DrawString("No.", font, XBrushes.Black,
                new XPoint(70, 150));
            gfx.DrawString(": " + no, font, XBrushes.Black,
                new XPoint(170, 150));
            gfx.DrawString("Date", font, XBrushes.Black,
               new XPoint(70, 165));
            gfx.DrawString(": " + date, font, XBrushes.Black,
                new XPoint(170, 165));

            gfx.DrawString("Buyer", font, XBrushes.Black,
                new XPoint(70, 180));
            gfx.DrawString(": " + buyer, font, XBrushes.Black,
                new XPoint(170, 180));
            gfx.DrawString("Bank", font, XBrushes.Black,
                new XPoint(70, 195));
            gfx.DrawString(": " + bank, font, XBrushes.Black,
                new XPoint(170, 195));
            gfx.DrawString("Account number", font, XBrushes.Black,
                new XPoint(70, 210));
            gfx.DrawString(": " + accountNumber, font, XBrushes.Black,
                new XPoint(170, 210));
            gfx.DrawString("Address", font, XBrushes.Black,
                new XPoint(70, 225));
            gfx.DrawString(": " + buyerAddress, font, XBrushes.Black,
                new XPoint(170, 225));

            string path = no + ".pdf";
            document.Save(path);
        }
        private void getData(out string no,
        out string date,
        out string buyer,
        out string phoneNumber,
        out string email,
        out string payment,
        out string buyerAddress)
        {
            string colon = ": ";
            Phase1Data p1 = phases.phase1;

            no = p1.no.ToString();
            date = colon + p1.date;
            buyer = colon + p1.buyer;
            phoneNumber = colon + p1.phoneNumber;
            email = colon + p1.email;
            payment = colon + p1.paymentMethod;
            buyerAddress = colon + p1.buyerAddress;
        }

        private void ChangeBox()
        {
            Phase1Data p1 = phases.phase1;

            invoiceNo.Text = p1.no.ToString();
            date.Text = p1.date;

            Buyer.Text = p1.buyer;
            phoneNumber.Text = p1.phoneNumber;
            email.Text = p1.email;
            payment.Text = p1.paymentMethod;
            buyerAdress.Text = p1.buyerAddress;
        }
        public void ReadData()
        {
            using (StreamReader sr = File.OpenText("phases.json"))
            {
                var obj = sr.ReadToEnd();
                phases = JsonConvert.DeserializeObject<InvoiceData>(obj);
            }
        }
        public void WriteData()
        {
            string update = JsonConvert.SerializeObject(phases);
            File.WriteAllText("phases.json", update);
        }

        private void Invoice_Load(object sender, EventArgs e)
        {

            CreateFile();
            attachFile("1234.pdf", "phases.json", "1234.pdf");
            Close();
        }
    }
    public class InvoiceData
    {
        public int phase;
        public Phase1Data phase1 { get; set; }
        public Phase2Data phase2 { get; set; }
        public Phase3Data phase3 { get; set; }
       
    }

    public class Phase1Data
    {
        public byte[] signature { get; set; }
        public string buyer { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string paymentMethod { get; set; }
        public string buyerAddress { get; set; }
        public int no { get; set; }
        public string date { get; set; }
        public int shippingCost { get; set; }
        public float discount { get; set; }
        public List<Product> Products { get; set; }

    }

    public class Phase2Data
    {
        public byte[] signature { get; set; }

        public string seller { get; set; }
        public string bank { get; set; }
        public string accountNumber { get; set; }
        public string sellerAddress { get; set; }
      
    }

    public class Phase3Data
    {
        public byte[] signature { get; set; }
        public string bank { get; set; }
        public string accountNumber { get; set; }
      
    }

    public class Product
    {
        public string description { get; set; }
        public int quantity { get; set; }
        public int unitPrice { get; set; }
        public int amount { get; set; }
    }
}

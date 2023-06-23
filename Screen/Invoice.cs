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
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace Screen
{
    public partial class Invoice : Form
    {
        Phases phases;
        public Invoice()
        {
            InitializeComponent();
            phases = new Phases();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadData();
            ChangeBox();
            string no;
            string date;
            string buyer;
            string phoneNumber;
            string email;
            string payment;
            string buyerAddress;
            string seller;
            string sellerAddress;

            getData(out no, out date, out buyer, out phoneNumber, out email, out payment, out buyerAddress, out seller, out sellerAddress);


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

            gfx.DrawString("Seller", font, XBrushes.Black,
                new XPoint(page.Width / 2 + 10, 130));
            gfx.DrawString(seller, font, XBrushes.Black,
                new XPoint(page.Width / 2 + 60, 130));
            gfx.DrawString("Address", font, XBrushes.Black,
                new XPoint(page.Width / 2 + 10, 145));
            gfx.DrawString(sellerAddress, font, XBrushes.Black,
                new XPoint(page.Width / 2 + 60, 145));

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

            XImage logo = XImage.FromFile("D:\\DigitalSignature\\Dilithium\\Screen\\dilithium.png");
            gfx.DrawImage(logo, new XRect(70, currentPosition.Y, logo.PointWidth / logo.PointHeight * 60, 60));

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


            string path = "D:\\DigitalSignature\\Dilithium\\DiditalSignature\\FilePDF\\" + no + ".pdf";
            document.Save(path);
            Process.Start(path);
        }

        private void getData(out string no,
        out string date,
        out string buyer,
        out string phoneNumber,
        out string email,
        out string payment,
        out string buyerAddress,
        out string seller,
        out string sellerAddress)
        {
            string colon = ": ";
            Phase1 p1 = phases.p1;

            no = p1.no.ToString();
            date = colon + p1.date;
            buyer = colon + p1.buyer;
            phoneNumber = colon + p1.phoneNumber;
            email = colon + p1.email;
            payment = colon + p1.paymentMethod;
            buyerAddress = colon + p1.buyerAddress;
            seller = colon + p1.seller;
            sellerAddress = colon + p1.sellerAddress;
        }

        private void ChangeBox()
        {
            Phase1 p1 = phases.p1;

            invoiceNo.Text = p1.no.ToString();
            date.Text = p1.date;

            Buyer.Text = p1.buyer;
            phoneNumber.Text = p1.phoneNumber;
            email.Text = p1.email;
            payment.Text = p1.paymentMethod;
            buyerAdress.Text = p1.buyerAddress;

            seller.Text = p1.seller;
            sellerAddress.Text = p1.sellerAddress;


        }
        public void ReadData()
        {
            using (StreamReader sr = File.OpenText("D://DigitalSignature//Dilithium//Screen//data.json"))
            {
                var obj = sr.ReadToEnd();
                phases = JsonConvert.DeserializeObject<Phases>(obj);
            }
        }
    }
    public class Phases {
        public Phase1 p1 {set; get;}
        public Phase2 p2 {set;get;}
        public Phase3 p3 { set;get;} 
    }
public class Phase1
{
        public string signature { set; get; }
        public string buyer { set; get; }
        public string phoneNumber { set; get; }
        public string email { set; get; }
        public string paymentMethod { set; get; }
        public string buyerAddress { set; get; }
        public string seller { set; get; }
        public string sellerAddress { set; get; }
        public int no { set; get; }
        public string date { set; get; }
        public int shippingCost { set; get; }
        public float discount { set; get; }
        public List<Product> Products { set; get; }
    }
    public class Phase2
    {
        public string signature { set; get; }
        public string bank { set; get; }
        public string accountBank { set; get; }
    }
    public class Phase3
    {
        public string signature { set; get; }
        public string bank { set; get; }
        public string accountBank { set; get; }
    }
    public class Product
    {
        public string description { set; get; }
        public int quantity { set; get; }
        public int unitPrice { set; get; }
        public int amount { set; get; }
    }
}

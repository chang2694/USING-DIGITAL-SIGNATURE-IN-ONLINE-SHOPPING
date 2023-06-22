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
        InvoiceData invoiceData;
        public Invoice()
        {
            InitializeComponent();
            invoiceData = new InvoiceData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadData();
            ChangeBox();
            //string no;
            //string date;
            //string buyer;
            //string phoneNumber;
            //string email;
            //string payment;
            //string buyerAddress;
            //string seller;
            //string sellerAddress;
            
            //getData(out no, out date, out buyer, out phoneNumber, out email, out payment, out buyerAddress, out seller, out sellerAddress);


            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //PdfDocument document = new PdfDocument();
            //PdfPage page = document.AddPage();
            //XGraphics gfx = XGraphics.FromPdfPage(page);
            //XFont font = new XFont("Microsoft Sans Serif", 13);
            //XFont fontHeader = new XFont("Microsoft Sans Serif", 13, XFontStyle.Bold);

            //gfx.DrawString("INVOICE", new XFont("Microsoft Sans Serif", 40, XFontStyle.Bold), XBrushes.Blue,
            //    new XPoint(50, 70));

            //gfx.DrawString("No.", font, XBrushes.Black,
            //    new XPoint(400, 50));
            //gfx.DrawString(": " + no, font, XBrushes.Black,
            //    new XPoint(430, 50));
            //gfx.DrawString("Date", font, XBrushes.Black,
            //    new XPoint(400, 65));
            //gfx.DrawString(date, font, XBrushes.Black,
            //    new XPoint(430, 65));

            //gfx.DrawString("Buyer", font, XBrushes.Black,
            //    new XPoint(50, 130));
            //gfx.DrawString(buyer, font, XBrushes.Black,
            //    new XPoint(150, 130));
            //gfx.DrawString("Phone number", font, XBrushes.Black,
            //    new XPoint(50, 145));
            //gfx.DrawString(phoneNumber, font, XBrushes.Black,
            //    new XPoint(150, 145));
            //gfx.DrawString("Email", font, XBrushes.Black,
            //    new XPoint(50, 160));
            //gfx.DrawString(email, font, XBrushes.Black,
            //    new XPoint(150, 160));
            //gfx.DrawString("Payment method", font, XBrushes.Black,
            //    new XPoint(50, 175));
            //gfx.DrawString(payment, font, XBrushes.Black,
            //    new XPoint(150, 175));
            //gfx.DrawString("Adress", font, XBrushes.Black,
            //    new XPoint(50, 190));
            //gfx.DrawString(buyerAddress, font, XBrushes.Black,
            //    new XPoint(150, 190));

            //gfx.DrawString("Seller", font, XBrushes.Black,
            //    new XPoint(page.Width/2 + 10, 130));
            //gfx.DrawString(seller, font, XBrushes.Black,
            //    new XPoint(page.Width / 2 + 60, 130));
            //gfx.DrawString("Address", font, XBrushes.Black,
            //    new XPoint(page.Width / 2 + 10, 145));
            //gfx.DrawString(sellerAddress, font, XBrushes.Black,
            //    new XPoint(page.Width / 2 + 60, 145));

            //gfx.DrawString("NO.", fontHeader, XBrushes.Blue,
            //    new XPoint(50, 250));
            //gfx.DrawString("DESCRIPTION", fontHeader, XBrushes.Blue,
            //    new XPoint(100, 250));
            //gfx.DrawString("QUANTITY", fontHeader, XBrushes.Blue,
            //    new XPoint(300, 250));
            //gfx.DrawString("UNIT PRICE", fontHeader, XBrushes.Blue,
            //    new XPoint(400, 250));
            //gfx.DrawString("AMOUNT", fontHeader, XBrushes.Blue,
            //    new XPoint(500, 250));

            //Point currentPosition = new Point(50, 265);

            //// add list of product

            //if (currentPosition.Y >= page.Height - 110)
            //{
            //    page = document.AddPage();
            //    currentPosition.X = 50;
            //    currentPosition.Y = 50;
            //}

            //XImage logo = XImage.FromFile("D:\\DigitalSignature\\Dilithium\\Screen\\dilithium.png");
            //gfx.DrawImage(logo, new XRect(70, currentPosition.Y, logo.PointWidth/logo.PointHeight*60, 60));

            //gfx.DrawString("Sub total", font, XBrushes.Black,
            //    new XPoint(page.Width / 2 + 10, currentPosition.Y));
            //currentPosition.Y += 15;
            //gfx.DrawString("Shipping cost", font, XBrushes.Black,
            //    new XPoint(page.Width / 2 + 10, currentPosition.Y));
            //currentPosition.Y += 15;
            //gfx.DrawString("Discount", font, XBrushes.Black,
            //    new XPoint(page.Width / 2 + 10, currentPosition.Y));
            //currentPosition.Y += 15;
            //gfx.DrawString("Total", fontHeader, XBrushes.Blue,
            //    new XPoint(page.Width / 2 + 10, currentPosition.Y));


            //string path = "D:\\DigitalSignature\\Dilithium\\DiditalSignature\\FilePDF\\" + no + ".pdf";
            //document.Save(path);
            //Process.Start(path);
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

            no = "1234_signed";
            date = colon + "07/06/2023";
            buyer = colon + "chang";
            phoneNumber = colon + "123456789";
            email = colon + "gmail.com";
            payment = colon + "Momo";
            buyerAddress = colon + "KTX khu A";
            seller = colon + "trang";
            sellerAddress = colon + "Bien Hoa";
        }
        
        private void ChangeBox()
        {
            if (invoiceData == null)
            {
                return;
            }

            invoiceNo.Text = invoiceData.no.ToString();
            date.Text = invoiceData.date;

            Buyer.Text = invoiceData.buyer;
            phoneNumber.Text = invoiceData.phoneNumber;
            email.Text = invoiceData.email;
            payment.Text = invoiceData.email;
            buyerAdress.Text = invoiceData.buyerAddress;

            seller.Text = invoiceData.seller;
            sellerAddress.Text = invoiceData.sellerAddress;

            
        }
        public void ReadData()
        {
            using (StreamReader sr = File.OpenText("D://DigitalSignature//Dilithium//Screen//data.json"))
            {
                var obj = sr.ReadToEnd();
                invoiceData = JsonConvert.DeserializeObject<InvoiceData>(obj);
            }
        }
    }
    public class InvoiceData
    {
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
    public class Product
    {
        public string description { set; get; }
        public int quantity { set; get; }
        public int unitPrice { set; get; }
        public int amount { set; get; }
    }
}

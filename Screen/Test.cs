using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;

using OpenSSL.Crypto;
using OpenSSL.X509;
using System.Security.Cryptography.X509Certificates;
using OpenSSL.Core;


namespace Screen
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }
        
        private void Test_Load(object sender, EventArgs e)
        {
            GeneratePfxCertificate("./1234.txt", "./MyPrivatekey.key", "./1234.pfx", "./root.txt", "nhom11");
        }

        public X509Certificate2 GeneratePfxCertificate(string certificatePath, string privateKeyPath,
        string certificatePfxPath, string rootCertificatePath, string pkcs12Password)
        {
            string keyFileContent = File.ReadAllText(privateKeyPath);
            string certFileContent = File.ReadAllText(certificatePath);
            string rootCertFileContent = File.ReadAllText(rootCertificatePath);

            var certBio = new BIO(certFileContent);
            var rootCertBio = new BIO(rootCertFileContent);

            CryptoKey cryptoKey = CryptoKey.FromPrivateKey(keyFileContent, string.Empty);
            var certificate = new OpenSSL.X509.X509Certificate(certBio);
            var rootCertificate = new OpenSSL.X509.X509Certificate(rootCertBio);

            using (var certChain = new OpenSSL.Core.Stack<OpenSSL.X509.X509Certificate> { rootCertificate })
            using (var p12 = new PKCS12(pkcs12Password, cryptoKey, certificate, certChain))
            using (var pfxBio = BIO.MemoryBuffer())
            {
                p12.Write(pfxBio);
                var pfxFileByteArrayContent =
                    pfxBio.ReadBytes((int)pfxBio.BytesPending).Array;

                File.WriteAllBytes(certificatePfxPath, pfxFileByteArrayContent);
            }

            return new X509Certificate2(certificatePfxPath, pkcs12Password);
        }
    }
}

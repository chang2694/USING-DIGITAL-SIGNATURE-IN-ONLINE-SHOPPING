using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;


namespace TestBlob
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void UploadBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Uploading a File from memory");

            string fileName = string.Empty;
            string basepath = Application.StartupPath;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = basepath + "\\File\\";
                openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                }
            }

            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=savefilepdf;AccountKey=BkvyfQpWve844ZneZbywFR/DCiyhqKkmlDKf8QPMdbVI+MwkLkbpUEh8jh1WYg9nTr5pFs8LDNLx+AStdsPJCw==";
            var blobStorageContainerName = "reports";
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);

            var blob = container.GetBlobClient(fileName);

            var stream = File.OpenRead(fileName);

            await blob.UploadAsync(stream);

            MessageBox.Show("Upload thành công", "Thông báo");
        }

        private async void downloadBtn_Click(object sender, EventArgs e)
        {
            // Tạo và cấu hình SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.Title = "Save PDF File";
            saveFileDialog.FileName = "downloaded_file.pdf";

            // Hiển thị SaveFileDialog và kiểm tra kết quả
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string localFilePath = saveFileDialog.FileName;
                var fileName = "[1]_feng2002.pdf";

                var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=savefilepdf;AccountKey=BkvyfQpWve844ZneZbywFR/DCiyhqKkmlDKf8QPMdbVI+MwkLkbpUEh8jh1WYg9nTr5pFs8LDNLx+AStdsPJCw==";
                var blobStorageContainerName = "reports";
                var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);
                var blob = container.GetBlobClient(fileName);

                await DownloadBlobToFileAsync(blob, localFilePath);

                MessageBox.Show("Download thành công", "Thông báo");
            }
        }

        private async Task DownloadBlobToFileAsync(BlobClient blobClient, string localFilePath)
        {
            using (FileStream fileStream = File.OpenWrite(localFilePath))
            {
                await blobClient.DownloadToAsync(fileStream);
            }
        }
    }
}
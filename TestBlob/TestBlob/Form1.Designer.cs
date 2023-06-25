namespace TestBlob
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            UploadBtn = new Button();
            downloadBtn = new Button();
            SuspendLayout();
            // 
            // UploadBtn
            // 
            UploadBtn.Location = new Point(297, 141);
            UploadBtn.Name = "UploadBtn";
            UploadBtn.Size = new Size(185, 74);
            UploadBtn.TabIndex = 0;
            UploadBtn.Text = "Upload";
            UploadBtn.UseVisualStyleBackColor = true;
            UploadBtn.Click += UploadBtn_Click;
            // 
            // downloadBtn
            // 
            downloadBtn.Location = new Point(297, 259);
            downloadBtn.Name = "downloadBtn";
            downloadBtn.Size = new Size(185, 74);
            downloadBtn.TabIndex = 1;
            downloadBtn.Text = "Download";
            downloadBtn.UseVisualStyleBackColor = true;
            downloadBtn.Click += downloadBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(downloadBtn);
            Controls.Add(UploadBtn);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button UploadBtn;
        private Button downloadBtn;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screen
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerForm frmServer = new ServerForm();
            frmServer.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClientForm frmClient = new ClientForm();
            frmClient.Show();
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLab2Form
{
    public partial class formServer : Form
    {
        private String path;
        private int port;
        private Server server;
        public delegate void TextBoxDelegate(string status);

        public formServer()
        {
            InitializeComponent();
            buttonStart.Enabled = false;
            buttonStop.Enabled = false;
        }

        public void UpdatingTextBox(string status)
        {
            if (this.textBoxLog.InvokeRequired)
                this.textBoxLog.Invoke(new TextBoxDelegate(UpdatingTextBox), new object[] { status });
            else
                this.textBoxLog.Text = status;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {   
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            server = new Server(port, path, this);
            server.Start();
            textBoxLog.Visible = true;
            Height = 662;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            server.Stop();   
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            path = textBoxSciezka.Text;
            if (int.TryParse(textBoxPort.Text, out port))
            {
                if (Directory.Exists(textBoxSciezka.Text))
                {
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Podana sciezka nie istnieje");
                }
            }
            else
            {
                MessageBox.Show("Zly format portu");
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxSciezka.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}

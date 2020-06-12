using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FingerPrintReaderTestConsole
{
    public partial class Form1 : Form
    {
        public Form1(Image image)
        {
            InitializeComponent();
            pictureBox1.Image = image;
        }

        private void Bu_Save_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog Location = new SaveFileDialog())
            {
                Location.InitialDirectory = Environment.CurrentDirectory;
                if(Location.ShowDialog() == DialogResult.OK)
                {
                    if(!Location.CheckPathExists)
                    {
                        MessageBox.Show("Error finding path!");
                        return;
                    }

                    pictureBox1.Image.Save(Location.FileName, ImageFormat.MemoryBmp);

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Host_FT_stepoverAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }





    
       
  

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    this.ShowInTaskbar = false;
                }
            }
        }

    }
}

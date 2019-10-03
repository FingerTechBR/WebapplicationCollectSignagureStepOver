using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FT_stepoverAPI;


using System.Windows.Forms;

namespace Host_FT_stepoverAPI
{
    static class Program
    {
       
        [STAThread]
        static void Main()
        {

            string baseAddress = "http://localhost:9000";            
            Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress);          

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

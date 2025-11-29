using System;
using System.Windows.Forms;
using QuanLyXe;
using MENUQL;

namespace QuanlyTaiXe
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu());
        }
    }
}
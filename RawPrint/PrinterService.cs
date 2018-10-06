using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RawPrint
{
    public partial class PrinterService : ServiceBase
    {
        public static string SERVER_IP = "127.0.0.1";
        public static int PORT_NO = 9100;
        WebServer ws;

        public PrinterService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ws = new WebServer(SendResponse, "http://localhost:9100/");
            ws.Run();
        }

        protected override void OnStop()
        {
            ws.Stop();
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            string printer_name = "";
            string print_data = "";
            string querystring = "";

            foreach (string key in request.QueryString.Keys)
            {
                var values = request.QueryString.GetValues(key);
                foreach (string value in values)
                {
                    //querystring += "{ \"key\": \"" + value + "\" }";
                    if (key == "p")
                    {
                        printer_name = value;
                    }
                    else if (key == "d")
                    {
                        print_data = value;
                    }
                }
            }

            if (printer_name.Length > 0)
            {
                //send data to printer
                bool success = RawPrinterHelper.SendStringToPrinter(printer_name, print_data);
                if (success)
                {
                    return print_data;
                }
                else
                {
                    return "ERROR SENDING DATA TO PRINTER";
                }
            }
            
            return GetPrinters();
        }

        private static string GetPrinters()
        {
            List<string> printers = new List<string>();
            ManagementScope objScope = new ManagementScope(ManagementPath.DefaultPath); //For the local Access
            objScope.Connect();

            SelectQuery selectQuery = new SelectQuery();
            selectQuery.QueryString = "Select * from win32_Printer";
            ManagementObjectSearcher MOS = new ManagementObjectSearcher(objScope, selectQuery);
            ManagementObjectCollection MOC = MOS.Get();
            foreach (ManagementObject mo in MOC)
            {
                //printers.Add(mo["Name"].ToString());
                printers.Add("{ \"Name\": \"" + mo["Name"].ToString() + "\" }");
            }

            return "[" + string.Join(",", printers) + "]";
        }
    }
}

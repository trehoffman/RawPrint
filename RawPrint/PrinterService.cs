using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RawPrint
{
    public partial class PrinterService : ServiceBase
    {
        public static string BASE_URL = ConfigurationManager.AppSettings["Base URL"];
        public static string PORT = ConfigurationManager.AppSettings["Port"];
        WebServer ws;

        public PrinterService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string baseUrl = BASE_URL + ":" + PORT + "/";
            ws = new WebServer(SendResponse, baseUrl);
            ws.Run();
        }

        protected override void OnStop()
        {
            ws.Stop();
        }

        public void onDebug()
        {
            OnStart(null);
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            try
            {
                string printer_name = "";
                string print_data = "";

                if (request.HttpMethod == "GET")
                {
                    foreach (string key in request.QueryString.Keys)
                    {
                        var values = request.QueryString.GetValues(key);
                        foreach (string value in values)
                        {
                            if (key == "p" || key == "printer")
                            {
                                printer_name = value;
                            }
                            else if (key == "d" || key == "data")
                            {
                                print_data = value;
                            }
                        }
                    }
                }
                if (request.HttpMethod == "POST")
                {
                    string json = new StreamReader(request.InputStream).ReadToEnd();
                    dynamic body = JsonConvert.DeserializeObject(json);
                    printer_name = (body.printer == null ? "" : (string)body.printer);
                    print_data = (body.data == null ? "" : (string)body.data);
                }

                if (printer_name.Length > 0)
                {
                    if (print_data.Length == 0) throw new Exception("NO PRINT DATA PROVIDED");
                    if (RawPrinterHelper.SendStringToPrinter(printer_name, print_data))
                    {
                        return JsonConvert.SerializeObject(new { printer = printer_name, data = print_data });
                    }
                    else
                    {
                        throw new Exception("ERROR SENDING DATA TO PRINTER");
                    }
                }

                return GetPrinters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }

        private static string GetPrinters()
        {
            try
            {
                var printers = new List<dynamic>();
                var objScope = new ManagementScope(ManagementPath.DefaultPath); //For the local Access
                objScope.Connect();

                var selectQuery = new SelectQuery();
                selectQuery.QueryString = "Select * from win32_Printer";
                var MOS = new ManagementObjectSearcher(objScope, selectQuery);
                var MOC = MOS.Get();
                foreach (var mo in MOC)
                {
                    printers.Add(new { Name = mo["Name"].ToString() });
                }

                return JsonConvert.SerializeObject(printers);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }
    }
}

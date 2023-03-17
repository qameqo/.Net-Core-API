using dotnetCore_API.Models;
using dotnetCore_API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace dotnetCore_API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var assemblyAll = Assembly.GetEntryAssembly().GetName();

            IndexModel model = new IndexModel();
            model.IP = GetIPAddress();
            model.AssemblyName = assemblyAll.Name;
            model.AssemblyVersion = assemblyAll.Version.ToString();
            model.Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            model.Modified = GetDateModified();
            return View(model);
        }

        private string GetIPAddress()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                return Dns.GetHostEntry(Dns.GetHostName()).HostName;
            }
            catch
            {
                return "";
            }
        }

        private string GetDateModified()
        {
            var assemblyExe = Assembly.GetExecutingAssembly();
            FileInfo file = new FileInfo(assemblyExe.Location);
            return file.LastWriteTime.ToString("dd/MM/yyyy : HH:mm:ss");
        }
    }
}

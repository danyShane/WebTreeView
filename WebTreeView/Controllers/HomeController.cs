using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using WebTreeView.Models;

namespace WebTreeView.Controllers
{
    public class HomeController : Controller
    {
        private List<string> lista = new List<string>();
        private string cadena = "";

        public IActionResult Index()
        {
            //obtenemos los datos del fichero de configuracion
            //var items = JArray.Parse(System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Items.json"));
            //var builder = new ConfigurationBuilder()
            //      .SetBasePath(Directory.GetCurrentDirectory())
            //      .AddJsonFile("Items.json", optional: true);

            //var items = builder.Build();
            string lines = "";
            object lockToread = new object();

            lock (lockToread)
            {
                FileStream fileStream = new FileStream("Items.json", FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    lines = reader.ReadToEnd();
                }                               
            }

            JToken jToken = JToken.Parse(lines);

            JToken[] padre = jToken.Children().ToArray();            

            load(padre);

            var lista2 = lista;

            int cont = jToken.Children().Count();                                        

            ViewData["items"] = cadena;
            return View();
            
        }

        private void load(JToken[] padre)
        {
            bool ul = false;

            if (padre.Count() > 0)
            {
                cadena = cadena + "<ul>";
                ul = true;
            }           
            
            foreach (var hijo in padre)
            {                
                if (null != hijo.SelectToken("Name") )
                {
                    cadena = cadena + "<li>" + hijo.SelectToken("Name").ToString() + "</li>";
                    lista.Add(hijo.SelectToken("Name").ToString());
                    load(hijo.SelectToken("Children").ToArray());
                }
                else
                {
                    load(hijo.Children().ToArray());
                }
            }
            if (ul) cadena = cadena + "</ul>";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 

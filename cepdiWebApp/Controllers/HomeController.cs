using cepdiWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace cepdiWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string URL = "https://localhost:5001/api/v1/";
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Login");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Sesiones/ValidarSesion");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (!response.IsSuccessStatusCode && !response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                return RedirectToAction("Index", "Login");


            return View();
        }

        public IActionResult Privacy()
        {
            

            return View();
        }

        public IActionResult Medicamentos(string sortOrder, string nombre, string presentacion, string concentracion)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Medicamentos");

            // Add an Accept header for JSON format.
            /*client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));*/
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc3VhcmlvIjoiMDM4MiIsIkNhZHVjaWRhZCI6IjAyLzIyLzIwMjMgMDg6NTI6MzEiLCJuYmYiOjE2NzY3OTY3NTEsImV4cCI6MTY3NzA1NTk1MSwiaWF0IjoxNjc2Nzk2NzUxfQ.8CWotAX_opO5p_YoKKBPeNWVgi8D0fDADGNi-YUKNsA");
            string urlParameters = "?pageSize=10&pageNumber=1";
            urlParameters += !string.IsNullOrEmpty(nombre) ? $"&nombre={nombre}" : string.Empty;
            urlParameters += !string.IsNullOrEmpty(presentacion) ? $"&presentacion={presentacion}" : string.Empty;
            urlParameters += !string.IsNullOrEmpty(concentracion) ? $"&concentracion={concentracion}" : string.Empty;


            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                // Parse the response body.
                //var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                var datos = JsonConvert.DeserializeObject<List<Models.Medicamento>>(dataObjects);


                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                var lista = from d in datos
                               select d;
                switch (sortOrder)
                {
                    case "name_desc":
                        lista = lista.OrderByDescending(l => l.NOMBRE);
                        break;
                        /*case "Date":
                            lista = lista.OrderBy(l => l.EnrollmentDate);
                            break;
                        case "date_desc":
                            lista = lista.OrderByDescending(l => l.EnrollmentDate);
                            break;
                        default:
                            lista = lista.OrderBy(l => l.LastName);
                            break;*/
                }
                return View(lista.ToList());

            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            // Make any other calls using HttpClient here.

            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
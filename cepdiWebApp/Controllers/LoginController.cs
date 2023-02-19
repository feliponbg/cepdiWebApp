using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Security.Principal;
using cepdiWebApp.Models.ViewModels;

namespace cepdiWebApp.Controllers
{
    public class LoginController : Controller
    {

        private const string URL = "https://localhost:5001/api/v1/";

        //[HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Entrar(Models.ViewModels.Sesion model)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Sesiones/IniciarSesion");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            var credencial = new
            {
                usuario = model.Usuario,
                contraseña = model.Contraseña,
                mantenerSesionIniciada = model.MantenerSesionIniciada
            };

            string json = JsonConvert.SerializeObject(credencial);

            // List data response.
            HttpResponseMessage response = client.PostAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                var datos = JsonConvert.DeserializeObject<Models.Sesion>(dataObjects);


                HttpContext.Session.SetString("Usuario", datos.Usuario);
                HttpContext.Session.SetString("Token", datos.Token);

            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return RedirectToAction("Index", "Home");
        }


    }
}

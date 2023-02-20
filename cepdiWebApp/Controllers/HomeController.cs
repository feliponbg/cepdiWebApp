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
        private const string URL = "https://localhost:44383/api/v1/";
        

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
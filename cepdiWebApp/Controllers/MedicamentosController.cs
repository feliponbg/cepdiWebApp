using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace cepdiWebApp.Controllers
{



    public class MedicamentosController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string URL = "https://localhost:44383/api/v1/";

        private async Task<bool> ValidarSesion()
        {
            string token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return false;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Sesiones/ValidarSesion");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (!response.IsSuccessStatusCode && !response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                return false;

            return true;
        }

        public async Task<IActionResult> Index(/*int pageSize, */int pageNumber, string sortOrder, string nombre, string presentacion, string concentracion)
        {
            bool validacion = await ValidarSesion();
            if (!validacion)
                return RedirectToAction("Index", "Login");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Medicamentos");
            string token = HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string urlParameters = $"?pageSize=5&pageNumber={(pageNumber > 0 ? pageNumber.ToString() : "1")}";
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


                //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                var lista = from d in datos
                            select d;
                /*switch (sortOrder)
                {
                    case "name_desc":
                        lista = lista.OrderByDescending(l => l.NOMBRE);
                        break;
                    case "Date":
                        lista = lista.OrderBy(l => l.EnrollmentDate);
                        break;
                    case "date_desc":
                        lista = lista.OrderByDescending(l => l.EnrollmentDate);
                        break;
                    default:
                        lista = lista.OrderBy(l => l.LastName);
                        break;
                }*/

                IList<int> paginas = new List<int>();
                string tamañoPagina = "1";
                if (response.Headers.Contains("PageSize"))
                    tamañoPagina = response.Headers.GetValues("PageSize").First();

                string totalRegistros = "1";
                if (response.Headers.Contains("TotalRecords"))
                    totalRegistros = response.Headers.GetValues("TotalRecords").First();
                int totalPaginas = Convert.ToInt32(totalRegistros) / Convert.ToInt32(tamañoPagina);

                float resultadoModulo = (Convert.ToSingle(totalRegistros) / Convert.ToSingle(tamañoPagina)) % (Convert.ToInt32(totalRegistros) / Convert.ToInt32(tamañoPagina));

                if (resultadoModulo > 0)
                    totalPaginas++;

                for (int i = 0; i < totalPaginas; i++)
                    paginas.Add(i + 1);
                ViewBag.Paginacion = paginas;
                ViewBag.PaginaActual = pageNumber > 0 ? pageNumber : 1;
                ViewBag.TotalPaginas = Convert.ToInt32(totalPaginas + 1);
                ViewBag.TotalRegistros = Convert.ToInt32(totalRegistros);
                return View(lista.ToList());

            }
            else if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent))
            {
                IEnumerable<Models.Medicamento> listaVacia = new List<Models.Medicamento>();
                ViewBag.Paginacion = new List<int>();
                return View(listaVacia);
            }

            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            return View();
        }


        public async Task<IActionResult> Details(long? id)
        {

            string token = HttpContext.Session.GetString("Token");
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Medicamentos/{id}");

            // Add an Accept header for JSON format.
            //client.DefaultRequestHeaders.Accept.Add(
            //new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            Models.ViewModels.Medicamento datos = null;
            if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                datos = JsonConvert.DeserializeObject<Models.ViewModels.Medicamento>(dataObjects);

            }

            int selectedId = (int)datos.IIDFORMAFARMACEUTICA;


            ViewBag.FormasFarmaceuticas = await ObtenerListaFF(seleccionado: (int)datos.IIDFORMAFARMACEUTICA);

            return View(datos);
        }

        private async Task<IList<SelectListItem>> ObtenerListaFF(int seleccionado)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}FormasFarmaceuticas");
            string token = HttpContext.Session.GetString("Token");

            // Add an Accept header for JSON format.
            //client.DefaultRequestHeaders.Accept.Add(
            //new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                var datos = JsonConvert.DeserializeObject<List<Models.FormaFarmaceutica>>(dataObjects);


                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var item in datos)
                {
                    if (seleccionado > 0 && item.IIDFORMAFARMACEUTICA == seleccionado)
                        items.Add(new SelectListItem { Text = item.NOMBRE, Value = item.IIDFORMAFARMACEUTICA.ToString(), Selected = true });
                    else
                        items.Add(new SelectListItem { Text = item.NOMBRE, Value = item.IIDFORMAFARMACEUTICA.ToString() });
                }

                /*items.Add(new SelectListItem { Text = "Action", Value = "0" });

                items.Add(new SelectListItem { Text = "Drama", Value = "1" });

                items.Add(new SelectListItem { Text = "Comedy", Value = "2", Selected = true });

                items.Add(new SelectListItem { Text = "Science Fiction", Value = "3" });*/
                return items;
            }
            return null;
        }

        public async Task<IActionResult> Create(Models.ViewModels.Medicamento medicamento)
        {
            ViewBag.FormasFarmaceuticas = await ObtenerListaFF(-1);

            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri($"{URL}Medicamentos");
                    string token = HttpContext.Session.GetString("Token");

                    medicamento.IIDFORMAFARMACEUTICA = Convert.ToInt32(Request.Form["FormasFarmaceuticas"].ToString());

                    // Add an Accept header for JSON format.
                    //client.DefaultRequestHeaders.Accept.Add(
                    //new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(medicamento);

                    // List data response.
                    HttpResponseMessage response = client.PostAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                    if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.Created))
                    {
                        // Parse the response body.
                        var dataObjects = response.Content.ReadAsStringAsync().Result;
                        var datos = JsonConvert.DeserializeObject<Models.Sesion>(dataObjects);

                    }
                    else
                    {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (DataException)// dex )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View();
        }

        // GET: Medicamentos/Edit/5
        public async Task<IActionResult> Edit(int id, Models.ViewModels.Medicamento medicamento)
        {
            string token = HttpContext.Session.GetString("Token");
            try
            {
                if (ModelState.IsValid)
                {
                    HttpClient client2 = new HttpClient();
                    client2.BaseAddress = new Uri($"{URL}Medicamentos");


                    // Add an Accept header for JSON format.
                    //client.DefaultRequestHeaders.Accept.Add(
                    //new MediaTypeWithQualityHeaderValue("application/json"));
                    medicamento.IIDFORMAFARMACEUTICA = Convert.ToInt32(Request.Form["FormasFarmaceuticas"].ToString());

                    client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(medicamento);

                    // List data response.
                    HttpResponseMessage response2 = client2.PutAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                    if (response2.IsSuccessStatusCode && response2.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                    {
                        // Parse the response body.
                        var dataObjects = response2.Content.ReadAsStringAsync().Result;
                        var datos2 = JsonConvert.DeserializeObject<Models.Sesion>(dataObjects);


                        /*HttpContext.Session.SetString("Usuario", datos2.Usuario);
                        HttpContext.Session.SetString("Token", datos2.Token);*/

                    }

                    return RedirectToAction("Index");
                }
            }
            catch (DataException)// dex )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Medicamentos/{id}");

            // Add an Accept header for JSON format.
            //client.DefaultRequestHeaders.Accept.Add(
            //new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            Models.ViewModels.Medicamento datos = null;
            if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                datos = JsonConvert.DeserializeObject<Models.ViewModels.Medicamento>(dataObjects);

            }

            int selectedId = (int)datos.IIDFORMAFARMACEUTICA;


            ViewBag.FormasFarmaceuticas = await ObtenerListaFF(seleccionado: (int)datos.IIDFORMAFARMACEUTICA);

            return View(datos);


        }

        // GET: Medicamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string token = HttpContext.Session.GetString("Token");
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Medicamentos/{id}");

            // Add an Accept header for JSON format.
            //client.DefaultRequestHeaders.Accept.Add(
            //new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            Models.ViewModels.Medicamento datos = null;
            if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                datos = JsonConvert.DeserializeObject<Models.ViewModels.Medicamento>(dataObjects);

            }

            int selectedId = (int)datos.IIDFORMAFARMACEUTICA;


            ViewBag.FormasFarmaceuticas = await ObtenerListaFF(seleccionado: (int)datos.IIDFORMAFARMACEUTICA);

            return View(datos);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string token = HttpContext.Session.GetString("Token");
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{URL}Medicamentos/{id}");

            // Add an Accept header for JSON format.
            //client.DefaultRequestHeaders.Accept.Add(
            //new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // List data response.
            HttpResponseMessage response = client.DeleteAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            Models.ViewModels.Medicamento datos = null;
            if (response.IsSuccessStatusCode && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                datos = JsonConvert.DeserializeObject<Models.ViewModels.Medicamento>(dataObjects);

            }

            return RedirectToAction(nameof(Index));
        }


    }

}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Net;
using System.Net.Http.Headers;

namespace cepdiWebApp.Controllers
{



    public class MedicamentosController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string URL = "https://localhost:5001/api/v1/";

        public IActionResult Index(string sortOrder, string nombre, string presentacion, string concentracion)
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


        /*public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }*/

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)// dex )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }*/

        /*[HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException)// dex )
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }*/

        /*public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }*/

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (DataException)// dex )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }*/

    }

}

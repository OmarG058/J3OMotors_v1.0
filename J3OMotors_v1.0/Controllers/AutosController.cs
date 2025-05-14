using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using J3OMotors_v1._0.Models.Autos;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace J3OMotors_v1._0.Controllers
{
    public class AutosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        public AutosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        public  IActionResult Index()
        {
          return View();
        }


        // Metodo para mostrar la vista del formulario de creación de autos
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Envío del formulario para agregar un nuevo auto
        [HttpPost]
        public async Task<IActionResult> Create(AutosViewModel auto, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("archivo", "Debes seleccionar una imagen.");
                return View(auto);
            }

            var cliente = _httpClient.CreateClient();
            var form = new MultipartFormDataContent();

            // Agregar otros campos del formulario
            form.Add(new StringContent(auto.Modelo), "Modelo");
            form.Add(new StringContent(auto.FechaModelo), "FechaModelo");
            form.Add(new StringContent(auto.Costo.ToString()), "Costo");
            form.Add(new StringContent(auto.Fabricante), "Fabricante");
            form.Add(new StringContent(auto.NumeroSerie), "NumeroSerie");

            // Crear el contenido del archivo sin usar 'using' para que el Stream no se cierre antes
            var stream = archivo.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(archivo.ContentType);
            form.Add(fileContent, "archivo", archivo.FileName);

            try
            {
                // Realizar el POST a la API
                var response = await cliente.PostAsync("https://localhost:7174/api/autos", form);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index"); // Redirige al listado de autos
                }
                else
                {
                    ModelState.AddModelError("", "Error al crear el auto.");
                    return View(auto);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de que falle el HTTP request
                ModelState.AddModelError("", $"Error al procesar la solicitud: {ex.Message}");
                return View(auto);
            }
        }
    }
}
    


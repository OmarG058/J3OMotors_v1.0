using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using J3OMotors_v1._0.Models.Autos;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ComTypes;



namespace J3OMotors_v1._0.Controllers
{
    public class AutosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        public AutosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
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

        // Envío del formulario para agregar un nuevo auto
        [HttpGet]
        public async Task<IActionResult> Index() //ENTIENDO QUE NO INGRESO CON UN CAMPO SI NO QUE LO MANDO A LLAMAR Y DESEREALIZAR A LA VEZ MAS ABAJO *duda para omar jeje*
        {
            var cliente = _httpClient.CreateClient();
            // Realizar la solicitud HTTP GET
            var response = await cliente.GetAsync("https://localhost:7174/api/autos");

            if (response.IsSuccessStatusCode)
            {
                // Leer y deserializar los datos de la API
                var jsonData = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<List<AutosViewModel>>(jsonData);

                // Pasar los datos a la vista
                return View(model);
            }

            // Manejar errores
            return View(new List<AutosViewModel>());
        }
        //para hacer el metodo de edit ocupo un get para mostrar el formulario y un post que hace los cambios (el put solo es para la api)
        // get solo para mostrar cuando presiono un boton, y el post para yo como usuario enviar algo 


        [HttpGet]
        public IActionResult Edit(string id)
        {
            var cliente = _httpClient.CreateClient();
            // Realizar la solicitud HTTP GET
            var response = cliente.GetAsync($"https://localhost:7174/api/autos/{id}");
            if (response.Result.IsSuccessStatusCode)
            {
                // Leer y deserializar los datos de la API
                var jsonData = response.Result.Content.ReadAsStringAsync().Result;
                var model = JsonConvert.DeserializeObject<AutosViewModel>(jsonData);

                // Pasar los datos a la vista
                return View(model);
            }
            return RedirectToAction("TablaAutos","Autos"); // Redirige al listado de autos si no se encuentra el auto

        }


        [HttpPost]
        public async Task<IActionResult> Edit(AutosViewModel auto, IFormFile archivo, string Id)
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
                var response = await cliente.PutAsync($"https://localhost:7174/api/autos/{auto.Id}", form);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("TablaAutos"); // Redirige al listado de autos
                }
                else
                {
                    ModelState.AddModelError("", "Error al modificar el auto.");
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

        [HttpGet]
        public async Task<IActionResult> TablaAutos() 
        {
            var cliente = _httpClient.CreateClient();
            // Realizar la solicitud HTTP GET
            var response = await cliente.GetAsync("https://localhost:7174/api/autos");

            if (response.IsSuccessStatusCode)
            {
                // Leer y deserializar los datos de la API
                var jsonData = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<List<AutosViewModel>>(jsonData);

                // Pasar los datos a la vista
                return View(model);
            }

            // Manejar errores
            return View(new List<AutosViewModel>());
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var cliente = _httpClient.CreateClient();
            // Realizar la solicitud HTTP GET
            var response = cliente.GetAsync($"https://localhost:7174/api/autos/{id}");
            if (response.Result.IsSuccessStatusCode)
            {
                // Leer y deserializar los datos de la API
                var jsonData = response.Result.Content.ReadAsStringAsync().Result;
                var model = JsonConvert.DeserializeObject<AutosViewModel>(jsonData);

                // Pasar los datos a la vista
                return View(model);
            }
            return RedirectToAction("TablaAutos", "Autos"); // Redirige al listado de autos si no se encuentra el auto
        }


        [HttpPost]
        public async Task<IActionResult> Delete(AutosViewModel auto)
        {

            var cliente = _httpClient.CreateClient();
            try
            {
                // Realizar el POST a la API
                var response = await cliente.DeleteAsync($"https://localhost:7174/api/autos/{auto.Id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("TablaAutos"); // Redirige al listado de autos
                }
                else
                {
                    ModelState.AddModelError("", "Error al eliminar el auto.");
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
    


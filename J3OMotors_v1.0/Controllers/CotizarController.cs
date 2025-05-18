using J3OMotors_v1._0.Helpers.Rutas;
using J3OMotors_v1._0.Models;
using J3OMotors_v1._0.Models.Autos;
using J3OMotors_v1._0.Models.Cliente;
using J3OMotors_v1._0.Models.Cotizar;
using J3OMotors_v1._0.Models.Seguro;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace J3OMotors_v1._0.Controllers
{
    public class CotizarController : Controller
    {
        private readonly HttpClient _httpClient;
        public CotizarController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create(string idAuto)
        {
            //validar que este logueado
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //usuario logueado
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            //hacemos consulta a la api para verificar que exista el cliente con su id de usuario
            var clienteResponse = await _httpClient.GetAsync(Routes.UrlBaseApiCliente + $"/Usuario/{idUsuario}");

            if (!clienteResponse.IsSuccessStatusCode) return RedirectToAction("Create", "Cliente");

            //Obtener imagen del auto a cotizar
            // Obtener el auto desde la API
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://localhost:7174/api/autos/{idAuto}");

            if (!response.IsSuccessStatusCode)
            {
                // Manejo de error si no encuentra el auto
                return NotFound();
            }

            var contenido = await response.Content.ReadAsStringAsync();
            var auto = JsonConvert.DeserializeObject<AutosViewModel>(contenido);

            // Pasar la imagen al ViewBag
            ViewBag.ImagenAuto = auto.RutaImagen;         


            if (clienteResponse.IsSuccessStatusCode)
            {
                var content = await clienteResponse.Content.ReadAsStringAsync();

                var cliente = JsonConvert.DeserializeObject<ClienteViewModel>(content);

                int? idCliente = cliente.IdCliente;

                // Ahora puedes usar idCliente para lo que necesites
                // Obtener accesorios por tipo (Asiento, Color, Rin) desde API accesorios
                var asientosResponse = await _httpClient.GetAsync("https://localhost:7174/api/accesorios/nombre/Asientos");
                var coloresResponse = await _httpClient.GetAsync("https://localhost:7174/api/accesorios/nombre/Color");
                var rinesResponse = await _httpClient.GetAsync("https://localhost:7174/api/accesorios/nombre/Rines");

                var asientos = await asientosResponse.Content.ReadFromJsonAsync<List<AccesorioViewModel>>();
                var colores = await coloresResponse.Content.ReadFromJsonAsync<List<AccesorioViewModel>>();
                var rines = await rinesResponse.Content.ReadFromJsonAsync<List<AccesorioViewModel>>();

                // Obtener seguros desde API seguros (si tienes)
                var segurosResponse = await _httpClient.GetAsync("https://localhost:7174/api/seguros");
                var seguros = await segurosResponse.Content.ReadFromJsonAsync<List<SeguroAutoViewModel>>();

                // Pasar al ViewBag para que la vista tenga las listas para selects
                ViewBag.Asientos = asientos;
                ViewBag.Colores = colores;
                ViewBag.Rines = rines;
                ViewBag.Seguros = seguros;

                // TipoPago y Plazos está fijo, se puede pasar como lista también
                ViewBag.TiposPago = new List<string> { "Contado", "Credito" };
                ViewBag.Plazos = new List<int> { 12, 24, 36 };

                var model = new CotizacionViewModel
                {
                    IdAuto = idAuto,
                    IdCliente = idCliente.Value,
                    Fecha = DateTime.Now
                };

                return View(model);
            }
            else
            {
               
                ModelState.AddModelError(string.Empty, "Error al obtener los datos del cliente.");
                return RedirectToAction("Index", "Login");
            }   

        }
        [HttpPost]
        public async Task<IActionResult> Create(CotizacionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Si hay errores, recargar los datos necesarios para los selects y volver a mostrar la vista
                await CargarListasAsync(model.IdCliente); // Método que carga ViewBags (te lo muestro abajo)
                return View(model);
            }

            // Aquí llamas a tu API para guardar la cotización (puedes usar _httpClient.PostAsJsonAsync o similar)
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7174/api/Cotizaciones", model);

            if (response.IsSuccessStatusCode)
            {
                // Si guardó bien, rediriges a otra vista o acción (por ejemplo, a la lista de cotizaciones)
                return RedirectToAction("ListarCotizacionesByCliente", "Cotizar");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al guardar la cotización.");
                await CargarListasAsync(model.IdCliente);
                return View(model);
            }
        }

        // Método auxiliar para cargar las listas necesarias en ViewBag
        private async Task CargarListasAsync(int idCliente)
        {
            var asientosResponse = await _httpClient.GetAsync("https://localhost:7174/api/accesorios/nombre/Asientos");
            var coloresResponse = await _httpClient.GetAsync("https://localhost:7174/api/accesorios/nombre/Color");
            var rinesResponse = await _httpClient.GetAsync("https://localhost:7174/api/accesorios/nombre/Rines");
            var segurosResponse = await _httpClient.GetAsync("https://localhost:7174/api/seguros");

            ViewBag.Asientos = await asientosResponse.Content.ReadFromJsonAsync<List<AccesorioViewModel>>();
            ViewBag.Colores = await coloresResponse.Content.ReadFromJsonAsync<List<AccesorioViewModel>>();
            ViewBag.Rines = await rinesResponse.Content.ReadFromJsonAsync<List<AccesorioViewModel>>();
            ViewBag.Seguros = await segurosResponse.Content.ReadFromJsonAsync<List<SeguroAutoViewModel>>();
            ViewBag.TiposPago = new List<string> { "Contado", "Credito" };
            ViewBag.Plazos = new List<int> { 12, 24, 36 };
        }

        //Metodo para lsitar las cotizacion de un cliente en espesifico
        [HttpGet]
        public async Task<IActionResult> ListarCotizacionesByCliente()
        {
            // Validar que esté logueado
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            // Consultar cliente por ID de usuario
            var clienteResponse = await _httpClient.GetAsync(Routes.UrlBaseApiCliente + $"/Usuario/{idUsuario}");
            if (!clienteResponse.IsSuccessStatusCode) return RedirectToAction("Create", "Cliente");

            var content = await clienteResponse.Content.ReadAsStringAsync();
            var cliente = JsonConvert.DeserializeObject<ClienteViewModel>(content);
            int? idCliente = cliente?.IdCliente;

            // Consultar cotizaciones
            var cotizacionesResponse = await _httpClient.GetAsync(Routes.UrlCotizacionDTO + idCliente);

            if (!cotizacionesResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error al obtener las cotizaciones.");
                return View(new List<CotizacionDTO>());
            }

            var cotizaciones = await cotizacionesResponse.Content.ReadFromJsonAsync<List<CotizacionDTO>>();

            return View("ListarCotizacionesByCliente", cotizaciones);

        }

        //Metodo para eliminar una cotizacion
        [HttpPost]
        public async Task<IActionResult> EliminarCotizacion(int idCotizacion)
        {
            // Validar que esté logueado
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Llamar a la API para eliminar la cotización
            var response = await _httpClient.DeleteAsync(Routes.UrlBaseApiCotizacion+$"/{idCotizacion}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListarCotizacionesByCliente");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar la cotización.");
                return RedirectToAction("ListarCotizacionesByCliente");
            }
        }


    }

}

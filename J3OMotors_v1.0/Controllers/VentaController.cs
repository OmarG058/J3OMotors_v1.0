using J3OMotors_v1._0.Helpers.Rutas;
using J3OMotors_v1._0.Models.Cliente;
using J3OMotors_v1._0.Models.Cotizar;
using J3OMotors_v1._0.Models.Venta;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace J3OMotors_v1._0.Controllers
{
    public class VentaController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public VentaController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //listar compras del cliente
        public async Task<IActionResult> ListarComprasCliente()
        {
            // Validación de sesión
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("Index", "Login");

            // Consultar el cliente con el id de usuario
            var _httpClient = clientFactory.CreateClient();
            var response = await _httpClient.GetAsync(Routes.UrlBaseApiCliente + $"/Usuario/{idUsuario}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener el cliente.";
                return RedirectToAction("Create", "Cliente");
            }
            var cliente = JsonConvert.DeserializeObject<ClienteViewModel>(await response.Content.ReadAsStringAsync());

            
            //consultar las compras con el id del cliente   
            var comprasResponse = await _httpClient.GetAsync(Routes.UrlBaseApiVentas + $"/Cliente/{cliente.IdCliente}");


            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener la lista de compras.";
                return RedirectToAction("Index", "Home");
            }

            var jsonResponse = await comprasResponse.Content.ReadAsStringAsync();
            var ventas = JsonConvert.DeserializeObject<List<Venta>>(jsonResponse);

            return View(ventas);
        }



        [HttpPost]
        public async Task<IActionResult> CreateVenta(int idCotizacion)
        {
            // Validación de sesión
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("Index", "Login");

            // Consultar la cotización desde la API
            var _httpClient = clientFactory.CreateClient();
            var cotizacionResponse = await _httpClient.GetAsync(Routes.UrlBaseApiCotizacion+$"/{idCotizacion}");
            if (!cotizacionResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener la cotización.";
                return RedirectToAction("ListarCotizacionesByCliente", "Cotizar");
            }

            var cotizacionJson = await cotizacionResponse.Content.ReadAsStringAsync();
            var cotizacion = JsonConvert.DeserializeObject<CotizacionViewModel>(cotizacionJson);

            // Aquí creas el objeto Venta
            var nuevaVenta = new CrearVentaRequest
            {
                IdCotizacion = idCotizacion
            };

            // Consumir tu API para registrar la venta
            var ventaResponse = await _httpClient.PostAsJsonAsync(Routes.UrlBaseApiVentas + $"/nuevo",nuevaVenta);

            if (!ventaResponse.IsSuccessStatusCode)
            {
                var errorMessage = await ventaResponse.Content.ReadAsStringAsync();

                // Validar si ya existe la venta
                if (ventaResponse.StatusCode == HttpStatusCode.BadRequest &&
                    errorMessage.Contains("Ya existe una venta para esta cotización."))
                {
                    TempData["Warning"] = "Ya has generado una venta con esta cotización.";
                }
                else
                {
                    TempData["Error"] = "Error al registrar la venta.";
                }

                return RedirectToAction("ListarCotizacionesByCliente", "Cotizar");
            }

            TempData["Success"] = "¡Venta generada correctamente!";
            return RedirectToAction("ListarCotizacionesByCliente", "Cotizar");
        }
    }
}

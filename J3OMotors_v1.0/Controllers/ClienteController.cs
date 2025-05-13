using J3OMotors_v1._0.Helpers.Rutas;
using J3OMotors_v1._0.Models.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using System.Text;

namespace J3OMotors_v1._0.Controllers
{
    public class ClienteController : Controller
    {
        private readonly HttpClient _httpClient;
        public ClienteController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: Cliente/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Obtener idUsuario desde la sesión
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var nuevoCliente = new ClienteViewModel
            {
                IdUsuario = idUsuario.Value
            };

            return View(nuevoCliente);
        }

        // POST: Cliente/Create
        [HttpPost]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(Routes.UrlBaseApiCliente, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Perfil");
            }

            ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los datos.");
            return View(model);
        }
    }




}


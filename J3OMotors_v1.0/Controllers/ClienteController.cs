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
                return RedirectToAction("Index", "Home"); //redirigirlo a una pagina de no autorizado cuando acceda al ruta sin su id de usuario
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


        //mostrar el formulario de editar del usuario
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Validar que el usuario esté logueado
            var idUsuarioSesion = HttpContext.Session.GetInt32("IdUsuario");
            var tipoUsuario = HttpContext.Session.GetInt32("TipoUsuario"); // 1 = admin, 2 = cliente

            if (idUsuarioSesion == null || tipoUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            //// Validar acceso: si es cliente solo puede editar su propio perfil
            //if (tipoUsuario == 2 && idUsuarioSesion != id)
            //{
            //    return Forbid(); // o puedes redirigir a una vista personalizada de acceso denegado
            //}

            // Validar que el id sea válido
            if (id <= 0)
            {
                TempData["Error"] = "ID inválido.";
                return RedirectToAction("Index", "Perfil");
            }

            try
            {
                var response = await _httpClient.GetAsync(Routes.ClientePorId(id));

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudieron obtener los datos del cliente.";
                    return RedirectToAction("Index", "Perfil");
                }

                var cliente = await response.Content.ReadFromJsonAsync<ClienteViewModel>();

                if (cliente == null)
                {
                    TempData["Error"] = "El cliente no fue encontrado.";
                    return RedirectToAction("Index", "Perfil");
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al procesar la solicitud.";
                // Puedes registrar el error en logs si usas ILogger
                return RedirectToAction("Index", "Perfil");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClienteViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{Routes.ActualizarClientePorId((int)model.IdCliente)}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Los datos se actualizaron correctamente.";
                return RedirectToAction("Index", "Perfil"); // o donde muestres la lista
            }

            ModelState.AddModelError(string.Empty, "Error al actualizar el cliente.");
            return View(model);
        }
        //------------------------------------------------   ESTE ES TU ESPACIO JORGE PARA QUE HAGAS TUS CONTROLADORES --------------------
        //IndexTablaClientes te muestra la tabla de clientes 
        [HttpGet]
        public async Task<IActionResult> IndexTablaClientes()
        {
            return View();
        }

        


    }

}


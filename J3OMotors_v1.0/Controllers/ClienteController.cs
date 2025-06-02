using J3OMotors_v1._0.Helpers.Rutas;
using J3OMotors_v1._0.Models.Cliente;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Net;

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
            var tipoUsuario = HttpContext.Session.GetInt32("TipoUsuario");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(Routes.UrlBaseApiCliente, content);

            if (response.IsSuccessStatusCode)
            {
                if(tipoUsuario == 1)
                {
                    return RedirectToAction("IndexTablaClientes", "Cliente");
                }
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
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al procesar la solicitud.";
                return RedirectToAction("Index", "Perfil");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClienteViewModel model)
        {
            var tipoUsuario = HttpContext.Session.GetInt32("TipoUsuario");
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{Routes.ActualizarClientePorId((int)model.IdCliente)}", content);

            if (response.IsSuccessStatusCode)
            {
                if (tipoUsuario == 1)
                {
                    TempData["SuccessMessage"] = "Los datos se actualizaron correctamente.";
                    TempData["DesdeEdit"] = true;
                    return RedirectToAction("IndexTablaClientes", "Cliente");
                  
                }
                TempData["SuccessMessage"] = "Los datos se actualizaron correctamente.";
                TempData["DesdeEdit"] = true;
                return RedirectToAction("Index", "Perfil");
               

            }

            ModelState.AddModelError(string.Empty, "Error al actualizar el cliente.");
            return View(model);
        }

        //------------------------------------------------   ESTE ES TU ESPACIO JORGE PARA QUE HAGAS TUS CONTROLADORES --------------------



        // GET: Buscar cliente por nombre y apellidos
        [HttpGet]
        public async Task<IActionResult> BuscarClientePorNombreYApellido(string nombre, string apellidos)
        {
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos))
            {
                TempData["Error"] = "Debe proporcionar nombre y apellidos para la búsqueda.";
                return RedirectToAction("IndexTablaClientes");
            }

            try
            {
                var url = Routes.BuscarClientePorNombreYApellido(nombre, apellidos);
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se encontraron clientes con esos datos.";
                    return RedirectToAction("IndexTablaClientes");
                }

                var clientes = await response.Content.ReadFromJsonAsync<List<ClienteViewModel>>();
                return View("IndexTablaClientes", clientes);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al buscar los clientes.";
                return RedirectToAction("IndexTablaClientes");
            }
        }
        //buscar cliente por id usuario
        [HttpGet]
        public async Task<IActionResult> BuscarClientePorIdUsuario(int idUsuario)
        {
            try
            {
                var urlCliente = Routes.BuscarClientePorIdUsuario(idUsuario);
                var responseCliente = await _httpClient.GetAsync(urlCliente);

                if (!responseCliente.IsSuccessStatusCode)
                {
                    // Verifica si es 404 para dar un mensaje más específico
                    if (responseCliente.StatusCode == HttpStatusCode.NotFound)
                    {
                        TempData["Error"] = $"No se encontró un cliente con el ID de usuario {idUsuario}.";
                    }
                    else
                    {
                        TempData["Error"] = "Error al buscar el cliente por ID de usuario.";
                    }

                    return RedirectToAction("IndexTablaClientes");
                }

                var clienteBuscado = await responseCliente.Content.ReadFromJsonAsync<ClienteViewModel>();

                if (clienteBuscado == null || clienteBuscado.IdCliente == 0)
                {
                    TempData["Error"] = "El cliente no fue encontrado o es inválido.";
                    return RedirectToAction("IndexTablaClientes");
                }

                // Cargar la lista completa de clientes
                var responseLista = await _httpClient.GetAsync(Routes.UrlBaseApiCliente);
                var clientes = new List<ClienteViewModel>();

                if (responseLista.IsSuccessStatusCode)
                {
                    clientes = await responseLista.Content.ReadFromJsonAsync<List<ClienteViewModel>>();
                }

                // Pasar el cliente encontrado para destacarlo
                ViewBag.ClienteBuscado = clienteBuscado;
                return View("IndexTablaClientes", clientes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction("IndexTablaClientes");
            }
        }



        // POST: Eliminar cliente por id
        [HttpPost]
        public async Task<IActionResult> EliminarClientePorId(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID inválido para eliminar.";
                return RedirectToAction("IndexTablaClientes");
            }

            try
            {
                var url = Routes.EliminarClientePorId(id);
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cliente eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "Error al eliminar el cliente.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al eliminar el cliente.";
            }

            return RedirectToAction("IndexTablaClientes");
        }

        //IndexTablaClientes te muestra la tabla de clientes 
        [HttpGet]
        public async Task<IActionResult> IndexTablaClientes()
        {
            try
            {
                var response = await _httpClient.GetAsync(Routes.UrlBaseApiCliente); // URL base de la API para obtener clientes
                if (response.IsSuccessStatusCode)
                {
                    var clientes = await response.Content.ReadFromJsonAsync<List<ClienteViewModel>>();
                    return View(clientes);  // PASAMOS la lista de clientes a la vista
                }
                else
                {
                    TempData["Error"] = "No se pudieron cargar los clientes desde la API.";
                    return View(new List<ClienteViewModel>()); // Para evitar error, enviamos lista vacía
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al conectar con el servidor.";
                return View(new List<ClienteViewModel>());
            }
        }
    }
}


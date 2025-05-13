using J3OMotors_v1._0.Helpers.Rutas;
using J3OMotors_v1._0.Models.Cliente;
using J3OMotors_v1._0.Models.Perfil;
using J3OMotors_v1._0.Models.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace J3OMotors_v1._0.Controllers
{
    public class PerfilController : Controller
    {
        private readonly HttpClient _httpClient;
        public PerfilController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtener idUsuario desde sesión
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            // Llamar a la API para obtener los datos
            var usuario = await _httpClient.GetFromJsonAsync<UsuarioCreateViewModel>(Routes.UsuarioPorId((int) idUsuario));

            var response = await _httpClient.GetAsync(Routes.BuscarClientePorIdUsuario((int)idUsuario));

            ClienteViewModel cliente = null;

            if (response.IsSuccessStatusCode) 
            {
                cliente = await response.Content.ReadFromJsonAsync<ClienteViewModel>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Cliente no encontrado: esto es lo esperado si aún no se ha registrado como cliente
                return RedirectToAction("Create", "Cliente");
            }


            //if (cliente == null)
            //{
            //    // Redirigir a completar los datos si no hay cliente
            //    return RedirectToAction("Home", "Index");//, new { idUsuario = idUsuario.Value });
            //}

            var perfilVM = new PerfilViewModel
            {
                Usuario = usuario,
                Cliente = cliente
            };

            return View(perfilVM);
        }
    }
}

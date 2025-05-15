using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using J3OMotors_v1._0.Models.Usuario;
using System.Text.Json; 
using System.Threading.Tasks;
using System.Net.Http;
using J3OMotors_v1._0.Helpers.Rutas;


namespace J3OMotors_v1._0.Controllers
{
    public class UsuarioController : Controller  
    {
        private readonly HttpClient _httpClient;

        public UsuarioController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var ruta = Routes.UrlBaseApiUsuarioRegister;
            var response = await _httpClient.PostAsync(ruta, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["RegistroExitoso"] = "Cuenta creada exitosamente. Ahora puedes iniciar sesión.";
                return RedirectToAction("Index", "Login");
            } // Redirige al login tras registrarse

            ModelState.AddModelError("", "Error al registrar usuario.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction("Index", "Perfil");
            }

            // Llamada a la API para obtener el usuario por ID
            var response = await _httpClient.GetAsync($"{Routes.UrlBaseApiUsuario}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "No se pudieron obtener los datos del usuario.";
                return RedirectToAction("Index", "Perfil");
            }

            var usuario = await response.Content.ReadFromJsonAsync<UsuarioCreateViewModel>();

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "El usuario no existe.";
                return RedirectToAction("Index", "Perfil");
            }

            return View(usuario);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioCreateViewModel model) 
        {
            if (!ModelState.IsValid)
                return View(model);
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{Routes.UrlBaseApiUsuario}/{model.idUsuario}",content);

            if (response.IsSuccessStatusCode) 
            {
                TempData["SuccessMessage"] = "Los datos se actualizaron correctamente.";
                return RedirectToAction("Index", "Perfil"); // o donde muestres la lista
            }
            ModelState.AddModelError(string.Empty, "Error al actualizar el cliente.");
            return View(model);

        }

    }
}

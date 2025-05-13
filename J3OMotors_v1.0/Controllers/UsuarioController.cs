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



    }
}

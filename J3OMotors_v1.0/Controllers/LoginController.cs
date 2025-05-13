using J3OMotors_v1._0.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using J3OMotors_v1._0.Helpers.Rutas;

namespace J3OMotors_v1._0.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Llamar tu API de login
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var ruta = Routes.UrlBaseApiAuth;

            var response = await _httpClient.PostAsync(ruta, content);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>(); // LoginResponse = clase que mapea lo que te devuelve la API
                                                                                           

                HttpContext.Session.SetString("Token", resultado.Token);
                HttpContext.Session.SetInt32("TipoUsuario", resultado.Tipo);
                HttpContext.Session.SetString("Email", model.Email); // opcional, si lo necesitas después
                HttpContext.Session.SetString("Password", model.Password);
                HttpContext.Session.SetInt32("IdUsuario",resultado.idUsuario);

                if (resultado.Tipo == 2)
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Index", "Home");//Admin == 1
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return View(model);
        }

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }

}

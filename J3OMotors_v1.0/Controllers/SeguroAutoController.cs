﻿using J3OMotors_v1._0.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using J3OMotors_v1._0.Helpers.Rutas;
using J3OMotors_v1._0.Models.Seguro;
using J3OMotors_v1._0.Models.Autos;
using J3OMotors_v1._0.Models.Usuario;


namespace J3OMotors_v1._0.Controllers
{
    public class SeguroAutoController : Controller
    {
        private readonly IHttpClientFactory _httpClient;

        public SeguroAutoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ruta = Routes.UrlBaseApiSeguros;

            var cliente = _httpClient.CreateClient();
            var response = await cliente.GetAsync(ruta);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var seguros = JsonSerializer.Deserialize<List<SegurosViewModel>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(seguros);
            }
            else
            {
                ModelState.AddModelError("", "Error al obtener los seguros.");
            }

            return View(new List<SegurosViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SegurosViewModel seguro)
        {
            var cliente = _httpClient.CreateClient();
            var json = JsonSerializer.Serialize(seguro);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync(Routes.AgregarSeguro(), content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Error al agregar el seguro.");
            }
            return View(seguro);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = _httpClient.CreateClient();   
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction("Index", "SeguroAuto");
            }

            // Llamada a la API para obtener el seguro por ID
            var response = await cliente.GetAsync(Routes.SeguroPorId(id));

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "No se pudieron obtener los datos del seguro.";
                return RedirectToAction("Index", "SeguroAuto");
            }

            var seguro = await response.Content.ReadFromJsonAsync<SegurosViewModel>();

            if (seguro == null)
            {
                TempData["ErrorMessage"] = "El seguro no existe.";
                return RedirectToAction("Index", "Perfil");
            }

            return View(seguro);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(SegurosViewModel seguro)
        {
            var cliente = _httpClient.CreateClient();
            var json = JsonSerializer.Serialize(seguro);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await cliente.PutAsync(Routes.ModificarSeguroPorId(seguro.IdSeguro), content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Error al modificar el seguro.");
            }
            return View(seguro);
        }

        [HttpGet]
        public async Task <IActionResult> Delete(int id)
        {

            var cliente = _httpClient.CreateClient();
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction("Index", "SeguroAuto");
            }

            // Llamada a la API para obtener el seguro por ID
            var response = await cliente.GetAsync(Routes.SeguroPorId(id));

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "No se pudieron obtener los datos del seguro.";
                return RedirectToAction("Index", "SeguroAuto");
            }

            var seguro = await response.Content.ReadFromJsonAsync<SegurosViewModel>();

            if (seguro == null)
            {
                TempData["ErrorMessage"] = "El seguro no existe.";
                return RedirectToAction("Index", "Perfil");
            }

            return View(seguro);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(SegurosViewModel seguro)
        {
            var cliente = _httpClient.CreateClient();
            var response = await cliente.DeleteAsync(Routes.EliminarSeguroPorId(seguro.IdSeguro));

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Error al eliminar el seguro.");
            }

            return View(seguro);
        }

    }
}

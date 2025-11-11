using DSWI_cibertec_demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DSWI_cibertec_demo.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UsuarioModel model)
        {
            if (!ModelState.IsValid)
            {
                // Si la validación falla vuelve a mostrar el formulario con los errores
                return View(model);
            }

            // Simulamos registro
            TempData["Message"] = $"Usuario {model.Nombre} registrado correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}

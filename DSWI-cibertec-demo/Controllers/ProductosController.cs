using DSWI_cibertec_demo.Data;
using DSWI_cibertec_demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DSWI_cibertec_demo.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoRepository productoRepo;

        public ProductosController(ProductoRepository productoRepo)
        {
            this.productoRepo = productoRepo;
        }

        // Obtener productos totales
        //public async Task<IActionResult> Index()
        //{
        //    var productos = await productoRepo.ObtenerProductosAsync();
        //    return View(productos);
        //}

        // Obtener productos paginados
        public async Task<IActionResult> Index(int pagina = 1)
        {
            int pageSize = 5; // Cantidad de productos por página
            var (productos, totalRegistros) = await productoRepo.ObtenerProductosPaginadoAsync(pagina, pageSize);
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);
            return View(productos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoModel productoModel)
        {
            if (!ModelState.IsValid)
            {
                return View(productoModel);
            }

            await productoRepo.AgregarProductoAsync(productoModel);
            return RedirectToAction(nameof(Index));
        }
    }
}

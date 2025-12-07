using DSWI_cibertec_demo.Data;
using DSWI_cibertec_demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DSWI_cibertec_demo.Controllers
{
    [Route("api/productos")]
    [ApiController]
    public class ProductosApiController : ControllerBase
    {
        private readonly ProductoRepository _repo;

        public ProductosApiController(ProductoRepository repo)
        {
            _repo = repo;
        }

        // GET api/productos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _repo.ObtenerProductosAsync();
            return Ok(productos);
        }

        // GET api/productos/paginado?page=1&pageSize=5
        [HttpGet("paginado")]
        public async Task<IActionResult> GetPaginado([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var (productos, total) = await _repo.ObtenerProductosPaginadoAsync(page, pageSize);
            return Ok(new { data = productos, total });
        }

        // GET api/productos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _repo.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        // POST api/productos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoModel producto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var id = await _repo.AgregarProductoAsync(producto);
            return CreatedAtAction(nameof(GetById), new { id }, producto);
        }

        // PUT api/productos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoModel producto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existe = await _repo.ObtenerProductoPorIdAsync(id);
            if (existe == null)
            {
                return NotFound();
            }
            await _repo.ActualizarProductoAsync(producto);
            return NoContent();
        }

        // DELETE api/productos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existe = await _repo.ObtenerProductoPorIdAsync(id);
            if (existe == null)
            {
                return NotFound();
            }
            await _repo.EliminarProductoAsync(id);
            return NoContent();
        }
    }
}
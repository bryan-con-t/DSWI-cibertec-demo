using DSWI_cibertec_demo.Data;
using DSWI_cibertec_demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DSWI_cibertec_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosApiController : ControllerBase
    {
        private readonly ProductoRepository _repo;

        public ProductosApiController(ProductoRepository repo)
        {
            _repo = repo;
        }
    }
}

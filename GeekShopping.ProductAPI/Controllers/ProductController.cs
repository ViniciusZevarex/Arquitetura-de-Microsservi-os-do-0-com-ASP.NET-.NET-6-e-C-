using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _productRepository;

        public ProductController(
            IProductRepository productRepository
        )
        {
            _productRepository = productRepository;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> FindById(long id)
        {
            var product = await _productRepository.FindById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FindAll()
        {
            var product = await _productRepository.FindAll();
            if (product == null) return NotFound();
            return Ok(product);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ProductVO vo)
        {
            if (vo == null) return BadRequest();
            var product = await _productRepository.Create(vo);
            return Ok(product);
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ProductVO vo)
        {
            if (vo == null) return BadRequest();
            var product = await _productRepository.Update(vo);
            return Ok(product);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(long id)
        {
            var findProduct = await _productRepository.FindById(id);
            if (findProduct == null) return NotFound();
            var product = await _productRepository.Delete(id);
            return Ok(product);
        }
    }
}

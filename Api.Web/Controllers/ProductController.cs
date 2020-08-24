using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager _productManager;

        private readonly IProductRepository _productRepository;

        public ProductController(IProductManager productManager, IProductRepository productRepository)
            => (_productManager, _productRepository) = (productManager, productRepository);

        #region snippet_GetAll

        [HttpGet]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return Ok(product);
        }

        #endregion

        #region snippet_Create
        
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(Product product)
        {
            await _productManager.CreateAsync(product);
            return Created("", product);
        }

        #endregion

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<Product> replaceProduct)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await _productManager.UpdateByIdAsync(id, product, replaceProduct);
            return Created("", product);
        }

        #endregion

        #region snippet_Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _productManager.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion
    }
}
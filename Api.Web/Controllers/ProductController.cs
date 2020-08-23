using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
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

        /// <summary>
        /// GET
        /// </summary>

        #region snippet_GetAll

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return Ok(product);
        }

        #endregion

        /// <summary>
        /// POST
        /// </summary>

        #region snippet_Create

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Product product)
        {
            await _productManager.CreateAsync(product);
            return Created("", product);
        }

        #endregion

        /// <summary>
        /// PATCH
        /// </summary>

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<Product> replaceProduct)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await _productManager.UpdateByIdAsync(id, product, replaceProduct);
            return Created("", product);
        }

        #endregion

        /// <summary>
        /// DELETE
        /// </summary>

        #region snippet_Delete

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _productManager.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion
    }
}
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Common;
using Api.Web.Handlers;
using Api.Web.Attributes;
using Api.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/products")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _collection = "products";
        private readonly IProductManager _productManager;
        private readonly IProductRepository _productRepository;
        private readonly IOperationHandler _operationHandler;
        private readonly IRedisClientsManagerAsync _redisManager;

        public ProductController
        (
            IProductManager productManager,
            IProductRepository productRepository,
            IOperationHandler operationHandler,
            IRedisClientsManagerAsync redisManager
        )
        {
            _productManager = productManager;
            _productRepository = productRepository;
            _operationHandler = operationHandler;
            _redisManager = redisManager;
        }

        #region snippet_GetAll

        [HttpGet]
        [ProducesResponseType(typeof(ListProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.All } )]
        [SetPaginate]
        public async Task<IActionResult> GetAllAsync([FromQuery] ListResourceRequest request)
        {
            var totalDocuments = await _productRepository.CountAsync(request);
            var products = await _productRepository.GetAllAsync(request);
            return Ok(new ListProductResponse
            {
                Data = products,
                Paginator = Paginator.Paginate(request, totalDocuments)
            });
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SingleProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.All })]
        [ProductExists]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            await using var redisClient = await _redisManager.GetClientAsync();

            var product = await redisClient.GetAsync<Product>(id) ??
                await _productRepository.GetByIdAsync(id);

            return Ok(new SingleProductResponse { Data = product });
        }

        #endregion

        #region snippet_Create
        
        [HttpPost]
        [ProducesResponseType(typeof(SingleProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        public async Task<IActionResult> CreateAsync(Product product)
        {
            await _productManager.CreateAsync(product);
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Create,
                Collection = _collection,
                Id = product.Id,
                Model = product
            });

            return Created("", new SingleProductResponse { Data = product });
        }

        #endregion

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(SingleProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [ProductExists]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<Product> replaceProduct)
        {
            await using var redisClient = await _redisManager.GetClientAsync();

            var product = await redisClient.GetAsync<Product>(id) ??
                await _productRepository.GetByIdAsync(id);

            await _productManager.UpdateByIdAsync(id, product, replaceProduct);

            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Update,
                Collection = _collection,
                Id = id,
                Model = product
            });

            await redisClient.RemoveAsync(id);

            return Ok(new SingleProductResponse { Data = product });
        }

        #endregion

        #region snippet_Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [ProductExists]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await using var redisClient = await _redisManager.GetClientAsync();
            await _productManager.DeleteByIdAsync(id);

            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Delete,
                Collection = _collection,
                Id = id,
                Model = null
            });

            await redisClient.RemoveAsync(id);

            return NoContent();
        }

        #endregion
    }
}
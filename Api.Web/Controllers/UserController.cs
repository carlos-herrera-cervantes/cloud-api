using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Repository.Extensions;
using Api.Services.Services;
using Api.Web.Common;
using Api.Web.Extensions;
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
    [Route("api/v1/users")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _collection = "users";
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IOperationHandler _operationHandler;
        private readonly IRedisClientsManagerAsync _redisManager;

        public UserController
        (
            IUserManager userManager,
            IUserRepository userRepository,
            IOperationHandler operationHandler,
            IRedisClientsManagerAsync redisManager
        )
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _operationHandler = operationHandler;
            _redisManager = redisManager;
        }

        #region snippet_Get

        [HttpGet]
        [ProducesResponseType(typeof(ListUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(new[] { Roles.SuperAdmin })]
        [SetPaginate]
        public async Task<IActionResult> GetAllAsync([FromQuery] ListResourceRequest request)
        {
            var totalDocuments = await _userRepository.CountAsync(request);
            var users = await _userRepository.GetAllAsync(request);

            return Ok(new ListUserResponse
            {
                Data = users,
                Paginator = Paginator.Paginate(request, totalDocuments)
            });
        }

        [HttpGet("station")]
        [ProducesResponseType(typeof(ListUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(new[] { Roles.SuperAdmin, Roles.StationAdmin })]
        [SetPaginate]
        public async Task<IActionResult> GetByStation([FromQuery] ListResourceRequest request)
        {
            var token = HttpContext.Request.Headers.ExtractJsonWebToken();
            var station = token.SelectClaim("station");

            request.Filters = string.IsNullOrEmpty(request.Filters)?
                $"stationId={station}" :
                request.Filters + $",stationId={station}";

            var totalDocuments = await _userRepository.CountAsync(request);
            var users = await _userRepository.GetAllAsync(request);

            return Ok(new ListUserResponse
            {
                Data = users,
                Paginator = Paginator.Paginate(request, totalDocuments)
            });
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(SingleUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(new[] { Roles.SuperAdmin, Roles.StationAdmin })]
        public async Task<IActionResult> GetMeAsync()
        {
            var token = HttpContext.Request.Headers.ExtractJsonWebToken();
            var sub = token.SelectClaim("nameid");

            var user = await _userRepository.GetByIdAsync(sub);

            return Ok(new SingleUserResponse { Data = user });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SingleUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(new[] { Roles.SuperAdmin, Roles.StationAdmin })]
        [UserExists]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            await using var redisClient = await _redisManager.GetClientAsync();

            var user = await redisClient.GetAsync<User>(id) ??
                await _userRepository.GetByIdAsync(id);

            return Ok(new SingleUserResponse { Data = user });
        }

        #endregion

        #region snippet_Post

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(SingleUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(User user)
        {
            await _userManager.CreateAsync(user);
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Create,
                Collection = _collection,
                Id = user.Id,
                StationId = user.StationId,
                Model = user
            });

            return Created("", new SingleUserResponse { Data = user });
        }

        #endregion

        #region snippet_Patch

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(SingleUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(new[] { Roles.SuperAdmin, Roles.StationAdmin })]
        [UserExists]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<User> replaceUser)
        {
            await using var redisClient = await _redisManager.GetClientAsync();

            var user = await redisClient.GetAsync<User>(id) ??
                await _userRepository.GetByIdAsync(id);

            await _userManager.UpdateByIdAsync(id, user, replaceUser);

            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Update,
                Collection = _collection,
                Id = id,
                StationId = user.StationId,
                Model = user
            });

            await redisClient.RemoveAsync(id);
            
            return Ok(new SingleUserResponse { Data = user });
        }

        #endregion

        #region snippet_Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(new[] { Roles.SuperAdmin, Roles.StationAdmin })]
        [UserExists]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await using var redisClient = await _redisManager.GetClientAsync();

            var user = await redisClient.GetAsync<User>(id) ??
                await _userRepository.GetByIdAsync(id);

            await _userManager.DeleteByIdAsync(id);

            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Delete,
                Collection = _collection,
                Id = id,
                StationId = user.StationId,
                Model = null
            });

            await redisClient.RemoveAsync(id);

            return NoContent();
        }

        #endregion
    }
}
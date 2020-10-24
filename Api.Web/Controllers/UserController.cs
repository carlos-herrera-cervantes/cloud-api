using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Common;
using Api.Web.Handlers;
using Api.Web.Middlewares;
using Api.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/users")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _collection = "users";
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IOperationHandler _operationHandler;

        public UserController(IUserManager userManager, IUserRepository userRepository, IOperationHandler operationHandler)
            => (_userManager, _userRepository, _operationHandler) = (userManager, userRepository, operationHandler);

        #region snippet_GetAll

        [HttpGet]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] Request request)
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(new { Status = true, Data = users });
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [UserExists]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return Ok(new { Status = true, Data = user });
        }

        #endregion

        #region snippet_Create

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
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

            return Created("", new { Status = true, Data = user });
        }

        #endregion

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [UserExists]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<User> replaceUser)
        {
            var user = await _userRepository.GetByIdAsync(id);
            await _userManager.UpdateByIdAsync(id, user, replaceUser);
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Update,
                Collection = _collection,
                Id = id,
                StationId = user.StationId,
                Model = user
            });
            
            return Created("", new { Status = true, Data = user });
        }

        #endregion

        #region snippet_Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [UserExists]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            await _userManager.DeleteByIdAsync(id);
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Delete,
                Collection = _collection,
                Id = id,
                StationId = user.StationId,
                Model = null
            });

            return NoContent();
        }

        #endregion
    }
}
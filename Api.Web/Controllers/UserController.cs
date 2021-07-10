using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Common;
using Api.Web.Extensions;
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
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _collection = "users";
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IOperationHandler _operationHandler;

        public UserController
        (
            IUserManager userManager,
            IUserRepository userRepository,
            IOperationHandler operationHandler
        )
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _operationHandler = operationHandler;
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
            return Ok(new
            {
                Status = true,
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
            var handler = new JwtSecurityTokenHandler();
            var decoded = handler.ReadJwtToken(token);

            string station = ((List<Claim>)decoded.Claims)?
                .Where(claim => claim.Type == "station")
                .Select(claim => claim.Value)
                .SingleOrDefault();

            request.Filters = string.IsNullOrEmpty(request.Filters)?
                $"stationId={station}" :
                request.Filters + $",stationId={station}";

            var totalDocuments = await _userRepository.CountAsync(request);
            var users = await _userRepository.GetAllAsync(request);

            return Ok(new
            {
                Status = true,
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
            var handler = new JwtSecurityTokenHandler();
            var decoded = handler.ReadJwtToken(token);

            string sub = ((List<Claim>)decoded.Claims)?
                .Where(claim => claim.Type == "nameid")
                .Select(claim => claim.Value)
                .SingleOrDefault();

            var user = await _userRepository.GetByIdAsync(sub);

            return Ok(new { Status = true, Data = user });
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
            var user = await _userRepository.GetByIdAsync(id);
            return Ok(new { Status = true, Data = user });
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

            return Created("", new { Status = true, Data = user });
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(new[] { Roles.SuperAdmin, Roles.StationAdmin })]
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
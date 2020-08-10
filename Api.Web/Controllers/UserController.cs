using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;

        public UserController(IUserManager userManager, IUserRepository userRepository)
            => (_userManager, _userRepository) = (userManager, userRepository);

        /// <summary>
        /// GET
        /// </summary>

        #region snippet_GetAll

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return Ok(user);
        }

        #endregion

        /// <summary>
        /// POST
        /// </summary>

        #region snippet_Create

        [HttpPost]
        public async Task<IActionResult> CreateAsync(User user)
        {
            await _userManager.CreateAsync(user);
            return Created("", user);
        }

        #endregion

        /// <summary>
        /// PATCH
        /// </summay>

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<User> replaceUser)
        {
            var user = await _userRepository.GetByIdAsync(id);
            await _userManager.UpdateByIdAsync(id, user, replaceUser);
            return Created("", user);
        }

        #endregion

        /// <summary>
        /// DELETE
        /// </summary>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _userManager.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
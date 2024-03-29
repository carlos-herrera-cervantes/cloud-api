using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Repository.Extensions;
using Api.Services.Services;
using Api.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/auth")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager _tokenManager;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public LoginController
        (
            IUserRepository userRepository, 
            IConfiguration configuration, 
            ITokenManager tokenManager,
            IStringLocalizer<SharedResources> localizer
        )
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenManager = tokenManager;
            _localizer = localizer;
        }

        #region snippet_ActionMethods

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessAuth))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] Credentials credentials)
        {
            var user = await _userRepository
                .GetOneAsync(Builders<User>.Filter.Where(u => u.Email == credentials.Email));

            if (user is null || credentials.Password != user.Password) return BadRequest();

            await DeleteSepecificTokens(user.Id);

            var token = GetToken(credentials, user);

            if (token is null || user.Role == Roles.Employee) return NotFound(new
            { 
                    Status = false,
                    Code = "InvalidCredentials",
                    Message = _localizer["InvalidCredentials"].Value
            });

            var accessToken = new AccessToken
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role
            };

            await _tokenManager.CreateAsync(accessToken);
            return Ok(new StringResponse { Data = token });
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Headers.ExtractJsonWebToken();
            var id = token.SelectClaim("nameid");

            await DeleteSepecificTokens(id);
            return NoContent();
        }

        #endregion

        #region snippet_Helpers

        /// <summary>
        /// Returns the Json Web Token
        /// </summary>
        /// <param name="credentials">User email and password</param>
        /// <param name="user">User model</param>
        /// <returns>Json Web Token</returns>
        private string GetToken(Credentials credentials, User user)
        {
            var claims = new ClaimsIdentity(new[]
            { 
                new Claim(ClaimTypes.Email, credentials.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("station", string.IsNullOrEmpty(user.StationId) ? "" : user.StationId)
            });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey"))),
                        SecurityAlgorithms.HmacSha256Signature
                    ),
                Expires = DateTime.UtcNow.AddDays(5)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(createdToken);
        }

        /// <summary>
        /// Deletes a set of tokens that belongs to the user in session
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Delete result</returns>
        private async Task DeleteSepecificTokens(string id)
            => await _tokenManager.DeleteManyAsync(Builders<AccessToken>.Filter.Where(t => t.UserId == id));

        #endregion
    }
}
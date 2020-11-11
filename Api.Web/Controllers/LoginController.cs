using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager _tokenManager;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public LoginController(
            IUserRepository userRepository, 
            IConfiguration configuration, 
            ITokenManager tokenManager,
            IStringLocalizer<SharedResources> localizer
        )
        => (_userRepository, _configuration, _tokenManager, _localizer) = (userRepository, configuration, tokenManager, localizer);

        #region snippet_Login

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(Credentials credentials)
        {
            var token = await GetToken(credentials);

            if (token is false) return NotFound(new { Status = false, Code = "InvalidCredentials", Message = _localizer["InvalidCredentials"].Value });

            var user = await GetUserByEmail(credentials.Email);
            var accessToken = new AccessToken
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role
            };

            await _tokenManager.CreateAsync(accessToken);
            return Ok(new { Status = true, Data = token });
        }

        #endregion

        #region snippet_Logout

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout() => await Task.FromResult(NoContent());

        #endregion

        #region snippet_GetToken
        private async Task<dynamic> GetToken(Credentials credentials)
        {
            var isValidCredentials = await ValidateCredentials(credentials);
            
            if (isValidCredentials is false) return false;

            var claims = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, credentials.Email) });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey"))), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(createdToken);
        }

        #endregion

        #region snippet_ValidateCredentials
        private async Task<bool> ValidateCredentials(Credentials credentials)
        {
            var user = await GetUserByEmail(credentials.Email);
            
            if (user is false) return false;

            var isValidPassword = credentials.Password == user.Password;
            
            if (isValidPassword) return true;

            return false;
        }

        #endregion

        #region GetUserByEmail
        private async Task<dynamic> GetUserByEmail(string email)
        {
            var request = new Request { Filters = new [] { $"Email={email}" } };
            var user = await _userRepository.GetOneAsync(request);
            if (user is null) return false;
            return user;
        }

        #endregion
    }
}
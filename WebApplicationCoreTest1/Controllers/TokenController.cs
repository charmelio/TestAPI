using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplicationCoreTest1.Interfaces;
using WebApplicationCoreTest1.Models;

namespace WebApplicationCoreTest1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] Account userParam)
        {
            var userToken = await _tokenService.AuthenticateAsync(userParam.Username, userParam.Password).ConfigureAwait(false);

            if (userToken == null) return BadRequest(new { message = "Username or password is incorrect." });

            return Ok(userToken);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var tokens = await _tokenService.GetAllAsync().ConfigureAwait(false);
            return Ok(tokens);
        }
    }
}
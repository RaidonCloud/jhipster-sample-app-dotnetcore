using MyCompany.Dto;
using MyCompany.Security.Jwt;
using MyCompany.Domain.Services.Interfaces;
using MyCompany.Web.Extensions;
using MyCompany.Web.Filters;
using MyCompany.Crosscutting.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MyCompany.Controllers {
    [Route("api")]
    [ApiController]
    public class UserJwtController : ControllerBase {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILogger<UserJwtController> _log;
        private readonly ITokenProvider _tokenProvider;

        public UserJwtController(ILogger<UserJwtController> log, IAuthenticationService authenticationService,
            ITokenProvider tokenProvider)
        {
            _log = log;
            _authenticationService = authenticationService;
            _tokenProvider = tokenProvider;
        }

        [HttpPost("authenticate")]
        [ValidateModel]
        public async Task<ActionResult<JwtToken>> Authorize([FromBody] LoginDto LoginDto)
        {
            var user = await _authenticationService.Authenticate(LoginDto.Username, LoginDto.Password);
            var rememberMe = LoginDto.RememberMe;
            var jwt = _tokenProvider.CreateToken(user, rememberMe);
            var httpHeaders = new HeaderDictionary {
                [JwtConstants.AuthorizationHeader] = $"{JwtConstants.BearerPrefix} {jwt}"
            };
            return Ok(new JwtToken(jwt)).WithHeaders(httpHeaders);
        }
    }

    public class JwtToken {
        public JwtToken(string idToken)
        {
            IdToken = idToken;
        }

        [JsonProperty("id_token")] private string IdToken { get; }
    }
}

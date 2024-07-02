using System;
using System.Threading.Tasks;
using webapi_ENVIA.Entities;
using webapi_ENVIA.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;


namespace webapi_ENVIA.Extensions
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
    }
    public class BasicAuthHander : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly IUserService _userService;
        public BasicAuthHander
            (
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService
            ) : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            User user = null;

            try
            {
                var auth = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(auth.Parameter);//Ex:     Mike:123
                var credentials = Encoding.UTF8.GetString(bytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                user = await _userService.Authenticate(username, password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (user == null)
                return AuthenticateResult.Fail("Invalid username or password");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IB.React.Core.Database.Services;
using IB.React.Core.Model.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IB.React.Core.Auth
{
	public class JwtHandlerOptions : AuthenticationSchemeOptions
	{
	}

	public class JwtHandler : AuthenticationHandler<JwtHandlerOptions>
	{
		public const string SchemeName = "IB.Jwt";
		
		private readonly IJwtService jwtService;
		private readonly IUserService userService;
		
		public JwtHandler(
			IOptionsMonitor<JwtHandlerOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock,
			IUserService userService,
			IJwtService jwtService) : base(options, logger, encoder, clock)
		{
			this.userService = userService;
			this.jwtService = jwtService;
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!(Request.Cookies.ContainsKey(JwtService.Constants.AccessToken) ||
			    Request.Cookies.ContainsKey(JwtService.Constants.RefreshToken)))
			{
				return AuthenticateResult.Fail("There is no token for authenticate.");
			}

			var accessToken = Request.Cookies[JwtService.Constants.AccessToken];
			var refreshToken = Request.Cookies[JwtService.Constants.RefreshToken];
			
			var accessPayload = jwtService.DecodeToken(accessToken);
			var refreshPayload = jwtService.DecodeToken(refreshToken);

			if (!(jwtService.IsValid(accessToken) && jwtService.IsValid(refreshToken)))
			{
				return AuthenticateResult.Fail("Tokens are invalid");
			}
			else
			{
				if (!jwtService.IsValid(accessToken))
				{
					jwtService.ReissueToken(accessToken, refreshToken, Response.HttpContext);
				}
				
				return AuthenticateResult.Success(GenerateTicket(accessPayload ?? refreshPayload));
			}
		}

		private AuthenticationTicket GenerateTicket(JwtPayload payload)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Role, payload.Role),
			};

			var claimsIdentity = new ClaimsIdentity(claims, nameof(JwtHandler));

			var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

			return ticket;
		}
	}
}
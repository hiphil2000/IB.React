using System;
using System.Net.Http;
using IB.React.Core.Auth;
using IB.React.Core.Database.Services;
using IB.React.Core.Model;
using IB.React.Core.Model.Auth;
using IB.React.Core.Model.Database;
using Microsoft.AspNetCore.Mvc;
using TokenModel = IB.React.Core.Model.Auth.TokenModel;

namespace IB.React.Demo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : Controller
	{
		private IJwtService jwtService;
		private IUserService userService;
		private ITokenService tokenService;

		public AuthController(IJwtService jwtService,
			IUserService userService,
			ITokenService tokenService)
		{
			this.jwtService = jwtService;
			this.userService = userService;
			this.tokenService = tokenService;
		}
		
		/// <summary>
		/// 요청에 따라 로그인을 진행합니다.
		/// 성공 시 토큰이 반환됩니다.
		/// </summary>
		/// <param name="payload">로그인 Payload</param>
		/// <returns></returns>
		[HttpPost]
		[Route("Login")]
		// [AllowAnonymous]
		public IActionResult Login([FromBody] LoginPayload payload)
		{
			TokenModel token = null;
			UserModel user = null;
			string message = null;

			// 토큰을 생성합니다. 실패 시 오류 메시지를 전달합니다.
			try
			{
				user = userService.Login(payload.UserId, payload.Password);
				if (user != null)
				{
					token = new TokenModel()
					{
						AccessToken = jwtService.CreateToken(TokenType.AccessToken, user.UserNo),
						RefreshToken = jwtService.CreateToken(TokenType.RefreshToken, user.UserNo),
					};
				}
				else
				{
					message = "mismatch ID or Password.";
				}
			}
			catch (Exception e)
			{
				message = e.Message;
			}

			// 인증에 성공한 경우, 쿠키를 생성합니다.
			if (token != null)
			{
				jwtService.AddCookies(Response.HttpContext, token.AccessToken, token.RefreshToken);
			}

			return new JsonResult(new CommonResponse<AuthenticationResult>()
			{
				Success = token != null,
				Data = new AuthenticationResult
				{
					Token = token,
					User = user
				},
				Message = message
			});
		}

		[HttpPost]
		[Route("Logout")]
		public IActionResult Logout()
		{
			var response = new HttpResponseMessage();
			
			var accessToken = jwtService.GetJwtToken(HttpContext, TokenType.AccessToken);
			var refreshToken = jwtService.GetJwtToken(HttpContext, TokenType.RefreshToken);


			// 토큰이 아예 없는 경우: 무시
			if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
			{
				return new OkResult();
			}

            // DB에서 쿠키를 사용안함 처리합니다.
            jwtService.RemoveCookies(HttpContext, TokenType.AccessToken);
            jwtService.RemoveCookies(HttpContext, TokenType.RefreshToken);

            return new OkResult();
		}

		/// <summary>
		/// 현재 사용자 정보를 조회합니다.
		/// 토큰이 올바르지 않거나, 정보가 없다면 null이 반환됩니다.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("CurrentUser")]
		public IActionResult GetCurrentUser()
		{
			var payload = jwtService.GetJwtPayload(HttpContext);

			UserModel target = null;

			if (payload != null)
			{
				target = userService.GetUser(payload.UserNo);
			}

			return new JsonResult(new CommonResponse<UserModel>
			{
				Success = target != null,
				Data = target
			});
		}

		[HttpPost]
		[Route("ValidateToken")]
		public IActionResult ValidateToken()
		{
			var payload = jwtService.GetJwtPayload(HttpContext);
			var valid = jwtService.IsValid(payload);

			return new JsonResult(new CommonResponse<JwtPayload>
			{
				Success = valid,
				Data = valid ? payload : null
			});
		}
	}
}
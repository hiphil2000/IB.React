using System;
using System.Collections.Generic;
using System.Text;
using IB.React.Core.Database.Services;
using IB.React.Core.Model.Auth;
using IB.React.Core.Model.Database;
using IB.React.Core.Model.Settings;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IB.React.Core.Auth
{
	/// <summary>
	/// JWT 서비스의 DI용 인터페이스입니다.
	/// </summary>
	public interface IJwtService
	{
		string CreateToken(TokenType type, long userNo);
		void ReissueToken(string accessToken, string refreshToken, HttpContext response);
		string? EncodeToken(JwtPayload? payload);
		JwtPayload? DecodeToken(string token);
		bool IsValid(string token);
		bool IsValid(JwtPayload? payload);
		void AddCookies(HttpContext context, string accessToken, string refreshToken);
		void RemoveCookies(HttpContext context, TokenType type);
		JwtPayload? GetJwtPayload(HttpContext context);
		JwtPayload? GetJwtPayload(HttpContext context, TokenType type);
		string? GetJwtToken(HttpContext context, TokenType type);
	}
	
	/// <summary>
	/// JWT 관련 처리를 하는 서비스입니다.
	/// </summary>
	public class JwtService : IJwtService
	{
		#region Constants

		/// <summary>
		/// JWT Helper 클래스의 상수 관리용 클래스입니다.
		/// </summary>
		public static class Constants
		{
			/// <summary>
			/// AccessToken
			/// </summary>
			public const string AccessToken = "access_token";
			
			/// <summary>
			/// RefreshToken
			/// </summary>
			public const string RefreshToken = "refresh_token";

			/// <summary>
			/// 토큰 타입을 string constant로 변환합니다.
			/// </summary>
			/// <param name="token">토큰 타입</param>
			/// <returns></returns>
			public static string? TypeToString(TokenType token)
			{
				return token switch
				{
					TokenType.AccessToken => AccessToken,
					TokenType.RefreshToken => RefreshToken,
					_ => null
				};
			}

			/// <summary>
			/// 토큰 타입 명을 토큰 타입으로 변환합니다.
			/// </summary>
			/// <param name="subject">토큰 타입 명</param>
			/// <returns></returns>
			public static TokenType ParseTokenType(string subject)
			{
				return subject switch
				{
					AccessToken => TokenType.AccessToken,
					RefreshToken => TokenType.RefreshToken,
					_ => TokenType.Unknown
				};
			}
		}

		#endregion
		
		#region Member Variables

		#region Service DIs

		private IOptions<JwtSettings> settings;
		private IConfiguration configuration;
		private IUserService userService;
		private ITokenService tokenService;

		#endregion

		#endregion

		#region Constructor & Initializer

		public JwtService(IConfiguration configuration,
			IOptions<JwtSettings> settings,
			IUserService userService,
			ITokenService tokenService)
		{
			this.configuration = configuration;
			this.settings = settings;
			this.userService = userService;
			this.tokenService = tokenService;
		}

		#endregion

		#region 토큰 발행
		
		/// <summary>
		/// 토큰을 발행합니다.
		/// </summary>
		/// <param name="type">토큰 타입</param>
		/// <param name="userNo">대상 유저 ID</param>
		/// <returns></returns>
		public string CreateToken(TokenType type, long userNo)
		{
			// 토큰 ID 생성
			var tokenId = Guid.NewGuid();
			var iat = DateTime.Now;
			
			// 대상 사용자 조회
			var user = userService.GetUser(userNo);
			
			// 토큰을 발행합니다.
			var token = JwtBuilder.Create()
				.WithAlgorithm(GetAlgorithm(settings.Value.Algorithm))
				.WithSecret(settings.Value.SecretKey)
				.Subject(Constants.TypeToString(type))
				.IssuedAt(iat)
				.Audience(settings.Value.Audience)
				.Issuer(settings.Value.Issuer)
				.Id(tokenId)
				.ExpirationTime(GetExpiryTime(type, iat))
				.AddClaim(JwtClaims.UserNo, userNo.ToString())
				.AddClaim(JwtClaims.Role, user.Role)
				.Encode();
			
			// 토큰을 DB에 등록합니다.
			var decoded = DecodeToken(token)!;
			tokenService.AddToken(
				decoded.UserNo,
				decoded.JwtId.ToString(),
				decoded.Subject,
				decoded.IssuedAt,
				decoded.ExpirationTime,
				token.Split(".")[2]
				);

			return token;
		}

		/// <summary>
		/// 토큰을 재발급하여 response 객체의 쿠키로 넣습니다.
		/// </summary>
		/// <param name="accessToken">Access Token 값</param>
		/// <param name="refreshToken">Refresh Token 값</param>
		/// <param name="context">쿠키를 넣을 Response 객체</param>
		public void ReissueToken(string? accessToken, string? refreshToken, HttpContext context)
		{	
			// 1. Access Token이 유효한 경우: 권한 있음, RefreshToken 재발급
			if (accessToken != null && IsValid(accessToken))
			{
				var accessPayload = DecodeToken(accessToken);
				
				// Refresh Token이 유효하지 않으면 재발급합니다.
				if (!(refreshToken != null && IsValid(refreshToken)))
				{
					var newRefreshToken = CreateToken(TokenType.RefreshToken, accessPayload.UserNo);
					
					AddCookie(context, TokenType.RefreshToken, newRefreshToken);
				}
			}
			// 2. Access Token이 유효하지 않으나, Refresh Token 유효한 경우: 권한 있음, Access Token 재발급
			else if (refreshToken != null && IsValid(refreshToken))
			{
				var refreshPayload = DecodeToken(refreshToken);
				
				// AccessToken을 재발급합니다.
				var newAccessToken = CreateToken(TokenType.AccessToken, refreshPayload.UserNo);
				
				AddCookie(context, TokenType.AccessToken, newAccessToken);
			}
		}
		
		#endregion

		#region 토큰 암호화/해독 및 검증

		/// <summary>
		/// 토큰을 다시 암호화합니다.
		/// payload가 null이면 null을 반환합니다.
		/// </summary>
		/// <param name="payload">JWT Payload</param>
		/// <returns></returns>
		public string? EncodeToken(JwtPayload? payload)
		{
			if (payload == null)
			{
				return null;
			}

			return JwtBuilder.Create()
				.WithAlgorithm(GetAlgorithm(settings.Value.Algorithm))
				.WithSecret(settings.Value.SecretKey)
				.IssuedAt(payload.IssuedAt)
				.Issuer(payload.Issuer)
				.ExpirationTime(payload.ExpirationTime)
				.Id(Guid.NewGuid())
				.Encode();
		}

		/// <summary>
		/// 토큰을 해독합니다.
		/// 올바르지 않은 토큰이면 null을 반환합니다.
		/// </summary>
		/// <param name="token">토큰</param>
		/// <returns></returns>
		public JwtPayload? DecodeToken(string? token)
		{
			// 입력이 null이면 null로 반환합니다.
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}
			
			try
			{
				var payload = JwtBuilder.Create()
					.WithAlgorithm(GetAlgorithm(settings.Value.Algorithm))
					.WithSecret(settings.Value.SecretKey)
					.MustVerifySignature()
					.Decode<Dictionary<string, object>>(token);
				
				return new JwtPayload(payload);
			}
			catch (TokenExpiredException e)
			{
				return null;
			}
		}

		/// <summary>
		/// 토큰이 유효한지 확인합니다.
		/// </summary>
		/// <param name="token">토큰</param>
		/// <returns></returns>
		public bool IsValid(string token)
		{
			return !string.IsNullOrEmpty(token) && IsValid(DecodeToken(token));
		}

		/// <summary>
		/// 토큰이 유효한지 확인합니다
		/// </summary>
		/// <param name="payload">해독된 토큰</param>
		/// <returns></returns>
		public bool IsValid(JwtPayload? payload)
		{
			// 토큰이 존재하고, 사용중인 토큰인 경우, 유효한 토큰으로 처리합니다.
			return payload != null && tokenService.IsUsingToken(payload.JwtId.ToString());;
		}

		#endregion

		#region Utils

		/// <summary>
		/// 알고리즘 이름을 받아서 JWT 토큰 암호화 알고리즘 구현체를 반환합니다.
		/// </summary>
		/// <param name="algorithm">알고리즘 이름</param>
		/// <returns>JWT 토큰 암호화 구현체</returns>
		private IJwtAlgorithm GetAlgorithm(string algorithm)
		{
			return algorithm.ToLower() switch
			{
				"sha256" => new HMACSHA256Algorithm(),
				"sha384" => new HMACSHA384Algorithm(),
				"sha512" => new HMACSHA512Algorithm(),
				_ => new NoneAlgorithm()
			};
		}

		/// <summary>
		/// 토큰 타입을 받아서 만료 시간을 계산합니다.
		/// </summary>
		/// <param name="type">토큰 타입</param>
		/// <returns>토큰의 만료 시간</returns>
		private DateTime GetExpiryTime(TokenType type, DateTime from)
		{
			var minutes = type switch {
				TokenType.AccessToken => settings.Value.Cookies.AccessToken.Expiry,
				TokenType.RefreshToken => settings.Value.Cookies.RefreshToken.Expiry,
				_ => -1
			};
			
			return from.AddMinutes(minutes);
		}
		
		/// <summary>
		/// 요청 컨텍스트에 토큰 쿠키를 추가합니다. 
		/// </summary>
		/// <param name="context">요청 컨텍스트</param>
		/// <param name="type">토큰 타입</param>
		/// <param name="token">토큰 값</param>
		public void AddCookie(HttpContext context, TokenType type, string token)
		{
			var expiry = DecodeToken(token)?.ExpirationTime;
			
			context.Response.Cookies.Append(Constants.TypeToString(type), token, new CookieOptions()
			{
				Path = "/",
				Expires = expiry,
				HttpOnly = true,
				Secure = true
			});
		}

		/// <summary>
		/// 요청 컨텍스트에 토큰 쿠키들을 추가합니다.
		/// </summary>
		/// <param name="context">요청 컨텍스트</param>
		/// <param name="accessToken">Access Token</param>
		/// <param name="refreshToken">Refresh Token</param>
		public void AddCookies(HttpContext context, string accessToken, string refreshToken)
		{
			AddCookie(context, TokenType.AccessToken, accessToken);
			AddCookie(context, TokenType.RefreshToken, refreshToken);
		}

		/// <summary>
		/// 요청에서 토큰을 추출합니다.
		/// AccessToken > RefreshToken 순으로 찾아서 먼저 나오는 것을 반환합니다.
		/// </summary>
		/// <param name="context">HTTP 컨텍스트</param>
		/// <returns></returns>
		public JwtPayload? GetJwtPayload(HttpContext context)
		{
			// 1. AccessToken 검색
			var access = GetJwtPayload(context, TokenType.AccessToken);
			if (access != null)
			{
				return access;
			}
			
			// 2. RefreshToken 검색
			var refresh = GetJwtPayload(context, TokenType.RefreshToken);
			return refresh;
		}
		
		/// <summary>
		/// 요청에서 토큰을 추출합니다.
		/// </summary>
		/// <param name="context">HTTP 컨텍스트</param>
		/// <param name="type">토큰 타입</param>
		/// <returns></returns>
		public JwtPayload? GetJwtPayload(HttpContext context, TokenType type)
		{	
			var cookie = GetJwtToken(context, type);
			return cookie != null ? DecodeToken(cookie) : null;
		}

		/// <summary>
		/// 요청에서 JWT 토큰을 추출합니다.
		/// </summary>
		/// <param name="context">HTTP 컨텍스트</param>
		/// <param name="type">토큰 타입</param>
		/// <returns></returns>
		public string? GetJwtToken(HttpContext context, TokenType type)
		{
			var request = context.Request;
			
			return type switch
			{
				TokenType.AccessToken => request.Cookies[settings.Value.Cookies.AccessToken.Name],
				TokenType.RefreshToken => request.Cookies[settings.Value.Cookies.RefreshToken.Name],
				_ => null
			};
		}
		
		/// <summary>
		/// 쿠키를 삭제합니다.
		/// </summary>
		/// <param name="context">요청 컨텍스트</param>
		/// <param name="type">쿠키 타입</param>
		public void RemoveCookies(HttpContext context, TokenType type)
		{
			var payload = GetJwtPayload(context, type);
			if (payload == null)
			{
				return;
			}

			// DB에서 토큰 삭제
			tokenService.RemoveToken(payload.JwtId.ToString());
			
			// Response에서 토큰 쿠키 삭제
			var cookieName = type switch
			{
				TokenType.AccessToken => settings.Value.Cookies.AccessToken.Name,
				TokenType.RefreshToken => settings.Value.Cookies.RefreshToken.Name,
				_ => null
			};
			context.Response.Cookies.Delete(cookieName ?? "");
		}

		#endregion
	}
}
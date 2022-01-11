using System.Data;
using System.Security.Cryptography;
using IB.React.Core.Auth.Models;
using IB.React.Core.Database;
using IB.React.Core.Database.Services;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using Microsoft.AspNetCore.Http;

namespace IB.React.Core.Auth.Jwt
{
	/// <summary>
	/// JWT 관련 기능의 Helper Class입니다.
	/// </summary>
	public static class JwtHelper
	{
		private ITokenService tokenService;
		
		#region Constants & Member Variables

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
		}

		#endregion

		#region Member Variables

		/// <summary>
		/// JWT 암호화 키 입니다.
		/// </summary>
		private static string JwtSecret;
		
		/// <summary>
		/// JWT 암호화 알고리즘 명 입니다.
		/// </summary>
		private static string JwtAlgorithm;
		
		/// <summary>
		/// AccessToken 만료시간(분) 입니다.
		/// </summary>
		private static int AccessTokenExpiry;
		
		/// <summary>
		/// RefreshToken 만료시간(분) 입니다. 
		/// </summary>
		private static int RefreshTokenExpiry;

		#endregion

		#region Properties

		/// <summary>
		/// AccessToken의 만료 일자입니다.
		/// </summary>
		private static DateTime AccessTokenExpiryMin => DateTime.Now.AddMinutes(AccessTokenExpiry);

		/// <summary>
		/// RefreshToken의 만료 일자입니다.
		/// </summary>
		private static DateTime RefreshTokenExpiryMin => DateTime.Now.AddMinutes(RefreshTokenExpiry);

		#endregion
		
		#endregion

		#region Utils
		
		/// <summary>
		/// JWT Helper를 초기화합니다.
		/// </summary>
		/// <param name="jwtSecret">JWT 암호화 키</param>
		/// <param name="jwtAlgorithm">JWT 암호화 알고리즘</param>
		/// <param name="accessTokenExpiry">AccessToken 만료시간(분)</param>
		/// <param name="refreshTokenExpiry">RefreshToken 만료시간(분)</param>
		public static void Initialize(string jwtSecret, string jwtAlgorithm, int accessTokenExpiry = 15, int refreshTokenExpiry = 24 * 60)
		{
			JwtSecret = jwtSecret;
			JwtAlgorithm = jwtAlgorithm;
			AccessTokenExpiry = accessTokenExpiry;
			RefreshTokenExpiry = refreshTokenExpiry;
		}

		/// <summary>
		/// 토큰을 해독합니다.
		/// 올바르지 않은 토큰이면 null을 반환합니다.
		/// </summary>
		/// <param name="token">토큰</param>
		/// <returns></returns>
		public static JwtPayload? DecodeToken(string token)
		{
			// 입력이 null이면 null로 반환합니다.
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}
			
			try
			{
				var payload = JwtBuilder.Create()
					.WithAlgorithm(GetAlgorithm(JwtSecret))
					.WithSecret(JwtSecret)
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
		/// 토큰을 다시 암호화합니다.
		/// payload가 null이면 null을 반환합니다.
		/// </summary>
		/// <param name="payload">JWT Payload</param>
		/// <returns></returns>
		public static string? EncodeToken(JwtPayload? payload)
		{
			if (payload == null)
			{
				return null;
			}

			return JwtBuilder.Create()
				.WithAlgorithm(GetAlgorithm(JwtSecret))
				.WithSecret(JwtSecret)
				.IssuedAt(payload.IssuedAt)
				.Issuer(payload.Issuer.ToString())
				.ExpirationTime(payload.ExpirationTime)
				.Id(Guid.NewGuid())
				.Encode();
		}
		
		/// <summary>
		/// 토큰이 유효한지 확인합니다.
		/// </summary>
		/// <param name="token">토큰</param>
		/// <returns></returns>
		public static bool IsValid(string token)
		{
			return !string.IsNullOrEmpty(token) && IsValid(DecodeToken(token));
		}

		/// <summary>
		/// 토큰이 유효한지 확인합니다
		/// </summary>
		/// <param name="payload">해독된 토큰</param>
		/// <returns></returns>
		public static bool IsValid(JwtPayload? payload)
		{
			// 토큰이 존재하고, 사용중인 토큰인 경우, 유효한 토큰으로 처리합니다.
			return payload != null && IsUsingToken(payload.JwtId.ToString());;
		}
		
		/// <summary>
		/// 토큰 타입을 string 형태로 Parse합니다.
		/// </summary>
		/// <param name="type">토큰 타입</param>
		/// <returns></returns>
		public static string? ParseType(TokenType type)
		{
			return type switch
			{
				TokenType.AccessToken => Constants.AccessToken,
				TokenType.RefreshToken => Constants.RefreshToken,
				_ => null
			};
		}

		/// <summary>
		/// 토큰 만료 일자를 조회합니다.
		/// </summary>
		/// <param name="type">대상 토큰 타입</param>
		/// <returns></returns>
		private static DateTime GetExpiryTime(TokenType type)
		{
			return type switch
			{
				TokenType.AccessToken => AccessTokenExpiryMin,
				TokenType.RefreshToken => RefreshTokenExpiryMin,
				_ => DateTime.Now.AddMinutes(-1)
			};
		}
		
		/// <summary>
		/// Jwt 암호화 알고리즘을 얻습니다.
		/// </summary>
		/// <param name="key">RSA 알고리즘에 사용될 키</param>
		/// <returns></returns>
		private static IJwtAlgorithm GetAlgorithm(string key)
		{
			return JwtAlgorithm switch
			{
				"sha256" => new HMACSHA256Algorithm(),
				"sha384" => new HMACSHA384Algorithm(),
				"sha512" => new HMACSHA512Algorithm(),
				"rs256" => new RS256Algorithm(RSA.Create()),
				"rs384" => new RS384Algorithm(RSA.Create()),
				"rs512" => new RS512Algorithm(RSA.Create()),
				"rs1024" => new RS1024Algorithm(RSA.Create()),
				_ => new NoneAlgorithm()
			};
		}

		/// <summary>
		/// 토큰의 쿠키 값를 생성합니다.
		/// </summary>
		/// <param name="response">쿠키를 넣을 Response 객체</param>
		/// <param name="type">토큰 타입(JwtHelper.Constants)</param>
		/// <param name="token">토큰 값</param>
		/// <returns></returns>
		public static void AddCookie(HttpResponse response, string type, string token)
		{
			var expiry = DecodeToken(token)?.ExpirationTime;
			
			response.Cookies.Append(type, token, new CookieOptions()
			{
				Path = "/",
				Expires = expiry,
				HttpOnly = true,
				Secure = true
			});
		}


		/// <summary>
		/// 토큰의 쿠키 값를 생성합니다.
		/// </summary>
		/// <param name="response">쿠키를 넣을 Response 객체</param>
		/// <param name="accessToken">AccessToken</param>
		/// <param name="refreshToken">RefreshToken</param>
		/// <returns></returns>
		public static void AddCookies(HttpResponse response, string accessToken, string refreshToken)
		{
			AddCookie(response, Constants.AccessToken, accessToken);
			AddCookie(response, Constants.RefreshToken, refreshToken);
		}

		public static void RemoveCookies(HttpResponse response)
		{
			response.Cookies.Delete(Constants.AccessToken);
			response.Cookies.Delete(Constants.RefreshToken);
		}

		#endregion
		
		#region Database

		#endregion

		#region Public Methods

		/// <summary>
		/// 토큰을 생성합니다.
		/// </summary>
		/// <param name="type">토큰 타입</param>
		/// <param name="userNo">사용자 No</param>
		/// <returns></returns>
		public static string CreateToken(TokenType type, long userNo)
		{
			var tokenId = Guid.NewGuid();
			var token = JwtBuilder.Create()
				.WithAlgorithm(GetAlgorithm(JwtSecret))
				.WithSecret(JwtSecret)
				.IssuedAt(DateTime.Now)
				.Issuer(userNo.ToString())
				.Id(tokenId)
				.ExpirationTime(GetExpiryTime(type))
				.Encode();

			var decoded = DecodeToken(token);
			
			RegisterToken(decoded, token.Split(".")[2]);

			return token;
		}
		
		/// <summary>
		/// 토큰을 재발급하여 response 객체의 쿠키로 넣습니다.
		/// </summary>
		/// <param name="accessToken">Access Token 값</param>
		/// <param name="refreshToken">Refresh Token 값</param>
		/// <param name="response">쿠키를 넣을 Response 객체</param>
		public static void ReissueToken(string accessToken, string refreshToken, HttpResponse response)
		{
			// 1. Access Token이 유효한 경우: 권한 있음, RefreshToken 재발급
			if (accessToken != null && IsValid(accessToken))
			{
				var accessPayload = DecodeToken(accessToken);
				
				// Refresh Token이 유효하지 않으면 재발급합니다.
				if (!(refreshToken != null && IsValid(refreshToken)))
				{
					var newRefreshToken = CreateToken(TokenType.RefreshToken, accessPayload.Issuer);
					
					AddCookie(response, Constants.RefreshToken, newRefreshToken);
				}
			}
			// 2. Access Token이 유효하지 않으나, Refresh Token 유효한 경우: 권한 있음, Access Token 재발급
			else if (refreshToken != null && IsValid(refreshToken))
			{
				var refreshPayload = DecodeToken(refreshToken);
				
				// AccessToken을 재발급합니다.
				var newAccessToken = CreateToken(TokenType.AccessToken, refreshPayload.Issuer);
				
				AddCookie(response, Constants.AccessToken, newAccessToken);
			}
		}

		#endregion
	}
	
}
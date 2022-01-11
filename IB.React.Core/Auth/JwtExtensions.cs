using IB.React.Core.Auth.Models;
using Microsoft.AspNetCore.Mvc;

namespace IB.React.Core.Auth.Jwt
{
	/// <summary>
	/// JWT 관련 기능의 확장 메서드 클래스입니다.
	/// </summary>
	public static class JwtExtensions
	{
		
		/// <summary>
		/// 현재 요청의 JwtPayload를 조회합니다.
		/// </summary>
		/// <param name="controller">요청 컨트롤러 인스턴스</param>
		/// <returns></returns>
		public static JwtPayload? GetJwtPayload(this ControllerBase controller)
		{	
			
			// AccessToken을 조회합니다.
			var accessToken = GetJwtToken(controller, TokenType.AccessToken);

			if (accessToken != null)
			{
				// 존재한다면 AccessToken을 해독하여 페이로드를 반환합니다.
				return JwtHelper.DecodeToken(accessToken);
			}
			else
			{
				// AccessToken이 없다면 RefreshToken을 사용합니다.
				var refreshToken = GetJwtToken(controller, TokenType.RefreshToken);
				
				return refreshToken != null ? JwtHelper.DecodeToken(refreshToken) : null;
			}

		}

		/// <summary>
		/// 현재 요청의 JwtToken을 조회합니다.
		/// </summary>
		/// <param name="controller">요청 컨트롤러 인스턴스</param> 
		/// <returns></returns>
		public static string GetJwtToken(this ControllerBase controller, TokenType tokenType)
		{
			return controller.Request.Cookies[JwtHelper.ParseType(tokenType)];
		}
	}
}
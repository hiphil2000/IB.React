namespace IB.React.Core.Model.Settings
{
	/// <summary>
	/// 토큰 쿠키 설정 래퍼 타입입니다.
	/// appsettings.json / Jwt / Cookies
	/// </summary>
	public class CookieSettings
	{
		/// <summary>
		/// AccessToken 설정입니다.
		/// </summary>
		public TokenSettings AccessToken { get; set; }
		
		/// <summary>
		/// RefreshToken 설정입니다.
		/// </summary>
		public TokenSettings RefreshToken { get; set; }
	}
}
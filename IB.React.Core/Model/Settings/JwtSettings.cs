namespace IB.React.Core.Model.Settings
{
	/// <summary>
	/// JWT 설정 타입입니다.
	/// </summary>
	public class JwtSettings
	{
		public const string Key = "Jwt";
		
		/// <summary>
		/// JWT 발글 시 사용할 발급자 이름입니다.
		/// </summary>
		public string Issuer { get; set; }
		
		/// <summary>
		/// JWT 발급 시 사용할 대상자 이름입니다.
		/// </summary>
		public string Audience { get; set; }
		
		/// <summary>
		/// JWT 토큰 암호화 키 입니다.
		/// </summary>
		public string SecretKey { get; set; }
		
		/// <summary>
		/// JWT 토큰 암호화 알고리즘입니다
		/// </summary>
		public string Algorithm { get; set; }
		
		/// <summary>
		/// JWT 쿠키 관련 설정입니다.
		/// </summary>
		public CookieSettings Cookies { get; set; }
	}
}
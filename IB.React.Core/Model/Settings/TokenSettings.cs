namespace IB.React.Core.Model.Settings
{
	/// <summary>
	/// 토큰 쿠키 설정 타입입니다.
	/// appsettings.json / Jwt / Cookies / TokenSettings
	/// </summary>
	public class TokenSettings
	{
		/// <summary>
		/// 토큰 쿠키 이름입니다.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// 토큰 유효 시간(분) 입니다.
		/// </summary>
		public int Expiry { get; set; }
	}
}
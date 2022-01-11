namespace IB.React.Core.Auth
{
	public static class JwtClaims
	{
		private const string PublicClaimBase = "https://ib.react.com/jwt_claims/";

		#region Default Claims

		/// <summary>
		/// 토큰 발행 대상자입니다.
		/// </summary>
		public const string Audience = "aud";

		/// <summary>
		/// 토큰 발행 주체입니다.
		/// </summary>
		public const string Issuer = "iss";

		/// <summary>
		/// 토큰이 발행된 시간입니다.
		/// </summary>
		public const string IssuedAt = "iat";

		/// <summary>
		/// 토큰 만료 시간입니다.
		/// </summary>
		public const string ExpirationTime = "exp";

		/// <summary>
		/// 토큰의 ID입니다.
		/// </summary>
		public const string JwtId = "jti";

		/// <summary>
		/// 토큰의 목적입니다.
		/// </summary>
		public const string Subject = "sub";
		

		#endregion
		
		#region Public Claims
		
		public const string Role = PublicClaimBase + "role";
		
		public const string UserNo = PublicClaimBase + "user_no";

		#endregion
	}
}
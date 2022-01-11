using IB.React.Core.Model.Database;

namespace IB.React.Core.Model.Auth
{
	/// <summary>
	/// 인증 결과 모델입니다.
	/// </summary>
	public class AuthenticationResult
	{
		/// <summary>
		/// 발행된 토큰 상세입니다.
		/// </summary>
		public TokenModel Token { get; set; }
		
		/// <summary>
		/// 로그인 유저의 기본 정보입니다.
		/// </summary>
		public UserModel User { get; set; }
	}
}
using System.Runtime.Serialization;

namespace IB.React.Core.Model.Auth
{
	/// <summary>
	/// JWT Token 데이터 모델입니다.
	/// </summary>
	[DataContract]
	public class TokenModel
	{
		/// <summary>
		/// AccessToken입니다.
		/// </summary>
		[DataMember] public string AccessToken { get; set; }
		
		/// <summary>
		/// RefreshToken입니다.
		/// </summary>
		[DataMember] public string RefreshToken { get; set; }
	}
}
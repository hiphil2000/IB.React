using System;
using System.Data;
using IB.React.Core.Database.Core;

namespace IB.React.Core.Model.Database
{
	/// <summary>
	/// 토큰 데이터 모델입니다.
	/// </summary>
	public class TokenModel
	{
		/// <summary>
		/// 토큰 ID입니다.
		/// </summary>
		public Guid TokenId { get; set; }
		
		/// <summary>
		/// 토큰 대상 사용자입니다.
		/// </summary>
		public long UserNo { get; set; }
		
		/// <summary>
		/// 토큰 타입입니다.
		/// </summary>
		public string Subject { get; set; }
		
		/// <summary>
		/// 토큰 생성일자입니다.
		/// </summary>
		public DateTime CreateDateTime { get; set; }
		
		/// <summary>
		/// 토큰 만료일자입니다.
		/// </summary>
		public DateTime ExpiryDateTime { get; set; }
		
		/// <summary>
		/// 토큰 사용안함 처리 여부입니다.
		/// </summary>
		public bool DestroyYn { get; set; }
		
		/// <summary>
		/// 토큰 사용안함 처리 일자입니다.
		/// </summary>
		public DateTime DestroyDateTime { get; set; }

		/// <summary>
		/// 토큰 시그니처 값입니다.
		/// </summary>
		public string Signature { get; set; }

		public TokenModel()
		{
		}

		public TokenModel(DataRow dr)
		{
			TokenId = dr.GetGuid("TokenId");
			UserNo = dr.GetLong("UserNo");
			Subject = dr.GetString("Subject");
			CreateDateTime = dr.GetDateTime("CreateDateTime");
			ExpiryDateTime = dr.GetDateTime("ExpiryDateTime");
			DestroyYn = dr.GetBool("DestroyYN");
			DestroyDateTime = dr.GetDateTime("DestroyDateTime");
			Signature = dr.GetString("Signature");
		}
	}
}
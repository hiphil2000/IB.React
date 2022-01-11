using System;
using System.Collections.Generic;
using IB.React.Core.Auth;

namespace IB.React.Core.Model.Auth
{
	public class JwtPayload
	{
		public Guid JwtId { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public long UserNo { get; set; }
		public string Role { get; set; }
		public DateTime IssuedAt { get; set; }
		public DateTime ExpirationTime { get; set; }
		public TokenType Subject { get; set; }

		public JwtPayload()
		{
		}

		public JwtPayload(Dictionary<string, object> payload)
		{
			try
			{
				// 잘못된 Payload라면, 초기화하지 않습니다.
				if (!(
					    Guid.TryParse(payload[JwtClaims.JwtId].ToString() ?? "", out var jwtId) &&
					    !string.IsNullOrEmpty(payload[JwtClaims.Issuer].ToString()) &&
					    !string.IsNullOrEmpty(payload[JwtClaims.Audience].ToString()) &&
					    long.TryParse(payload[JwtClaims.UserNo].ToString() ?? "", out var userNo) &&
					    long.TryParse(payload[JwtClaims.IssuedAt].ToString() ?? "", out var issuedAt) &&
					    long.TryParse(payload[JwtClaims.ExpirationTime].ToString() ?? "", out var expirationTime) &&
					    JwtService.Constants.ParseTokenType(payload[JwtClaims.Subject].ToString() ?? "") != TokenType.Unknown &&
					    !string.IsNullOrEmpty(payload[JwtClaims.Role].ToString())
				    ))
				{
					return;
				}

				JwtId = jwtId;
				Issuer = payload[JwtClaims.Issuer].ToString()!;
				Audience = payload[JwtClaims.Audience].ToString()!;
				UserNo = userNo;
				IssuedAt = ParseDateTime(issuedAt);
				ExpirationTime = ParseDateTime(expirationTime);
				Subject = JwtService.Constants.ParseTokenType(payload[JwtClaims.Subject].ToString() ?? "");
				Role = payload[JwtClaims.Role].ToString()!;
			} catch (Exception e)
			{
				// ignored		
			}
		}

		/// <summary>
		/// JavascriptTimestamp로 관리하는 DateTime을 변환합니다.
		/// </summary>
		/// <param name="javaTimestamp"></param>
		/// <returns></returns>
		private static DateTime ParseDateTime(long javaTimestamp)
		{
			var datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return datetime.AddSeconds(javaTimestamp);
		}
	}
}
using System;
using System.Data;
using System.Linq;
using IB.React.Core.Database.Core;
using IB.React.Core.Model.Auth;
using Microsoft.Extensions.Configuration;
using TokenModel = IB.React.Core.Model.Database.TokenModel;

namespace IB.React.Core.Database.Services
{
	public interface ITokenService
	{
		bool AddToken(long userNo, string tokenId, TokenType tokenType, DateTime issuedAt, DateTime expiredAt,
			string signature);

		bool RemoveToken(string tokenId);

		bool IsUsingToken(string tokenId);
	}

	public class TokenService : ITokenService
	{
		private readonly IConfiguration configuration;

		public TokenService(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		
		/// <summary>
		/// 토큰을 등록합니다.
		/// </summary>
		/// <param name="userNo">사용자 Id</param>
		/// <param name="tokenId">토큰 Id</param>
		/// <param name="tokenType">토큰 타입</param>
		/// <param name="issuedAt"></param>
		/// <param name="expiredAt"></param>
		/// <param name="signature"></param>
		/// <returns></returns>
		public bool AddToken(long userNo, string tokenId, TokenType tokenType, DateTime issuedAt, DateTime expiredAt,
			string signature)
		{
			try
			{
				using var db = new SqlService(configuration);
				
				db.AddParameter("@UserNo", DbType.Int32, userNo);
				db.AddParameter("@TokenId", DbType.String, tokenId);
				db.AddParameter("@Subject", DbType.String, tokenType);
				db.AddParameter("@IssuedAt", DbType.DateTime2, issuedAt);
				db.AddParameter("@ExpirationAt", DbType.DateTime2, expiredAt);
				db.AddParameter("@Signature", DbType.String, signature);

				_ = db.ExecuteQueryDataSet("uSP_AddToken", commandType: CommandType.StoredProcedure);
				
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		/// <summary>
		/// 토큰을 삭제처리합니다.
		/// </summary>
		/// <param name="tokenId">대상 토큰 Id</param>
		/// <returns></returns>
		public bool RemoveToken(string tokenId)
		{
			try
			{
				using var db = new SqlService(configuration);

				db.AddParameter("@TokenId", DbType.String, tokenId);

				_ = db.ExecuteQueryDataSet("uSP_RemoveToken", commandType: CommandType.StoredProcedure);

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		/// <summary>
		/// DB에 해당 토큰이 사용중인지 여부를 확인합니다.
		/// </summary>
		/// <param name="tokenId">대상 토큰 Id</param>
		/// <returns></returns>
		public bool IsUsingToken(string tokenId)
		{
			TokenModel? found = null;

			try
			{
				using var db = new SqlService(configuration);

				db.AddParameter("@TokenId", DbType.String, tokenId);

				var query = db.ExecuteQueryDataSet("uSP_GetToken", commandType: CommandType.StoredProcedure);

				if (query.HasData())
				{
					found = query.Tables[0]
						.AsEnumerable()
						.Select(dr => new TokenModel(dr))
						.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				// ignored
			}

			return found != null;
		}
	}
}
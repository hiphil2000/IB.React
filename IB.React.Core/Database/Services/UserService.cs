using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IB.React.Core.Database.Core;
using IB.React.Core.Model.Database;
using Microsoft.Extensions.Configuration;

namespace IB.React.Core.Database.Services
{
	public interface IUserService
	{
		UserModel? GetUser(long userNo);
		IEnumerable<UserModel> GetUsers();
		UserModel? Login(string userId, string userPassword);
	}
	
	public class UserService : IUserService
	{
		private readonly IConfiguration configuration;

		public UserService(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		/// <summary>
		/// 특정 사용자를 조회합니다.
		/// </summary>
		/// <param name="userNo">사용자 고유번호</param>
		/// <returns></returns>
		public UserModel? GetUser(long userNo)
		{
			UserModel? result = null;

			try
			{
				using var db = new SqlService(configuration);
				db.AddParameter("@UserNo", DbType.Int64, userNo);

				var query = db.ExecuteQueryDataSet("uSP_User_GET", commandType: CommandType.StoredProcedure);
				
				if (query.HasData())
				{
					result = query.Tables[0]
						.AsEnumerable()
						.Select(r => new UserModel(r))
						.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				// ignored
			}

			return result;
		}

		/// <summary>
		/// 사용자 목록을 조회합니다.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<UserModel> GetUsers()
		{
			var result = new List<UserModel>();

			try
			{
				using var db = new SqlService(configuration);

				var query = db.ExecuteQueryDataSet("uSP_GetUsers", commandType: CommandType.StoredProcedure);

				if (query.HasData())
				{
					var users = query.Tables[0]
						.AsEnumerable()
						.Select(dr => new UserModel(dr))
						.ToList();

					result.AddRange(users);
				}
			}
			catch (Exception e)
			{
				// ignored
			}

			return result;
		}

		/// <summary>
		/// 로그인을 진행합니다.
		/// </summary>
		/// <param name="userId">사용자 ID</param>
		/// <param name="userPassword">SHA256으로 해싱된 사용자 비밀번호</param>
		/// <returns></returns>
		public UserModel? Login(string userId, string userPassword)
		{
			UserModel? result = null;

			try
			{
				using var db = new SqlService(configuration);
				db.AddParameter("@UserId", DbType.String, userId);
				db.AddParameter("@Password", DbType.String, userPassword);

				var query = db.ExecuteQueryDataSet("uSP_Login", commandType: CommandType.StoredProcedure);

				if (query.HasData())
				{
					result = query.Tables[0]
						.AsEnumerable()
						.Select(r => new UserModel(r))
						.First();
				}
			}
			catch (Exception e)
			{
				// ignored
			}

			return result;
		}
	}
}
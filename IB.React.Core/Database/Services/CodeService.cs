using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IB.React.Core.Database.Core;
using IB.React.Core.Model.Database;
using Microsoft.Extensions.Configuration;

namespace IB.React.Core.Database.Services
{
	public interface ICodeService
	{
		IEnumerable<CodeGroupModel> GetGroups();
		CodeGroupModel? GetGroup(string groupId);
		IEnumerable<CodeModel> GetCodes(string groupId);
		CodeModel? GetCode(string groupId, string codeId);
	}
	
	public class CodeService : ICodeService
	{
		private readonly IConfiguration configuration;

		public CodeService(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		/// <summary>
		/// 코드 그룹 목록을 조회합니다.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CodeGroupModel> GetGroups()
		{
			var result = new List<CodeGroupModel>();

			try
			{
				using var db = new SqlService(configuration);

				var rs = db.ExecuteQueryDataSet("uSP_CodeGroup_GET", commandType: CommandType.StoredProcedure);

				if (rs.HasData())
				{
					result.AddRange(rs.Tables[0]
						.AsEnumerable()
						.Select(dr => new CodeGroupModel(dr))
						.ToList()
					);
				}
			}
			catch (Exception e)
			{
				// ignored
			}

			return result;
		}

		/// <summary>
		/// 특정 코드 그룹을 조회합니다.
		/// </summary>
		/// <param name="groupId">조회할 그룹 ID</param>
		/// <returns></returns>
		public CodeGroupModel? GetGroup(string groupId)
		{
			
			CodeGroupModel? result = null;

			try
			{
				using var db = new SqlService(configuration);
				db.AddParameter("@GroupId", DbType.String, groupId);

				var rs = db.ExecuteQueryDataSet("uSP_CodeGroup_GET", commandType: CommandType.StoredProcedure);

				if (rs.HasData())
				{
					result = rs.Tables[0]
						.AsEnumerable()
						.Select(dr => new CodeGroupModel(dr))
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
		/// 그룹의 코드 목록을 조회합니다.
		/// </summary>
		/// <param name="groupId">조회할 그룹 ID</param>
		/// <returns></returns>
		public IEnumerable<CodeModel> GetCodes(string groupId)
		{
			var result = new List<CodeModel>();

			try
			{
				using var db = new SqlService(configuration);
				db.AddParameter("@GroupId", DbType.String, groupId);

				var rs = db.ExecuteQueryDataSet("uSP_Code_GET", commandType: CommandType.StoredProcedure);

				if (rs.HasData())
				{
					result.AddRange(rs.Tables[0]
						.AsEnumerable()
						.Select(dr => new CodeModel(dr))
						.ToList()
					);
				}
			}
			catch (Exception e)
			{
				// ignored
			}

			return result;
		}

		/// <summary>
		/// 특정 공통 코드를 조회합니다.
		/// </summary>
		/// <param name="groupId">그룹 ID</param>
		/// <param name="codeId">코드 ID</param>
		/// <returns></returns>
		public CodeModel? GetCode(string groupId, string codeId)
		{
			CodeModel? result = null;

			try
			{
				using var db = new SqlService(configuration);
				db.AddParameter("@GroupId", DbType.String, groupId);
				db.AddParameter("@CodeId", DbType.String, codeId);

				var rs = db.ExecuteQueryDataSet("uSP_Code_GET", commandType: CommandType.StoredProcedure);

				if (rs.HasData())
				{
					result = rs.Tables[0]
						.AsEnumerable()
						.Select(dr => new CodeModel(dr))
						.FirstOrDefault();
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
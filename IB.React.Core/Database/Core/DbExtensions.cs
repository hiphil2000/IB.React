using System;
using System.Data;

namespace IB.React.Core.Database.Core
{
	public static class DbExtensions
	{
		/// <summary>
		/// DB Boolean 값을 조회합니다.
		/// Y: true, else: false
		/// </summary>
		/// <param name="dr">데이터 Row</param>
		/// <param name="column">데이터 컬럼</param>
		/// <returns></returns>
		public static bool GetBool(this DataRow dr, string column)
		{
			return (dr[column].ToString() ?? "").ToUpper().Equals("Y");
		}

		/// <summary>
		/// DB Int32 값을 조회합니다.
		/// </summary>
		/// <param name="dr">데이터 Row</param>
		/// <param name="column">데이터 컬럼</param>
		/// <param name="fallbackValue">파싱 실패 시 사용할 값</param>
		/// <returns></returns>
		public static int GetInt(this DataRow dr, string column, int fallbackValue = 0)
		{
			var success = int.TryParse((dr[column].ToString() ?? ""), out var parsed);

			return success ? parsed : fallbackValue;
		}
		
		/// <summary>
		/// DB Int64 값을 조회합니다.
		/// </summary>
		/// <param name="dr">데이터 Row</param>
		/// <param name="column">데이터 컬럼</param>
		/// <param name="fallbackValue">파싱 실패 시 사용할 값</param>
		/// <returns></returns>
		public static long GetLong(this DataRow dr, string column, long fallbackValue = 0)
		{
			var success = long.TryParse((dr[column].ToString() ?? ""), out var parsed);

			return success ? parsed : fallbackValue;
		}

		/// <summary>
		/// DB Guid 값을 조회합니다.
		/// </summary>
		/// <param name="dr">데이터 Row</param>
		/// <param name="column">데이터 컬럼</param>
		/// 
		/// <returns></returns>
		public static Guid GetGuid(this DataRow dr, string column)
		{
			var success = Guid.TryParse(dr[column].ToString() ?? "", out var parsed);

			return success ? parsed : Guid.Empty;
		}

		/// <summary>
		/// DB String 값을 조회합니다.
		/// </summary>
		/// <param name="dr">데이터 Row</param>
		/// <param name="column">데이터 컬럼</param>
		/// <returns></returns>
		public static string? GetString(this DataRow dr, string column)
		{
			return dr[column].ToString() ?? null;
		}

		public static DateTime GetDateTime(this DataRow dr, string column)
		{
			var success = DateTime.TryParse(dr.GetString(column), out var parsed);

			return success ? parsed : DateTime.MinValue;
		}

		/// <summary>
		/// DataSet에 데이터가 있는지 검사합니다.
		/// </summary>
		/// <param name="ds">검사할 DataSet</param>
		/// <returns></returns>
		public static bool HasData(this DataSet ds)
		{
			return ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Count > 0;
		}

		public static TEnum GetEnum<TEnum>(this DataRow dr, string column)
		{
			var success = Enum.TryParse(typeof(TEnum), dr.GetString(column), out var parsed);

			return (TEnum)parsed;
		}
	}
}
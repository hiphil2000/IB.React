using System.Data;
using IB.React.Core.Database.Core;
using Newtonsoft.Json;

namespace IB.React.Core.Model.Database
{
	public class UserModel
	{
		/// <summary>
		/// 사용자의 고유 번호입니다.
		/// </summary>
		public long UserNo { get; set; }
		
		/// <summary>
		/// 사용자의 로그인 ID입니다.
		/// </summary>
		public string UserId { get; set; }
		
		/// <summary>
		/// 사용자의 로그인 비밀번호입니다.
		/// </summary>
		[JsonIgnore]
		public string UserPassword { get; set; }
		
		/// <summary>
		/// 사용자의 이름입니다.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 사용자의 역할입니다.
		/// </summary>
		public string Role { get; set; }

		public UserModel()
		{
			
		}

		public UserModel(DataRow row)
		{
			UserNo = row.GetLong("UserNo");
			UserId = row.GetString("UserId");
			UserName = row.GetString("UserName");
			Role = row.GetString("Role");
		}
	}
}
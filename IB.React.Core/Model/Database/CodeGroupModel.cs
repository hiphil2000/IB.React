using System.Data;
using IB.React.Core.Database.Core;

namespace IB.React.Core.Model.Database
{
	/// <summary>
	/// 공통코드 그룹 모델입니다.
	/// </summary>
	public class CodeGroupModel
	{
		/// <summary>
		/// 그룹 ID입니다.
		/// </summary>
		public string GroupId { get; set; }
		
		/// <summary>
		/// 그룹 이름입니다.
		/// </summary>
		public string GroupName { get; set; }
		
		/// <summary>
		/// 그룹 설명입니다.
		/// </summary>
		public string Description { get; set; }
		
		/// <summary>
		/// 그룹 사용 여부입니다.
		/// </summary>
		public bool UseYn { get; set; }

		public CodeGroupModel()
		{
			
		}

		public CodeGroupModel(DataRow dr)
		{
			GroupId = dr.GetString("GroupId");
			GroupName = dr.GetString("GroupName");
			Description = dr.GetString("Description");
			UseYn = dr.GetBool("UseYN");
		}
	}
}
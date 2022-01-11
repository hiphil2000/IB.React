using System.Data;
using IB.React.Core.Database.Core;

namespace IB.React.Core.Model.Database
{
	/// <summary>
	/// 공통코드 데이터 모델입니다.
	/// </summary>
	public class CodeModel
	{
		/// <summary>
		/// 공통코드 ID입니다.
		/// </summary>
		public string CodeId { get; set; }
		
		/// <summary>
		/// 공통코드 그룹 ID입니다.
		/// </summary>
		public string GroupId { get; set; }
		
		/// <summary>
		/// 공통코드 이름입니다.
		/// </summary>
		public string CodeName { get; set; }
		
		/// <summary>
		/// 공통코드 설명입니다.
		/// </summary>
		public string Description { get; set; }
		
		/// <summary>
		/// 공통코드 사용 여부입니다.
		/// </summary>
		public bool UseYn { get; set; }

		public CodeModel()
		{
			
		}

		public CodeModel(DataRow dr)
		{
			CodeId = dr.GetString("CodeId");
			GroupId = dr.GetString("GroupId");
			CodeName = dr.GetString("CodeName");
			Description = dr.GetString("Description");
			UseYn = dr.GetBool("UseYn");
		}
	}
}
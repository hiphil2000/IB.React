using System.ComponentModel;

namespace IB.React.Core.Utility
{
	/// <summary>
	/// Enum 관련 작업 헬퍼 클래스입니다.
	/// </summary>
	public static class EnumHelper
	{
		/// <summary>
		/// Enum에 설정된 Description Attribute 값을 조회합니다.
		/// </summary>
		/// <param name="value"></param>
		/// <typeparam name="TEnum"></typeparam>
		/// <returns></returns>
		public static string GetStringValue<TEnum>(this TEnum value)
		{
			var attributes = (DescriptionAttribute[])value
				.GetType()
				.GetField(value.ToString() ?? string.Empty)
				?.GetCustomAttributes(typeof(DescriptionAttribute), false);

			return attributes?.Length > 0 ? attributes[0].Description : string.Empty;
		}
	}
}
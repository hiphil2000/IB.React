namespace IB.React.Core.Model
{
	/// <summary>
	/// 기본 응답 데이터 클래스입니다.
	/// </summary>
	/// <typeparam name="TData">응답 내부 데이터 타입</typeparam>
	public class CommonResponse<TData>
	{
		public bool Success { get; set; }
		
		public TData Data { get; set; }
		
		public string Message { get; set; }
	}
}
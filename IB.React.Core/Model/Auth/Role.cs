using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IB.React.Core.Model.Auth
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Role
	{
		Anonymous,
		User,
		Admin
	}
}
using Microsoft.Extensions.Configuration;

namespace IB.React.Core.Database.Core
{
	public static class ConnectionStringExtensions
	{
		public static ConnectionStringCollection ConnectionStrings(this IConfiguration configuration,
			string section = "ConnectionStrings")
		{
			var collection = configuration.GetSection(section).Get<ConnectionStringCollection>();
			return collection ?? new ConnectionStringCollection();
		}

		public static ConnectionStringItem ConnectionString(this IConfiguration configuration, string name,
			string section = "ConnectionStrings")
		{
			var collection = configuration.ConnectionStrings();
			return !collection.TryGetValue(name, out var item) ? null : item;
		}
	}
}
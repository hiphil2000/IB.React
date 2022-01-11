namespace IB.React.Core.Database.Core
{
	public class ConnectionStringItem
	{
		public string Name { get; set; }
		public string ConnectionString { get; set; }
		public string? ProviderName { get; set; }

		public ConnectionStringItem()
		{
			
		}

		public ConnectionStringItem(string name, string connectionString, string? providerName = null)
		{
			Name = name;
			ConnectionString = connectionString;
			ProviderName = providerName;
		}
	}
}
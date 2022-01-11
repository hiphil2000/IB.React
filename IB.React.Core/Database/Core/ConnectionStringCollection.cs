using System.Collections;
using System.Collections.Generic;

namespace IB.React.Core.Database.Core
{
	public class ConnectionStringCollection : IDictionary<string, ConnectionStringItem>
	{
		private readonly Dictionary<string, ConnectionStringItem> collection;

		public ConnectionStringCollection()
		{
			collection = new Dictionary<string, ConnectionStringItem>();
		}

		public ConnectionStringCollection(int capacity)
		{
			collection = new Dictionary<string, ConnectionStringItem>(capacity);
		}

		#region IEnumerable Implements

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable<> Implements

		public IEnumerator<KeyValuePair<string, ConnectionStringItem>> GetEnumerator()
		{
			return collection.GetEnumerator();
		}

		#endregion

		#region ICollection Implements

		public void Add(KeyValuePair<string, ConnectionStringItem> item)
		{
			((ICollection<KeyValuePair<string, ConnectionStringItem>>)collection).Add(item);
		}

		public void Clear()
		{
			((ICollection<KeyValuePair<string, ConnectionStringItem>>)collection).Clear();
		}

		public bool Contains(KeyValuePair<string, ConnectionStringItem> item)
		{
			return ((ICollection<KeyValuePair<string, ConnectionStringItem>>)collection).Contains(item);
		}

		public void CopyTo(KeyValuePair<string, ConnectionStringItem>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, ConnectionStringItem>>)collection).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, ConnectionStringItem> item)
		{
			return ((ICollection<KeyValuePair<string, ConnectionStringItem>>)collection).Remove(item);
		}

		public int Count => ((ICollection<KeyValuePair<string, ConnectionStringItem>>)collection).Count;
		public bool IsReadOnly => ((ICollection<KeyValuePair<string, ConnectionStringItem>>)collection).IsReadOnly;
		

		#endregion
		
		#region IDictionary Implements
		
		public void Add(string key, ConnectionStringItem value)
		{
			collection.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return collection.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return collection.Remove(key);
		}

		public bool TryGetValue(string key, out ConnectionStringItem value)
		{
			return collection.TryGetValue(key, out value);
		}

		public ConnectionStringItem this[string key]
		{
			get => collection[key];
			set => collection[key] = value;
		}

		public ICollection<string> Keys => collection.Keys;
		public ICollection<ConnectionStringItem> Values => collection.Values;

		#endregion

	}
}
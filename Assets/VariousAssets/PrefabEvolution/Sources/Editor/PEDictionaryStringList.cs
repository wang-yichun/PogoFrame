using System.Collections.Generic;
using System.Linq;

namespace PrefabEvolution
{
	[System.Serializable]
	class PEDictionaryStringList
	{
		[System.Serializable]
		public class Element
		{
			public string key;
			public List<string> value;
		}

		public List<Element> list = new List<Element>();

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		void Add(string key, List<string> value)
		{
			list.Add(new Element { key = key, value = value });
		}

		public List<string> this[string key]
		{
			get
			{
				var l = list.FirstOrDefault(i => i.key == key);
				return l == null ? this[key] = new List<string>() : l.value;
			}
			set
			{
				var l = list.FirstOrDefault(i => i.key == key);

				if (l == null)
				{
					Add(key, value);
					return;
				}
				l.value = value;
			}
		}
	}
}

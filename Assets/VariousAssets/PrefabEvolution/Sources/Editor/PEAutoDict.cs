using System.Collections.Generic;

class PEAutoDict<TKey, TValue>
{
	private Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

	private TValue defaultValue;
	public PEAutoDict(TValue defaultValue)
	{
		this.defaultValue = defaultValue;
	}

	public PEAutoDict()
	{
		this.defaultValue = default(TValue);
	}

	public TValue this[TKey key]
	{
		get
		{
			TValue result;
			if (dict.TryGetValue(key, out result))
				return result;
			return defaultValue;
		}
		set
		{
			if (dict.ContainsKey(key))
				dict[key] = value;
			else
				dict.Add(key, value);
		}
	}
}
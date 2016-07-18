namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Utilities;
	using Newtonsoft.Json.Linq;

	public class AssetsPathRootInfoListConverter : JsonConverter
	{
		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue (JsonConvert.SerializeObject (value, Formatting.None));
		}

		public override object ReadJson (JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize (reader, objectType);
		}

		public override bool CanConvert (System.Type objectType)
		{
			return objectType == typeof(AssetsPathRootInfo);
		}
	}
}
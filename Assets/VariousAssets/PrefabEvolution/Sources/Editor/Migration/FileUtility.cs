using System;

namespace PrefabEvolution.Migration
{
	static class FileUtility
	{
		static public byte[] ReadAllBytes(string path)
		{
			using (var file = System.IO.File.OpenRead(path))
			{
				var result = new byte[file.Length];
				file.Read(result, 0, result.Length);
				return result;
			}
		}

		static public string ReadAllText(string path, System.Text.Encoding encoding = null)
		{
			if (encoding == null)
				encoding = System.Text.Encoding.UTF8;

			return encoding.GetString(ReadAllBytes(path));
		}

		static public void WriteAllText(string path, string text, System.Text.Encoding encoding = null)
		{
			if (encoding == null)
				encoding = System.Text.Encoding.UTF8;

			WriteAllBytes(path, encoding.GetBytes(text));
		}

		static public void WriteAllBytes(string path, byte[] data)
		{
			using (var file = System.IO.File.Create(path))
			{
				file.Write(data, 0, data.Length);
			}
		}
	}
}





namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System;
	using System.IO;
	using Newtonsoft.Json;
	using System.Net;
	using System.Net.FtpClient;
	using System.Threading;

	public class FilesCopyWithFTP
	{
		static ManualResetEvent m_reset = new ManualResetEvent (false);

		public static void uploadDictionary (FtpClient client, string localDictionary, string remotePath, string remoteHost = "", string remotePort = "", string userName = "", string password = "")
		{
			if (client == null) {
				// 连接 FTP
				if (string.IsNullOrEmpty (remotePort)) {
					remotePort = "21";
				}

				using (FtpClient conn = new FtpClient ()) {
					m_reset.Reset ();

					conn.Host = remoteHost;
					conn.Port = Convert.ToInt32 (remotePort);
					conn.Credentials = new NetworkCredential (userName, password);
					conn.BeginDeleteDirectory (remotePath, true, new AsyncCallback (DeleteDirectoryCallback), conn);

					m_reset.WaitOne ();

					uploadDictionary (conn, localDictionary, remotePath);

					conn.Disconnect ();
				}

			} else {

				if (!client.DirectoryExists (remotePath)) {
					client.CreateDirectory (remotePath);
				}

				Debug.Log (remotePath);


				//拷贝文件
				//获取所有文件名称
				string[] fileName = Directory.GetFiles (localDictionary);

				foreach (string filePath in fileName) {
					//根据每个文件名称生成对应的目标文件名称
					string filePathTemp = remotePath + "/" + filePath.Substring (localDictionary.Length + 1);

					UploadFiles (client, filePath, filePathTemp);
				}


				//拷贝子目录       
				//获取所有子目录名称
				string[] directionName = Directory.GetDirectories (localDictionary);

				foreach (string directionPath in directionName) {
					//根据每个子目录名称生成对应的目标子目录名称
					string tempRemotePath = remotePath + "/" + directionPath.Substring (localDictionary.Length + 1);

					//递归下去
					uploadDictionary (client, directionPath, tempRemotePath);
				}

			}
		}

		static void DeleteDirectoryCallback (IAsyncResult ar)
		{
			FtpClient conn = ar.AsyncState as FtpClient;

			try {
				if (conn == null)
					throw new InvalidOperationException ("The FtpControlConnection object is null!");

				conn.EndDeleteDirectory (ar);
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
			} finally {
				m_reset.Set ();
			}
		}

		static void UploadFiles (FtpClient client, string localFile, string remoteFile)
		{
			Debug.Log ("UploadFiles: " + localFile + " -> " + remoteFile);

			using ( 
				Stream istream = new FileStream (localFile, FileMode.Open, FileAccess.Read),
				ostream = client.OpenWrite (remoteFile, FtpDataType.ASCII)) {

				byte[] buf = new byte[8192];
				int read = 0;

				try {
					while ((read = istream.Read (buf, 0, buf.Length)) > 0) {
						ostream.Write (buf, 0, read);
					}
				} finally {
					ostream.Close ();
					istream.Close ();
				}

				if (client.HashAlgorithms != FtpHashAlgorithm.NONE) {
					Debug.Assert (client.GetHash (remoteFile).Verify (localFile), "The computed hashes don't match!");
				}
			}
		}
	}
}
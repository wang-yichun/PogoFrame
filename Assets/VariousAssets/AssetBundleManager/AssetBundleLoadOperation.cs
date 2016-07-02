using UnityEngine;
using System.Collections;

namespace AssetBundles
{
	public abstract class AssetBundleLoadOperation : IEnumerator
	{
		public object Current {
			get {
				return null;
			}
		}

		public bool MoveNext ()
		{
			return !IsDone ();
		}

		public void Reset ()
		{
		}

		abstract public bool Update ();

		abstract public bool IsDone ();
	}
	
	#if UNITY_EDITOR
	public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
	{
		AsyncOperation m_Operation = null;

	
		public AssetBundleLoadLevelSimulationOperation (string assetBundleName, string levelName, bool isAdditive)
		{
			string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, levelName);
			if (levelPaths.Length == 0) {
				///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
				//        from that there right scene does not exist in the asset bundle...
				
				Debug.LogError ("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
				return;
			}
			
			if (isAdditive)
				m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode (levelPaths [0]);
			else
				m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode (levelPaths [0]);
		}

		public override bool Update ()
		{
			return false;
		}

		public override bool IsDone ()
		{		
			return m_Operation == null || m_Operation.isDone;
		}
	}
	
	#endif
	public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
	{
		protected string m_AssetBundleName;
		protected string m_LevelName;
		protected bool m_IsAdditive;
		protected string m_DownloadingError;
		protected AsyncOperation m_Request;

		protected int m_url_id;

		public AssetBundleLoadLevelOperation (string assetbundleName, string levelName, bool isAdditive)
		{
			m_AssetBundleName = assetbundleName;
			m_LevelName = levelName;
			m_IsAdditive = isAdditive;
		}

		public override bool Update ()
		{
			if (m_Request != null)
				return false;
			
			LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
			if (bundle != null) {
				if (m_IsAdditive)
//					m_Request = Application.LoadLevelAdditiveAsync (m_LevelName);
				m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
				else
//					m_Request = Application.LoadLevelAsync (m_LevelName);
					m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
				return false;
			} else
				return true;
		}

		public override bool IsDone ()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null && m_DownloadingError != null) {
				Debug.LogError (m_DownloadingError);
				return true;
			}
			
			return m_Request != null && m_Request.isDone;
		}
	}

	public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
	{
		public abstract T GetAsset<T> () where T : UnityEngine.Object;
	}

	public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
	{
		Object m_SimulatedObject;

		public AssetBundleLoadAssetOperationSimulation (Object simulatedObject)
		{
			m_SimulatedObject = simulatedObject;
		}

		public override T GetAsset<T> ()
		{
			return m_SimulatedObject as T;
		}

		public override bool Update ()
		{
			return false;
		}

		public override bool IsDone ()
		{
			return true;
		}
	}

	public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
	{
		protected string m_AssetBundleName;
		protected string m_AssetName;
		protected string m_DownloadingError;
		protected System.Type m_Type;
		protected AssetBundleRequest	m_Request = null;

		protected string m_url_id;

		public AssetBundleLoadAssetOperationFull (string bundleName, string assetName, System.Type type, string url_id)
		{
			m_AssetBundleName = bundleName;
			m_AssetName = assetName;
			m_Type = type;
			m_url_id = url_id;
		}

		public override T GetAsset<T> ()
		{
			if (m_Request != null && m_Request.isDone)
				return m_Request.asset as T;
			else
				return null;
		}
		
		// Returns true if more Update calls are required.
		public override bool Update ()
		{
			if (m_Request != null)
				return false;
	
			LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
			if (bundle != null) {
				///@TODO: When asset bundle download fails this throws an exception...
				m_Request = bundle.m_AssetBundle.LoadAssetAsync (m_AssetName, m_Type);
				return false;
			} else {
				return true;
			}
		}

		public override bool IsDone ()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null && m_DownloadingError != null) {
				Debug.LogError (m_DownloadingError);
				return true;
			}
	
			return m_Request != null && m_Request.isDone;
		}
	}

	public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
	{
		public AssetBundleLoadManifestOperation (string bundleName, string assetName, System.Type type, string url_id)
			: base (bundleName, assetName, type, url_id)
		{
		}

		public override bool Update ()
		{
			base.Update ();
			
			if (m_Request != null && m_Request.isDone) {
				AssetBundleManager.SetAssetBundleManifestObject (m_url_id, GetAsset<AssetBundleManifest> ());
//				if (m_url_id == 0) {
//					AssetBundleManager.AssetBundleManifestObject = GetAsset<AssetBundleManifest> ();
//				} else {
//					AssetBundleManager.AssetBundleManifestObject2 = GetAsset<AssetBundleManifest> ();
//				}
				return false;
			} else
				return true;
		}
	}
	
	
}

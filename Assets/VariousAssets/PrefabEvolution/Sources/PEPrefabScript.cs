using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PrefabEvolution
{
	[SelectionBase]
	[AddComponentMenu("")]
	public class PEPrefabScript : MonoBehaviour, ISerializationCallbackReceiver
	{		
		[HideInInspector]
		public PEExposedProperties Properties = Utils.Create<PEExposedProperties>();
		[HideInInspector]
		public PELinkage Links = Utils.Create<PELinkage>();
		[HideInInspector]
		public PEModifications Modifications;

		public string ParentPrefabGUID;

		public string PrefabGUID;

		private PrefabInternalData _prefabInternalData;

		public GameObject ParentPrefab
		{
			get
			{
				return EditorBridge.GetAssetByGuid(ParentPrefabGUID);
			}
			set
			{
				var guid = EditorBridge.GetAssetGuid(value);
				if (!string.IsNullOrEmpty(guid))
					ParentPrefabGUID = guid;
			}
		}
		public GameObject Prefab
		{
			get
			{
				return  EditorBridge.GetAssetByGuid(PrefabGUID);
			}
			set
			{
				var guid = EditorBridge.GetAssetGuid(value);
				if (!string.IsNullOrEmpty(guid))
					PrefabGUID = guid;
			}
		}

		void OnValidate()
		{
#if UNITY_EDITOR
#if PE_STRIP
			if (Utils.IsBuildingPlayer)
			{
				_prefabInternalData = new PrefabInternalData(this);
				ClearInternalData();
			}
#endif
			if (!Utils.IsBuildingPlayer && PrefabGUID == "STRIPPED")
				Debug.LogError("Prefab internal data stripping error");
#endif
			this.hideFlags |= (HideFlags)32;
			if (EditorBridge.OnValidate != null)
				EditorBridge.OnValidate(this);
		}

		public event Action OnBuildModifications;

		public void InvokeOnBuildModifications()
		{
			if (OnBuildModifications != null)
				OnBuildModifications();
		}

		#region ISerializationCallbackReceiver implementation
		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			if (!Utils.IsBuildingPlayer && _prefabInternalData != null)
			{
				_prefabInternalData.Fill(this);
				_prefabInternalData = null;
			}
#endif
		}

		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			this.Properties.PrefabScript = this;
			this.Properties.InheritedProperties = null;
#endif
		}

		private void ClearInternalData()
		{
			Properties = null;
			Links = null;
			Modifications = null;
			ParentPrefabGUID = null;
			PrefabGUID = "STRIPPED";
		}

		#endregion

		static public class EditorBridge
		{
			public static Action<PEPrefabScript> OnValidate;
			public static Func<GameObject, string> GetAssetGuid;
			public static Func<string, GameObject> GetAssetByGuid;
		}

		private class PrefabInternalData
		{
			private readonly PEExposedProperties Properties;
			private readonly PELinkage Links;
			private readonly PEModifications Modifications;
			private readonly string ParentPrefabGUID;
			private readonly string PrefabGUID;

			public PrefabInternalData(PEPrefabScript script)
			{
				this.Properties = script.Properties;
				this.Links = script.Links;
				this.Modifications = script.Modifications;

				this.ParentPrefabGUID = script.ParentPrefabGUID;
				this.PrefabGUID = script.PrefabGUID;
			}

			public void Fill(PEPrefabScript script)
			{
				script.Properties = this.Properties;
				script.Links = this.Links;
				script.Modifications = this.Modifications;
				script.ParentPrefabGUID = this.ParentPrefabGUID;
				script.PrefabGUID = this.PrefabGUID;
			}
		}
	}

	static public class Utils
	{
		static public T Create<T>() where T : class, new()
		{
#if UNITY_EDITOR
			return new T();
#else
			return null;
#endif
		}

		static public bool IsBuildingPlayer
		{
			get
			{
#if UNITY_EDITOR
				return BuildPipeline.isBuildingPlayer;
#else
				return false;
#endif
			}
		}
	}
}
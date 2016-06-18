using UnityEngine;
using UnityEditor;
using System.Collections;

namespace AssetBundles
{
	public class AssetBundlesMenuItems
	{
		const string kSimulationMode = "Assets/AssetBundles/Simulation Mode";
	
		[MenuItem(kSimulationMode)]
		public static void ToggleSimulationMode ()
		{
			AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
		}
	
		[MenuItem(kSimulationMode, true)]
		public static bool ToggleSimulationModeValidate ()
		{
			Menu.SetChecked(kSimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
			return true;
		}
		
		[MenuItem ("Assets/AssetBundles/Build AssetBundles/ to AssetBundles")]
		static public void BuildAssetBundles ()
		{
			BuildScript.BuildAssetBundles();
		}

		[MenuItem ("Assets/AssetBundles/Build AssetBundles/ to Assets:StreamingAssets #&b")]
		static public void BuildAssetBundles_StreamingAssets ()
		{
			BuildScript.BuildAssetBundles_StreamingAssets();
		}
	}
}
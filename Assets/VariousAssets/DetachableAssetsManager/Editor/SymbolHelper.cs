namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using System.Linq;

	public static class SymbolHelper
	{
		public static List<BuildTargetGroup> targetGroup = new List<BuildTargetGroup> () {
			BuildTargetGroup.Standalone,
			BuildTargetGroup.iOS,
			BuildTargetGroup.Android
		};

		// add new symbol
		public static void AddNewSymbol (string symbol)
		{
			string new_symbol = symbol;
			foreach (BuildTargetGroup buildTarget in targetGroup) {
				string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (buildTarget);
				string[] symbols_list = symbols.Split (new char[]{ ';' });
				if (symbols_list.Contains (new_symbol) == false) {
					symbols += ";" + new_symbol;
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup (buildTarget, symbols); 
			}
		}

		public static bool ExistSymbol (string symbol)
		{
			int count = 0;
			foreach (BuildTargetGroup buildTarget in targetGroup) {
				string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (buildTarget);
				string[] symbols_list = symbols.Split (new char[]{ ';' });

				if (symbols_list.Contains (symbol)) {
					count++;
				}
			}

			if (count == targetGroup.Count) {
				return true;
			}
			return false;
		}

		public static void DeleteSymbol (string symbol)
		{
			foreach (BuildTargetGroup buildTarget in targetGroup) {
				string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (buildTarget);
				string[] symbols_list = symbols.Split (new char[]{ ';' });
				symbols_list = symbols_list.Where (_ => _ != symbol).ToArray ();
				symbols = string.Join (";", symbols_list);
				PlayerSettings.SetScriptingDefineSymbolsForGroup (buildTarget, symbols); 
			}
		}
	}
}
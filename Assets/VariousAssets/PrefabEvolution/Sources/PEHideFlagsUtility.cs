using UnityEngine;

namespace PrefabEvolution
{
	static class PEHideFlagsUtility
	{
		static internal void HideFlagsSet(this Object obj, HideFlags flags, bool value)
		{
			if (value)
				AddHideFlags(obj, flags);
			else
				RemoveHideFlags(obj, flags);
		}

		static internal void AddHideFlags(this Object obj, HideFlags flags)
		{
			obj.hideFlags = obj.hideFlags | flags;
		}

		static internal void RemoveHideFlags(this Object obj, HideFlags flags)
		{
			obj.hideFlags = obj.hideFlags & ~flags;
		}
	}
}


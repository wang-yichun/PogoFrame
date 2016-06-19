using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;

namespace PrefabEvolution
{
	#region Icons
	static public class PEResources
	{
		static internal Texture2D _icon;

		static internal Texture2D icon
		{
			get
			{
				if (_icon == null)
					_icon = Resources.Load<Texture2D>("Icon");

				return _icon;
			}
		}

		static internal Texture2D _addIcon;

		static internal Texture2D addIcon
		{
			get
			{
				if (_addIcon == null)
					_addIcon = Resources.Load<Texture2D>("AddIcon");

				return _addIcon;
			}
		}

		static internal Texture2D _editIcon;

		static internal Texture2D editIcon
		{
			get
			{
				if (_editIcon == null)
					_editIcon = Resources.Load<Texture2D>("EditIcon");

				return _editIcon;
			}
		}

		static internal Texture2D _removeIcon;

		static internal Texture2D removeIcon
		{
			get
			{
				if (_removeIcon == null)
					_removeIcon = Resources.Load<Texture2D>("RemoveIcon");

				return _removeIcon;
			}
		}
	}
	#endregion
	
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Malee;

public class Example : MonoBehaviour {

	public List<ExampleChild> list1;

	[Reorderable]
	public ExampleChildList list2;

	[System.Serializable]
	public class ExampleChild {

		public string name;
		public float value;
	}

	[System.Serializable]
	public class ExampleChildList : ReorderableArray<ExampleChild> {
	}
}

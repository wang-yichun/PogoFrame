using UnityEngine;
using System.Collections;
using StansAssets.Animation;




public static class SA_TweenExtensions  {


	public static void MoveTo(this GameObject go, Vector3 position, float time) {
		SA_ValuesTween tw = go.AddComponent<SA_ValuesTween>();
		tw.DestoryGameObjectOnComplete = false;
		tw.VectorTo(go.transform.position, position, time);
	}

}

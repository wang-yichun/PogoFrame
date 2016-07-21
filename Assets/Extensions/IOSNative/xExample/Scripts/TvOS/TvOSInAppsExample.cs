using UnityEngine;
using System.Collections;

public class TvOSInAppsExample : MonoBehaviour {

	// Use this for initialization
	public void Init () {
		Debug.Log("Init");
		//IOSMessage.Create("Init", "Init");
		PaymentManagerExample.init();
	}
	
	// Update is called once per frame
	public void Buy () {
		PaymentManagerExample.buyItem(PaymentManagerExample.SMALL_PACK);
	}
}

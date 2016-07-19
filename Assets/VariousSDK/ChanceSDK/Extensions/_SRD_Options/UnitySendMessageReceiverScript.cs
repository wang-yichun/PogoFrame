using UnityEngine;
using System.Collections;

public class UnitySendMessageReceiverScript : MonoBehaviour
{
	void JavaMessage (string message)
	{ 
		Debug.Log ("message from java: " + message); 
	}
}

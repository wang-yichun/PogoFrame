using UnityEngine;
using System.Collections;

public class Drifter : MonoBehaviour {

    public float initVelocityRange = 10f;
    public float initAngularVelocityRange = 5f;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-initVelocityRange * 0.5f, initVelocityRange * 0.5f), Random.Range(-initVelocityRange * 0.5f, initVelocityRange * 0.5f), 0f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, Random.Range(-initAngularVelocityRange * 0.5f, initAngularVelocityRange * 0.5f)
            );
	}
	
	// Update is called once per frame
	void Update () {
	    
	}


}

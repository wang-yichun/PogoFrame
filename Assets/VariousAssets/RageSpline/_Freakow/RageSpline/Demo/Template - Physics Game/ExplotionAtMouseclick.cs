using UnityEngine;

public class ExplotionAtMouseclick : MonoBehaviour {

    private Rigidbody[] rigidbodies;
    public float force;
    public float radius;

	// Use this for initialization
	void Start () {
        rigidbodies = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            //mousePos.z = 1.0f;
			mousePos.z = transform.position.z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            foreach (Rigidbody body in rigidbodies)
                body.AddExplosionForce(force, worldPos, radius);
        }
	}
}

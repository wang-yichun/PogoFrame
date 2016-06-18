using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Boundary : MonoBehaviour
{

	BoxCollider2D boxCollider2d;

	void Start ()
	{
		boxCollider2d = GetComponent<BoxCollider2D> ();
		float half_height = Camera.main.orthographicSize;
		float half_width = half_height * Camera.main.aspect;
		boxCollider2d.size = new Vector2 (half_width * 2, half_height * 2);
	}
}

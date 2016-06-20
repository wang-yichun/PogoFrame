using UnityEngine;
using System.Collections;
using System;

namespace uFrame.ExampleProject
{
	public enum BallType
	{
		BLUE,
		RED
	}

	public class BallControl : MonoBehaviour
	{
		Rigidbody2D rigidbody2d;

		[SerializeField]
		BallType type;

		public void GetLocalVars ()
		{
			rigidbody2d = GetComponent<Rigidbody2D> ();
			if (CompareTag ("BlueBall")) {
				type = BallType.BLUE;
			} else if (CompareTag ("RedBall")) {
				type = BallType.RED;
			}
		}

		[SerializeField]
		private bool isStandby;

		public bool IsStandby {
			set {
				isStandby = value;
				SetKinematic ();
			}
			get {
				return isStandby;
			}
		}

		public void SetKinematic ()
		{
			if (isStandby) {
				rigidbody2d.isKinematic = true;
			} else {
				rigidbody2d.isKinematic = false;
			}
		}

		Collider2D[] magnetScopes = new Collider2D[10];

		public void ApplyMagnetEffect ()
		{
			int layerMask = LayerMask.NameToLayer ("MagnetScopeBlue");

			int mCount = Physics2D.OverlapCircleNonAlloc (
				             this.transform.position,
				             .5f,
				             magnetScopes,
				             1 << layerMask
			             );

			for (int i = 0; i < mCount; i++) {
				Collider2D c2d = magnetScopes [i];

				if (c2d.CompareTag ("BlueMagnetScope")) {
					Vector3 targetPosition = c2d.transform.position;
					Vector3 sourcePosition = transform.position;

					rigidbody2d.AddForce ((Vector2)((targetPosition - sourcePosition).normalized * 2f), ForceMode2D.Force);
				}
			}

		}
	}
}
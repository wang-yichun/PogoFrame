using UnityEngine;
using System.Collections;
using System;

namespace uFrame.ExampleProject
{
	public class MagnetControl : MonoBehaviour
	{

		public enum MagnetType
		{
			BLUE,
			RED
		}

		[SerializeField]
		MagnetType type;

		CircleCollider2D collider;
		SpriteRenderer scopeSprite;

		public void GetLocalVars ()
		{
			collider = GetComponentInChildren<CircleCollider2D> ();
			scopeSprite = transform.FindChild ("Magnet_Scope").GetComponent<SpriteRenderer> ();

			if (CompareTag ("BlueMagnetScope")) {
				type = MagnetType.BLUE;
			} else if (CompareTag ("RedMagnetScope")) {
				type = MagnetType.RED;
			}
		}


		[SerializeField]
		private bool isStandby;

		public bool IsStandby {
			set {
				isStandby = value;
				SetEffectorEnable ();
			}
			get {
				return isStandby;
			}
		}

		[SerializeField]
		private bool isEffectOn;

		public bool IsEffectOn {
			set {
				isEffectOn = value;
				SetEffectorEnable ();
			}
			get {
				return isEffectOn;
			}
		}

		public void SetEffectorEnable ()
		{
			if (isStandby) {
				collider.usedByEffector = false;
			} else {
				if (isEffectOn) {
					collider.usedByEffector = true;
				} else {
					collider.usedByEffector = false;
				}
			}

			scopeSprite.enabled = collider.usedByEffector;
		}
	}
}
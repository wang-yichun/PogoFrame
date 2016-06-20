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
			RED,
			FINISHING
		}

		[SerializeField]
		MagnetType type;

		SpriteRenderer scopeSprite;

		public void GetLocalVars ()
		{
			scopeSprite = transform.FindChild ("Magnet_Scope").GetComponent<SpriteRenderer> ();

			if (scopeSprite.gameObject.CompareTag ("BlueMagnetScope")) {
				type = MagnetType.BLUE;
			} else if (scopeSprite.gameObject.CompareTag ("RedMagnetScope")) {
				type = MagnetType.RED;
			} else if (scopeSprite.gameObject.CompareTag ("FinishingMagnetScope")) {
				type = MagnetType.FINISHING;
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

		public bool FinallyEffectOn;

		public void SetEffectorEnable ()
		{
			if (type == MagnetType.FINISHING) {
				FinallyEffectOn = true;
			} else {

				if (isStandby) {
					FinallyEffectOn = true;
				} else {
					if (isEffectOn) {
						FinallyEffectOn = true;
					} else {
						FinallyEffectOn = false;
					}
				}
			}

			scopeSprite.enabled = FinallyEffectOn;
		}
	}
}
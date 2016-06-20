using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Linq;
using System.Linq;

namespace uFrame.ExampleProject
{
	public class LevelContainer : MonoBehaviour
	{
		public Transform LevelNode;

		public List<BallControl> BallControls;
		public List<MagnetControl> MagnetControls;

		public void SetLevelNode (Transform levelNode)
		{
			if (levelNode == null) {
				gameObject.Children ().Destroy ();
				BallControls.Clear ();
				this.LevelNode = null;
			} else {
				if (this.LevelNode != null) {
					this.LevelNode.gameObject.Destroy ();
				}
				levelNode.SetParent (this.transform);
				this.LevelNode = levelNode;

				AddScriptComponents ();
			}
		}

		public void AddScriptComponents ()
		{
			BallControls = new List<BallControl> ();

			GameObject[] gos = LevelNode.gameObject.DescendantsAndSelf ().ToArray<GameObject> ();
			for (int i = 0; i < gos.Length; i++) {
				GameObject go = gos [i];
				if (go.CompareTag ("BlueBall") || go.CompareTag ("RedBall")) {
					BallControl bc = go.AddComponent<BallControl> ();
					bc.GetLocalVars ();
					BallControls.Add (bc);
				} else if (go.CompareTag ("Magnet")) {
					MagnetControl mc = go.AddComponent<MagnetControl> ();
					mc.GetLocalVars ();
					MagnetControls.Add (mc);
				}
			}
		}

		public void SetBalls_Standby (bool value)
		{
			for (int i = 0; i < BallControls.Count; i++) {
				BallControl bc = BallControls [i];
				bc.IsStandby = value;
			}
		}

		public void SetBallMagnetEffect ()
		{
			for (int i = 0; i < BallControls.Count; i++) {
				BallControl bc = BallControls [i];
				bc.ApplyMagnetEffect ();
			}
		}

		public void SetMagnets_Standby (bool value)
		{
			for (int i = 0; i < MagnetControls.Count; i++) {
				MagnetControl bc = MagnetControls [i];
				bc.IsStandby = value;
				bc.IsEffectOn = false;
			}
		}
	}
}
////////////////////////////////////////////////////////////////////////////////
//  
// @module <module_name>
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using StansAssets.Animation;

public class SA_TweenedGameobject : MonoBehaviour  {
	/*

	private bool _IsTweenPlaying = false;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void AnimateOpacity(float valueFrom, float valueTo, float time) {
		AnimateOpacity(valueFrom, valueTo, time, iTween.EaseType.linear);
	}

	public void AnimateOpacity(float valueFrom, float valueTo, float time, iTween.EaseType ease) {
		SA_ValuesTween tw = SA_ValuesTween.Create();
		tw.MoveTo(valueFrom, valueTo, time, OnOpacityAnimationEvent, ease);
	}


	

	public float opacity {
		 get {
			return color.a;
		}
		
		set {
			OnOpacityAnimationEvent(value);
		}
	}
	


	public void LocalMoveTo(Vector3 pos, float time) {
		LocalMoveTo(pos, time, iTween.EaseType.linear);
	}

	public void LocalMoveTo(Vector3 pos, float time, iTween.EaseType ease) {
		_IsTweenPlaying = true;
		SA_ValuesTween tw = SA_ValuesTween.Create();
		tw.VectorTo(transform.localPosition, pos, time, OnLocalMoveEvent, ease);
		tw.OnComplete = OnTwenComplete;
	}

	public void LocalMoveTo(float x, float y, float z, float time, iTween.EaseType ease) {
		Vector3 pos = transform.localPosition;
		pos.x += x;
		pos.y += y;
		pos.z += z;
		LocalMoveTo(pos, time, ease);
	}
	
	public void LocalMoveToS(Vector3 pos, float speed) {
		LocalMoveToS(pos, speed, iTween.EaseType.linear);
	}

	public void LocalMoveToS(Vector3 pos, float speed, iTween.EaseType ease) {
		_IsTweenPlaying = true;
		SA_ValuesTween tw = SA_ValuesTween.Create();
		tw.VectorToS(transform.localPosition, pos, speed, OnLocalMoveEvent, ease);
		tw.OnComplete = OnTwenComplete;
	}

	public void LocalMoveToS(float x, float y, float z, float speed, iTween.EaseType ease) {
		Vector3 pos = transform.localPosition;
		pos.x += x;
		pos.y += y;
		pos.z += z;
		LocalMoveToS(pos, speed, ease);
	}


	public void LocalScaleTo(Vector3 pos, float time) {
		LocalScaleTo(pos, time, iTween.EaseType.linear);
	}

	public void LocalScaleTo(Vector3 pos, float time, iTween.EaseType ease) {
		_IsTweenPlaying = true;
		SA_ValuesTween tw = SA_ValuesTween.Create();
		tw.VectorTo(transform.localScale, pos, time, OnLocalScaleEvent, ease);
		tw.OnComplete = OnTwenComplete;
	}

	public void LocalRotatationTo(Vector3 rotation, float time) {
		LocalRotatationTo(rotation, time, iTween.EaseType.linear);
	}

	public void LocalRotatationTo(Vector3 rotation, float time, iTween.EaseType ease) {
		_IsTweenPlaying = true;
		SA_ValuesTween tw = SA_ValuesTween.Create();
		tw.VectorTo(transform.localRotation.eulerAngles, rotation, time, OnLocalRotationEvent, ease);
		tw.OnComplete = OnTwenComplete;
	}

	public void LocalRotatationToS(Vector3 rotation, float speed) {
		_IsTweenPlaying = true;
		SA_ValuesTween tw = SA_ValuesTween.Create();
		tw.VectorToS(transform.localRotation.eulerAngles, rotation, speed, OnLocalRotationEvent);
		tw.OnComplete = OnTwenComplete;
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public bool IsTweenPlaying {
		get {
			return _IsTweenPlaying;
		}
	}

	public Color color {
		get {
			
			Material m = GetComponent<Renderer>().sharedMaterial;
			if(m.HasProperty("_Color")) {
				return m.color;
			} else {
				if(m.HasProperty("_TintColor")) {
					return m.GetColor ("_TintColor");
				} else {
					return Color.white;
				}

			}
		}

		set {
			if(GetComponent<Renderer>().sharedMaterial.HasProperty("_Color")) {
				GetComponent<Renderer>().sharedMaterial.color = value;
			}  else {
				if(GetComponent<Renderer>().sharedMaterial.HasProperty ("_TintColor")) {
					GetComponent<Renderer>().sharedMaterial.SetColor ("_TintColor", value);
				}

			}
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	
	private void OnOpacityAnimationEvent(float val) {
		if(this == null) {
			return;
		}
		
		if(gameObject == null) {
			return;
		}
		
		Color c = color;
		c.a = val;
		color = c;
	}
	
	private void OnLocalRotationEvent(Vector3 r) {
		if(this == null) {
			return;
		}
		
		if(gameObject == null) {
			return;
		}
		
		transform.localRotation = Quaternion.Euler(r);
	}

	private void OnLocalMoveEvent(Vector3 r) {
		if(this == null) {
			return;
		}
		
		if(gameObject == null) {
			return;
		}
		
		transform.localPosition = r;
	}

	private void OnLocalScaleEvent(Vector3 r) {
		if(this == null) {
			return;
		}
		
		if(gameObject == null) {
			return;
		}
		
		transform.localScale = r;
	}


	protected virtual void OnTwenComplete() {
		_IsTweenPlaying = false;
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------
*/
}

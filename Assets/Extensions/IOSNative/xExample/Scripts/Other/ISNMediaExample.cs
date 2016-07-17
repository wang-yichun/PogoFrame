using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ISNMediaExample : BaseIOSFeaturePreview {




	void Awake() {

		ISN_MediaController.ActionQueueUpdated += HandleActionQueueUpdated;
		ISN_MediaController.ActionMediaPickerResult += HandleActionMediaPickerResult;

		ISN_MediaController.ActionPlaybackStateChanged += HandleActionPlaybackStateChanged;
		ISN_MediaController.ActionNowPlayingItemChanged += HandleActionNowPlayingItemChanged;

	}

	void HandleActionNowPlayingItemChanged (MP_MediaItem item) {
		ISN_Logger.Log("Now Playing Item Changed: " + ISN_MediaController.Instance.NowPlayingItem.Title);
	}

	void HandleActionPlaybackStateChanged (MP_MusicPlaybackState state) {
		ISN_Logger.Log("Playback State Changed: " + ISN_MediaController.Instance.State.ToString());
	}

	void HandleActionQueueUpdated (MP_MediaPickerResult res) {

		if(res.IsSucceeded) {
			foreach(MP_MediaItem item in res.Items) {
				ISN_Logger.Log("Item: " + item.Title + " / " + item.Id);
			}
		} else {
			ISN_Logger.Log("Queue Updated failed: " + res.Error.Description);
		}


	}

	void HandleActionMediaPickerResult (MP_MediaPickerResult res) {
		if(res.IsSucceeded) {
			ISN_Logger.Log("Media piacker Succeeded");
		} else {
			ISN_Logger.Log("Media piacker failed: " + res.Error.Description);
		}
	}



	void OnGUI() {
		UpdateToStartPos();


		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Selecting Songs", style);
		
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Picker")) {
			ISN_MediaController.Instance.ShowMediaPicker();
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Set Perviostly Picked Song")) {
			ISN_MediaController.Instance.Pause();
			ISN_Logger.Log(ISN_MediaController.Instance.CurrentQueue[0].Title);
			ISN_MediaController.Instance.SetCollection(ISN_MediaController.Instance.CurrentQueue[0]);
			ISN_MediaController.Instance.Play();
		}
		


		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Controling Playback", style);

		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Play")) {
			ISN_MediaController.Instance.Play();
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Pause")) {
			ISN_MediaController.Instance.Pause();
		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Next")) {
			ISN_MediaController.Instance.SkipToNextItem();
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Previous")) {
			ISN_MediaController.Instance.SkipToPreviousItem();
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Skip To Beginning")) {
			ISN_MediaController.Instance.SkipToBeginning();
		}


	}

}

//#define PICKER_API
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////




using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR && PICKER_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_FilePicker : SA_Singleton<ISN_FilePicker> {


	#if (UNITY_IPHONE && !UNITY_EDITOR && PICKER_API) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISN_PickImages(int maxItemsCount);

	[DllImport ("__Internal")]
	private static extern void _ISN_InitPicerAPI(float compressionRate, int maxSize, int encodingType);

	#endif


	//Actions
	public static event Action<ISN_FilePickerResult> MediaPickFinished = delegate{};


	//--------------------------------------
	// Initialization
	//--------------------------------------


	void Awake() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && PICKER_API) || SA_DEBUG_MODE
		_ISN_InitPicerAPI(IOSNativeSettings.Instance.JPegCompressionRate, IOSNativeSettings.Instance.MaxImageLoadSize, (int) IOSNativeSettings.Instance.GalleryImageFormat);
		#endif

		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	//Public Methods Events
	//--------------------------------------

	public void PickFromCameraRoll(int maxItemsCount = 0) {
		
		#if (UNITY_IPHONE && !UNITY_EDITOR && PICKER_API) || SA_DEBUG_MODE
		_ISN_PickImages(maxItemsCount);
		#endif
	}




	//--------------------------------------
	//Internal Events
	//--------------------------------------

	private void OnSelectImagesComplete(string data) {



		string[] DataArray = data.Split(new string[] { IOSNative.DATA_SPLITTER2 }, StringSplitOptions.None);
		ISN_FilePickerResult result = new ISN_FilePickerResult();




		if(data.Equals(string.Empty)) {
			MediaPickFinished(result);
			return;
		}


		for(int i = 0; i < DataArray.Length; i++) {
			if(DataArray[i] == IOSNative.DATA_EOF) {
				break;
			}

			string ImageData = DataArray[i];
			byte[] decodedFromBase64 = System.Convert.FromBase64String(ImageData);
			Texture2D image = new Texture2D(1, 1);
			image.LoadImage(decodedFromBase64);
			image.hideFlags = HideFlags.DontSave;

			result.PickedImages.Add(image);

		}

	

		MediaPickFinished(result);

	}
}

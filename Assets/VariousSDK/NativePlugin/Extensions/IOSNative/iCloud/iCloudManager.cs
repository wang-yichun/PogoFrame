//#define SA_DEBUG_MODE
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
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class iCloudManager : ISN_Singleton<iCloudManager> {
	

	//Actions
	public static event Action<ISN_Result> OnCloudInitAction = delegate {};
	public static event Action<iCloudData> OnCloudDataReceivedAction = delegate {};
	public static event Action<List<iCloudData>> OnStoreDidChangeExternally = delegate {};



	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _initCloud();

	[DllImport ("__Internal")]
	private static extern void _setString(string key, string val);

	[DllImport ("__Internal")]
	private static extern void _setDouble(string key, float val);
	
	[DllImport ("__Internal")]
	private static extern void _setData(string key, string val);

	[DllImport ("__Internal")]
	private static extern void _requestDataForKey(string key);
	#endif
	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void init() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_initCloud ();
		#endif
	}

	public void setString(string key, string val) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_setString(key, val);
		#endif
	}

	public void setFloat(string key, float val) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_setDouble(key, val);
		#endif
	}

	public void setData(string key, byte[] val) {


		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			string bytesString = System.Convert.ToBase64String (val);
			_setData(key, bytesString);
		#endif
	}

	public void requestDataForKey(string key) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_requestDataForKey(key);
		#endif
	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnCloudInit() {
		ISN_Result res =  new ISN_Result(true);
		OnCloudInitAction(res);
	}

	private void OnCloudInitFail() {
		ISN_Result res =  new ISN_Result(false);
		OnCloudInitAction(res);
	}

	private void OnCloudDataChanged(string data) {

		List<iCloudData> changedData =  new List<iCloudData>();

		string[] DataArray = data.Split(IOSNative.DATA_SPLITTER); 

		for(int i = 0; i < DataArray.Length; i += 2 ) {
			if(DataArray[i] == IOSNative.DATA_EOF) {
				break;
			}

			iCloudData pair =  new iCloudData(DataArray[i], DataArray[i + 1]);
			changedData.Add(pair);
		}

		OnStoreDidChangeExternally(changedData);

	}


	private void OnCloudData(string array) {
		string[] data;
		data = array.Split(IOSNative.DATA_SPLITTER); 

		iCloudData package = new iCloudData (data[0], data [1]);

		OnCloudDataReceivedAction(package);
	}

	private void OnCloudDataEmpty(string array) {
		string[] data;
		data = array.Split(IOSNative.DATA_SPLITTER); 

		iCloudData package = new iCloudData (data[0], "null");


		OnCloudDataReceivedAction(package);
	}



	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}

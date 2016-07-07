
using UnityEngine;
using System;
#if !UNITY_WEBGL && !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) || UNITY_EDITOR
using System.Threading;
using System.IO;
 #endif
using System.Collections;
using System.Collections.Generic;


#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
 using File = UnityEngine.Windows.File;
 #else
 using File = System.IO.File;
 #endif

#if NETFX_CORE
    #if UNITY_WSA_10_0
    using System.Threading.Tasks;
    using static System.IO.Directory;
    using static System.IO.File;
    using static System.IO.FileStream;
    #endif
#endif

public class benchmark : MonoBehaviour{
#if !UNITY_WEBPLAYER


    private int lzres = 0, zipres = 0, lz4res = 0, flzres = 0;
	#if !UNITY_WEBGL
		private int brres = 0;
	#endif

    private bool pass1, pass2;

    //for counting the time taken to decompress the 7z file.
    private float t1, tim;

    //the test file to download.
    private string myFile = "testimg2.7z", myFile2 = "testimg.zip", uncFile = "testimg.tif";

    //the adress from where we download our test file
    private string uri = "https://dl.dropboxusercontent.com/u/13373268/tests/";

    private WWW www, www2;

    private string ppath;

    private string log="";
	
	private bool downloadDone, benchmarkStarted;
	
	private long tsize;

	GUIStyle style;

    //A 1 item integer array to get the current extracted file of the 7z archive. Compare this to the total number of the files to get the progress %.
    private int[] progress = new int[1];
	private int[] progress1 = new int[1];
	private ulong[] progress2 = new ulong[1];
	private float[] progress3 = new float[1];
	#if !UNITY_WEBGL
		private int[] progress4 = new int[1];
	#endif
	private int[] bytes = new int[1];

    void  Start(){

		ppath = Application.persistentDataPath;

		//
		//we are setting the lzma.persitentDataPath so the get7zinfo, get7zSize, decode2Buffer functions can work on separate threads!
		//on WSA it must be 'Application.persistentDataPath' !
		lzma.persitentDataPath = Application.persistentDataPath;
		//


		#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			ppath=".";
		#endif

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

		if (!File.Exists(ppath + "/" + myFile)) StartCoroutine(Download7ZFile()); else downloadDone = true;

		benchmarkStarted = false;

		style = new GUIStyle ();
		style.richText = true;
		GUI.color = Color.black;
    }
	
	

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
	

    void OnGUI(){

        if (downloadDone){

			if(!benchmarkStarted) {
				if (GUI.Button(new Rect(10, 10, 170, 50), "start Benchmark (48 MB)")){
					benchmarkStarted = true;
					log = "";
					lzres = 0; zipres = 0; lz4res = 0; flzres = 0;
			
					StartCoroutine(decompressFunc());
				}
			}

        } 

		GUI.TextArea(new Rect(10, 70, Screen.width - 20, Screen.height - 170), log,style);


    }


	
    //call from separate thread. here you can get the progress of the extracted files through a referenced integer.
	IEnumerator decompressFunc(){

		//decompress 7zip
		log += "<color=white>decompressing 7zip ...\n</color>";
		t1 = Time.realtimeSinceStartup;
		lzres = lzma.doDecompress7zip(ppath + "/"+myFile , ppath + "/",  true,true);
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ lzres +" |  7z time: "+tim+" sec\n\n</color>";
		#if !UNITY_WSA && !NETFX_CORE || UNITY_EDITOR
		log += "<color=grey>compressing lzma ...\n</color>";
		#else
		log += "<color=grey>compressing zip ...\n</color>";
		#endif
		yield return true;

		//compress lzma alone
		#if !UNITY_WSA && !NETFX_CORE || UNITY_EDITOR
		t1 = Time.realtimeSinceStartup;
		lzma.setProps(9);
		if(File.Exists(ppath +"/"+ uncFile+".lzma")) File.Delete(ppath +"/"+ uncFile+".lzma");
		lzres = lzma.LzmaUtilEncode( ppath +"/"+ uncFile, ppath +"/"+ uncFile+".lzma");
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=grey>status: "+ lzres +" |  lzma time: "+tim+" sec\n\n</color>";
		log += "<color=grey>compressing zip ...\n</color>";
		yield return true;
		#endif

		//compress zip
		t1 = Time.realtimeSinceStartup;
		if(File.Exists(ppath + "/"+myFile2)) File.Delete(ppath + "/"+myFile2);
		zipres = lzip.compress_File(9, ppath + "/"+myFile2, ppath + "/"+uncFile);
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=grey>status: "+ zipres +" |  zip time: "+tim+" sec\n\n</color>";
		log += "<color=white>decompressing zip ...\n</color>";
		yield return true;
		
		//decompress zip
		t1 = Time.realtimeSinceStartup;
		zipres = lzip.decompress_File(ppath + "/"+myFile2, ppath+"/", progress, null, progress1);
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ zipres +" |  zip time: "+tim+" sec\n\n</color>";
		log += "<color=grey>Compressing to flz ...\n</color>";
		yield return true;

		//compress flz
		t1 = Time.realtimeSinceStartup;
		flzres = fLZ.compressFile(ppath+ "/" + uncFile, ppath + "/" + uncFile + ".flz", 2, true, progress2);
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=grey>status: "+ flzres +" |  flz time: </color><color=orange>"+tim+" sec\n\n</color>";
		log += "<color=white>Decompressing flz ...\n</color>";
		yield return true;

		//decompress flz
		t1 = Time.realtimeSinceStartup;
		flzres  = fLZ.decompressFile(ppath + "/" + uncFile + ".flz", ppath + "/" + uncFile , true,  progress2);
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ flzres +" |  flz time: "+tim+" sec\n\n</color>";
		log += "<color=grey>Compressing to LZ4 ...\n</color>";
		yield return true;

		//compress lz4
		t1 = Time.realtimeSinceStartup;
		lz4res = (int) LZ4.compress(ppath+ "/" + uncFile, ppath + "/" + uncFile + ".lz4", 9,  progress3);
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=grey>status: "+ lz4res +" |  LZ4 time: "+tim+" sec\n\n</color>";
		log += "<color=white>Decompressing LZ4 ...\n</color>";
		yield return true;

		//decompress lz4
		t1 = Time.realtimeSinceStartup;
		lz4res = LZ4.decompress(ppath + "/" + uncFile + ".lz4", ppath + "/" + uncFile ,  bytes);
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ lz4res +" |  LZ4 time: </color><color=lime>"+tim+" sec\n\n</color>";
		#if !UNITY_WEBGL
			log += "<color=grey>Compressing to brotli ...\n</color>";
		#endif
		yield return true;

		#if !UNITY_WEBGL
			//compress brotli
			t1 = Time.realtimeSinceStartup;
			brres = (int) brotli.compressFile(ppath+ "/" + uncFile, ppath + "/" + uncFile + ".br",   progress4);
			tim = Time.realtimeSinceStartup - t1;
			log += "<color=grey>status: "+ brres +" |  brotli time: "+tim+" sec\n\n</color>";
			log += "<color=white>Decompressing brotli ...\n</color>";
			yield return true;

			//decompress brotli
			t1 = Time.realtimeSinceStartup;
			brres = brotli.decompressFile(ppath + "/" + uncFile + ".br", ppath + "/" + uncFile ,   progress4);
			tim = Time.realtimeSinceStartup - t1;
			log += "<color=white>status: "+ brres +" |  brotli time: </color><color=lime>"+tim+" sec\n\n</color>";
			yield return true;

			//test setting file permissions
			Debug.Log(lzma.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
			Debug.Log(lzip.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
			Debug.Log(fLZ.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
			Debug.Log(LZ4.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
			Debug.Log(brotli.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
		#endif

		benchmarkStarted = false;
    }



    IEnumerator Download7ZFile(){
		
		Debug.Log("downloading 7zip file");
		log+="<color=white>downloading 7zip file ...\n</color>";
		//make sure a previous 7z file having the same name with the one we want to download does not exist in the ppath folder
		if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile);

        www = new WWW(uri + myFile);

		yield return www;
		if (www.error != null) {  Debug.Log(www.error); yield break; }
		downloadDone = true;
		log="";
        		 
		//write the downloaded 7z file to the ppath directory so we can have access to it
		//depending on the Install Location you have set for your app, set the Write Access accordingly!
		File.WriteAllBytes(ppath + "/" + myFile, www.bytes);
		//www.Dispose(); www = null;
	}


#else
        void Start(){
            Debug.Log("Does not work on WebPlayer!");
        }
#endif
}


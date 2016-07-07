using UnityEngine;
using System;
#if !UNITY_WEBGL && !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) || UNITY_EDITOR
using System.Threading;
using System.IO;
#endif
using System.Collections;



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

public class SevenZipTest : MonoBehaviour{
#if !UNITY_WEBPLAYER

    //we use some integer to get error codes from the lzma library (look at lzma.cs for the meaning of these error codes)
    private int lzres = 0, lzres4 = 0;
    #if !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 ||UNITY_WINRT_8_1 ) || UNITY_EDITOR
    private int lzres2 = 0, lzres3 = 0;
    #endif
    private bool pass1, pass2;

    //for counting the time taken to decompress the 7z file.
    private float t1, t;

    //the test file to download.
    private string myFile = "test.7z";

    //the adress from where we download our test file
    private string uri = "https://dl.dropboxusercontent.com/u/13373268/tests/";

    private WWW www;

    private string ppath, saPath;

    private string log;
	
	private bool compressionStarted, downloadDone;
	
	private long tsize;

	//reusable buffer for lzma alone buffer to buffer encoding/decoding
	private byte[] buff;

	//fixed size buffers, that don't get resized, to perform compression/decompression of buffers in them and avoid memory allocations.
	private byte[] fixedInBuffer = new byte[1024*256];
	private byte[] fixedOutBuffer = new byte[1024*256];

	#if (!UNITY_WEBGL && !UNITY_WSA_8_1 && !UNITY_WP_8_1 && !UNITY_WINRT_8_1) || UNITY_EDITOR
		#if !NETFX_CORE
			Thread th = null;
		#endif
		#if NETFX_CORE && UNITY_WSA_10_0
			Task task = null;
		#endif
	#endif

    //A 1 item integer array to get the current extracted file of the 7z archive. Compare this to the total number of the files to get the progress %.
    private int[] progress = new int[1];


    void Start(){

		#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
			ppath = UnityEngine.Windows.Directory.localFolder;
		#else
			ppath = Application.persistentDataPath;
		#endif
		
		//
		//we are setting the lzma.persitentDataPath so the get7zinfo, get7zSize, decode2Buffer functions can work on separate threads!
		//on WSA it must be 'Application.persistentDataPath' !
		
		#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
			lzma.persitentDataPath = UnityEngine.Windows.Directory.localFolder;
		#else
			lzma.persitentDataPath = Application.persistentDataPath;
		#endif
		//


		#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			ppath=".";
		#endif

		//Streaming Assets path on various platforms
		#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
			saPath = Application.dataPath + "/StreamingAssets";
		#else
			#if UNITY_IOS
				saPath = Application.dataPath + "/Raw";
			#endif
			#if UNITY_ANDROID
				saPath = "jar:file://" + Application.dataPath + "!/assets/";
			#endif
		#endif

		// a reusable buffer to compress/decopmress data in/from buffers
		buff = new byte[0];

        Debug.Log(ppath);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //download a 7z test file
        //StartCoroutine(Download7ZFile());
        if (!File.Exists(ppath + "/" + myFile)) StartCoroutine(Download7ZFile()); else downloadDone = true;

        //download an lzma alone format file to test buffer 2 buffer encoding/decoding functions
        StartCoroutine(buff2buffTest());

		#if UNITY_ANDROID && !UNITY_EDITOR
            getFromStreamingAssets("test2.7z", "t2.7z");
		#endif
    }
	
	

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
	

    void OnGUI(){

        if (downloadDone == true){
            GUI.Label(new Rect(50, 0, 350, 30), "package downloaded, ready to extract");
            GUI.Label(new Rect(50, 30, 450, 40), ppath);

            //when we call the decompress of 7z archives function, show a referenced integer that indicate the current file beeing extracted.
			#if (!UNITY_WEBGL && !UNITY_WSA_8_1 && !UNITY_WP_8_1 && !UNITY_WINRT_8_1) || UNITY_EDITOR
				#if !NETFX_CORE
					if (th != null){
						GUI.Label(new Rect(Screen.width - 90, 10, 90, 50), progress[0].ToString());
					}
				#endif
				#if NETFX_CORE && UNITY_WSA_10_0
					if (task != null) GUI.Label(new Rect(Screen.width - 90, 10, 90, 50), progress[0].ToString());
				#endif
			#endif

            GUI.Label(new Rect(50, 70, 400, 30),  "decompress Buffer: " + pass1.ToString());
            GUI.TextArea(new Rect(50, 290, 640, 140),tsize.ToString()+"\n"+ log);

            #if !(UNITY_WSA || UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1 ) || UNITY_EDITOR
            GUI.Label(new Rect(50, 100, 400, 30), "compress Buffer  : " + pass2.ToString());
			#endif
			if (GUI.Button(new Rect(50, 150, 250, 50), "start 7z test")){
				//delete the known files that are extracted from the downloaded example z file
				//it is important to do this when you re-extract the same files  on some platforms.
				if (File.Exists(ppath + "/1.txt")) File.Delete(ppath + "/1.txt");
				if (File.Exists(ppath + "/2.txt")) File.Delete(ppath + "/2.txt");
				log = "";
				compressionStarted = true;
				//call the decompresion demo functions.
				DoDecompression();
			}

			//decompress file from buffer
			#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
				if (GUI.Button(new Rect(320, 150, 250, 50), "File Buffer test")){
					doFileBufferTest();
				}
			#endif
        }

        if (compressionStarted){
            //if the return code is 1 then the decompression was succesful.
            GUI.Label(new Rect(50, 210, 250, 40), "7z return code: " + lzres.ToString());
            //time took to decompress
            GUI.Label(new Rect(50, 250, 250, 50), "time: " + t1.ToString());
			
			#if !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 ||UNITY_WINRT_8_1 ) || UNITY_EDITOR
            if (lzres2!=0) GUI.Label(new Rect(50, 450, 250, 30),"lzma encoded "+lzres2.ToString());
			if (lzres3!=0) GUI.Label(new Rect(50, 480, 250, 30),"lzma decoded "+lzres3.ToString());
			#endif
			
			if(lzres4>0) GUI.Label(new Rect(50, 510, 250, 30),"decoded to buffer: ok");
        }

    }


	

	void DoDecompression(){
		
		//Now decompress the 7z file (Note: you might decompress it to any folder you have in the ppath directory)
        //Parameters: lzma.doDecompress7zip(string filePath, string exctractionPath, int[] progress, bool largeFiles=false, bool fullPaths=false, string entry=null, byte[] FileBuffer=null);
		
		//filePath			: the full path to the archive, including the archives name. (/myPath/myArchive.7z)
		//exctractionPath	: the path in where you want your files to be extracted
        //progress          : a referenced integer to get the progress of the extracted files (second function without the need of ref progress exists also. see examples.)
		//largeFiles		: set this to true if you are extracting files larger then 30-40 Mb. It is slower though but prevents crashing your app when extracting large files!
		//fullPaths			: set this to true if you want to keep the folder structure of the 7z file.
		//entry				: set the name of a single file file you want to extract from your archive. If the file resides in a folder, the full path should be added.
		//					   (for examle  game/meshes/small/table.mesh )
		//FileBuffer		: A buffer that holds a 7zip file. When assigned the function will decompress from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)

		t = Time.realtimeSinceStartup;

        //the referenced progress int is new. It will indicate the current index of file beeing decompressed. Use in a separate thread to show it realtime.
		lzres = lzma.doDecompress7zip(ppath + "/" + myFile, ppath + "/", true,true);
		
		//read file names and file sizes of the 7z archive, store them in the lzma.ninfo & lzma.sinfo ArrayLists and return the total uncompressed size of the included files.
		//On WSA call get7zInfo with 'Application.persistentDataPath + "/sevenZip.log"' as the 2nd parameter!
		tsize = lzma.get7zInfo(ppath + "/" + myFile);

		Debug.Log("Total Size: " + tsize + "      trueTotalFiles: " + lzma.trueTotalFiles);
		
		//Look through the ninfo and info ArrayLists where the file names and sizes are stored.
		if(lzma.ninfo != null){
			for (int i = 0; i < lzma.ninfo.Count; i++){
				log += lzma.ninfo[i] + " - " + lzma.sinfo[i] + "\n";
				//Debug.Log(i.ToString()+" " +lzma.ninfo[i]+"|"+lzma.sinfo[i].ToString());
			}
		}
	
		//get size of a specific file. (if the file path is null it will look in the arraylists created by the get7zInfo function
		Debug.Log("--->"+lzma.get7zSize(ppath + "/" + myFile, "1.txt"));

			#if !(UNITY_WSA || UNITY_WSA_8_1 ||  UNITY_WP_8_1 ||UNITY_WINRT_8_1 ) || UNITY_EDITOR

			//setup the lzma compression level. (see more at the function declaration at lzma.cs)
			//This function is not multiple threads safe so call it before starting multiple threads with lzma compress operations.
			lzma.setProps(9);

            //these functions do not work for now on windows phone or windows store apps
		    //encode an archive to lzma alone format
		    lzres2 = lzma.LzmaUtilEncode( ppath + "/1.txt", ppath + "/1.txt.lzma");
		
		    //decode an archive from lzma alone format
		    lzres3 = lzma.LzmaUtilDecode( ppath + "/1.txt.lzma", ppath + "/1BCD.txt");
			#endif
		
		//decode a specific file from a 7z archive to a byte buffer
		var buffer = lzma.decode2Buffer( ppath + "/" + myFile, "1.txt");
		
		if (buffer != null) {
			File.WriteAllBytes(ppath + "/1AAA.txt", buffer);
			if (buffer.Length > 0) { Debug.Log("Decode2Buffer: " + buffer.Length); lzres4=1; } 
		}

        //you might want to call this function in another thread to not halt the main thread and to get the progress of the extracted files.
        //for example:
		#if !UNITY_WEBGL && !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) || UNITY_EDITOR
			#if !NETFX_CORE
				th = new Thread(Decompress);  th.Start(); // faster then coroutine
			#endif
			#if NETFX_CORE && UNITY_WSA_10_0
				task = new Task(new Action(Decompress)); task.Start();
			#endif
		#endif

        //calculate the time it took to decompress the file
        t1 = Time.realtimeSinceStartup - t;
	}

	
    //call from separate thread. here you can get the progress of the extracted files through a referenced integer.
	void Decompress(){
		lzres = lzma.doDecompress7zip(ppath + "/"+myFile , ppath + "/", progress, true,true);
        compressionStarted = true;
    }


#if UNITY_ANDROID && !UNITY_EDITOR
    void getFromStreamingAssets(string file, string outFile)
    {
       string filePath =   saPath + file;
       WWW www2 = new WWW(filePath);

        while (www2.isDone == false) { }

        if (www2.error ==null){
            //new method, extract directly from www.bytes. The new plugins can handle a byte buffer as a file.
            int res2 = lzma.doDecompress7zip(null, ppath + "/", false,true,"5.txt", www2.bytes);
            log+= "decompress from Streaming Assets: "+res2.ToString()+ "\n";

            www2.Dispose(); www2 = null;
        }
        else
        {
            Debug.Log("error reading from Streaming Assets");
        }
    }
#endif

	void doFileBufferTest() {
		//For iOS, Android, MacOSX and Linux the plugins can handle a byte buffer as a file. (in this case the www.bytes)
		//This way you can extract the file or parts of it without writing it to disk.
		//
		// !!! Caution !!! Linux: While this works ok with unity 4.x.x, there is a bug introduced with unity5.x versions and might crash the app on
		// some Linux distributions. Use with caution!
		//
       #if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
	   if (File.Exists(ppath + "/" + myFile)) {
			byte[] www = File.ReadAllBytes(ppath + "/" + myFile);
			log="";
            lzres = lzma.doDecompress7zip(null, ppath + "/", true,true,null, www);
                log+=lzres.ToString()+" | ";
            lzres = lzma.doDecompress7zip(null, ppath + "/", progress, false,true,null, www);
                log += lzres+" - prg: "+progress[0].ToString()+" | ";
		    tsize = lzma.get7zInfo(null,null, www);
		         log += "tsiz: " + tsize.ToString() + " #files: " + lzma.trueTotalFiles.ToString()+" | ";
            tsize = lzma.get7zSize(null, null, null, www);
                log += "tsiz: " + tsize.ToString()+" | ";

		    var buffer = lzma.decode2Buffer( null, "2.txt",null, www);
		
		    if (buffer != null) {
                 log += "dec2buffer: ok"+ "\n";
			    File.WriteAllBytes(ppath + "/2AAA_FILEBUFFER.txt", buffer);
			    if (buffer.Length > 0) { Debug.Log("FileBuffer_Decode2Buffer: " + buffer.Length);  } 
		    }
		}
        #endif
	}

    IEnumerator Download7ZFile(){
		
		Debug.Log("starting download");
	
		//make sure a previous 7z file having the same name with the one we want to download does not exist in the ppath folder
		if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile);

        www = new WWW(uri + myFile);
		yield return www;
		if (www.error != null) Debug.Log(www.error);
		downloadDone = true;
		log="";
        		 
		//write the downloaded 7z file to the ppath directory so we can have access to it
		//depending on the Install Location you have set for your app, set the Write Access accordingly!
		File.WriteAllBytes(ppath + "/" + myFile, www.bytes);
		www.Dispose(); www = null;
	}


	IEnumerator buff2buffTest(){
		//Example on how to decompress an lzma compressed asset bundle from the streaming assets folder.
		//(editor, standalone example) for iOS, Android or WSA use an apropriate method to get the bytes buffer from the file.
		#if (UNITY_STANDALONE_LINUX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA_10_0) && (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
			byte[] sa = File.ReadAllBytes( saPath + "/female_eyes.assetbundle");

			if (lzma.decompressAssetBundle(sa , ref buff) == 0) {
				File.WriteAllBytes(ppath+"/female_eyes.assetbundle", buff);
				Debug.Log(" #-> AssetBundle Decompressed: ok");
			}
			sa = null;
		#endif

        //BUFFER TO BUFFER lzma alone compression/decompression EXAMPLE
		//
		//An example on how to decompress an lzma alone file downloaded through www without storing it to disk
		//using just the www.bytes buffer.
		//Download a file.
		WWW w = new WWW("https://dl.dropboxusercontent.com/u/13373268/tests/google.jpg.lzma");
		yield return w;

		if(w.error==null){
			//we decompress the lzma file in the buff buffer.
			if(lzma.decompressBuffer( w.bytes, ref buff )==0){
                pass1 = true;
                //we write it to disk just to check that the decompression was ok
				File.WriteAllBytes( ppath+"/google.jpg",buff);
			}else{
				Debug.Log("Error decompressing www.bytes to buffer"); pass1 = false; 
			}
		}else{ 
			Debug.Log(w.error); 
		}

		w.Dispose(); w=null;
        #if !(UNITY_WSA || UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1 ) || UNITY_EDITOR
        //Example on how to compress a buffer.
        if (File.Exists(ppath+"/google.jpg")){
			byte[] bt = File.ReadAllBytes( ppath+"/google.jpg");

			//compress the data buffer into a compressed buffer
			if(lzma.compressBuffer( bt ,ref buff)){

                pass2=true;
                //write it to disk just for checking purposes
				File.WriteAllBytes( ppath+"/google.jpg.lzma",buff);
				//print info
				Debug.Log("uncompressed size in lzma: "+BitConverter.ToUInt64(buff,5));
				Debug.Log("lzma size: "+buff.Length);
			}else{
                pass2=false;
				Debug.Log("could not compress to buffer ...");
			}

				//FIXED BUFFER FUNCTIONS:
				int compressedSize = lzma.compressBufferFixed(bt, ref fixedInBuffer);
				Debug.Log(" #-> Compress Fixed size Buffer: " + compressedSize);

				if(compressedSize>0){
					int decommpressedSize = lzma.decompressBufferFixed(fixedInBuffer, ref fixedOutBuffer);
					if(decommpressedSize > 0) Debug.Log(" #-> Decompress Fixed size Buffer: " + decommpressedSize);
				}

			bt =null;  	
		}
		#else
			yield return false;
		#endif
	}
#else
        void Start(){
            Debug.Log("Does not work on WebPlayer!");
        }
#endif
}


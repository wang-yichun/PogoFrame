using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_WEBGL && !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) || UNITY_EDITOR
using System.Threading;
using System.IO;
#endif

#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
 using File = UnityEngine.Windows.File;
 #else
 using File = System.IO.File;
 #endif

#if NETFX_CORE
#if UNITY_WSA_10_0
		using System.Threading.Tasks;
        using System.IO.IsolatedStorage;
        using static System.IO.Directory;
        using static System.IO.File;
        using static System.IO.FileStream;
#endif
#endif

public class lz4test : MonoBehaviour {
#if !UNITY_WEBPLAYER

    //some variables to get status returns from the functions
    private float lz1 = 0;
    private int  lz2 = -1, lz3, lz4, fbuftest;

	//A single item integer array to get the bytes being decompressed
    private int[] bytes = new int[1];

	//A single item float array to get progress of compression.
    private float[] progress = new float[1];

    //a test file that will be downloaded to run the tests
    private string myFile = "testLZ4.tif";

    //the adress from where we download our test file
    private string uri = "https://dl.dropboxusercontent.com/u/13373268/tests/";

    private WWW www;

    //our path where we do the tests
    private string ppath;

    private bool compressionStarted;
    private bool downloadDone;

    //a reusable buffer
    private byte[] buff;

	//fixed size buffer, that don't gets resized, to perform decompression of buffers in them and avoid memory allocations.
	private byte[] fixedOutBuffer = new byte[1024*768];


    // Use this for initialization
    void Start () {

		#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
			ppath = UnityEngine.Windows.Directory.localFolder;
		#else
			ppath = Application.persistentDataPath;
		#endif

        #if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			ppath=".";
        #endif

        buff = new byte[0];

        Debug.Log(ppath);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if(!File.Exists(ppath + "/" + myFile)) StartCoroutine(DownloadTestFile()); else downloadDone = true;

    }


	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }


    void OnGUI()
    {
        if (downloadDone == true)
        {
            GUI.Label(new Rect(50, 0, 350, 30), "package downloaded, ready to extract");
            GUI.Label(new Rect(50, 30, 450, 90), ppath);
        }

        if (downloadDone)
        {
            if (GUI.Button(new Rect(50, 150, 250, 50), "start LZ4 test"))
            {
                compressionStarted = true;
                //call the decompresion demo functions.
                // DoTests();
                //we call the test function on a thread to able to see progress. WebGL does not support threads.
				#if (!UNITY_WEBGL && !UNITY_WSA_8_1 && !UNITY_WP_8_1 && !UNITY_WINRT_8_1) || UNITY_EDITOR
					#if NETFX_CORE && UNITY_WSA_10_0
						Task task = new Task(new Action(DoTests)); task.Start();
					#endif
					#if !NETFX_CORE
						Thread th = new Thread(DoTests); th.Start();
					#endif
				#else
					StartCoroutine(DoTestsWebGL());//DoTests();
				#endif
            }
        }

        if (compressionStarted){
            //if the return code is 1 then the decompression was succesful.
            GUI.Label(new Rect(50, 220, 250, 40), "LZ4 Compress:    " + (lz1).ToString() + "%");
            GUI.Label(new Rect(300, 220, 120, 40), progress[0].ToString() + "%");

            GUI.Label(new Rect(50, 260, 250, 40), "LZ4 Decompress: " + (lz2+1).ToString());
            GUI.Label(new Rect(300, 260, 250, 40), bytes[0].ToString());

            GUI.Label(new Rect(50, 300, 250, 40), "Buffer Compress:    " + lz3.ToString());
            GUI.Label(new Rect(50, 340, 250, 40), "Buffer Decompress: " + lz4.ToString());

			#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
				GUI.Label(new Rect(50, 380, 250, 40), "FileBuffer test: " + (fbuftest+1).ToString());
			#endif
        }

     }


    void DoTests() {
    
        //File tests
        //compress a file to lz4 with highest level of compression (9).
        lz1 = LZ4.compress(ppath+ "/" + myFile, ppath + "/" + myFile + ".lz4", 9,  progress);

        //decompress the previously compressed archive
        lz2 = LZ4.decompress(ppath + "/" + myFile + ".lz4", ppath + "/" + myFile + "B.tif",  bytes);


        //Buffer tests
        if (File.Exists(ppath + "/" + myFile)){
            byte[] bt = File.ReadAllBytes(ppath + "/" + myFile);

            //compress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (LZ4.compressBuffer(bt, ref buff, 9, true)){
                lz3 = 1;
                File.WriteAllBytes(ppath + "/buffer1.lz4buf", buff);
            }

            byte[] bt2 = File.ReadAllBytes(ppath + "/buffer1.lz4buf");

            //decompress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (LZ4.decompressBuffer(bt2, ref buff, true)){
                lz4 = 1;
                File.WriteAllBytes(ppath + "/buffer1.tif", buff);
            }

				//FIXED BUFFER FUNCTION:
				int decommpressedSize = LZ4.decompressBufferFixed(bt2, ref fixedOutBuffer);
				if(decommpressedSize > 0) Debug.Log(" # Decompress Fixed size Buffer: " + decommpressedSize);

			bt2= null; bt = null;
        }

		//make FileBuffer test on supported platfoms.
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			//make a temp buffer to read an lz4 file in.
			if (File.Exists(ppath + "/" + myFile + ".lz4")){
				byte[] FileBuffer = File.ReadAllBytes(ppath + "/" + myFile + ".lz4");
				fbuftest = LZ4.decompress(null, ppath + "/" + myFile + ".FBUFF.tif",  bytes, FileBuffer);
			}
		#endif

    }

#if (UNITY_WEBGL || UNITY_WSA_8_1 || UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
	IEnumerator DoTestsWebGL() {
		yield return true;
        //File tests
        //compress a file to lz4 with highest level of compression (9).
        lz1 = LZ4.compress(ppath+ "/" + myFile, ppath + "/" + myFile + ".lz4", 9,  progress);

        //decompress the previously compressed archive
        lz2 = LZ4.decompress(ppath + "/" + myFile + ".lz4", ppath + "/" + myFile + "B.tif",  bytes);

        //Buffer tests
        if (File.Exists(ppath + "/" + myFile)){
            byte[] bt = File.ReadAllBytes(ppath + "/" + myFile);

            //compress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (LZ4.compressBuffer(bt, ref buff, 9, true))
            {
                lz3 = 1;
                File.WriteAllBytes(ppath + "/buffer1.lz4buf", buff);
            }

            byte[] bt2 = File.ReadAllBytes(ppath + "/buffer1.lz4buf");

            //decompress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (LZ4.decompressBuffer(bt2, ref buff, true))
            {
                lz4 = 1;
                File.WriteAllBytes(ppath + "/buffer1.tif", buff);
            }
			bt2= null; bt = null;
        }
    }
#endif

   IEnumerator DownloadTestFile()
    {
        Debug.Log("starting download");

        //make sure a previous lz4 file having the same name with the one we want to download does not exist in the ppath folder
        if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile);

        //replace the link to the lz4 file with your own (although this will work also)
        // string esc = WWW.UnEscapeURL(uri + myFile);
        www = new WWW(uri + myFile);
        yield return www;
        if (www.error != null) Debug.Log(www.error);

        downloadDone = true;

        //write the downloaded lz4 file to the ppath directory so we can have access to it
        //depending on the Install Location you have set for your app, set the Write Access accordingly!
		File.WriteAllBytes(ppath + "/" + myFile, www.bytes);
        www.Dispose(); www = null;
    }

#else
        void Start(){
            Debug.Log("Does not work on WebPlayer!");
        }
#endif
}

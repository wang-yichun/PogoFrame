using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
 using File = UnityEngine.Windows.File;
 #else
 using File = System.IO.File;
 #endif

#if NETFX_CORE
    #if UNITY_WSA_10_0
        using System.IO.IsolatedStorage;
        using static System.IO.Directory;
        using static System.IO.File;
        using static System.IO.FileStream;
    #endif
#endif


public class lzip
{
#if !UNITY_WEBPLAYER

#if (UNITY_IPHONE || UNITY_WEBGL || UNITY_IOS) && !UNITY_EDITOR
	#if (UNITY_IOS  || UNITY_IPHONE)
	[DllImport("__Internal")]
	private static extern int zsetPermissions(string filePath, string _user, string _group, string _other);
	#endif
    [DllImport("__Internal")]
    private static extern int zipGetTotalFiles(string zipArchive, IntPtr FileBuffer, int fileBufferLength);
    [DllImport("__Internal")]
    private static extern int zipGetInfo(string zipArchive, string path, IntPtr FileBuffer, int fileBufferLength);
    [DllImport("__Internal")]
    private static extern void releaseBuffer(IntPtr buffer);
    [DllImport("__Internal")]
    private static extern int zipGetEntrySize(string zipArchive, string entry, IntPtr FileBuffer, int fileBufferLength);
    [DllImport("__Internal")]
    private static extern int zipCD(int levelOfCompression, string zipArchive, string inFilePath, string fileName, string comment);
    [DllImport("__Internal")]
    private static extern bool zipBuf2File(int levelOfCompression, string zipArchive, string arc_filename, IntPtr buffer, int bufferSize);
    [DllImport("__Internal")]
    private static extern bool zipEntry2Buffer(string zipArchive, string entry, IntPtr buffer, int bufferSize, IntPtr FileBuffer, int fileBufferLength);
    [DllImport("__Internal")]
    private static extern IntPtr zipCompressBuffer(IntPtr source, int sourceLen, int levelOfCompression, ref int v);
    [DllImport("__Internal")]
    private static extern IntPtr zipDecompressBuffer(IntPtr source, int sourceLen, ref int v);
    [DllImport("__Internal")]
	private static extern int zipEX(string zipArchive, string outPath, IntPtr progress, IntPtr FileBuffer, int fileBufferLength, IntPtr proc);
    [DllImport("__Internal")]
    private static extern int zipEntry(string zipArchive, string arc_filename, string outpath, IntPtr FileBuffer, int fileBufferLength, IntPtr proc);
	//gzip section
    [DllImport("__Internal")]
    internal static extern int zipGzip(IntPtr source, int sourceLen, IntPtr outBuffer, int levelOfCompression, bool addHeader, bool addFooter);
    [DllImport("__Internal")]
    internal static extern int zipUnGzip(IntPtr source, int sourceLen, IntPtr outBuffer, bool hasHeader, bool hasFooter);
#endif

#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR
	private const string libname = "zipw";
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA
	private const string libname = "libzipw";
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA || UNITY_ANDROID || UNITY_STANDALONE_LINUX
	#if (UNITY_STANDALONE_OSX  || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)&& !UNITY_EDITOR_WIN
		[DllImport(libname, EntryPoint = "zsetPermissions")]
		internal static extern int zsetPermissions(string filePath, string _user, string _group, string _other);
	#endif
	[DllImport(libname, EntryPoint = "zipGetTotalFiles")]
    internal static extern int zipGetTotalFiles(string zipArchive, IntPtr FileBuffer, int fileBufferLength);
    [DllImport(libname, EntryPoint = "zipGetInfo")]
    internal static extern int zipGetInfo(string zipArchive, string path, IntPtr FileBuffer, int fileBufferLength);
    [DllImport(libname, EntryPoint = "releaseBuffer")]
    internal static extern void releaseBuffer(IntPtr buffer);
    [DllImport(libname, EntryPoint = "zipGetEntrySize")]
    internal static extern int zipGetEntrySize(string zipArchive, string entry, IntPtr FileBuffer, int fileBufferLength);
    [DllImport(libname, EntryPoint = "zipCD")]
    internal static extern int zipCD(int levelOfCompression, string zipArchive, string inFilePath, string fileName, string comment);
    [DllImport(libname, EntryPoint = "zipBuf2File")]
    internal static extern bool zipBuf2File(int levelOfCompression, string zipArchive, string arc_filename, IntPtr buffer, int bufferSize);
    [DllImport(libname, EntryPoint = "zipEntry2Buffer")]
    internal static extern bool zipEntry2Buffer(string zipArchive, string entry, IntPtr buffer, int bufferSize, IntPtr FileBuffer, int fileBufferLength);
    [DllImport(libname, EntryPoint = "zipCompressBuffer")]
    internal static extern IntPtr zipCompressBuffer(IntPtr source, int sourceLen, int levelOfCompression, ref int v);
    [DllImport(libname, EntryPoint = "zipDecompressBuffer")]
    internal static extern IntPtr zipDecompressBuffer(IntPtr source, int sourceLen, ref int v);
    [DllImport(libname, EntryPoint = "zipEX")]
	internal static extern int zipEX(string zipArchive, string outPath, IntPtr progress, IntPtr FileBuffer, int fileBufferLength, IntPtr proc);
    [DllImport(libname, EntryPoint = "zipEntry")]
    internal static extern int zipEntry(string zipArchive, string arc_filename, string outpath, IntPtr FileBuffer,int fileBufferLength, IntPtr proc);
	//gzip section
    [DllImport(libname, EntryPoint = "zipGzip")]
    internal static extern int zipGzip(IntPtr source, int sourceLen, IntPtr outBuffer, int levelOfCompression, bool addHeader, bool addFooter);
    [DllImport(libname, EntryPoint = "zipUnGzip")]
    internal static extern int zipUnGzip(IntPtr source, int sourceLen, IntPtr outBuffer, bool hasHeader, bool hasFooter);
#endif

	// set permissions of a file in user, group, other.
	// Each string should contain any or all chars of "rwx".
	// returns 0 on success
	public static int setFilePermissions(string filePath, string _user, string _group, string _other){
		#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
			return zsetPermissions(filePath, _user, _group, _other);
		#else
			return -1;
		#endif
	}

    //A function that returns the total number of files in a zip archive (files + folders).
    //
    //zipArchive        : the zip to be checked
	//FileBuffer		: A buffer that holds a zip file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    //ERROR CODES
    //                  : -1 = failed to access zip archive
    //                  :  any number>0 = the number of files in the zip archive
    //
    public static int getTotalFiles(string zipArchive, byte[] FileBuffer = null) {
		int res;
		
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = zipGetTotalFiles(null, fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
				return res;
			}
		#endif

        res = zipGetTotalFiles(@zipArchive, IntPtr.Zero, 0);

		return res;
    }
     
    //Lists get filled with filenames (including path if the file is in a folder) and uncompressed file sizes
    //Call getFileInfo(string zipArchive, string path) to get them filled. After that you can iterate through them to get the info you want.
    public static List<string> ninfo = new List<string>();//filenames
    public static List<long> uinfo = new List<long>();//uncompressed file sizes
    public static List<long> cinfo = new List<long>();//compressed file sizes
    public static int zipFiles, zipFolders;// global integers that store the number of files and folders in a zip file.

    //This function fills the Lists with the filenames and file sizes that are in the zip file
    //Returns			: the total size of uncompressed bytes of the files in the zip archive 
    //
    //zipArchive		: the full path to the archive, including the archives name. (/myPath/myArchive.zip)
    //path              : the path where a temporary file will be created (recommended to use the Application.persistentDataPath);
	//FileBuffer		: A buffer that holds a zip file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    //ERROR CODES       : -1 = Input file not found
    //                  : -2 = Path to write the temporary log does not exist (for mobiles the Application.persistentDataPath is recommended)
    //                  : -3 = Error of info data of the zip file
    //                  : -4 = Log file not found
    //
    public static long getFileInfo(string zipArchive, string path, byte[] FileBuffer = null) {
        ninfo.Clear(); uinfo.Clear(); cinfo.Clear();
        zipFiles = 0; zipFolders = 0;

		int res;
		
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = zipGetInfo(null, @path, fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
			}else{
				res = zipGetInfo(@zipArchive, @path, IntPtr.Zero, 0);
			}
		#else
				res = zipGetInfo(@zipArchive, @path, IntPtr.Zero, 0);
		#endif


        if (res == -1) { /*Debug.Log("Input file not found.");*/ return -1; }

		#if (UNITY_WP8_1 || UNITY_WSA) && !UNITY_EDITOR
			if(!UnityEngine.Windows.Directory.Exists(@path)) { /*Debug.Log("Path does not exist: " + path);*/ return -2; }
		#else
        	if (res == -2) { /*Debug.Log("Path does not exist: " + path);*/ return -2; }
		#endif
        if (res == -3) { /*Debug.Log("Entry info error.");*/ return -3; }

        string logPath = @path + "/uziplog.txt";


        if (!File.Exists(logPath)) { /*Debug.Log("Info file not found.");*/ return -4; }

		#if !NETFX_CORE
        StreamReader r = new StreamReader(logPath);
		#endif
		#if NETFX_CORE
			#if UNITY_WSA_10_0
			IsolatedStorageFile ipath = IsolatedStorageFile.GetUserStoreForApplication();
			StreamReader r = new StreamReader(new IsolatedStorageFileStream("uziplog.txt", FileMode.Open, ipath));
			#endif
			#if UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1
			var data = UnityEngine.Windows.File.ReadAllBytes(logPath);
			string ss = System.Text.Encoding.UTF8.GetString(data,0,data.Length);
			StringReader r = new StringReader(ss);
			#endif
		#endif
        string line;
        string[] rtt;
        long t = 0, sum = 0;

        while ((line = r.ReadLine()) != null)
        {
            rtt = line.Split('|');
            ninfo.Add(rtt[0]);

            long.TryParse(rtt[1], out t);
            sum += t;
            uinfo.Add(t);
            if (t > 0) zipFiles++; else zipFolders++;

            long.TryParse(rtt[2], out t);
            cinfo.Add(t);
        }

		#if !NETFX_CORE
        r.Close(); 
		#endif
        r.Dispose();

        File.Delete(logPath);

        return sum;

    }


    //A function that returns the uncompressed size of a file in a zip archive.
    //
    //zipArchive    : the zip archive to get the info from.
    //entry         : the entry for which we want to know it uncompressed size.
	//FileBuffer	: A buffer that holds a zip file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    public static int getEntrySize(string zipArchive, string entry, byte[] FileBuffer = null) {
		int res;
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = zipGetEntrySize(null, entry, fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
				return res;
			}
		#endif

        res = zipGetEntrySize(@zipArchive, entry, IntPtr.Zero, 0);

		return res;
    }


    //A function that compresses a byte buffer to a zlib stream compressed buffer. Provide a reference buffer to write to. This buffer will be resized.
    //
    //source                : the input buffer
    //outBuffer             : the referenced output buffer
    //levelOfCompression    : (0-10) recommended 9 for maximum (10 is highest but slower and not zlib compatible)
    //
    //ERROR CODES   : true  = success
    //              : false = failed
    //
    public static bool compressBuffer(byte[] source, ref byte[] outBuffer, int levelOfCompression) {
        if (levelOfCompression < 0) levelOfCompression = 0; if (levelOfCompression > 10) levelOfCompression = 10;

        GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
        IntPtr ptr;
        int siz = 0;

        ptr = zipCompressBuffer(sbuf.AddrOfPinnedObject(), source.Length, levelOfCompression, ref siz);

        if (siz == 0 || ptr == IntPtr.Zero) { sbuf.Free(); releaseBuffer(ptr); return false; }

        System.Array.Resize(ref outBuffer, siz);
        Marshal.Copy(ptr, outBuffer, 0, siz);

        sbuf.Free();
        releaseBuffer(ptr);

        return true;
    }

	//same as the compressBuffer function, only this function will put the result in a fixed size buffer to avoid memory allocations.
	//the compressed size is returned so you can manipulate it at will.
	//
	//safe: if set to true the function will abort if the compressed resut is larger the the fixed size output buffer.
	//Otherwise compressed data will be written only until the end of the fixed output buffer.
	//
	public static int compressBufferFixed(byte[] source, ref byte[] outBuffer, int levelOfCompression, bool safe = true) {
        if (levelOfCompression < 0) levelOfCompression = 0; if (levelOfCompression > 10) levelOfCompression = 10;

        GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
        IntPtr ptr;
        int siz = 0;

        ptr = zipCompressBuffer(sbuf.AddrOfPinnedObject(), source.Length, levelOfCompression, ref siz);

        if (siz == 0 || ptr == IntPtr.Zero) { sbuf.Free(); releaseBuffer(ptr); return 0; }

		if (siz>outBuffer.Length) {
			if(safe) { sbuf.Free(); releaseBuffer(ptr); return 0; } else { siz = outBuffer.Length; }
		}

        Marshal.Copy(ptr, outBuffer, 0, siz);

        sbuf.Free();
        releaseBuffer(ptr);

        return siz;
    }

    //A function that compresses a byte buffer to a zlib stream compressed buffer. Returns a new buffer with the compressed data.
    //
    //source                : the input buffer
    //levelOfCompression    : (0-10) recommended 9 for maximum (10 is highest but slower and not zlib compatible)
    //
    //ERROR CODES           : a valid byte buffer = success
    //                      : null                = failed
    //
    public static byte[] compressBuffer(byte[] source,  int levelOfCompression) {
        if (levelOfCompression < 0) levelOfCompression = 0; if (levelOfCompression > 10) levelOfCompression = 10;

        GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
        IntPtr ptr;
        int siz = 0;

        ptr = zipCompressBuffer(sbuf.AddrOfPinnedObject(), source.Length, levelOfCompression, ref siz);

        if (siz == 0 || ptr == IntPtr.Zero) { sbuf.Free(); releaseBuffer(ptr); return null; }

        byte[] buffer = new byte[siz];
        Marshal.Copy(ptr, buffer, 0, siz);

        sbuf.Free();
        releaseBuffer(ptr);

        return buffer;
    }


    //A function that decompresses a zlib compressed buffer to a referenced outBuffer. The outbuffer will be resized.
    //
    //source            : a zlib compressed buffer.
    //outBuffer         : a referenced out buffer provided to extract the data. This buffer will be resized to fit the uncompressed data.
    //
    //ERROR CODES       : true  = success
    //                  : false = failed
    //
    public static bool decompressBuffer(byte[] source, ref byte[] outBuffer) {
        GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
        IntPtr ptr;
        int siz = 0;

        ptr = zipDecompressBuffer(sbuf.AddrOfPinnedObject(), source.Length, ref siz);

        if (siz == 0 || ptr == IntPtr.Zero) { sbuf.Free(); releaseBuffer(ptr); return false; }

        System.Array.Resize(ref outBuffer, siz);
        Marshal.Copy(ptr, outBuffer, 0, siz);

        sbuf.Free();
        releaseBuffer(ptr);

        return true;
    }

	//same as the decompressBuffer function. Only this one outputs to a buffer of fixed which size isn't resized to avoid memory allocations.
	//The fixed buffer should have a size that will be able to hold the incoming decompressed data.
	//Returns the uncompressed size.
	//
	//safe: if set to true the function will abort if the decompressed resut is larger the the fixed size output buffer.
	//Otherwise decompressed data will be written only until the end of the fixed output buffer.
	//
    public static int decompressBufferFixed(byte[] source, ref byte[] outBuffer, bool safe = true) {
        GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
        IntPtr ptr;
        int siz = 0;

        ptr = zipDecompressBuffer(sbuf.AddrOfPinnedObject(), source.Length, ref siz);

        if (siz == 0 || ptr == IntPtr.Zero) { sbuf.Free(); releaseBuffer(ptr); return 0; }

		if (siz>outBuffer.Length) {
			if(safe) { sbuf.Free(); releaseBuffer(ptr); return 0; } else { siz = outBuffer.Length; }
		}

        Marshal.Copy(ptr, outBuffer, 0, siz);

        sbuf.Free();
        releaseBuffer(ptr);

        return siz;
    }

    //A function that decompresses a zlib compressed buffer and creates a new buffer.  Returns a new buffer with the uncompressed data.
    //
    //source                : a zlib compressed buffer.
    //
    //ERROR CODES           : a valid byte buffer = success
    //                      : null                = failed
    //
    public static byte[] decompressBuffer(byte[] source) {
        GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
        IntPtr ptr;
        int siz = 0;

        ptr = zipDecompressBuffer(sbuf.AddrOfPinnedObject(), source.Length, ref siz);

        if (siz == 0 || ptr == IntPtr.Zero) { sbuf.Free(); releaseBuffer(ptr); return null; }

        byte[] buffer = new byte[siz];
        Marshal.Copy(ptr, buffer, 0, siz);

        sbuf.Free();
        releaseBuffer(ptr);

        return buffer;
    }

    //A function that will decompress a file in a zip archive directly in a provided byte buffer.
    //
    //zipArchive        : the full path to the zip archive from which a specific file will be extracted to a byte buffer.
    //entry             : the file we want to extract to a buffer. (If the file resides in a directory, the directory should be included.
    //buffer            : a referenced byte buffer that will be resized and will be filled with the extraction data.
	//FileBuffer		: A buffer that holds a zip file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    //ERROR CODES       : true  = success
    //                  : false = failed
    //
    public static bool entry2Buffer(string zipArchive, string entry, ref byte[] buffer, byte[] FileBuffer = null) {

		int siz;
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				siz = zipGetEntrySize(null, entry, fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
			}else{
				siz = zipGetEntrySize( @zipArchive,  entry, IntPtr.Zero, 0);
			}
		#else
			siz = zipGetEntrySize( @zipArchive,  entry, IntPtr.Zero, 0);
		#endif

        if (siz <= 0) return false;

        System.Array.Resize(ref buffer, siz);

        GCHandle sbuf = GCHandle.Alloc(buffer, GCHandleType.Pinned);

		bool res;
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = zipEntry2Buffer(null, entry, sbuf.AddrOfPinnedObject(), siz, fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
			}else{
				res = zipEntry2Buffer( @zipArchive, entry, sbuf.AddrOfPinnedObject(), siz, IntPtr.Zero, 0);
			}
		#else
			res = zipEntry2Buffer( @zipArchive, entry, sbuf.AddrOfPinnedObject(), siz, IntPtr.Zero, 0);
		#endif

        sbuf.Free();

        return res;
    }

    //A function that will decompress a file in a zip archive in a new created and returned byte buffer.
    //
    //zipArchive        : the full path to the zip archive from which a specific file will be extracted to a byte buffer.
    //entry             : the file we want to extract to a buffer. (If the file resides in a directory, the directory should be included.
	//FileBuffer		: A buffer that holds a zip file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    //ERROR CODES       : non-null  = success
    //                  : null      = failed
    //
    public static byte[] entry2Buffer(string zipArchive, string entry, byte[] FileBuffer = null) {
		int siz;
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				siz = zipGetEntrySize(null, entry, fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
			}else{
				siz = zipGetEntrySize( @zipArchive,  entry, IntPtr.Zero, 0);
			}
		#else
			siz = zipGetEntrySize( @zipArchive,  entry, IntPtr.Zero, 0);
		#endif

        if (siz <= 0) return null;

        byte[] buffer = new byte[siz];

        GCHandle sbuf = GCHandle.Alloc(buffer, GCHandleType.Pinned);

		bool res;
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = zipEntry2Buffer(null, entry, sbuf.AddrOfPinnedObject(), siz, fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
			}else{
				res = zipEntry2Buffer( @zipArchive, entry, sbuf.AddrOfPinnedObject(), siz, IntPtr.Zero, 0);
			}
		#else
			res = zipEntry2Buffer( @zipArchive, entry, sbuf.AddrOfPinnedObject(), siz, IntPtr.Zero, 0);
		#endif

        sbuf.Free();
        if (!res) return null;
        else return buffer;
    }


    //A function that compresses a byte buffer and writes it to a zip file. I you set the append flag to true, the output will get appended to an existing zip archive.
    //
    //levelOfCompression    : (0-10) recommended 9 for maximum (10 is highest but slower and not zlib compatible)
    //zipArchive            : the full path to the zip archive to be created or append to.
    //arc_filename          : the name of the file that will be written to the archive.
    //buffer                : the buffer that will be compressed and will be put in the zip archive.
    //append                : set to true if you want the output to be appended to an existing zip archive.
    //
    //ERROR CODES           : true  = success
    //                      : false = failed
    //
    public static bool buffer2File(int levelOfCompression, string zipArchive, string arc_filename, byte[] buffer, bool append=false) {
        if (!append) { if (File.Exists(@zipArchive)) File.Delete(@zipArchive); }
        GCHandle sbuf = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        if (levelOfCompression < 0) levelOfCompression = 0; if (levelOfCompression > 10) levelOfCompression = 10;

        bool res = zipBuf2File(levelOfCompression, @zipArchive, arc_filename, sbuf.AddrOfPinnedObject(), buffer.Length);

        sbuf.Free();
        return res;
    }


    //A function that will extract only the specified file that resides in a zip archive.
    //
    //zipArchive         : the full path to the zip archive from which we want to extract the specific file.
    //arc_filename       : the specific file we want to extract. (If the file resides in a  directory, the directory path should be included. like dir1/dir2/myfile.bin)
    //outpath            : the full path to where the file should be extracted + the desired name for it.
	//FileBuffer		 : A buffer that holds a zip file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
	//proc:				 : a single item integer array that gets updated with the progress of the decompression in bytes.
	//					   (100% is reached when the compressed size of the file is reached.)
    //
    //ERROR CODES        : -1 = extraction failed
    //                   : -2 = could not initialize zip archive.
    //                   :  1 = success
    //
    public static int extract_entry(string zipArchive, string arc_filename, string outpath, byte[] FileBuffer = null, int[] proc = null) {
		int res;
		if(proc == null) proc = new int[1];
		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				if(proc != null) res = zipEntry(null, arc_filename, outpath, fbuf.AddrOfPinnedObject(), FileBuffer.Length, pbuf.AddrOfPinnedObject());
				else res = zipEntry(null, arc_filename, outpath, fbuf.AddrOfPinnedObject(), FileBuffer.Length, IntPtr.Zero);
				fbuf.Free();
				return res;
			}
		#endif

			if(proc != null) res = zipEntry(@zipArchive, arc_filename, outpath, IntPtr.Zero, 0, pbuf.AddrOfPinnedObject());
			else res = zipEntry(@zipArchive, arc_filename, outpath, IntPtr.Zero, 0, IntPtr.Zero);

			pbuf.Free();

		return res;
    }

    //A function that decompresses a zip file. If the zip contains directories, they will be created.
    //
    //zipArchive         : the full path to the zip archive that will be decompressed.
    //outPath            : the directory in which the zip contents will be extracted.
    //progress           : provide a single item integer array to write the current index of the file getting extracted. To use it in realtime, call
    //                   : this function in a separate thread.
	//FileBuffer		 : A buffer that holds a zip file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
	//proc:				 : a single item integer array that gets updated with the progress of the decompression in bytes.
	//					   (100% is reached when the compressed size of the file is reached.)
    //
    //ERROR CODES
    //                   : -1 = could not initialize zip archive.
    //                   : -2 = failed reading content info
    //                   : -3 = failed extraction
    //                   :  1 = success
    //
    public static int decompress_File(string zipArchive, string outPath, int[] progress,  byte[] FileBuffer = null, int[] proc = null) {
        //make a check if the last '/' exists at the end of the exctractionPath and add it if it is missing
        if (@outPath.Substring(@outPath.Length - 1, 1) != "/") { @outPath += "/"; }
		int res;
		GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
		if(proc == null) proc = new int[1];
		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);
		
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				if(proc != null) res = zipEX(null, @outPath, ibuf.AddrOfPinnedObject(), fbuf.AddrOfPinnedObject(), FileBuffer.Length, pbuf.AddrOfPinnedObject());
				else res = zipEX(null, @outPath, ibuf.AddrOfPinnedObject(), fbuf.AddrOfPinnedObject(), FileBuffer.Length, IntPtr.Zero);
				fbuf.Free(); ibuf.Free(); return res;
			}
		#endif

		if(proc != null) res = zipEX(@zipArchive, @outPath, ibuf.AddrOfPinnedObject(), IntPtr.Zero, 0, pbuf.AddrOfPinnedObject());
		else res = zipEX(@zipArchive, @outPath, ibuf.AddrOfPinnedObject(), IntPtr.Zero, 0, IntPtr.Zero);
		ibuf.Free(); pbuf.Free();
		return res;
    }


    //A function that compresses a file to a zip file. If the flag append is set to true then it will get appended to an existing zip file.
    //
    //levelOfCompression  : (0-10) recommended 9 for maximum (10 is highest but slower and not zlib compatible)
    //zipArchive          : the full path to the zip archive that will be created
    //inFilePath          : the full path to the file that should be compressed and added to the zip file.
    //append              : set to true if you want the input file to get appended to an existing zip file. (if the zip file does not exist it will be created.)
    //filename            : if you want the name of your file to be different then the one it has, set it here. If you add a folder structure to it,
    //                      like (dir1/dir2/myfile.bin) the directories will be created in the zip file.
    //comment             : add a comment for the file in the zip file header.
    //
    //ERROR CODES
    //                    : -1 = could not find the input file
    //                    : -2 = could not allocate memory
    //                    : -3 = could not read the input file
    //                    : -4 = error creating zip file
    //                    :  1 = success
    //
    public static int compress_File(int levelOfCompression, string zipArchive, string inFilePath,bool append=false, string fileName="", string comment="")
    {
        if (!append) { if (File.Exists(@zipArchive)) File.Delete(@zipArchive); }
        if (!File.Exists(@inFilePath)) return -10;

        if (fileName == "") fileName = Path.GetFileName(@inFilePath);
        if (levelOfCompression < 0) levelOfCompression = 0; if (levelOfCompression > 10) levelOfCompression = 10;
        
        return zipCD(levelOfCompression, @zipArchive, @inFilePath, fileName, comment);
    }


    //integer that stores the current compressed number of files
    public static int cProgress = 0;

    //Compress a directory with all its files and subfolders to a zip file
    //
    //sourceDir             : the directory you want to compress
    //levelOfCompression    : the level of compression (0-10).
    //zipArchive            : the full path+name to the zip file to be created .
    //includeRoot           : set to true if you want the root folder of the directory to be included in the zip archive. Otherwise leave it to false.
    //
    //If you want to get the progress of compression, call the getAllFiles function to get the total number of files
    //in a directory and its subdirectories. The compressDir when called from a separate thread will update the public static int cProgress.
    //Divide this with the total no of files (as floats) and you have the % of the procedure.
    public static void compressDir(string sourceDir, int levelOfCompression, string zipArchive, bool includeRoot = false) {
        string fdir = @sourceDir.Replace("\\", "/");
		if(Directory.Exists(fdir)){
			if (File.Exists(@zipArchive)) File.Delete(@zipArchive);
			string[] ss = fdir.Split('/');
			string rdir = ss[ss.Length - 1];
			string root = rdir;

			cProgress = 0;

			if (levelOfCompression < 0) levelOfCompression = 0; if (levelOfCompression > 10) levelOfCompression = 10;

			#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
			foreach (string f in Directory.GetFiles(fdir)){
			#else
			foreach (string f in Directory.GetFiles(fdir, "*", SearchOption.AllDirectories)){
			#endif
				string s = f.Replace(fdir, rdir).Replace("\\", "/");
				if (!includeRoot) s = s.Replace(root + "/", "");
				compress_File(levelOfCompression, @zipArchive, f, true, s); cProgress++;
			}
		} 
    }

    //Use this function to get the total files of a directory and its subdirectories.
    public static int getAllFiles(string Dir) {
		#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
		string[] filePaths = Directory.GetFiles(@Dir);
		#else
        string[] filePaths = Directory.GetFiles(@Dir, "*", SearchOption.AllDirectories);
		#endif

        int res = filePaths.Length;
        filePaths = null;
        return res;
    }


	//---------------------------------------------------------------------------------------------------------------------------
	//GZIP SECTION
	//---------------------------------------------------------------------------------------------------------------------------

	//compress a byte buffer to gzip format.
	//
	//returns the size of the compressed buffer.
	//
	//source:		the uncompressed input buffer.
	//outBuffer:	the provided output buffer where the compressed data will be stored (it should be at least the size of the input buffer +18 bytes).
	//level:		the level of compression (0-9)
	//addHeader:	if a gzip header should be added. (recommended if you want to write out a gzip file)
	//addFooter:	if a gzip footer should be added. (recommended if you want to write out a gzip file)
	//
	public static int gzip(byte[] source, byte[] outBuffer, int level, bool addHeader = true, bool addFooter = true) {

		GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
		GCHandle dbuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

		int res = zipGzip(sbuf.AddrOfPinnedObject(), source.Length, dbuf.AddrOfPinnedObject(), level, addHeader, addFooter);

		sbuf.Free(); dbuf.Free();
		int hf = 0;
		if(addHeader) hf += 10;
		if(addFooter) hf += 8;
		return res + hf;
	}



	//get the uncompressed size from a gzip buffer that has a footer included
	//
	//source:		the gzip compressed input buffer. (it should have at least a gzip footer)
	public static int gzipUncompressedSize(byte[] source) {
		int res = source.Length;
		uint size = ((uint)source[res-4] & 0xff) |
				((uint)source[res-3] & 0xff) << 8 |
				((uint)source[res-2] & 0xff) << 16 |
				((uint)source[res-1]& 0xff) << 24;
		return (int)size;
	}

	//decompress a gzip buffer
	//
	//returns:		uncompressed size. negative error code on error. 
	//
	//source:		the gzip compressed input buffer.
	//outBuffer:	the provided output buffer where the uncompressed data will be stored.
	//hasHeader:	if the buffer has a header.
	//hasFooter:	if the buffer has a footer.
	//
	public static int unGzip(byte[] source, byte[] outBuffer, bool hasHeader = true, bool hasFooter = true) {
		GCHandle sbuf = GCHandle.Alloc(source, GCHandleType.Pinned);
		GCHandle dbuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
		
		int res = zipUnGzip( sbuf.AddrOfPinnedObject(), source.Length, dbuf.AddrOfPinnedObject(), hasHeader, hasFooter );

		sbuf.Free(); dbuf.Free();
		return res;				
	}


#endif
}

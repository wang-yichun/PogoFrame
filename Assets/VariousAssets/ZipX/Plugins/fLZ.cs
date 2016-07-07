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


public class fLZ{

#if !UNITY_WEBPLAYER

    internal static bool isle = BitConverter.IsLittleEndian;

#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR
	private const string libname = "fastlz";
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA
	private const string libname = "libfastlz";
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA || UNITY_ANDROID || UNITY_STANDALONE_LINUX
	#if (UNITY_STANDALONE_OSX  || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)&& !UNITY_EDITOR_WIN
			[DllImport(libname, EntryPoint = "fsetPermissions")]
			internal static extern int fsetPermissions(string filePath, string _user, string _group, string _other);
		#endif
        [DllImport(libname, EntryPoint = "fLZcompressFile")]
		internal static extern int fLZcompressFile(int level, string inFile, string outFile, bool overwrite, IntPtr percent);
        [DllImport(libname, EntryPoint = "fLZdecompressFile")]
		internal static extern int fLZdecompressFile(string inFile, string outFile, bool overwrite, IntPtr percent, IntPtr FileBuffer, int fileBufferLength);
        [DllImport(libname, EntryPoint = "fLZreleaseBuffer")]
        internal static extern int fLZreleaseBuffer(IntPtr buffer);
        [DllImport(libname, EntryPoint = "fLZcompressBuffer")]
        internal static extern IntPtr fLZcompressBuffer(IntPtr buffer, int bufferLength, int level, ref int v);
        [DllImport(libname, EntryPoint = "fLZdecompressBuffer")]
        internal static extern int fLZdecompressBuffer(IntPtr buffer, int bufferLength, IntPtr outbuffer);
#endif

#if (UNITY_IOS || UNITY_WEBGL || UNITY_IPHONE) && !UNITY_EDITOR
		#if (UNITY_IOS  || UNITY_IPHONE)
		[DllImport("__Internal")]
		internal static extern int fsetPermissions(string filePath, string _user, string _group, string _other);
		#endif
		[DllImport("__Internal")]
		internal static extern int fLZcompressFile(int level, string inFile, string outFile, bool overwrite, IntPtr percent);
        [DllImport("__Internal")]
		internal static extern int fLZdecompressFile(string inFile, string outFile, bool overwrite, IntPtr percent, IntPtr FileBuffer, int fileBufferLength);
        [DllImport("__Internal")]
		internal static extern int fLZreleaseBuffer(IntPtr buffer);
        [DllImport("__Internal")]
        internal static extern IntPtr fLZcompressBuffer(IntPtr buffer, int bufferLength, int level, ref int v);
        [DllImport("__Internal")]
        internal static extern int fLZdecompressBuffer(IntPtr buffer, int bufferLength, IntPtr outbuffer);
#endif


	// set permissions of a file in user, group, other.
	// Each string should contain any or all chars of "rwx".
	// returns 0 on success
	public static int setFilePermissions(string filePath, string _user, string _group, string _other){
		#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
			return fsetPermissions(filePath, _user, _group, _other);
		#else
			return -1;
		#endif
	}


    //Compress a file to fLZ.
    //
    //Full paths to the files should be provided.
    //level:    level of compression (1 = faster/bigger, 2 = slower/smaller).
    //returns:  size of resulting archive in bytes
    //progress: provide a single item ulong array to get the progress of the compression in real time. (only when called from a thread/task)
    //

    public static int compressFile(string inFile, string outFile, int level, bool overwrite, ulong[] progress)
    {
        if (level < 1) level = 1;
        if (level > 2) level = 2;
		GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
        int res = fLZcompressFile(level, @inFile, @outFile,  overwrite,  ibuf.AddrOfPinnedObject());
        ibuf.Free();
        return res;
    }

    //Decompress an fLZ file.
    //
    //Full paths to the files should be provided.
    //returns: 1 on success.
    //progress: provide a single item ulong array to get the progress of the decompression in real time. (only when called from a thread/task)
	//FileBuffer: A buffer that holds an flz file. When assigned the function will decompress from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    public static int decompressFile(string inFile, string outFile, bool overwrite, ulong[] progress, byte[] FileBuffer = null)
    {
		int res = 0;
		GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer != null) {
                GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = fLZdecompressFile(null, @outFile, overwrite, ibuf.AddrOfPinnedObject(), fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free(); ibuf.Free();
				return res;
			}
		#endif
			res = fLZdecompressFile(inFile, @outFile, overwrite, ibuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
			ibuf.Free();
			return res;
    }


    //Compress a byte buffer in fLZ format.
    //
    //inBuffer:     the uncompressed buffer.
    //outBuffer:    a referenced buffer that will be resized to fit the fLZ compressed data.
    //includeSize:  include the uncompressed size of the buffer in the resulted compressed one because fLZ does not include this.
    //level:        level of compression (1 = faster/bigger, 2 = slower/smaller).
    //returns true on success
	//
    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, int level, bool includeSize = true)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since fLZ lib does not support this.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 2) level = 2;

        ptr = fLZcompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length,  level, ref res);

        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { fLZreleaseBuffer(ptr); return false; }

        System.Array.Resize(ref outBuffer, res + size);

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i+res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res  );

        fLZreleaseBuffer(ptr);
		bsiz = null;

        return true;
    }


    //Compress a byte buffer in fLZ format.
    //
    //inBuffer:     the uncompressed buffer.
    //outBuffer:    a referenced buffer that will be resized to fit the fLZ compressed data.
    //includeSize:  include the uncompressed size of the buffer in the resulted compressed one because fLZ does not include this.
    //level:        level of compression (1 = faster/bigger, 2 = slower/smaller).
	//returns: a new buffer with the compressed data.
    //
    public static byte[] compressBuffer(byte[] inBuffer, int level, bool includeSize = true)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since fLZ lib does not support this.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 9) level = 9;

        ptr = fLZcompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length,  level, ref res);
        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { fLZreleaseBuffer(ptr); return null; }

        byte[] outBuffer = new byte[res + size];

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res);

        fLZreleaseBuffer(ptr);
		bsiz = null;

        return outBuffer;
    }


    //Decompress an fLZ compressed buffer to a referenced buffer.
    //
    //inBuffer: the fLZ compressed buffer
    //outBuffer: a referenced buffer that will be resized to store the uncompressed data.
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
    //returns true on success
    //
    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = true, int customLength = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        //if the hacked in fLZ footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter) {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        }
        else {
            uncompressedSize = customLength; 
        }

        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);


        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res == 0) return true;

        return false;
    }

	//Decompress an flz compressed buffer to a referenced fixed size buffer.
    //
    //inBuffer: the flz compressed buffer
    //outBuffer: a referenced fixed size buffer where the data will get decompressed
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the useFooter is used!
    //returns uncompressedSize
    //
	public static int decompressBufferFixed(byte[] inBuffer, ref byte[] outBuffer, bool safe = true, bool useFooter = true, int customLength = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter){
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        }
        else{
            uncompressedSize = customLength; 
        }

		//Check if the uncompressed size is bigger then the size of the fixed buffer. Then:
		//1. write only the data that fits in it.
		//2. or return a negative number. 
		//It depends on if we set the safe flag to true or not.
		if(uncompressedSize > outBuffer.Length) {
			if(safe) return -101;  else  uncompressedSize = outBuffer.Length;
		 }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

		if(safe) { if (res != 0) return -101; }

        return uncompressedSize;
    }


    //Decompress an fLZ compressed buffer to a new buffer.
    //
    //inBuffer: the fLZ compressed buffer
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
	//returns: a new buffer with the uncompressed data.
    //
    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = true, int customLength = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        //if the hacked in fLZ footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter)
        {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        }
        else
        {
            uncompressedSize = customLength;
        }

        byte[] outBuffer = new byte[uncompressedSize];

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);


        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res != 1) return null;

        return outBuffer;
    }

#endif
}


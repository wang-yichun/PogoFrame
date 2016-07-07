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


public class brotli{

#if !UNITY_WEBPLAYER && !UNITY_WEBGL

    internal static bool isle = BitConverter.IsLittleEndian;

#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR
	private const string libname = "brotli";
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA
	private const string libname = "libbrotli";
#endif
	//int brDecompresss(const char* input_path, const char* output_path)
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA || UNITY_ANDROID || UNITY_STANDALONE_LINUX
	#if (UNITY_STANDALONE_OSX  || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)&& !UNITY_EDITOR_WIN
			[DllImport(libname, EntryPoint = "setPermissions")]
			internal static extern int setPermissions(string filePath, string _user, string _group, string _other);
		#endif
        [DllImport(libname, EntryPoint = "brCompress")]
		internal static extern int brCompress( string inFile, string outFile, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
        [DllImport(libname, EntryPoint = "brDecompresss")]
		internal static extern int brDecompresss(string inFile, string outFile, IntPtr proc, IntPtr FileBuffer, int fileBufferLength);
        [DllImport(libname, EntryPoint = "brReleaseBuffer")]
        internal static extern int brReleaseBuffer(IntPtr buffer);
        [DllImport(libname, EntryPoint = "brCompressBuffer")]
        internal static extern IntPtr brCompressBuffer( int bufferLength, IntPtr buffer, IntPtr encodedSize, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
		//this will work on small files with one meta block
        [DllImport(libname, EntryPoint = "brGetDecodedSize")]
        internal static extern int brGetDecodedSize(int bufferLength, IntPtr buffer);
        [DllImport(libname, EntryPoint = "brDecompressBuffer")]
        internal static extern int brDecompressBuffer(int bufferLength, IntPtr buffer, int outLength, IntPtr outbuffer);
#endif

#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
		[DllImport("__Internal")]
		internal static extern int setPermissions(string filePath, string _user, string _group, string _other);
		[DllImport("__Internal")]
		internal static extern int brCompress( string inFile, string outFile, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
        [DllImport("__Internal")]
		internal static extern int brDecompresss(string inFile, string outFile, IntPtr proc, IntPtr FileBuffer, int fileBufferLength);
        [DllImport("__Internal")]
		internal static extern int brReleaseBuffer(IntPtr buffer);
        [DllImport("__Internal")]
        internal static extern IntPtr brCompressBuffer( int bufferLength, IntPtr buffer, IntPtr encodedSize, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
		//this will work on small files with one meta block
		[DllImport("__Internal")]
		internal static extern int brGetDecodedSize(int bufferLength, IntPtr buffer);
        [DllImport("__Internal")]
        internal static extern int brDecompressBuffer(int bufferLength, IntPtr buffer,  int outLength, IntPtr outbuffer);
#endif


	// set permissions of a file in user, group, other.
	// Each string should contain any or all chars of "rwx".
	// returns 0 on success
	public static int setFilePermissions(string filePath, string _user, string _group, string _other){
		#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
			return setPermissions(filePath, _user, _group, _other);
		#else
			return -1;
		#endif
	}

    //Compress a file to brotli format.
    //
    //Full paths to the files should be provided.
	//inFile:		The input file
	//outFile:		The output file
	//proc:			A single item referenced int array to provide progress of compression
	//
    //quality:    (0  - 11) quality of compression (0 = faster/bigger - 11 = slower/smaller).
	//
	// Base 2 logarithm of the sliding window size. Range is 10 to 24.
	//lgwin  :    (10 - 24) memory used for compression (higher numbers use more ram)
	//
	// Base 2 logarithm of the maximum input block size. Range is 16 to 24.
	// If set to 0, the value will be set based on the quality.
	//lgblock:    0 for auto or 16-24
	//mode   :	  (0  -  2) 0 = default, 1 = utf8 text, 2 = woff 2.0 font
    //
	//error codes:	 1  : OK
	//				-1  : compression failed
	//				-2  : not enough memory
	//				-3  : could not close in file
	//				-4  : could not close out file
	//
    public static int compressFile(string inFile, string outFile,  int[] proc, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
        if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;
		GCHandle cbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);
        int res = brCompress( @inFile, @outFile,cbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);
		cbuf.Free();
		return res;
    }

    //Decompress a brotli file.
    //
    //Full paths to the files should be provided.
	//inFile:		The input file
	//outFile:		The output file
	//proc:			A single item referenced int array to provide progress of decompression
	//FileBuffer:	A buffer that holds a brotli file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //returns: 1 on success.
	//
	//error codes:	 1  : OK
	//				-1  : failed to write output
	//				-2  : corrupt input
	//				-3  : could not close in file
	//				-4  : could not close out file
	//
    public static int decompressFile(string inFile, string outFile, int[] proc, byte[] FileBuffer = null) {
		GCHandle cbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);
		int res;
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer!= null) {
				GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = brDecompresss(@inFile, @outFile, cbuf.AddrOfPinnedObject(), fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free();
				return res;
			}
		#endif
		res = brDecompresss(@inFile, @outFile, cbuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
		cbuf.Free();
		return res;
    }


	//get the uncompressed size of a brotli buffer.
	//this will work only on small buffers with one metablock
	//
	//inBuffer:	the input buffer that stores a brotli compressed buffer.
	public static int getDecodedSize(byte[] inBuffer) {
		GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
		int res = brGetDecodedSize(inBuffer.Length, cbuf.AddrOfPinnedObject());
		cbuf.Free();
		return res;
	}


    //Compress a byte buffer in brotli format.
    //
    //inBuffer:     the uncompressed buffer.
    //outBuffer:    a referenced buffer that will store the compressed data. (it should be large enough to store it.)
	//proc:			A single item referenced int array to provide progress of compression
	//
    //includeSize:  include the uncompressed size of the buffer in the resulted compressed one because brotli does not support it for larger then 1 metablock.
	//
    //quality:    (0  - 11) quality of compression (0 = faster/bigger - 11 = slower/smaller).
	//
	// Base 2 logarithm of the sliding window size. Range is 10 to 24.
	//lgwin  :    (10 - 24) memory used for compression (higher numbers use more ram)
	//
	// Base 2 logarithm of the maximum input block size. Range is 16 to 24.
	// If set to 0, the value will be set based on the quality.
	//lgblock:    0 for auto or 16-24
	//mode   :	  (0  -  2) 0 = default, 1 = utf8 text, 2 = woff 2.0 font
    //returns true on success
	//
    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
	    if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int  size = 0;
		byte[] bsiz = null;
		int[] esiz = new int[1];//the compressed size
		GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        //if the uncompressed size of the buffer should be included. This is a hack since brotli lib does not support this on larger buffers.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();

        if (ptr == IntPtr.Zero) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return false; }

        System.Array.Resize(ref outBuffer, esiz[0] + size);

		//add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        Marshal.Copy( ptr, outBuffer, 0, esiz[0] );

        brReleaseBuffer(ptr);
		esiz = null;
		bsiz = null;

        return true;
    }

	//same as above only this function returns a new created buffer with the compressed data.
	//
    public static byte[] compressBuffer(byte[] inBuffer,  int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
	    if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int  size = 0;
		byte[] bsiz = null;
		int[] esiz = new int[1];//the compressed size
		GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        //if the uncompressed size of the buffer should be included. This is a hack since brotli lib does not support this on larger buffers.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();

        if (ptr == IntPtr.Zero) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return null; }

		byte[] outBuffer = new byte[esiz[0] + size];

		//add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        Marshal.Copy( ptr, outBuffer, 0, esiz[0] );

        brReleaseBuffer(ptr);
		esiz = null;
		bsiz = null;

        return outBuffer;
    }

	//same as above, only this time the compressed buffer is written in a fixed size buffer.
	//the compressed size in bytes is returned.
	//
    public static int compressBuffer(byte[] inBuffer, byte[] outBuffer, int[] proc, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
	    if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;
		int res = 0;
		int[] esiz = new int[1];//the compressed size
		GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);
		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();
		res = esiz[0];

        if (ptr == IntPtr.Zero) { brReleaseBuffer(ptr); esiz = null;  return 0; }

        Marshal.Copy( ptr, outBuffer, 0, esiz[0] );

        brReleaseBuffer(ptr);
		esiz = null;

        return res;
    }

    //Decompress a brotli compressed buffer to a referenced buffer.
    //
    //inBuffer: the brotli compressed buffer
    //outBuffer: a referenced buffer that will be resized to store the uncompressed data.
    //useFooter: if the input Buffer has the uncompressed size info.
    //returns true on success
    //
    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = false) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
		int uncompressedSize = 0, res2 = inBuffer.Length;

        if (useFooter) {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        } else {
			//use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
            uncompressedSize = getDecodedSize(inBuffer); 
        }

        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if(res == 1) return true; else return false;
    }

	//same as above only this time the uncompressed data is returned in a new created buffer
	//
    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = false) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
		int uncompressedSize = 0, res2 = inBuffer.Length;

        if (useFooter) {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        } else {
			//use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
            uncompressedSize = getDecodedSize(inBuffer); 
        }

       // System.Array.Resize(ref outBuffer, uncompressedSize);
	   byte[] outBuffer = new byte[uncompressedSize];

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if(res == 1) return outBuffer; else return null;
    }


	//same as above only the decompressed data will be stored in a fixed size outBuffer.
	//make sure the fixed buffer is big enough to store the data.
	//
	//returns: uncompressed size in bytes.
	//
    public static int decompressBuffer(byte[] inBuffer, byte[] outBuffer, bool useFooter = false) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
		int uncompressedSize = 0, res2 = inBuffer.Length;

        if (useFooter) {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        } else {
			//use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
            uncompressedSize = getDecodedSize(inBuffer); 
        }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if(res == 1) return uncompressedSize; else return 0;
    }

#endif
}


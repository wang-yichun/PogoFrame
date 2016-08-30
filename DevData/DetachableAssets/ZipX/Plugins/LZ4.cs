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


public class LZ4
{
#if !UNITY_WEBPLAYER

    internal static bool isle = BitConverter.IsLittleEndian;

#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR
	private const string libname = "lz4";
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA
	private const string libname = "liblz4";
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WP8_1 || UNITY_WSA || UNITY_ANDROID || UNITY_STANDALONE_LINUX
		#if (UNITY_STANDALONE_OSX  || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)&& !UNITY_EDITOR_WIN
			[DllImport(libname, EntryPoint = "z4setPermissions")]
			internal static extern int z4setPermissions(string filePath, string _user, string _group, string _other);
		#endif
        [DllImport(libname, EntryPoint = "LZ4DecompressFile")]
		internal static extern int LZ4DecompressFile(string inFile, string outFile, IntPtr bytes, IntPtr FileBuffer, int fileBufferLength);
        [DllImport(libname, EntryPoint = "LZ4CompressFile")]
		internal static extern int LZ4CompressFile(string inFile, string outFile, int level, IntPtr percentage, ref float rate);
        [DllImport(libname, EntryPoint = "LZ4releaseBuffer")]
        internal static extern int LZ4releaseBuffer(IntPtr buffer);
        [DllImport(libname, EntryPoint = "LZ4CompressBuffer")]
        internal static extern IntPtr LZ4CompressBuffer(IntPtr buffer, int bufferLength, ref int v, int level);
        [DllImport(libname, EntryPoint = "LZ4DecompressBuffer")]
        internal static extern int LZ4DecompressBuffer(IntPtr buffer, IntPtr outbuffer, int bufferLength);
#endif


#if (UNITY_IOS || UNITY_WEBGL || UNITY_IPHONE) && !UNITY_EDITOR
		#if (UNITY_IOS  || UNITY_IPHONE)
			[DllImport("__Internal")]
			private static extern int z4setPermissions(string filePath, string _user, string _group, string _other);
		#endif
		[DllImport("__Internal")]
		private static extern int LZ4DecompressFile(string inFile, string outFile, IntPtr bytes, IntPtr FileBuffer, int fileBufferLength);
       	[DllImport("__Internal")]
		private static extern int LZ4CompressFile(string inFile, string outFile, int level, IntPtr percentage, ref float rate);
        [DllImport("__Internal")]
		private static extern int LZ4releaseBuffer(IntPtr buffer);
       	[DllImport("__Internal")]
        private static extern IntPtr LZ4CompressBuffer(IntPtr buffer, int bufferLength, ref int v, int level);
       	[DllImport("__Internal")]
        private static extern int LZ4DecompressBuffer(IntPtr buffer, IntPtr outbuffer, int bufferLength);
#endif


	// set permissions of a file in user, group, other.
	// Each string should contain any or all chars of "rwx".
	// returns 0 on success
	public static int setFilePermissions(string filePath, string _user, string _group, string _other){
		#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
			return z4setPermissions(filePath, _user, _group, _other);
		#else
			return -1;
		#endif
	}

    //Compress a file to LZ4.
    //
    //Full paths to the files should be provided.
    //level: level of compression (1 - 9).
    //returns: rate of compression.
    //progress: provide a single item float array to get the progress of the compression in real time. (only when called from a thread/task)
    //
    public static float compress(string inFile, string outFile, int level, float[] progress) {
        if (level < 1) level = 1;
        if (level > 9) level = 9;
        float rate = 0;
        progress[0] = 0;
		GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
		
        int res =  LZ4CompressFile(@inFile, @outFile, level, ibuf.AddrOfPinnedObject(), ref rate);
        
        ibuf.Free();
        if (res != 0) return -1;
        return rate;
    }

    //Decompress an LZ4 file.
    //
    //Full paths to the files should be provided.
    //returns: 0 on success.
    //bytes: provide a single item integer array to get the bytes currently decompressed in real time.  (only when called from a thread/task)
	//FileBuffer: A buffer that holds an LZ4 file. When assigned the function will decompress from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    //
    public static int decompress(string inFile, string outFile, int[] bytes, byte[] FileBuffer = null) {
        bytes[0] = 0;
		int res = 0;
		GCHandle ibuf = GCHandle.Alloc(bytes , GCHandleType.Pinned);
		
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			if(FileBuffer != null) {
                GCHandle fbuf = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
				res = LZ4DecompressFile(null, @outFile, ibuf.AddrOfPinnedObject(), fbuf.AddrOfPinnedObject(), FileBuffer.Length);
				fbuf.Free(); ibuf.Free();
				return res;
			}
		#endif
		res =LZ4DecompressFile(@inFile, @outFile, ibuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
		ibuf.Free();
        return res;
    }
	 

    //Compress a byte buffer in LZ4 format.
    //
    //inBuffer: the uncompressed buffer.
    //outBuffer: a referenced buffer that will be resized to fit the lz4 compressed data.
	//level: level of compression (1 - 9).
    //includeSize: include the uncompressed size of the buffer in the resulted compressed one because lz4 does not include this.
    //returns true on success
	//
    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, int level, bool includeSize = true) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since lz4 lib does not support this.
        if (includeSize){
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 9) level = 9;

        ptr = LZ4CompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, ref res, level);

        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { LZ4releaseBuffer(ptr); return false; }

        System.Array.Resize(ref outBuffer, res + size);

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i+res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res  );

        LZ4releaseBuffer(ptr);
		bsiz = null;
        return true;
    }



    //Compress a byte buffer in LZ4 format and return a new buffer compressed.
    //
    //inBuffer: the uncompressed buffer.
	//level: level of compression (1 - 9).
    //includeSize: include the uncompressed size of the buffer in the resulted compressed one because lz4 does not include this.
	//returns: a new buffer with the compressed data.
    //
    public static byte[] compressBuffer(byte[] inBuffer, int level, bool includeSize = true) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since lz4 lib does not support this.
        if (includeSize){
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 9) level = 9;

        ptr = LZ4CompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, ref res, level);
        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { LZ4releaseBuffer(ptr); return null; }

        byte[] outBuffer = new byte[res + size];

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res);

        LZ4releaseBuffer(ptr);
		bsiz = null;

        return outBuffer;
    }


	//Compress a byte buffer in LZ4 format at a specific position of a fixed size outBuffer
	//
	//inBuffer: the uncompressed buffer.
	//outBuffer: a referenced buffer of fixed size that could have already some lz4 compressed buffers stored.
	//partialIndex: the position at which the compressed data will be written to.
	//level: level of compression (1 - 9).
	//includeSize: include the uncompressed size of the buffer in the resulted compressed one because lz4 does not include this.
	//
	//returns compressed size (+4 bytes if footer is used)
	//
	public static int compressBufferPartialFixed (byte[] inBuffer, ref byte[] outBuffer,int partialIndex, int level, bool includeSize = true) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since lz4 lib does not support this.
        if (includeSize){
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 9) level = 9;

        ptr = LZ4CompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, ref res, level);

        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { LZ4releaseBuffer(ptr); return 0; }

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[partialIndex + res + i ] = bsiz[i];   }

        Marshal.Copy(ptr, outBuffer, partialIndex, res );

        LZ4releaseBuffer(ptr);
		bsiz = null;
        return res + size;
    }


	//A function to decompress a buffer stored in a fixed size buffer that stores many compressed lz4 buffers
	//
	//inBuffer: input buffer that stores multiple lz4 compressed buffers
	//outBuffer: a referenced fixed size buffer where the data will get decompressed
	//partialIndex: position of an lz4 compressed buffer in the inBuffer
	//compressedBufferSize: compressed size of the buffer to be decompressed 
	//safe: Check if the uncompressed size is bigger then the size of the fixed buffer.
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
	//
	//returns the uncompressed size
	public static int decompressBufferPartialFixed (byte[] inBuffer, ref byte[] outBuffer, int partialIndex , int compressedBufferSize, bool safe = true, bool useFooter = true, int customLength = 0) {

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);

        int uncompressedSize = 0;

		//be carefull with this. You must know exactly where your compressed data lies in the inBuffer
		int res2 = partialIndex + compressedBufferSize;

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
		//1. write only the data that fit in it.
		//2. or return a negative number. 
		//It depends on if we set the safe flag to true or not.
		if(uncompressedSize > outBuffer.Length) {
			if(safe) return -101;  else  uncompressedSize = outBuffer.Length;
		 }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

		IntPtr ptrPartial;
		ptrPartial = new IntPtr(cbuf.AddrOfPinnedObject().ToInt64() + partialIndex);

        //res should be the compressed size
        LZ4DecompressBuffer(ptrPartial,  obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

        return uncompressedSize;
    }


    //Decompress an lz4 compressed buffer to a referenced buffer.
    //
    //inBuffer: the lz4 compressed buffer
    //outBuffer: a referenced buffer that will be resized to store the uncompressed data.
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
    //returns true on success
    //
    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = true, int customLength = 0) {
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

        //byte[] outbuffer = new byte[uncompressedSize];
        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        //res should be the compressed size
        int res = LZ4DecompressBuffer(cbuf.AddrOfPinnedObject(),  obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

        if (res != res2) { /*Debug.Log("ERROR: " + res.ToString());*/ return false; }

        return true;
    }

	//Decompress an lz4 compressed buffer to a referenced fixed size buffer.
    //
    //inBuffer: the lz4 compressed buffer
    //outBuffer: a referenced fixed size buffer where the data will get decompressed
	//safe: Check if the uncompressed size is bigger then the size of the fixed buffer.
    //usefooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
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
		//1. write only the data that fit in it.
		//2. or return a negative number. 
		//It depends on if we set the safe flag to true or not.
		if(uncompressedSize > outBuffer.Length) {
			if(safe) return -101;  else  uncompressedSize = outBuffer.Length;
		 }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        //res should be the compressed size
        int res = LZ4DecompressBuffer(cbuf.AddrOfPinnedObject(),  obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

		if(safe) {
			if (res != res2) { /*Debug.Log("ERROR: " + res.ToString());*/ return -101; }
		}

        return uncompressedSize;
    }


    //Decompress an lz4 compressed buffer to a new buffer.
    //
    //inBuffer: the lz4 compressed buffer
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
	//returns: a new buffer with the uncompressed data.
    //
    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = true, int customLength = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
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

        //res should be the compressed size
        int res = LZ4DecompressBuffer(cbuf.AddrOfPinnedObject(), obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

        if (res != res2) { /*Debug.Log("ERROR: " + res.ToString());*/ return null; }

        return outBuffer;
    }

#endif
}
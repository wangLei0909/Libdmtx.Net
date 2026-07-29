using System.Runtime.InteropServices;

namespace Libdmtx.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct DmtxPixelLoc
{
    public int X;
    public int Y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DmtxVector2
{
    public double X;
    public double Y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DmtxRay2
{
    public double tMin;
    public double tMax;
    public DmtxVector2 p;
    public DmtxVector2 v;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DmtxTime
{
    public nint sec;       // time_t
    public nuint usec;     // unsigned long
}

[StructLayout(LayoutKind.Sequential)]
internal struct DmtxScanConstraint
{
    public nint maxTimeout;   // DmtxTime*
    public int maxIterations;
    public int iterations;
    public DmtxScanStatus stopCause;
}

// Opaque handle types — these are pointer-sized handles to native-allocated objects.
// We treat them as SafeHandle or just IntPtr in the managed layer.
// Native structs for reference only — actual layout is in C headers.
// libdmtx treats these as opaque; we only hold IntPtr to them.

// DmtxImage layout (for reference — will marshal as blittable)
[StructLayout(LayoutKind.Sequential)]
internal struct DmtxImageNative
{
    public int width;
    public int height;
    public int pixelPacking;
    public int bitsPerPixel;
    public int bytesPerPixel;
    public int rowPadBytes;
    public int rowSizeBytes;
    public int imageFlip;
    public int channelCount;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public int[] channelStart;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public int[] bitsPerChannel;
    public nint pxl;  // unsigned char*
}

// DmtxDecode layout
[StructLayout(LayoutKind.Sequential)]
internal struct DmtxDecodeNative
{
    public int edgeMin;
    public int edgeMax;
    public int scanGap;
    public int fnc1;
    public double squareDevn;
    public int sizeIdxExpected;
    public int edgeThresh;
    public int xMin;
    public int xMax;
    public int yMin;
    public int yMax;
    public int scale;
    public nint cache;     // unsigned char*
    public nint image;     // DmtxImage*
    // DmtxScanGrid follows — we'll treat the rest as opaque
}

// DmtxEncode layout
[StructLayout(LayoutKind.Sequential)]
internal struct DmtxEncodeNative
{
    public int method;
    public int scheme;
    public int sizeIdxRequest;
    public int marginSize;
    public int moduleSize;
    public int pixelPacking;
    public int imageFlip;
    public int rowPadBytes;
    public int fnc1;
    public nint message;    // DmtxMessage*
    public nint image;      // DmtxImage*
    // region + xfrm follow — opaque
}

// DmtxRegion layout
[StructLayout(LayoutKind.Sequential)]
internal struct DmtxRegionNative
{
    // Trail blazing values
    public int jumpToPos;
    public int jumpToNeg;
    public int stepsTotal;
    public DmtxPixelLoc finalPos;
    public DmtxPixelLoc finalNeg;
    public DmtxPixelLoc boundMin;
    public DmtxPixelLoc boundMax;
    public DmtxPointFlow flowBegin;
    // Orientation
    public int polarity;
    public int stepR;
    public int stepT;
    public DmtxPixelLoc locR;
    public DmtxPixelLoc locT;
    // ... more fields follow
}

[StructLayout(LayoutKind.Sequential)]
internal struct DmtxPointFlow
{
    public int plane;
    public int arrive;
    public int depart;
    public int mag;
    public DmtxPixelLoc loc;
}

// DmtxMessage layout
[StructLayout(LayoutKind.Sequential)]
internal struct DmtxMessageNative
{
    public nuint arraySize;    // size_t
    public nuint codeSize;     // size_t
    public nuint outputSize;   // size_t
    public int outputIdx;
    public int padCount;
    public int fnc1;
    public nint array;         // unsigned char*
    public nint code;          // unsigned char*
    public nint output;        // unsigned char*
}

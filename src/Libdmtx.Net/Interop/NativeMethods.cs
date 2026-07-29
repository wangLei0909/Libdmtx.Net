using System.Runtime.InteropServices;

namespace Libdmtx.Interop;

/// <summary>
/// Raw P/Invoke declarations for libdmtx native API.
/// The native library "libdmtx" resolves to libdmtx.dll (Windows),
/// libdmtx.so (Linux), or libdmtx.dylib (macOS).
/// </summary>
internal static partial class NativeMethods
{
    private const string DllName = "libdmtx";

    // ===== dmtximage.c =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxImageCreate(
        byte[] pxl, int width, int height, int pack);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxImageDestroy(ref nint img);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxImageSetProp(nint img, int prop, int value);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxImageGetProp(nint img, int prop);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxImageGetByteOffset(nint img, int x, int y);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxImageSetChannel(
        nint img, int channelStart, int bitsPerChannel);

    // ===== dmtxdecode.c =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxDecodeCreate(nint img, int scale);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxDecodeDestroy(ref nint dec);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxDecodeSetProp(nint dec, int prop, int value);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxDecodeGetProp(nint dec, int prop);

    // ===== dmtxregion.c =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxRegionFindNext(nint dec, ref DmtxTime timeout);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxRegionFindNext(nint dec, nint timeoutPtr);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxRegionFindNextDeterministic(
        nint dec, ref DmtxScanConstraint constraint);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxRegionCreate(nint reg);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxRegionDestroy(ref nint reg);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxRegionUpdateCorners(
        nint dec, nint reg,
        ref DmtxVector2 p00, ref DmtxVector2 p10,
        ref DmtxVector2 p11, ref DmtxVector2 p01);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxRegionUpdateXfrms(nint dec, nint reg);

    // ===== dmtxmessage.c =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxMessageCreate(int sizeIdx, int symbolFormat);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxMessageDestroy(ref nint msg);

    // ===== dmtxencode.c =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxEncodeCreate();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxEncodeDestroy(ref nint enc);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxEncodeSetProp(nint enc, int prop, int value);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxEncodeGetProp(nint enc, int prop);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxEncodeDataMatrix(
        nint enc, int n, byte[] s);

    // ===== dmtxtime.c =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern DmtxTime dmtxTimeNow();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern DmtxTime dmtxTimeAdd(DmtxTime t, long msec);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxTimeExceeded(DmtxTime timeout);

    // ===== dmtxsymbol.c =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int dmtxGetSymbolAttribute(int attribute, int sizeIdx);

    // ===== decode entry points =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxDecodeMatrixRegion(
        nint dec, nint reg, int fix);

    // ===== utility =====

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint dmtxVersion();
}

using Libdmtx.Interop;
using System.Runtime.InteropServices;

namespace Libdmtx;

/// <summary>
/// Main entry point for encoding and decoding Data Matrix barcodes
/// using the native libdmtx library.
/// </summary>
public static class Dmtx
{
    /// <summary>Gets the version string of the underlying libdmtx library.</summary>
    public static string? Version
    {
        get
        {
            try
            {
                nint ptr = NativeMethods.dmtxVersion();
                return ptr != nint.Zero ? Marshal.PtrToStringAnsi(ptr) : null;
            }
            catch { return null; }
        }
    }

    /// <summary>
    /// Decode Data Matrix barcodes from raw 24-bit BGR image data.
    /// </summary>
    /// <param name="pixelData">Raw pixel buffer (BGR24).</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="stride">Bytes per row (width*3 + row padding).</param>
    /// <param name="options">Decode options, or null for defaults.</param>
    public static DecodeResult[] Decode(
        byte[] pixelData, int width, int height, int stride,
        DecodeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(pixelData);
        options ??= DecodeOptions.Default;

        using var image = new DmtxImage(pixelData, width, height, stride);
        nint decode = NativeMethods.dmtxDecodeCreate(image.Handle, options.Shrink);
        if (decode == nint.Zero)
            throw new DmtxException("Failed to create decoder");

        try
        {
            ApplyDecodeProperties(decode, options);
            return ScanForSymbols(decode, height, options);
        }
        finally
        {
            NativeMethods.dmtxDecodeDestroy(ref decode);
        }
    }

    /// <summary>Decode from BGR24 data with default stride.</summary>
    public static DecodeResult[] Decode(
        byte[] pixelData, int width, int height,
        DecodeOptions? options = null)
        => Decode(pixelData, width, height, width * 3, options);

    /// <summary>Encode data into a Data Matrix barcode (returns 24-bit BGR pixels).</summary>
    public static EncodeResult Encode(byte[] data, EncodeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(data);
        options ??= EncodeOptions.Default;

        nint enc = NativeMethods.dmtxEncodeCreate();
        if (enc == nint.Zero)
            throw new DmtxException("Failed to create encoder");

        try
        {
            ApplyEncodeProperties(enc, options);

            int result = NativeMethods.dmtxEncodeDataMatrix(enc, data.Length, data);
            if (result == 0)
                throw new DmtxException("Encoding failed - data may be too large");

            nint img = ReadEncodeImagePtr(enc);
            int imgWidth = NativeMethods.dmtxImageGetProp(img, (int)DmtxProperty.Width);
            int imgHeight = NativeMethods.dmtxImageGetProp(img, (int)DmtxProperty.Height);
            int stride = imgWidth * 3;
            byte[] pixelData = new byte[stride * imgHeight];
            CopyEncodePixels(img, pixelData, stride, imgWidth, imgHeight);

            int sizeIdx = NativeMethods.dmtxEncodeGetProp(enc, (int)DmtxProperty.SizeRequest);
            int symRows = 0, symCols = 0;
            if (sizeIdx >= 0)
            {
                symRows = NativeMethods.dmtxGetSymbolAttribute((int)DmtxSymAttribute.SymbolRows, sizeIdx);
                symCols = NativeMethods.dmtxGetSymbolAttribute((int)DmtxSymAttribute.SymbolCols, sizeIdx);
            }

            return new EncodeResult
            {
                PixelData = pixelData,
                Width = imgWidth,
                Height = imgHeight,
                Stride = stride,
                SymbolRows = symRows,
                SymbolCols = symCols,
            };
        }
        finally
        {
            NativeMethods.dmtxEncodeDestroy(ref enc);
        }
    }

    /// <summary>Encode a string into a Data Matrix barcode.</summary>
    public static EncodeResult EncodeString(string text, EncodeOptions? options = null)
        => Encode(System.Text.Encoding.ASCII.GetBytes(text), options);

    // ========== Internal helpers ==========

    private static void ApplyDecodeProperties(nint decode, DecodeOptions o)
    {
        if (o.EdgeMin.HasValue) SetDecProp(DmtxProperty.EdgeMin, o.EdgeMin.Value);
        if (o.EdgeMax.HasValue) SetDecProp(DmtxProperty.EdgeMax, o.EdgeMax.Value);
        if (o.ScanGap.HasValue) SetDecProp(DmtxProperty.ScanGap, o.ScanGap.Value);
        if (o.SizeIdxExpected.HasValue) SetDecProp(DmtxProperty.SymbolSize, o.SizeIdxExpected.Value);
        if (o.EdgeThresh.HasValue) SetDecProp(DmtxProperty.EdgeThresh, o.EdgeThresh.Value);
        if (o.XMin.HasValue) SetDecProp(DmtxProperty.Xmin, o.XMin.Value);
        if (o.XMax.HasValue) SetDecProp(DmtxProperty.Xmax, o.XMax.Value);
        if (o.YMin.HasValue) SetDecProp(DmtxProperty.Ymin, o.YMin.Value);
        if (o.YMax.HasValue) SetDecProp(DmtxProperty.Ymax, o.YMax.Value);

        void SetDecProp(DmtxProperty prop, int value) =>
            NativeMethods.dmtxDecodeSetProp(decode, (int)prop, value);
    }

    private static void ApplyEncodeProperties(nint enc, EncodeOptions o)
    {
        NativeMethods.dmtxEncodeSetProp(enc, (int)DmtxProperty.MarginSize, o.MarginSize);
        NativeMethods.dmtxEncodeSetProp(enc, (int)DmtxProperty.ModuleSize, o.ModuleSize);
        NativeMethods.dmtxEncodeSetProp(enc, (int)DmtxProperty.Scheme, o.Scheme);
        NativeMethods.dmtxEncodeSetProp(enc, (int)DmtxProperty.SizeRequest, o.SizeIdxRequest);
        NativeMethods.dmtxEncodeSetProp(enc, (int)DmtxProperty.ImageFlip, (int)DmtxFlip.Y);
    }

    private static unsafe DecodeResult[] ScanForSymbols(nint decode, int imageHeight, DecodeOptions options)
    {
        var results = new List<DecodeResult>();
        int maxCodes = options.MaxCodes ?? int.MaxValue;
        int corrections = options.CorrectionsMax ?? -1;

        DmtxTime timeout = default;
        nint timeoutPtr = nint.Zero;
        if (options.TimeoutMs.HasValue)
        {
            timeout = NativeMethods.dmtxTimeAdd(NativeMethods.dmtxTimeNow(), options.TimeoutMs.Value);
            timeoutPtr = (nint)(&timeout);
        }

        try
        {
            while (results.Count < maxCodes)
            {
                nint region = NativeMethods.dmtxRegionFindNext(decode, timeoutPtr);
                if (region == nint.Zero) break;

                try
                {
                    nint msg = NativeMethods.dmtxDecodeMatrixRegion(decode, region, corrections);
                    if (msg != nint.Zero)
                    {
                        try
                        {
                            var nativeMsg = Marshal.PtrToStructure<DmtxMessageNative>(msg);
                            var result = new DecodeResult();

                            if (nativeMsg.output != nint.Zero && nativeMsg.outputSize > 0)
                            {
                                result.Data = new byte[nativeMsg.outputSize];
                                Marshal.Copy(nativeMsg.output, result.Data, 0, result.Data.Length);
                            }
                            result.PadCount = nativeMsg.padCount;

                            unsafe
                            {
                                int sizeIdx = *(int*)(region + 296);
                                if (sizeIdx >= 0)
                                {
                                    result.Rows = *(int*)(region + 300);
                                    result.Cols = *(int*)(region + 304);
                                    result.Capacity = NativeMethods.dmtxGetSymbolAttribute(
                                        (int)DmtxSymAttribute.SymbolDataWords, sizeIdx);
                                }

                                double* m = (double*)(region + 392);
                                double[] sx = { 0.0, 0.0, 1.0, 1.0 };
                                double[] sy = { 0.0, 1.0, 0.0, 1.0 };
                                result.Corners = new (int X, int Y)[4];
                                for (int k = 0; k < 4; k++)
                                {
                                    // dmtxMatrix3VMultiplyBy 使用列主序访问
                                    double tx = sx[k] * m[0] + sy[k] * m[3] + m[6];
                                    double ty = sx[k] * m[1] + sy[k] * m[4] + m[7];
                                    double tw = sx[k] * m[2] + sy[k] * m[5] + m[8];
                                    int cx = (int)(tx / tw + 0.5);
                                    int cy = imageHeight - 1 - (int)(ty / tw + 0.5);
                                    result.Corners[k] = (cx, cy);
                                }
                            }

                            // 按内容去重：相同文本只保留第一个
                            string text = System.Text.Encoding.ASCII.GetString(result.Data).TrimEnd(' ');
                            bool duplicate = false;
                            foreach (var r in results)
                            {
                                string existing = System.Text.Encoding.ASCII.GetString(r.Data).TrimEnd(' ');
                                if (existing == text)
                                {
                                    duplicate = true;
                                    break;
                                }
                            }
                            if (!duplicate)
                                results.Add(result);
                        }
                        finally { NativeMethods.dmtxMessageDestroy(ref msg); }
                    }
                }
                finally { NativeMethods.dmtxRegionDestroy(ref region); }
            }
        }
        catch (Exception ex) { throw new DmtxException("Error during decode scan", ex); }

        return results.ToArray();
    }

    private static unsafe nint ReadEncodeImagePtr(nint enc)
    {
        // DmtxEncode layout (x64): 9 ints (36 bytes) + padding(4) + Message*(8) + Image*(8) = offset 48
        int offset = IntPtr.Size == 8 ? 48 : 40;
        return *(nint*)(enc + offset);
    }

    private static unsafe void CopyEncodePixels(nint img, byte[] pixelData, int stride, int width, int height)
    {
        // DmtxImage layout: width(4)+height(4)+pixelPacking(4)+bitsPerPixel(4)+bytesPerPixel(4)
        //   +rowPadBytes(4)+rowSizeBytes(4)+imageFlip(4)+channelCount(4) = 36
        //   +channelStart[4](16)+bitsPerChannel[4](16) = 68
        //   +pxl*(8 on x64) = 76 on x64, 72 on x86
        // 9 ints(36) + channelStart[4](16) + bitsPerChannel[4](16) = 68
        // + 4 bytes padding for 8-byte alignment → pxl at 72 (x64)
        int pxlOffset = IntPtr.Size == 8 ? 72 : 68;
        nint pxlPtr = *(nint*)(img + pxlOffset);
        if (pxlPtr == nint.Zero) return;

        int rowBytes = width * 3;
        fixed (byte* dstBase = pixelData)
        {
            for (int y = 0; y < height; y++)
            {
                int srcOff = y * rowBytes;
                int dstOff = (height - 1 - y) * stride;
                Buffer.MemoryCopy((void*)(pxlPtr + srcOff), dstBase + dstOff, rowBytes, rowBytes);
            }
        }
    }
}

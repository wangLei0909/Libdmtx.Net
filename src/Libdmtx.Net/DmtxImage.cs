using Libdmtx.Interop;

namespace Libdmtx;

internal sealed class DmtxImage : IDisposable
{
    private nint _handle;
    private readonly byte[] _pixelData;
    private bool _disposed;

    public nint Handle => _handle;
    public int Width { get; }
    public int Height { get; }

    internal DmtxImage(byte[] pixelData, int width, int height, int stride,
        DmtxPackOrder packing = DmtxPackOrder._24bppBGR)
    {
        _pixelData = pixelData;
        Width = width;
        Height = height;

        _handle = NativeMethods.dmtxImageCreate(pixelData, width, height, (int)packing);
        if (_handle == nint.Zero)
            throw new DmtxException("Failed to create DmtxImage");

        int rowPadBytes = stride - width * 3;
        if (rowPadBytes > 0)
        {
            if (NativeMethods.dmtxImageSetProp(_handle, (int)DmtxProperty.RowPadBytes, rowPadBytes) == 0)
                throw new DmtxException("Failed to set row pad bytes");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            if (_handle != nint.Zero)
            {
                NativeMethods.dmtxImageDestroy(ref _handle);
                _handle = nint.Zero;
            }
        }
    }
}

namespace Libdmtx;

/// <summary>
/// Result of encoding data into a Data Matrix barcode.
/// </summary>
public class EncodeResult
{
    /// <summary>Raw image pixel data (24-bit BGR, bottom-up).</summary>
    public byte[] PixelData { get; internal set; } = [];

    /// <summary>Image width in pixels.</summary>
    public int Width { get; internal set; }

    /// <summary>Image height in pixels.</summary>
    public int Height { get; internal set; }

    /// <summary>Stride (bytes per row, includes padding).</summary>
    public int Stride { get; internal set; }

    /// <summary>Symbol rows (modules).</summary>
    public int SymbolRows { get; internal set; }

    /// <summary>Symbol columns (modules).</summary>
    public int SymbolCols { get; internal set; }
}

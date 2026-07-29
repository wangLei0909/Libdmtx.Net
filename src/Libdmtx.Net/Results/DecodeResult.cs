namespace Libdmtx;

/// <summary>
/// Result of decoding a single Data Matrix symbol.
/// </summary>
public class DecodeResult
{
    /// <summary>Decoded data bytes (may contain trailing null padding).</summary>
    public byte[] Data { get; internal set; } = [];

    /// <summary>Decoded data as a string (ASCII).</summary>
    public string Text => System.Text.Encoding.ASCII.GetString(Data).TrimEnd('\0');

    /// <summary>Symbol rows.</summary>
    public int Rows { get; internal set; }

    /// <summary>Symbol columns.</summary>
    public int Cols { get; internal set; }

    /// <summary>Four corners of the symbol in image coordinates.</summary>
    public (int X, int Y)[]? Corners { get; internal set; }

    /// <summary>Rotation angle in degrees.</summary>
    public int Angle { get; internal set; }

    /// <summary>Pad count (unused codewords).</summary>
    public int PadCount { get; internal set; }

    /// <summary>Total data capacity in codewords.</summary>
    public int Capacity { get; internal set; }
}

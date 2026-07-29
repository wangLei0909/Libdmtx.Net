namespace Libdmtx;

/// <summary>
/// Options for encoding Data Matrix barcodes.
/// </summary>
public class EncodeOptions
{
    /// <summary>Margin/padding around the symbol in modules (default: 1).</summary>
    public int MarginSize { get; set; } = 1;

    /// <summary>Size of each module in pixels (default: 5).</summary>
    public int ModuleSize { get; set; } = 5;

    /// <summary>Encoding scheme (default: AutoBest).</summary>
    public int Scheme { get; set; } = -1; // DmtxSchemeAutoBest

    /// <summary>Requested symbol size (default: SquareAuto = -2).</summary>
    public int SizeIdxRequest { get; set; } = -2;

    /// <summary>Creates default encode options.</summary>
    public static EncodeOptions Default => new();
}

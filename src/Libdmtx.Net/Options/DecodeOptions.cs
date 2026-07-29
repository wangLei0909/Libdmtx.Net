namespace Libdmtx;

/// <summary>
/// Options for decoding Data Matrix barcodes.
/// Set a property to null to use the libdmtx default.
/// </summary>
public class DecodeOptions
{
    /// <summary>Minimum edge length (pixels).</summary>
    public int? EdgeMin { get; set; }

    /// <summary>Maximum edge length (pixels).</summary>
    public int? EdgeMax { get; set; }

    /// <summary>Scan gap.</summary>
    public int? ScanGap { get; set; }

    /// <summary>Square deviation tolerance.</summary>
    public double? SquareDevn { get; set; }

    /// <summary>Expected symbol size.</summary>
    public int? SizeIdxExpected { get; set; }

    /// <summary>Edge threshold.</summary>
    public int? EdgeThresh { get; set; }

    /// <summary>Maximum number of codes to decode (null = unlimited).</summary>
    public int? MaxCodes { get; set; }

    /// <summary>Scan timeout in milliseconds (null = no timeout).</summary>
    public int? TimeoutMs { get; set; }

    /// <summary>Scan region: X min (null = full image).</summary>
    public int? XMin { get; set; }

    /// <summary>Scan region: X max.</summary>
    public int? XMax { get; set; }

    /// <summary>Scan region: Y min.</summary>
    public int? YMin { get; set; }

    /// <summary>Scan region: Y max.</summary>
    public int? YMax { get; set; }

    /// <summary>Image scale factor (1 = no scaling).</summary>
    public int Shrink { get; set; } = 1;

    /// <summary>
    /// Error correction passes. Higher values fix more damage but are slower.
    /// null = libdmtx default.
    /// </summary>
    public int? CorrectionsMax { get; set; }

    /// <summary>Creates default decode options.</summary>
    public static DecodeOptions Default => new();
}

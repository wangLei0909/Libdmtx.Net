namespace Libdmtx.Interop;

/// <summary>Data Matrix symbol format.</summary>
internal enum DmtxFormat
{
    Matrix = 0,
    Mosaic = 1,
}

internal enum DmtxSymAttribute
{
    SymbolRows,
    SymbolCols,
    DataRegionRows,
    DataRegionCols,
    HorizDataRegions,
    VertDataRegions,
    MappingMatrixRows,
    MappingMatrixCols,
    InterleavedBlocks,
    BlockErrorWords,
    BlockMaxCorrectable,
    SymbolDataWords,
    SymbolErrorWords,
    SymbolMaxCorrectable,
}

internal enum DmtxProperty
{
    // Encoding
    Scheme = 100,
    SizeRequest,
    MarginSize,
    ModuleSize,
    Fnc1,
    // Decoding
    EdgeMin = 200,
    EdgeMax,
    ScanGap,
    SquareDevn,
    SymbolSize,
    EdgeThresh,
    // Image
    Width = 300,
    Height,
    PixelPacking,
    BitsPerPixel,
    BytesPerPixel,
    RowPadBytes,
    RowSizeBytes,
    ImageFlip,
    ChannelCount,
    // Image modifiers
    Xmin = 400,
    Xmax,
    Ymin,
    Ymax,
    Scale,
}

internal enum DmtxPackOrder
{
    Custom = 100,
    _1bppK = 200,
    _8bppK = 300,
    _16bppRGB = 400,
    _16bppRGBX,
    _16bppXRGB,
    _16bppBGR,
    _16bppBGRX,
    _16bppXBGR,
    _16bppYCbCr,
    _24bppRGB = 500,
    _24bppBGR,
    _24bppYCbCr,
    _32bppRGBX = 600,
    _32bppXRGB,
    _32bppBGRX,
    _32bppXBGR,
    _32bppCMYK,
}

[Flags]
internal enum DmtxFlip
{
    None = 0x00,
    X = 0x01,
    Y = 0x02,
}

[Flags]
internal enum DmtxModule
{
    Off = 0x00,
    OnRed = 0x01,
    OnGreen = 0x02,
    OnBlue = 0x04,
    OnRGB = 0x07,
    On = 0x07,
    Unsure = 0x08,
    Assigned = 0x10,
    Visited = 0x20,
    Data = 0x40,
}

internal enum DmtxDirection
{
    None = 0x00,
    Up = 0x01,
    Left = 0x02,
    Down = 0x04,
    Right = 0x08,
    Horizontal = Left | Right,
    Vertical = Up | Down,
}

internal enum DmtxScheme
{
    AutoFast = -2,
    AutoBest = -1,
    Ascii = 0,
    C40,
    Text,
    X12,
    Edifact,
    Base256,
}

internal enum DmtxSymbolSize
{
    RectAuto = -3,
    SquareAuto = -2,
    ShapeAuto = -1,
    _10x10 = 0,
    _12x12,
    _14x14,
    _16x16,
    _18x18,
    _20x20,
    _22x22,
    _24x24,
    _26x26,
    _32x32,
    _36x36,
    _40x40,
    _44x44,
    _48x48,
    _52x52,
    _64x64,
    _72x72,
    _80x80,
    _88x88,
    _96x96,
    _104x104,
    _120x120,
    _132x132,
    _144x144,
    _8x18,
    _8x32,
    _12x26,
    _12x36,
    _16x36,
    _16x48,
}

internal enum DmtxScanStatus
{
    NotFound,
    Success,
    TimeLimit,
    IterLimit,
}

internal enum DmtxStatus
{
    Encoding,
    Complete,
    Invalid,
    Fatal,
}

[Flags]
internal enum DmtxCornerLoc
{
    Corner00 = 0x01,
    Corner10 = 0x02,
    Corner11 = 0x04,
    Corner01 = 0x08,
}

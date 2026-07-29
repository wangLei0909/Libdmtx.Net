using System.Drawing;
using System.Drawing.Imaging;
using Libdmtx;

if (args.Length > 0 && args[0] == "decode" && args.Length > 1)
{
    // 解码指定图片
    string imagePath = args[1];
    DecodeImageFile(imagePath);
    return;
}

Console.WriteLine("=== libdmtx .NET Demo ===");

string? version = Dmtx.Version;
Console.WriteLine($"libdmtx version: {version ?? "(library not loaded)"}");

if (version == null)
{
    Console.WriteLine("\nERROR: libdmtx native library not found!");
    Console.WriteLine("See native/BUILD.md for instructions on building the native library.");
    return;
}

// ---- Encode ----
Console.WriteLine("\n--- Encoding ---");
string text = "Hello, Data Matrix!";
Console.WriteLine($"Input text: \"{text}\"");

var encodeOpts = new EncodeOptions
{
    ModuleSize = 5,
    MarginSize = 2,
};

EncodeResult encoded = Dmtx.EncodeString(text, encodeOpts);
Console.WriteLine($"Encoded image: {encoded.Width}x{encoded.Height} px");

// ---- Decode (round-trip) ----
Console.WriteLine("\n--- Decoding (round-trip test) ---");
var decodeOpts = new DecodeOptions { TimeoutMs = 5000 };

DecodeResult[] decoded = Dmtx.Decode(encoded.PixelData, encoded.Width, encoded.Height, encoded.Stride, decodeOpts);
Console.WriteLine($"Found {decoded.Length} symbol(s)");

foreach (var r in decoded)
    Console.WriteLine($"  Text: \"{r.Text}\"  Bytes: {r.Data.Length}");

// ---- Save image ----
string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output_demo.bmp");
SaveAsBgr24Bmp(encoded.PixelData, encoded.Width, encoded.Height, outputPath);
Console.WriteLine($"\nImage saved to: {outputPath}");

// 如果有图片参数，也解码
if (args.Length > 0)
{
    Console.WriteLine($"\n--- Decoding file: {args[0]} ---");
    DecodeImageFile(args[0]);
}

Console.WriteLine("\n=== Demo complete ===");

// ===== 图片解码函数 =====
static void DecodeImageFile(string imagePath)
{
    if (!File.Exists(imagePath))
    {
        Console.WriteLine($"File not found: {imagePath}");
        return;
    }

    using var bitmap = new Bitmap(imagePath);
    Console.WriteLine($"Image: {bitmap.Width}x{bitmap.Height}, {bitmap.PixelFormat}");

    // LockBits 获取 BGR24 像素数据
    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    BitmapData bd = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
    try
    {
        int stride = Math.Abs(bd.Stride);
        byte[] pixels = new byte[stride * bitmap.Height];
        System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, pixels, 0, pixels.Length);

        var opts = new DecodeOptions
        {
            TimeoutMs = 10000,
            MaxCodes = 10,
        };

        DecodeResult[] results = Dmtx.Decode(pixels, bitmap.Width, bitmap.Height, stride, opts);
        Console.WriteLine($"Found {results.Length} Data Matrix symbol(s)");

        for (int i = 0; i < results.Length; i++)
        {
            Console.WriteLine($"\n  [{i}] Text: \"{results[i].Text}\"");
            Console.WriteLine($"      Bytes: {results[i].Data.Length}, Pad: {results[i].PadCount}");
        }

        if (results.Length == 0)
            Console.WriteLine("  (no Data Matrix barcode found)");
    }
    finally
    {
        bitmap.UnlockBits(bd);
    }
}

static void SaveAsBgr24Bmp(byte[] pixelData, int width, int height, string path)
{
    int padding = (4 - (width * 3 % 4)) % 4;
    int rowSize = width * 3 + padding;
    int pixelDataSize = rowSize * height;
    int fileSize = 14 + 40 + pixelDataSize;

    using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
    using var bw = new BinaryWriter(fs);

    bw.Write((byte)'B'); bw.Write((byte)'M');
    bw.Write(fileSize);
    bw.Write(0);
    bw.Write(54);

    bw.Write(40);
    bw.Write(width);
    bw.Write(height);
    bw.Write((short)1);
    bw.Write((short)24);
    bw.Write(0);
    bw.Write(pixelDataSize);
    bw.Write(2835);
    bw.Write(2835);
    bw.Write(0);
    bw.Write(0);

    for (int y = height - 1; y >= 0; y--)
    {
        int rowOffset = y * width * 3;
        bw.Write(pixelData, rowOffset, width * 3);
        for (int p = 0; p < padding; p++)
            bw.Write((byte)0);
    }
}

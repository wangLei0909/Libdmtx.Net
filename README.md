# Libdmtx.Net — .NET 8/9 Data Matrix Wrapper

> **Community-maintained** .NET wrapper for [libdmtx](https://github.com/dmtx/libdmtx),
> the open-source C library for encoding and decoding **Data Matrix ECC200** barcodes.
> This is **not** an official package from the libdmtx project.

## Features

- ✅ **.NET 8 & .NET 9** support (cross-platform)
- ✅ **No System.Drawing dependency** — uses raw `byte[]` pixel data
- ✅ Encode text/bytes → Data Matrix barcode image
- ✅ Decode Data Matrix barcode image → text/bytes
- ✅ Modern C# API with nullable annotations

## Installation

### From GitHub Packages



### From local NuGet package



---

## Quick Start

### 1. Build the native library

See [`native/BUILD.md`](native/BUILD.md) for platform-specific instructions.
After building, copy `libdmtx.dll` (or `.so`/`.dylib`) to your output directory.

### 2. Add the NuGet reference

```xml
<ProjectReference Include="path/to/src/Libdmtx.Net/Libdmtx.Net.csproj" />
```

### 3. Encode

```csharp
using Libdmtx;

byte[] data = Encoding.ASCII.GetBytes("Hello World!");
EncodeResult result = Dmtx.Encode(data, new EncodeOptions
{
    ModuleSize = 5,
    MarginSize = 2,
});

// result.PixelData contains 24-bit BGR pixels
// result.Width, result.Height give image dimensions
SaveAsBmp(result.PixelData, result.Width, result.Height, "barcode.bmp");
```

### 4. Decode

```csharp
using Libdmtx;

// Load image data as BGR24 byte array
byte[] imageData = File.ReadAllBytes("barcode.raw");
// ... or convert from Bitmap using LockBits ...

DecodeResult[] results = Dmtx.Decode(imageData, width, height, stride);

foreach (var r in results)
{
    string text = r.Text;  // auto-trims null padding
    Console.WriteLine($"Decoded: {text}");
}
```

## Project Structure

```
dotnet/
├── src/Libdmtx.Net/          # Class library
│   ├── Dmtx.cs               # Main entry point (static API)
│   ├── DmtxImage.cs          # Native image handle wrapper
│   ├── DmtxException.cs      # Custom exceptions
│   ├── Options/              # Encode/Decode options
│   ├── Results/              # Encode/Decode results
│   └── Interop/              
│       ├── NativeMethods.cs  # DllImport declarations
│       ├── NativeEnums.cs    # C# enums matching libdmtx defines
│       └── NativeStructs.cs  # C# structs for native types
├── samples/Libdmtx.Demo/     # Console demo app
└── native/BUILD.md           # Native library build guide
```

## API Overview

### `Dmtx.Encode(byte[] data, EncodeOptions? options)`
Encodes data into a Data Matrix barcode. Returns `EncodeResult` with raw pixel data.

### `Dmtx.EncodeString(string text, EncodeOptions? options)`
Shorthand for encoding ASCII text.

### `Dmtx.Decode(byte[] pixelData, int width, int height, int stride, DecodeOptions? options)`
Decodes one or more Data Matrix symbols from a BGR24 image.

### `Dmtx.Version`
Returns the native libdmtx version string, or `null` if the library isn't loaded.

## Notes

- The library expects **24-bit BGR** pixel data (matching libdmtx's native format).
- Input pixel data must remain pinned/rooted during decoding — the wrapper handles this.
- The native DLL must be placed where the runtime can find it (same directory as the .exe, or system PATH).

## License

The C# wrapper code in this package is licensed under **BSD-2-Clause**.

The bundled native library  is Copyright © Mike Laughton, Vadim A. Misbakh-Soloviov
and others, used under the **BSD-2-Clause** license (see the [libdmtx LICENSE](https://github.com/dmtx/libdmtx/blob/master/LICENSE) file for full terms).

This is a community-maintained package, not an official libdmtx release.

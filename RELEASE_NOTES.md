# Release Notes

## v0.7.9-dev — 2025-07-29

First public release of Libdmtx.Net — a community-maintained .NET 8/9 wrapper
for [libdmtx](https://github.com/dmtx/libdmtx) (Data Matrix ECC200 barcode
encoding/decoding).

### What's included

- ✅ .NET 8 and .NET 9 support
- ✅ Encode text/bytes → Data Matrix barcode image
- ✅ Decode Data Matrix barcode image → text/bytes
- ✅ No System.Drawing dependency (uses raw byte[] pixel data)
- ✅ Windows x64 native DLL bundled (built from libdmtx master @ 0.7.9-dev)

### Usage

```bash
# Add GitHub Packages source
dotnet nuget add source --name github https://nuget.pkg.github.com/wangLei0909/index.json

# Install
dotnet add package Libdmtx.Net
```

```csharp
using Libdmtx;

// Encode
var enc = Dmtx.EncodeString("Hello!");

// Decode
var dec = Dmtx.Decode(pixels, width, height, stride);
Console.WriteLine(dec[0].Text);
```

### License

- C# wrapper: BSD-2-Clause
- Bundled libdmtx.dll: BSD-2-Clause (Copyright Mike Laughton, Vadim A. Misbakh-Soloviov and others)

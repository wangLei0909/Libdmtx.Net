# Building the Native libdmtx Library

The .NET wrapper requires the native `libdmtx` shared library to be available
at runtime. It looks for:

| Platform | File Name      |
|----------|----------------|
| Windows  | `libdmtx.dll`  |
| Linux    | `libdmtx.so`   |
| macOS    | `libdmtx.dylib`|

---

## Windows (Visual Studio 2022) — 已验证可用 ✅

### Prerequisites
- Visual Studio 2022 with **"Desktop development with C++"** workload

### Build

```bash
# 在 Git Bash 或 cmd 中执行
# 先激活 VS 环境，再编译

# 方案 A：用批处理脚本
# =========== build.cmd ===========
@echo off
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvarsall.bat" x64
cd /d D:\path\to\libdmtx
if not exist build_msvc mkdir build_msvc
cd build_msvc
cmake .. -DBUILD_SHARED_LIBS=ON -DBUILD_TESTING=OFF
cmake --build . --config Release
# =================================

# CMake 输出在 build_msvc/Release/dmtx.dll（约 94KB）
```

### Deploy

```bash
# 复制到 .NET 项目的输出目录（同时保留两个名字以便兼容）
cp build_msvc/Release/dmtx.dll dotnet/samples/Libdmtx.Demo/bin/Debug/net8.0/libdmtx.dll
```

---

## Windows (MinGW)

```bash
pacman -S mingw-w64-ucrt-x86_64-{cmake,gcc,make}
cd /path/to/libdmtx
mkdir build && cd build
cmake .. -G "MSYS Makefiles" -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DBUILD_TESTING=OFF
make -j$(nproc)
```

---

## Linux

```bash
cd /path/to/libdmtx
mkdir build && cd build
cmake .. -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DBUILD_TESTING=OFF
make -j$(nproc)
sudo make install
```

---

## macOS

```bash
cd /path/to/libdmtx
mkdir build && cd build
cmake .. -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DBUILD_TESTING=OFF
make -j$(sysctl -n hw.logicalcpu)
sudo make install
```

---

## Verifying

```bash
cd dotnet
dotnet run --project samples/Libdmtx.Demo
# 预期输出:
# === libdmtx .NET Demo ===
# libdmtx version: 0.7.9
# --- Encoding ---
# Input text: "Hello, Data Matrix!"
# Encoded image: 94x94 px
# --- Decoding (round-trip test) ---
# Found 1 symbol(s)
#   [0] Text: "Hello, Data Matrix!"
# === Demo complete ===
```

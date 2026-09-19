using System.Runtime.InteropServices;
using System.Text;
using GXX.Core.Rtl;

namespace GXX.Core.Protocol;

/// <summary>
/// Delphi string[N]（ShortString）的字节兼容读写助手。
/// 内存布局：[0]=长度字节（实际字符数），[1..N]=GBK 字节。
/// </summary>
public static unsafe class ShortStr
{
    /// <summary>读取 fixed byte 缓冲中的 ShortString（GBK → string）。</summary>
    public static string Get(byte* p, int capacity)
    {
        if (p == null) return "";
        int len = p[0];
        if (len > capacity) len = capacity;
        if (len <= 0) return "";
        byte[] tmp = new byte[len];
        for (int i = 0; i < len; i++) tmp[i] = p[1 + i];
        return EncodingInit.GBK.GetString(tmp);
    }

    public static string Get(byte[] buf, int offset, int capacity)
    {
        if (buf == null || offset >= buf.Length) return "";
        int len = buf[offset];
        if (len > capacity) len = capacity;
        if (len <= 0 || offset + 1 + len > buf.Length) return "";
        return EncodingInit.GBK.GetString(buf, offset + 1, len);
    }

    /// <summary>写入 ShortString（string → GBK，超过容量截断，长度字节=实际字节数）。</summary>
    public static void Set(byte* p, int capacity, string value)
    {
        if (p == null) return;
        byte[] data = EncodingInit.GBK.GetBytes(value ?? "");
        if (data.Length > capacity) data = data.AsSpan(0, capacity).ToArray();
        p[0] = (byte)data.Length;
        for (int i = 0; i < data.Length; i++) p[1 + i] = data[i];
        for (int i = data.Length + 1; i <= capacity; i++) p[i] = 0;
    }

    public static void Set(byte[] buf, int offset, int capacity, string value)
    {
        byte[] data = EncodingInit.GBK.GetBytes(value ?? "");
        if (data.Length > capacity) data = data.AsSpan(0, capacity).ToArray();
        buf[offset] = (byte)data.Length;
        for (int i = 0; i < data.Length; i++) buf[offset + 1 + i] = data[i];
        for (int i = data.Length + 1; i <= capacity; i++) buf[offset + i] = 0;
    }
}

/// <summary>非 fixed 场景使用的短字符串缓冲（长度字节 + N 字节），可序列化。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ShortStringBuf
{
    public byte Len;
    public byte Data; // 可变长度跟随，见 ToBytes/FromBytes

    public static byte[] Create(string value, int capacity)
    {
        byte[] data = EncodingInit.GBK.GetBytes(value ?? "");
        if (data.Length > capacity) data = data.AsSpan(0, capacity).ToArray();
        byte[] buf = new byte[1 + capacity];
        buf[0] = (byte)data.Length;
        Array.Copy(data, 0, buf, 1, data.Length);
        return buf;
    }

    public static string Read(byte[] wire, int offset, int capacity)
        => ShortStr.Get(wire, offset, capacity);
}

/// <summary>结构体 ↔ 字节序列化助手（对应 Delphi 的 Move(@Rec, Buf^, SizeOf) 语义）。</summary>
public static class StructBytes
{
    public static int SizeOf<T>() where T : struct => Marshal.SizeOf<T>();

    public static byte[] BytesOf<T>(in T value) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] buf = new byte[size];
        unsafe
        {
            fixed (byte* p = buf)
            {
                Marshal.StructureToPtr(value, (IntPtr)p, false);
            }
        }
        return buf;
    }

    public static T FromBytes<T>(byte[] buf, int offset = 0) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        if (offset + size > buf.Length) throw new ArgumentException($"buffer too small: need {size}, got {buf.Length - offset}");
        unsafe
        {
            fixed (byte* p = &buf[offset])
            {
                return Marshal.PtrToStructure<T>((IntPtr)p);
            }
        }
    }

    public static void ToBytes<T>(in T value, byte[] dst, int offset) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        unsafe
        {
            fixed (byte* p = &dst[offset])
            {
                Marshal.StructureToPtr(value, (IntPtr)p, false);
            }
        }
    }
}

using System;
using System.IO;
using System.Text;
using GXX.Core.Rtl;

namespace GXX.Core.Util;

/// <summary>
/// CheckUnit.pas 1:1 转换：文件/缓冲区 CRC 校验 + PJW 哈希。
///
/// 覆盖审计（证据见 docs/并行报告-p2c-common-crypto.md）：
///  - <c>BufferCRC</c> / <c>StringCrc</c>（CheckUnit.pas:24-34）只是 CheckCrc.Crc32 的薄包装，
///    既有 <see cref="GXX.Core.Crypto.UnitDes.CalcCrc32"/> 已实现同一算法（zlib 标准 CRC-32，
///    多项式 $EDB88320，初值 $FFFFFFFF，结果取反）—— 本文件**转调**它，不重复实现；
///  - <c>FileCrc</c> / <c>CalcFileCRC</c> / <c>HashPJW</c> 在既有 src/GXX.Core/** 中无对应实现，
///    故在本文件新建。
///
/// 原文照抄的边界/缺陷：
///  1. <c>CalcFileCRC</c>（CheckUnit.pas:72-104）把文件长度**向下取整到 4 的倍数**再逐 DWord 异或，
///     即 **不是** CRC 而是一个"32 位整数异或和"，且丢弃文件尾部不足 4 字节的部分；
///     打开失败时原文用 `if nFileHandle = 0`（FileOpen 失败返回 -1，故该判断恒假）——原文如此保留；
///  2. <c>CalcFileCRC</c> 只做 DWord 异或、**不引用 CRC 表**，与 FileCrc 是两套完全不同的算法；
///  3. <c>HashPJW</c> 的 `G := Result and $F0000000` 用 Longint（有符号）高位掩码，逻辑与 Aho-Sethi-Ullman
///     的 PJW 哈希一致，这里逐位照抄。
/// </summary>
public static class CheckUnit
{
    /// <summary>
    /// 对应原文 BufferCRC(Buffer: PAnsiChar; nSize: Integer): Cardinal —— 转调 CheckCrc.Crc32。
    /// </summary>
    public static uint BufferCrc(byte[] buffer, int nSize)
        => Crypto.UnitDes.CalcCrc32(buffer, 0, nSize);

    /// <summary>对应原文 StringCrc(const Value: AnsiString): Cardinal（空串返回 0）。</summary>
    public static uint StringCrc(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        byte[] data = EncodingInit.GBK.GetBytes(value);
        return Crypto.UnitDes.CalcCrc32(data, 0, data.Length);
    }

    /// <summary>
    /// 对应原文 FileCrc(const FileName: string): Cardinal：
    /// 文件不存在或读失败返回 0，否则返回整个文件字节的 zlib CRC-32。
    /// </summary>
    public static uint FileCrc(string fileName)
    {
        if (!File.Exists(fileName))
            return 0;
        try
        {
            byte[] data = File.ReadAllBytes(fileName);
            return Crypto.UnitDes.CalcCrc32(data, 0, data.Length);
        }
        catch
        {
            return 0;   // 原文 except → MemoryStream.Free; Result := 0; Exit
        }
    }

    /// <summary>
    /// 对应原文 HashPJW(const Value: AnsiString): Longint（Aho/Sethi/Ullman PJW 哈希）。
    /// </summary>
    public static int HashPJW(string value)
    {
        int result = 0;
        byte[] data = EncodingInit.GBK.GetBytes(value ?? "");
        for (int i = 0; i < data.Length; i++)
        {
            result = (result << 4) + data[i];
            int g = result & unchecked((int)0xF0000000);
            if (g != 0)
                result = (result ^ (int)((uint)g >> 24)) ^ g;
        }
        return result;
    }

    /// <summary>
    /// 对应原文 CalcFileCRC(const sFileName: string): Integer：
    /// 文件不存在返回 0；否则把长度向下取整到 4 的倍数、逐 DWord 异或（**不是 CRC-32**）。
    /// 注意原文 `if nFileHandle = 0 then Exit`（FileOpen 失败返回 -1）恒假，故此处也不做该判断。
    /// </summary>
    public static int CalcFileCRC(string sFileName)
    {
        if (!File.Exists(sFileName))
            return 0;

        byte[] data = File.ReadAllBytes(sFileName);
        int nFileSize = data.Length;
        int nBuffSize = (nFileSize / 4) * 4;

        int nCrc = 0;
        for (int i = 0; i < nBuffSize / 4; i++)
        {
            // Int^ 读取的是本机字节序 DWord
            int v = data[i * 4] | (data[i * 4 + 1] << 8) | (data[i * 4 + 2] << 16) | (data[i * 4 + 3] << 24);
            nCrc ^= v;
        }
        return nCrc;
    }

    /// <summary>对应原文 CalcBufferCRC（private 单元级函数，此处公开以便测试）。</summary>
    public static int CalcBufferCRC(byte[] buffer, int nSize)
    {
        int nCrc = 0;
        for (int i = 0; i < nSize / 4; i++)
        {
            int v = buffer[i * 4] | (buffer[i * 4 + 1] << 8) | (buffer[i * 4 + 2] << 16) | (buffer[i * 4 + 3] << 24);
            nCrc ^= v;
        }
        return nCrc;
    }
}

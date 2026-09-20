using System;

// 源：Source/RunGate/RunGateUtils.pas:1867-1979 —— TRunGate.Run 里的 "检查网关是否合法" 应答散列。
// 这是 Bob Jenkins lookup2 的一个**手改变体**：原文注释写的移位量与代码实际移位量多处不一致
// （例如注释 `c ^= b >> 13` 而代码是 `c xor (b shr 15)`；第二段 `c ^= b >> 15` 而代码是 `c xor (b shl 12)`）。
// 移植时**一律以代码为准**，注释仅作线索；测试用独立复算的黄金向量锁定。
// 纯逻辑，无网络/线程。

namespace GXX.RunGate;

/// <summary>
/// RunGateUtils.pas:1867-1979 —— 挑战应答散列（Jenkins lookup2 变体）。
/// 生产路径输入恒为 16 字节（见 <see cref="BuildChallengeBuffer"/>），但散列本体按原文实现为通用长度。
/// </summary>
public static class RunGateChallengeHash
{
    /// <summary>RunGateUtils.pas:1883-1885 —— 三个初值（注意 b/c 的注释值 9E3779B9/E6359A60 与代码不同）。</summary>
    public const uint InitA = 0x68F4CE4Eu;
    public const uint InitB = 0x31A0C38Bu;
    public const uint InitC = 0x071C5DEFu;

    /// <summary>RunGateUtils.pas:1971 —— 每次应答把等待时间 +60 秒。</summary>
    public const uint ResponseTimeStep = 60000u;

    /// <summary>RunGateUtils.pas:1974 —— 连续应答 2 次后停止挑战。</summary>
    public const int RequiredResponseCount = 2;

    /// <summary>
    /// RunGateUtils.pas:1873-1877 —— S 的字节布局：
    /// LE32(data1 ^ data2) + LE32(data1) + LE32(data1 | data2) + LE32(data2)，共 16 字节。
    /// </summary>
    public static byte[] BuildChallengeBuffer(uint data1, uint data2)
    {
        uint a = data1 ^ data2;
        uint b = data1 | data2;
        var s = new byte[16];
        WriteLe32(s, 0, a);
        WriteLe32(s, 4, data1);
        WriteLe32(s, 8, b);
        WriteLe32(s, 12, data2);
        return s;
    }

    /// <summary>RunGateUtils.pas:1867-1968 —— 对 16 字节挑战缓冲求散列，返回 c。</summary>
    public static uint ComputeChallenge(uint data1, uint data2) => Compute(BuildChallengeBuffer(data1, data2));

    /// <summary>
    /// 散列本体（RunGateUtils.pas:1887-1966）。逐行对齐，输入任意长度；
    /// 生产调用长度为 16（此时主循环只跑 1 轮，尾巴走 <c>len = 4</c> 分支）。
    /// </summary>
    public static uint Compute(byte[] s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));

        uint a = InitA, b = InitB, c = InitC;
        int len = s.Length;
        int k = 0;                  // 原 k 从 1 开始（1-based string），这里用 0-based

        while (len >= 12)
        {
            a += Le32(s, k + 0);
            b += Le32(s, k + 4);
            c += Le32(s, k + 8);

            // 原 1894-1897
            a -= b; a -= c; a ^= (c >> 13);
            b -= c; b -= a; b ^= (a << 8);
            c -= a; c -= b; c ^= (b >> 15);
            // 原 1903-1906：注释写 12/16，实际移位是 9/7
            a -= b; a -= c; a ^= (c >> 9);
            b -= c; b -= a; b ^= (a << 7);
            c -= a; c -= b; c ^= (b >> 5);
            // 原 1912-1915：注释写 3/10/15，实际移位是 3/10/12
            a -= b; a -= c; a ^= (c >> 3);
            b -= c; b -= a; b ^= (a << 10);
            c -= a; c -= b; c ^= (b >> 12);

            k += 12;
            len -= 12;
        }

        // 原 1924：加的是**原始总长度**（Delphi 的 Length(S) 在循环中不变），不是剩余长度
        c += (uint)s.Length;

        if (len >= 11) c += (uint)s[k + 10] << 24;
        if (len >= 10) c += (uint)s[k + 9] << 16;
        if (len >= 9) c += (uint)s[k + 8] << 8;

        if (len >= 8) b += (uint)s[k + 7] << 24;
        if (len >= 7) b += (uint)s[k + 6] << 16;
        if (len >= 6) b += (uint)s[k + 5] << 8;
        if (len >= 5) b += (uint)s[k + 4];

        if (len >= 4) a += (uint)s[k + 3] << 24;
        if (len >= 3) a += (uint)s[k + 2] << 16;
        if (len >= 2) a += (uint)s[k + 1] << 8;
        if (len >= 1) a += (uint)s[k + 0];

        // 原 1942-1948
        a -= b; a -= c; a ^= (c >> 13);
        b -= c; b -= a; b ^= (a << 8);
        c -= a; c -= b; c ^= (b >> 15);
        // 原 1951-1954：注释写 12/16，实际是 11/16
        a -= b; a -= c; a ^= (c >> 11);
        b -= c; b -= a; b ^= (a << 16);
        // 原 1957：**这里是左移**（c xor (b shl 12)），照抄注释会写成右移
        c -= a; c -= b; c ^= (b << 12);
        // 原 1960-1966
        a -= b; a -= c; a ^= (c >> 7);
        b -= c; b -= a; b ^= (a << 9);
        c -= a; c -= b; c ^= (b >> 7);

        return c;
    }

    private static uint Le32(byte[] b, int i)
        => (uint)(b[i] | (b[i + 1] << 8) | (b[i + 2] << 16) | (b[i + 3] << 24));

    private static void WriteLe32(byte[] b, int i, uint v)
    {
        b[i] = (byte)v;
        b[i + 1] = (byte)(v >> 8);
        b[i + 2] = (byte)(v >> 16);
        b[i + 3] = (byte)(v >> 24);
    }
}

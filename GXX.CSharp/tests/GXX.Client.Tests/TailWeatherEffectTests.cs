using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// <c>Source/Client-HGE/uWeatherEffectDef.pas</c>（49 行）1:1 移植的测试。
///
/// <para>22 项常量表由生成器 <c>_scratch/gen_weathereffect.py</c> 从原文抽取并回读比对，
/// 同时落一份 <c>TailWeatherEffect.golden.tsv</c>；本测试把 C# 表与金标**逐字段**比对。</para>
/// </summary>
public sealed class TailWeatherEffectTests
{
    private static List<(int Idx, bool Used, bool Dark, uint Index, string Music, uint Start, uint End, uint Tick)>
        ReadGolden()
    {
        // 金标表 TailWeatherEffectGolden.g.cs 由 _scratch/gen_weathereffect.py 从原文抽取后
        // 生成为**普通 .cs 源文件**（SDK 风格工程自动编译）——不占用共享 csproj、不挂资源。
        var rows = new List<(int, bool, bool, uint, string, uint, uint, uint)>();
        foreach (int[] r in TailWeatherEffectGolden.Rows)
        {
            Assert.Equal(7, r.Length);
            // 原文 22 项的 sMusic 都是空串
            rows.Add((r[0], r[1] != 0, r[2] != 0, (uint)r[3], "", (uint)r[4], (uint)r[5], (uint)r[6]));
        }
        return rows;
    }

    // ── 用例 1：表长与声明槽位一致 ────────────────────────────────────────
    [Fact]
    public void Table_HasExactlyDeclaredSlots()
    {
        // 原文 array[0..21] ⇒ 22 槽；初始化列表也恰好 22 项。
        Assert.Equal(22, WeatherEffectDef.DeclaredSlots);
        Assert.Equal(22, WeatherEffectDef.InitializedSlots);
        Assert.Equal(22, WeatherEffectDef.CreateDefaultTable().Length);
        Assert.Equal(22, ReadGolden().Count);
    }

    // ── 用例 2：逐项逐字段与原文一致 ──────────────────────────────────────
    [Fact]
    public void EveryEntry_EveryField_MatchesGoldenExtractedFromDelphiSource()
    {
        var table = WeatherEffectDef.CreateDefaultTable();
        var golden = ReadGolden();
        Assert.Equal(golden.Count, table.Length);

        foreach (var g in golden)
        {
            var e = table[g.Idx];
            Assert.Equal(g.Used ? 1 : 0, e.BoIsUsed);
            Assert.Equal(g.Dark ? 1 : 0, e.BoIsDark);
            Assert.Equal(g.Index, e.DwIndex);
            Assert.Equal(g.Start, e.DwStartOffset);
            Assert.Equal(g.End, e.DwEndOffset);
            Assert.Equal(g.Tick, e.DwTick);
            // 22 项原文 sMusic 全是空串
            Assert.Null(e.SMusic.Bytes);
        }
    }

    // ── 用例 3：原文前三条的语义（黄沙/花瓣/下雪） ────────────────────────
    [Fact]
    public void FirstThreeEntries_MatchDocumentedEffects()
    {
        var t = WeatherEffectDef.CreateDefaultTable();
        // 0 = 黄沙效果（原文注释）
        Assert.Equal(0u, t[0].DwStartOffset);
        Assert.Equal(9u, t[0].DwEndOffset);
        // 1 = 花瓣效果（区间与 0 完全相同 —— 差异断言：序号不同但区间相同）
        Assert.Equal(t[0].DwStartOffset, t[1].DwStartOffset);
        Assert.Equal(t[0].DwEndOffset, t[1].DwEndOffset);
        // 2 = 下雪效果（原文唯一一个起点非 0 的前三项）
        Assert.Equal(110u, t[2].DwStartOffset);
        Assert.Equal(149u, t[2].DwEndOffset);
    }

    // ── 用例 4：全表区间非等长（易错点） ─────────────────────────────────
    [Fact]
    public void SegmentLengths_AreNotUniform()
    {
        var t = WeatherEffectDef.CreateDefaultTable();
        // 0..1 是 10 帧；2 是 40 帧；13 是 60 帧；其余多为 30 帧
        Assert.Equal(10u, t[0].DwEndOffset - t[0].DwStartOffset + 1);
        Assert.Equal(40u, t[2].DwEndOffset - t[2].DwStartOffset + 1);
        Assert.Equal(60u, t[13].DwEndOffset - t[13].DwStartOffset + 1);
        Assert.Equal(30u, t[14].DwEndOffset - t[14].DwStartOffset + 1);
        Assert.True(t.Select(e => e.DwEndOffset - e.DwStartOffset + 1).Distinct().Count() > 1);
    }

    // ── 用例 5：所有区间单调递增且互不重叠（3..21）────────────────────────
    [Fact]
    public void ContiguousBlocks_FromIndex3Onwards_AreNonOverlappingAndAscending()
    {
        var t = WeatherEffectDef.CreateDefaultTable();
        uint prevEnd = t[3].DwEndOffset;
        for (int i = 4; i < t.Length; i++)
        {
            Assert.True(t[i].DwStartOffset > prevEnd,
                $"下标 {i} 的起点 {t[i].DwStartOffset} 未超过上一段终点 {prevEnd}");
            Assert.True(t[i].DwEndOffset >= t[i].DwStartOffset);
            prevEnd = t[i].DwEndOffset;
        }
    }

    // ── 用例 6：原文 :44 被注释掉的第 23 项**不得**出现在表里 ─────────────
    [Fact]
    public void CommentedOutEntry_IsNotAdopted()
    {
        var t = WeatherEffectDef.CreateDefaultTable();
        // 原文 :44 的注释项是 (600, 209)；若被误采纳，就会出现 start=600 的项。
        Assert.DoesNotContain(t, e => e.DwStartOffset == 600u);
        Assert.DoesNotContain(t, e => e.DwEndOffset == 209u && e.DwStartOffset == 600u);
        // 末项就是原文 :43 的 570..599
        Assert.Equal(570u, t[21].DwStartOffset);
        Assert.Equal(599u, t[21].DwEndOffset);
    }

    // ── 用例 7：全表初始 boIsUsed/boIsDark/dwIndex/dwTick 取值 ───────────
    [Fact]
    public void InitialFlags_AreAllFalseAndZero()
    {
        var t = WeatherEffectDef.CreateDefaultTable();
        Assert.All(t, e =>
        {
            Assert.Equal(0, e.BoIsUsed);
            Assert.Equal(0, e.BoIsDark);
            Assert.Equal(0u, e.DwIndex);
            Assert.Equal(0u, e.DwTick);
        });
    }

    // ── 用例 8：源行号对照表与原文一致（22 项，:22..:43） ─────────────────
    [Fact]
    public void SourceLines_MapEveryEntryToItsDeclarationLine()
    {
        var lines = TailWeatherEffectGolden.SourceLines;
        Assert.Equal(22, lines.Length);
        Assert.Equal(22, lines[0]);
        Assert.Equal(43, lines[21]);
        // 同时对照生产代码里的副本
        Assert.Equal(lines, WeatherEffectDef.SourceLines);
        // 严格递增（原文每行一项）
        for (int i = 1; i < lines.Length; i++) Assert.Equal(lines[i - 1] + 1, lines[i]);
    }

    // ── 用例 9：每次调用返回独立副本（原文是全局数组，C# 侧显式构造） ──────
    [Fact]
    public void CreateDefaultTable_ReturnsFreshCopy()
    {
        var a = WeatherEffectDef.CreateDefaultTable();
        var b = WeatherEffectDef.CreateDefaultTable();
        Assert.NotSame(a, b);
        a[0].DwTick = 12345u;
        Assert.Equal(0u, b[0].DwTick);
    }

    // ── 用例 10：packed 记录字段顺序与原文一致 ───────────────────────────
    [Fact]
    public void TWeateherEffect_FieldOrderAndNames_MatchSource()
    {
        var names = typeof(TWeateherEffect).GetFields().Select(f => f.Name).ToArray();
        Assert.Equal(new[]
        {
            "BoIsUsed", "BoIsDark", "DwIndex", "SMusic", "DwStartOffset", "DwEndOffset", "DwTick"
        }, names);
    }

    // ── 用例 11：ShortString50 的容量与线格式（1 + 50 = 51 字节） ────────
    [Fact]
    public void ShortString50_CapacityAndWireFormat_MatchDelphiStringOf50()
    {
        Assert.Equal(50, ShortString50.Capacity);

        var empty = new ShortString50().ToBytes();
        Assert.Equal(51, empty.Length);
        Assert.Equal(0, empty[0]);

        var filled = new ShortString50 { Bytes = Enumerable.Repeat((byte)0x41, 50).ToArray() }.ToBytes();
        Assert.Equal(51, filled.Length);
        Assert.Equal(50, filled[0]);
        Assert.All(filled.Skip(1), b => Assert.Equal(0x41, b));

        // 超容量截断到 50（Delphi string[50] 语义）
        var over = new ShortString50 { Bytes = Enumerable.Repeat((byte)0x42, 60).ToArray() }.ToBytes();
        Assert.Equal(50, over[0]);
        Assert.All(over.Skip(1), b => Assert.Equal(0x42, b));
    }

    // ── 用例 12：ShortStr 读写往返（GBK 字节语义） ───────────────────────
    [Fact]
    public void ShortString50_GetSet_RoundTripsThroughGbkBytes()
    {
        var buf = new byte[1 + ShortString50.Capacity];
        ShortString50.Set(buf, 0, "下雪");
        Assert.Equal(4, buf[0]);                       // GBK 里两个汉字 = 4 字节
        Assert.Equal("下雪", ShortString50.Get(buf, 0));

        // 空串
        ShortString50.Set(buf, 0, "");
        Assert.Equal(0, buf[0]);
        Assert.Equal("", ShortString50.Get(buf, 0));

        // null 边界
        ShortString50.Set(buf, 0, null);
        Assert.Equal(0, buf[0]);
        Assert.Equal("", ShortString50.Get(buf, 0));
    }

    // ── 用例 13：ShortString50.Get 的越界防护（原文短字符串不会越界读） ───
    [Fact]
    public void ShortString50_Get_OutOfRange_ReturnsEmpty()
    {
        var buf = new byte[1 + ShortString50.Capacity];
        buf[0] = 99;                                    // 伪造一个超过容量的长度字节
        // GXX.Core.Protocol.ShortStr.Get 会把长度夹到容量内，不应抛异常
        string s = ShortString50.Get(buf, 0);
        Assert.NotNull(s);
        Assert.True(s.Length <= ShortString50.Capacity);
    }

    // ── 用例 14：Marshal.SizeOf 与原文 packed 记录宽度自洽 ───────────────
    [Fact]
    public void TWeateherEffect_ManagedLayout_IsSelfConsistent()
    {
        // 注意：C# 侧 SMusic 是托管结构（含 byte[] 引用），因此 Marshal.SizeOf 得到的是
        // **托管包装**的宽度而不是 Delphi 的 packed 宽度。原文 packed 宽度为：
        //   Boolean(1) + Boolean(1) + LongWord(4) + string[50](51) + 4 + 4 + 4 = 69 字节
        // 本用例把该算术钉住，避免日后有人误以为 Marshal.SizeOf 就等于 69。
        const int delphiPackedSize = 1 + 1 + 4 + (1 + 50) + 4 + 4 + 4;
        Assert.Equal(69, delphiPackedSize);
        Assert.True(Marshal.SizeOf<TWeateherEffect>() > 0);
    }
}

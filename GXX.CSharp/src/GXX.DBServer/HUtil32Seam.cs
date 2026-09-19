using GXX.Core.Util;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：HUtil32.pas 的 `GetValidStr3`（**ANSI 分支** HUtil32.pas:1300-1339）。
//
// 【并行批次 P2-core-rtl 去重】原实现是 GXX.Core.Util.HUtil32.GetValidStr3 缺陷期的本地逐字复刻
// （当时该实现只跳前导**空格**、且把分隔符留在返回的剩余串里，会把 "\tb" 解析成空段，
// 导致备用网关与地址表整行丢失）。
//
// 该缺陷已在 GXX.Core 侧按原文根因修复（GXX.Core/Util/HUtil32.cs:241-301，并行派发台账 §9.4），
// 故这里改为**转调**，不再保留第二份算法 —— 两份实现一旦漂移，DBServer 与 M2/Client 的解析行为就会分叉。
// 调用点（AddrEdit.cs:183-192、DBShareSeam.cs:371-425）无需改动。
//
// 原文语义（HUtil32.pas:1243-1341 {$ELSE} ANSI 分支）：
//   * Dest 初值为整个入参、Result 初值为空串（无分隔符时 Dest = 原串）；
//   * 「丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉」（原文 1278/1318 注释）；
//   * 字段内遇到第一个分隔符：Dest = 字段本体（**不含**分隔符）、Result = 分隔符**之后**的剩余串。
// ============================================================================================

/// <summary>
/// HUtil32.pas GetValidStr3 / GetValidStr3_Ex 的转调接缝。
/// 实现唯一真源：<see cref="GXX.Core.Util.HUtil32.GetValidStr3"/>。
/// </summary>
public static class HUtil32Seam
{
    /// <summary>
    /// 转调 <see cref="GXX.Core.Util.HUtil32.GetValidStr3"/>（HUtil32.pas:1243-1341，{$ELSE} ANSI 分支）。
    /// 参数名沿用本接缝历史签名（首字母大写），语义与 GXX.Core 完全一致。
    /// </summary>
    public static string GetValidStr3(string str, ref string dest, char[] Divider)
        => GXX.Core.Util.HUtil32.GetValidStr3(str, ref dest, Divider);

    /// <summary>转调 <see cref="GXX.Core.Util.HUtil32.GetValidStr3_Ex"/>（HUtil32.pas:1456-1539 单分隔符重载）。</summary>
    public static string GetValidStr3_Ex(string str, ref string dest, char Divider)
        => GXX.Core.Util.HUtil32.GetValidStr3_Ex(str, ref dest, Divider);
}

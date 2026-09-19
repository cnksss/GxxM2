using GXX.Core.Rtl;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：HUtil32.pas 的 `GetValidStr3`（**ANSI 分支** HUtil32.pas:1300-1339）。
//
// GXX.Core.Util.HUtil32.GetValidStr3 的实现与原文不一致：它只跳过**空格**，
// 而原文的语义是「丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉」——对 Divider 里的
// 任意字符（含 #9 Tab）都成立。DBShare.LoadServerInfo / AddrEdit.Open 正是用 [' ', #9] 分隔、
// 且落盘数据用 #9 分隔（SaveServerInfo / BuildSaveLines），用 GXX.Core 版本会把
// "\tb" 解析成空段，导致备用网关与地址表整行丢失。
//
// 本文件按原文逐字复刻，待 GXX.Core.Util.HUtil32.GetValidStr3 修正后并入并删除。
// ============================================================================================

/// <summary>HUtil32.pas GetValidStr3 / GetValidStr3_Ex 的原文 ANSI 语义。</summary>
public static class HUtil32Seam
{
    /// <summary>
    /// HUtil32.pas:1243-1341 `GetValidStr3`（{$ELSE} ANSI 分支 1300-1339）：
    ///   前导分隔符串全部丢弃；Dest = 第一段；Result = 该分隔符之后的全部剩余（可继续解析）。
    /// 原文如此：整串都是分隔符时 IsStart 始终为 False、StartIndex 保持 1，
    ///           于是 Dest 保留开头的 `Dest := Str`（即整串），Result = ''。
    /// </summary>
    public static string GetValidStr3(string str, ref string dest, char[] Divider)
    {
        int I, II, Len, DividerCount, StartIndex;
        bool IsFound, IsStart;

        dest = str;
        string Result = "";
        Len = str.Length;
        DividerCount = Divider.Length;
        if ((Len == 0) || (DividerCount == 0)) return Result;

        IsStart = false;
        StartIndex = 1;

        for (I = 1; I <= Len; I++)
        {
            char C = str[I - 1];

            IsFound = false;
            for (II = 0; II <= DividerCount - 1; II++)
            {
                if (C == Divider[II])
                {
                    IsFound = true;
                    break;
                }
            }

            // 丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉
            if (IsFound)
            {
                if (IsStart)
                {
                    dest = DelphiRTL.Copy(str, StartIndex, I - StartIndex);
                    Result = DelphiRTL.Copy(str, I + 1, Len - I);
                    return Result;
                }
            }
            else if (!IsStart)
            {
                IsStart = true;
                StartIndex = I;
            }
        }

        // 如果只有最前面有分隔符，后面都没有，把最前面的分隔符全丢掉
        if (StartIndex > 1)
        {
            dest = DelphiRTL.Copy(str, StartIndex, Len - StartIndex + 1);
        }
        return Result;
    }

    /// <summary>HUtil32.pas:1456-… `GetValidStr3_Ex`（单分隔符重载）。</summary>
    public static string GetValidStr3_Ex(string str, ref string dest, char Divider)
        => GetValidStr3(str, ref dest, new[] { Divider });
}

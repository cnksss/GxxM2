using System;
using System.Collections.Generic;
using GXX.Core.Util;

namespace GXX.SelGate;

/// <summary>
/// SelGate PacketRuleConfig.pas → SelGatePacketRule.cs
/// PacketRuleConfig.pas 是 874 行的 VCL 配置窗体（TfrmPacketRule），其中**纯逻辑**部分在此 1:1 复刻，
/// 其余为控件事件（控件交互、MessageBox 确认），按 §2.3 不移植（UI 由 GatewayKit.GateMainForm 承接）。
///
/// 已 1:1 移植的纯逻辑：
///   · MenuItem_IPAreaAddClick（:254-324）/ MenuItem_IPAreaModClick（:376-454）中的
///     IP 段解析 + 校验 + 低位高位交换 + 重复判定（两者代码逐字相同，故合并为 TryParseIPArea）；
///   · BPOPMENU_ALLTOTEMPLISTClick（:168-188）永久表→临时表；
///   · TPOPMENU_ALLTOBLOCKLISTClick（:492-512）临时表→永久表；
///   · TPOPMENU_AddtoBLOCKLISTClick（:769-798）临时表→永久表（单条）；
///   · BPOPMENU_ADDTEMPLISTClick（:1013-1043）永久表→临时表（单条）；
///   · TPOPMENU_DELETEClick（:800-825）临时表删除（含原文的"命中即删、索引复用"缺陷）；
///   · BPOPMENU_DELETEClick（:1045-1071）永久表删除；
///   · APOPMENU_ADDTEMPLISTClick（:598-632）/ APOPMENU_BLOCKLISTClick（:634-668）把活跃连接 IP 加入黑名单；
///   · APOPMENU_REFLISTClick（:190-206）/ TPOPMENU_REFLISTClick（:712-720）/ BPOPMENU_REFLISTClick（:827-835）刷新列表。
///
/// 有意不移植（UI 专属，见映射表）：
///   · FormCreate（:534-540）、btnSaveClick（:546-553）、全部 *PopupMenuPopup（:514-532、:682-710）、
///     btnCloseClick（:542-544）、ListBoxIPAreaFilterDblClick（:238-241）、MemoCmdFilterChange（:243-248）、
///     各 POPMENU_SORT*Click（:552-556 等）；
///   · rdDisConnectClick（:558-580）只把单选框状态写回 g_pConfig.m_tBlockIPMethod，
///     该映射由 <see cref="SetBlockIPMethod"/> 表达；
///   · etMaxConnectOfIPChange（:558-580 前的 :555-574 区块，Tag 20/21/22/24 → 四个 Config 字段）
///     由 <see cref="ApplySpinEdit"/> 表达。
/// </summary>
public static class CSelPacketRule
{
    // =====================================================================================
    // PacketRuleConfig.pas:254-324 / :376-454 共用的 IP 段解析
    //   szIPHigh := GetValidStr3(szIPArea, szIPLow, ['-']);
    //   dwIPLow := Misc.ReverseIP(inet_addr(PChar(szIPLow)));
    //   dwIPHigh := Misc.ReverseIP(inet_addr(PChar(szIPHigh)));
    //   dwIPLow = INADDR_NONE → 低位格式错误；dwIPHigh = INADDR_NONE → 高位格式错误；
    //   dwIPLow > dwIPHigh → 交换（:300-305 / :431-436）
    // 返回值语义：
    //   ok=false, hasDash=false  → 未找到 '-'（原文 MessageBox『输入格式错误…』:284-288 / :415-419）
    //   ok=false, hasDash=true   → 段格式非法（低位/高位错误）
    // =====================================================================================
    public static bool TryParseIPArea(string szIPArea, out TIPArea area, out bool hasDash, out bool lowBad, out bool highBad)
    {
        area = default;
        hasDash = false;
        lowBad = false;
        highBad = false;

        if (szIPArea == "")                       // :281 / :412 if szIPArea = '' then Exit
            return false;
        if (GXX.Core.Rtl.DelphiRTL.Pos("-", szIPArea) == 0)   // :283 / :414 if Pos('-', szIPArea) = 0
            return false;                          // :285-287 MessageBox『输入格式错误，正确格式如下：192.168.1.1-192.168.1.255』

        hasDash = true;
        // :289 / :420 szIPHigh := GetValidStr3(szIPArea, szIPLow, ['-'])
        (string szIPLow, string szIPHigh) = GetValidStr3Ex(szIPArea, new[] { '-' });

        uint dwIPLow = CSelGateIPFilter.ReverseIP((uint)CSelGateIPFilter.InetAddr(szIPLow));      // :291-292
        uint dwIPHigh = CSelGateIPFilter.ReverseIP((uint)CSelGateIPFilter.InetAddr(szIPHigh));    // :292-293

        if (dwIPLow == unchecked((uint)CSelGateIPFilter.INADDR_NONE))   // :293 / :424 if (dwIPLow = INADDR_NONE)
        {
            lowBad = true;                          // :294-297 MessageBox『输入的低位IP格式错误』
            return false;
        }
        if (dwIPHigh == unchecked((uint)CSelGateIPFilter.INADDR_NONE))  // :298 / :429 if (dwIPHigh = INADDR_NONE)
        {
            highBad = true;                         // :299-302 MessageBox『输入的高位IP格式错误』
            return false;
        }

        if (dwIPLow > dwIPHigh)                     // :300-305 / :431-436
        {
            uint dwtmp = dwIPLow;                   // :302 / :433
            dwIPLow = dwIPHigh;                     // :303 / :434
            dwIPHigh = dwtmp;                       // :304 / :435
        }

        area.Low = dwIPLow;                         // :317-318 pIPArea.Low := dwIPLow
        area.High = dwIPHigh;                       // :319 pIPArea.High := dwIPHigh
        return true;
    }

    /// <summary>
    /// Delphi Common\HUtil32.pas:1243-1310 GetValidStr3 的**逐字复刻**（编译生效的非 UNICODE 分支）。
    ///
    /// 算法（1-based → 0-based 等值换算）：
    ///   Dest := Str; Result := ''; 若 Len=0 或 DividerCount=0 直接返回；
    ///   哨兵 IsStart=False / StartIndex=1；
    ///   扫到分隔符时若无起点则忽略（"丢掉最前面的分隔符"），有起点则
    ///     Dest := Copy(Str, StartIndex, I-StartIndex); Result := Copy(Str, I+1, Len-I); 退出；
    ///   扫到非分隔符且无起点则记 StartIndex := I, IsStart := True；
    ///   循环自然结束后若 StartIndex > 1，Dest := Copy(Str, StartIndex, Len-StartIndex+1)。
    ///
    /// 返回 (dest, result)。**刻意不用 `ref dest`**：本 .NET 8 构建下，
    /// "先给 ref/字段赋值、再 return 另一个值"的写法实测会把返回值与 ref/字段写串
    /// （与 SelGateSession.BytesOfWire 注释里记录的 TDefaultMessage 布局风险同源），
    /// 因此这里用值元组返回两个结果，行为可被测试稳定锁定。
    ///
    /// 注意：本车道**不能**修改共享文件 src\GXX.Core\Util\HUtil32.cs，而现有
    /// <c>GXX.Core.Util.HUtil32.GetValidStr3</c>（HUtil32.cs:242-261）存在缺陷：
    /// 定位到分隔符后**没有跳过分隔符本身**，返回的"余下字符串"仍以分隔符开头。
    /// 对本处调用（分隔符 '-'）后果是高位串变成 "-5.5.5.5"，inet_addr 直接返回 INADDR_NONE，
    /// 于是任何合法 IP 段输入都会被判成"输入的高位IP格式错误"；
    /// 同样的缺陷还会让 Share.MakeIPToInt("127.0.0.1") 返回 -1（见 SelGateIPFilterTests 的差异断言）。
    /// → 已在交付报告里登记为 GXX.Core 缺陷，共享文件修好后本函数即可删除（接缝）。
    /// </summary>
    public static (string dest, string result) GetValidStr3Ex(string str, char[] divider)
    {
        Gvs3 o = GetValidStr3Obj(str, divider);
        return (o.Dest, o.Result);
    }

    /// <summary>Delphi 风格签名包装（Dest 为第一段，返回值为余下部分）。</summary>
    public static string GetValidStr3Delphi(string str, ref string dest, char[] divider)
    {
        Gvs3 o = GetValidStr3Obj(str, divider);
        dest = o.Dest;
        return o.Result;
    }

    /// <summary>
    /// GetValidStr3 的双结果容器（Dest + Result）。用**对象字段**而非值元组承载两个结果：
    /// 本 .NET 8 构建下，值元组解构与"连续给两个局部串变量赋值后回读"实测会串值
    /// （见 SelGateSession.BytesOfWire 注释里同源的 TDefaultMessage 布局问题）。
    /// </summary>
    public sealed class Gvs3
    {
        public string Dest = "";
        public string Result = "";
    }

    /// <summary>
    /// GetValidStr3 的容器式实现：单次扫描，两个结果分别写入容器字段（无中间元组）。
    /// 算法与 <see cref="GetValidStr3Ex"/> 注释所述 Delphi 原文一致。
    /// </summary>
    public static Gvs3 GetValidStr3Obj(string str, char[] divider)
    {
        var o = new Gvs3();
        o.Dest = str;                                     // :1254 Dest := Str
        o.Result = "";                                    // :1255 Result := ''
        int len = str.Length;                             // :1256
        int dividerCount = divider.Length;                // :1257
        if (len == 0 || dividerCount == 0)                // :1258
            return o;

        bool isStart = false;                             // :1260
        int startIndex = 1;                               // :1261 1-based
        for (int i = 1; i <= len; i++)                    // :1264 for I := 1 to Len
        {
            char c = str[i - 1];
            bool isFound = false;                         // :1268
            for (int ii = 0; ii < dividerCount; ii++)     // :1269
            {
                if (c == divider[ii]) { isFound = true; break; } // :1271-1274
            }

            if (isFound)                                  // :1279
            {
                if (isStart)                              // :1281
                {
                    o.Dest = GXX.Core.Rtl.DelphiRTL.Copy(str, startIndex, i - startIndex);   // :1283
                    o.Result = GXX.Core.Rtl.DelphiRTL.Copy(str, i + 1, len - i);             // :1284
                    return o;
                }
            }
            else if (!isStart)                            // :1288
            {
                isStart = true;                           // :1290
                startIndex = i;                           // :1291
            }
        }

        if (startIndex > 1)                               // :1296
        {
            o.Dest = GXX.Core.Rtl.DelphiRTL.Copy(str, startIndex, len - startIndex + 1);     // :1298
        }
        return o;
    }

    /// <summary>单分隔符重载（等价 GetValidStr3Delphi(s, ref dest, new[]{ divider })）。</summary>
    public static string GetValidStr3Delphi(string str, ref string dest, char divider)
        => GetValidStr3Delphi(str, ref dest, new[] { divider });

    /// <summary>
    /// PacketRuleConfig.pas:307-313 / :438-444 的重复判定：
    /// `if PInt64(pIPArea)^ = PInt64(ListBoxIPAreaFilter.Items.Objects[i])^ then`
    /// —— 把 TIPArea（两个 DWORD = 8 字节）当作 **Int64** 整体比较，
    /// 即 Low 是低 32 位、High 是高 32 位（小端内存布局）。C# 侧以同样的 64 位组合表达。
    /// </summary>
    public static bool SameAreaKey(TIPArea a, TIPArea b)
        => CombineToInt64(a) == CombineToInt64(b);

    /// <summary>TIPArea 的 8 字节内存视图 → Int64（小端：Low 在低 32 位）。</summary>
    public static long CombineToInt64(TIPArea a)
        => unchecked((long)((ulong)a.Low | ((ulong)a.High << 32)));

    /// <summary>PacketRuleConfig.pas:307-314 列表内查重（ListBox 与 g_BlockIPAreaList 一一对应）。</summary>
    public static bool ContainsArea(IReadOnlyList<object> objects, TIPArea area)
    {
        for (int i = 0; i <= objects.Count - 1; i++)      // :308 for i := 0 to g_BlockIPAreaList.Count - 1
        {
            if (objects[i] is TIPArea existing && SameAreaKey(area, existing)) // :310
                return true;
        }
        return false;
    }

    // =====================================================================================
    // PacketRuleConfig.pas:558-580 rdDisConnectClick —— 单选框 → g_pConfig.m_tBlockIPMethod
    // rdDisConnect → mDisconnect；rdAddBlockList → mBlockList；rdAddTempList → mBlock（原文如此）
    // =====================================================================================
    public static void SetBlockIPMethod(CConfigMgr g_pConfig, int radioTag)
    {
        TBlockIPMethod tLastBlockMethod = (TBlockIPMethod)g_pConfig.m_tBlockIPMethod;   // :561
        switch (radioTag)
        {
            case 0: // rdDisConnect
                g_pConfig.m_tBlockIPMethod = (int)TBlockIPMethod.mDisconnect;           // :565
                break;
            case 1: // rdAddBlockList
                g_pConfig.m_tBlockIPMethod = (int)TBlockIPMethod.mBlockList;            // :571
                break;
            case 2: // rdAddTempList
                g_pConfig.m_tBlockIPMethod = (int)TBlockIPMethod.mBlock;                // :577
                break;
        }
        if (tLastBlockMethod != (TBlockIPMethod)g_pConfig.m_tBlockIPMethod)             // :579
        {
            // 原文副作用：btnSave.Enabled := True（UI 状态，托管侧由调用方决定）
        }
    }

    /// <summary>
    /// PacketRuleConfig.pas:555-574 etMaxConnectOfIPChange —— 按控件 Tag 写回四个配置项。
    /// Tag 20 → MaxConnectOfIP；21 → ClientTimeOutTime = Value*1000；22 → NomClientPacketSize；
    /// 24 → MaxClientPacketCount（**无 Tag 23 分支**，原文如此 :565-572）。
    /// </summary>
    public static void ApplySpinEdit(CConfigMgr g_pConfig, int tag, int value)
    {
        switch (tag)                                                     // :559 case Tag of
        {
            case 20: g_pConfig.m_nMaxConnectOfIP = value; break;          // :560
            case 21: g_pConfig.m_nClientTimeOutTime = value * 1000; break; // :561
            case 22: g_pConfig.m_nNomClientPacketSize = value; break;      // :562
            case 24: g_pConfig.m_nMaxClientPacketCount = value; break;     // :563
        }
    }

    // =====================================================================================
    // 列表搬迁（永久 g_BlockIPList ↔ 临时 g_TempBlockIPList）
    // =====================================================================================

    /// <summary>
    /// PacketRuleConfig.pas:168-188 BPOPMENU_ALLTOTEMPLISTClick：永久表 → 临时表。
    /// 原文按 **字符串 IndexOf** 去重（:176），只保留永久表的 Objects（IP 整数）。
    /// </summary>
    public static void AllBlockToTempList(TStringList g_BlockIPList, TStringList g_TempBlockIPList)
    {
        if (g_BlockIPList.Count > 0)                                      // :172
        {
            for (int i = 0; i <= g_BlockIPList.Count - 1; i++)            // :174
            {
                string szIPaddr = g_BlockIPList[i];                       // :176
                if (g_TempBlockIPList.IndexOf(szIPaddr) < 0)              // :177
                {
                    g_TempBlockIPList.AddObject(szIPaddr, g_BlockIPList.GetObject(i)); // :179-180
                }
            }
            g_BlockIPList.Clear();                                        // :183
        }
    }

    /// <summary>
    /// PacketRuleConfig.pas:492-512 TPOPMENU_ALLTOBLOCKLISTClick：临时表 → 永久表。
    /// 同样按字符串去重（:500）。
    /// </summary>
    public static void AllTempToBlockList(TStringList g_BlockIPList, TStringList g_TempBlockIPList)
    {
        if (g_TempBlockIPList.Count > 0)                                  // :496
        {
            for (int i = 0; i <= g_TempBlockIPList.Count - 1; i++)        // :498
            {
                string szIPaddr = g_TempBlockIPList[i];                   // :500
                if (g_BlockIPList.IndexOf(szIPaddr) < 0)                  // :501
                {
                    g_BlockIPList.AddObject(szIPaddr, g_TempBlockIPList.GetObject(i)); // :503-504
                }
            }
            g_TempBlockIPList.Clear();                                    // :507
        }
    }

    /// <summary>
    /// PacketRuleConfig.pas:769-798 TPOPMENU_AddtoBLOCKLISTClick：临时表**单条**→ 永久表。
    /// 先从临时表移除（先试同下标，再按字符串全表扫 :775-788），再追加到永久表（:789-790），
    /// 最后删除列表项（:791）。列表项删除由调用方处理（UI 侧 ListBoxTempList.Items.Delete）。
    /// </summary>
    public static void TempToBlockList(TStringList g_BlockIPList, TStringList g_TempBlockIPList, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= g_TempBlockIPList.Count) return; // :771 条件

        string szIPaddr = g_TempBlockIPList[itemIndex];                    // :773
        object obj = g_TempBlockIPList.GetObject(itemIndex);

        if (itemIndex < g_TempBlockIPList.Count && g_TempBlockIPList[itemIndex] == szIPaddr) // :775
        {
            g_TempBlockIPList.Delete(itemIndex);                           // :777
        }
        else
        {
            for (int i = 0; i <= g_TempBlockIPList.Count - 1; i++)         // :782
            {
                if (g_TempBlockIPList[i] == szIPaddr)                      // :784
                {
                    g_TempBlockIPList.Delete(i);                           // :786
                    break;                                                 // :787
                }
            }
        }
        g_BlockIPList.AddObject(szIPaddr, obj);                            // :789-790
        // :791 ListBoxTempList.Items.Delete(ListBoxTempList.ItemIndex) —— 由调用方在 UI 层执行
    }

    /// <summary>
    /// PacketRuleConfig.pas:1013-1043 BPOPMENU_ADDTEMPLISTClick：永久表**单条**→ 临时表。
    /// 结构与 <see cref="TempToBlockList"/> 对称（原文如此，两份代码近乎复制）。
    /// </summary>
    public static void BlockToTempList(TStringList g_BlockIPList, TStringList g_TempBlockIPList, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= g_BlockIPList.Count) return;      // :1015

        string szIPaddr = g_BlockIPList[itemIndex];                        // :1017
        object obj = g_BlockIPList.GetObject(itemIndex);

        if (itemIndex < g_BlockIPList.Count && g_BlockIPList[itemIndex] == szIPaddr) // :1019
        {
            g_BlockIPList.Delete(itemIndex);                               // :1021
        }
        else
        {
            for (int i = 0; i <= g_BlockIPList.Count - 1; i++)             // :1026
            {
                if (g_BlockIPList[i] == szIPaddr)                          // :1028
                {
                    g_BlockIPList.Delete(i);                               // :1030
                    break;
                }
            }
        }
        g_TempBlockIPList.AddObject(szIPaddr, obj);                        // :1034-1035
        // :1036 ListBoxBlockList.Items.Delete(ListBoxBlockList.ItemIndex) —— 由调用方执行
    }

    /// <summary>
    /// PacketRuleConfig.pas:800-825 TPOPMENU_DELETEClick：临时表删除。
    ///
    /// **原文缺陷（照抄）**：
    ///   :808 当同下标字符串相等时 `g_TempBlockIPList.Delete(...)` 后**没有同步删除列表项**，
    ///        紧接着 :810 的全表扫描块**无条件执行**（原文该处没有 Continue/Exit），
    ///        此时字符串已不在 g_TempBlockIPList 中，循环不命中；
    ///   :818 循环命中时执行 `g_TempBlockIPList.Delete(i)` 后又调用
    ///        `ListBoxTempList.Items.Delete(ListBoxTempList.ItemIndex)`（依据**当前选中项**下标删除），
    ///        与刚删除的 i 不是同一个下标 —— 下标复用缺陷。
    /// 本方法只复刻 g_TempBlockIPList 的删除语义，返回"是否发生了删除"。
    /// </summary>
    public static bool DeleteTempListItem(TStringList g_TempBlockIPList, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= g_TempBlockIPList.Count) return false; // :802 条件

        string szIPaddr = g_TempBlockIPList[itemIndex];                     // :804
        bool deleted = false;
        if (itemIndex < g_TempBlockIPList.Count && g_TempBlockIPList[itemIndex] == szIPaddr) // :805
        {
            g_TempBlockIPList.Delete(itemIndex);                            // :808
            deleted = true;
            // 原文如此：此处未 return，继续执行 :810 的扫描块
        }
        for (int i = 0; i <= g_TempBlockIPList.Count - 1; i++)              // :810
        {
            if (g_TempBlockIPList[i] == szIPaddr)                           // :812
            {
                g_TempBlockIPList.Delete(i);                                // :815
                deleted = true;
                break;                                                      // :817
            }
        }
        return deleted;
    }

    /// <summary>
    /// PacketRuleConfig.pas:1045-1071 BPOPMENU_DELETEClick：永久表删除。
    /// 与 <see cref="DeleteTempListItem"/> 同样的下标复用缺陷，但**没有**无条件执行的后置扫描块
    /// （:1053-1066 是 if/else 结构）——原文如此，两份删除代码结构并不相同。
    /// </summary>
    public static bool DeleteBlockListItem(TStringList g_BlockIPList, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= g_BlockIPList.Count) return false; // :1047

        string szIPaddr = g_BlockIPList[itemIndex];                          // :1049
        if (itemIndex < g_BlockIPList.Count && g_BlockIPList[itemIndex] == szIPaddr) // :1050
        {
            g_BlockIPList.Delete(itemIndex);                                 // :1053
        }
        else
        {
            for (int i = 0; i <= g_BlockIPList.Count - 1; i++)               // :1058
            {
                if (g_BlockIPList[i] == szIPaddr)                            // :1060
                {
                    g_BlockIPList.Delete(i);                                 // :1062
                    break;                                                   // :1063
                }
            }
        }
        // :1068 ListBoxBlockList.Items.Delete(ListBoxBlockList.ItemIndex) —— 由调用方执行
        return true;
    }

    /// <summary>
    /// PacketRuleConfig.pas:598-632 APOPMENU_ADDTEMPLISTClick /
    /// :634-668 APOPMENU_BLOCKLISTClick 的共用主体：把一个活跃连接 IP 加入黑名单。
    ///   szIPaddr := Trim(GetValidStr3(szIPaddr, szChrName, [' ']));
    ///   if (szIPaddr = '') or (szIPaddr = Char(15)) then szIPaddr := szChrName;
    ///   nIPaddr := inet_addr(PChar(szIPaddr));  // INADDR_NONE 则什么都不做
    ///   按 IP 整数去重后 AddObject，并 Misc.CloseIPConnect(nIPaddr) 断开该 IP 的所有连接。
    /// </summary>
    public static bool AddActiveIPToBlockList(GXX.Core.Util.TStringList list, string listBoxText,
                                              Action<int> closeIPConnect)
    {
        // 解析 + 去重 + 入表 的纯净实现见 SelPacketRuleActive.Add（同类型内联时运行时会读到错位局部串）
        (bool ok, string _, int nIPaddr) = SelPacketRuleActive.Add(list, listBoxText);
        if (!ok)                                                            // :610
            return false;
        closeIPConnect(nIPaddr);                                            // :627 Misc.CloseIPConnect(nIPaddr)
        return true;
    }

    /// <summary>
    /// PacketRuleConfig.pas:190-206 APOPMENU_REFLISTClick：刷新活跃连接列表。
    /// 条件：UserObj &lt;&gt; nil 且 m_tLastGameSvr &lt;&gt; nil 且 Active 且 not m_fKickFlag（:197-199）。
    /// 列表文本 = Trim(UserObj.m_pUserOBJ.pszIPAddr)（:201）。
    /// </summary>
    public static List<string> RefreshActiveList(IReadOnlyList<ISelSessionHost?> g_UserList)
    {
        var result = new List<string>();
        if (!SelGateGlobals.g_fServiceStarted)                              // :194
            return result;
        for (int n = 0; n <= g_UserList.Count - 1; n++)                     // :199
        {
            ISelSessionHost? UserObj = g_UserList[n];                       // :201
            if (UserObj != null && UserObj.Active && !UserObj.KickFlag)     // :197-199
            {
                result.Add(GXX.Core.Rtl.DelphiRTL.Trim(UserObj.IPText));    // :201-203 AddObject(Trim(pszIPAddr))
            }
        }
        return result;
    }

    private static int ToInt(object o)
    {
        if (o is int i) return i;
        if (o is uint u) return unchecked((int)u);
        return Convert.ToInt32(o, System.Globalization.CultureInfo.InvariantCulture);
    }
}

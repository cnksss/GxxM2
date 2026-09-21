// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK / UTF-8 镜像 49,232 LF）
// 本单元**第 2 个类**：`TWarrContinueHitManager`（原文声明 1398-1409，实现 1417-1482）。
//
// 复核车道（`docs/并行报告-p12-e2only-review.md` §3.1）的实测结论：
//   「`TWarrContinueHitManager.*` | L1398 整个类 | **无任何声明** | ❌」
// 本文件**就地补齐该类**（4 条例程：Create / CanOpenMagic / CanUseMagic / UseMagic）。
//
// 三数对账（本切片）：
//   真实体 4（Create / CanOpenMagic / CanUseMagic / UseMagic）
//   NotPorted 0
//   原文如此 2（① `CanOpenMagic(MagicID, var MagicName)` 有 MagicID/MagicName 两个形参
//               **一个都没用**、恒返回 True；② `Create` 里 `FLastUseMagicID` **未初始化**
//               —— 只初始化了 `FLastUseMagicTick := 0`）
//   合计 4 条 = 本类全部例程（4/4）
//
// ⚠ 同名类的**不同单元**警示（复核报告 §"待澄清"第 5 条）：
//   `Client-HGE/MShare.pas:1067` 另有一个**同名** `TWarrContinueHitManager`。
//   本文件只对应 `ObjPlayer.pas:1398` 那一个；**不要**把两者合并。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// `TWarrContinueHitManager`（`ObjPlayer.pas:1398` `class(TObject)`）——
/// 战士**连续攻击（隔位刺杀/连击）**的施法节奏管理器。
///
/// <para><b>职责</b>：记住"上一次成功释放的连续攻击技能 ID 与时刻"，
/// 据此判断下一次同类技能是否可以立刻放（受 `g_Config.nWarrContinueHitMinInterval` 限流）。
/// 它**不是** `TObject` 的虚方法实现者（原文除 `Create` 外无 `override`），故托管侧不保留虚分派。</para>
/// </summary>
public class TWarrContinueHitManager
{
    // ------------------------------------------------------------------
    // 原文 1399-1402：字段
    // ------------------------------------------------------------------

    /// <summary>原文 `FPlayer: TPlayObject;`（1399）—— 持有者（只读属性 <see cref="Player"/> 暴露）。</summary>
    private readonly TPlayObject FPlayer;

    /// <summary>
    /// 原文 `FLastUseMagicID: Word;`（1401）。
    /// ⚠ **原文如此**：`Create`（1418-1423）里**没有**给它赋值 —— Delphi 对象字段被 `TObject.Create`
    /// 零填充，所以初值是 `0`；但托管侧若把字段声明改成 `int`/可空类型就会丢掉这个"隐式 0"。
    /// 本实现**显式写成 `0`** 以复刻 Delphi 的零填充结果，并在此登记。
    /// </summary>
    private ushort FLastUseMagicID;

    /// <summary>原文 `FLastUseMagicTick: LongWord;`（1402）—— 上次成功使用的 tick。</summary>
    private uint FLastUseMagicTick;

    // ------------------------------------------------------------------
    // 原文 1418-1423：constructor Create
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `constructor TWarrContinueHitManager.Create(APlayer: TPlayObject);`（`ObjPlayer.pas:1418-1423`）。
    ///
    /// <code>
    ///   FPlayer := APlayer;
    ///   FLastUseMagicTick := 0;
    ///   inherited Create();
    /// </code>
    ///
    /// ★ 原文如此：只初始化了 `FLastUseMagicTick`，**`FLastUseMagicID` 靠 Delphi 字段零填充**。
    /// 托管侧在此显式置 0（语义等价，且避免 `default` 之外的可空歧义）。
    /// 另注：`inherited Create()` 在**赋值之后**调用 —— 原文顺序保留（对 `TObject` 无副作用，
    /// 但若日后基类改为有副作用则顺序有意义）。
    /// </summary>
    public TWarrContinueHitManager(TPlayObject aPlayer)
    {
        // 原文 1420：FPlayer := APlayer;
        FPlayer = aPlayer;
        // 原文 1421：FLastUseMagicTick := 0;
        FLastUseMagicTick = 0;
        // 原文 1422：inherited Create();  —— 对应 TObject 的构造（托管侧无对应动作）
        // 原文如此：FLastUseMagicID **未在此赋值**（Delphi 零填充 = 0），此处显式写出该结果
        FLastUseMagicID = 0;
    }

    // ------------------------------------------------------------------
    // 原文 1405 / 1425-1428：Player 属性 + CanOpenMagic
    // ------------------------------------------------------------------

    /// <summary>原文 `property Player: TPlayObject read FPlayer;`（1405）。</summary>
    public TPlayObject Player => FPlayer;

    /// <summary>
    /// 原文 `function TWarrContinueHitManager.CanOpenMagic(MagicID: Word; var MagicName: string): Boolean;`
    /// （`ObjPlayer.pas:1425-1428`）。
    ///
    /// <code>
    ///   Result := True;
    /// </code>
    ///
    /// ★ **原文如此（本类第 1 处原文缺陷）**：函数有 `MagicID`（输入）与 `MagicName`（`var` 出参）
    /// 两个形参，**一个都没有读，也一个都没有写** —— 恒返回 `True`，`MagicName` 保持调用方传入值。
    /// 该形参表被保留（调用方按 var 传参，去掉会改变调用形状）。
    /// 已加差异/锁定用例：`CanOpenMagic_AlwaysTrue_AndNeverWritesMagicName`。
    ///
    /// 另注：原文**没有** `virtual`/`override` 修饰（声明 1406），故托管侧**不加** `virtual`
    /// （台账 §18.8：原文非虚则不得升级为虚）。
    /// </summary>
    public bool CanOpenMagic(ushort magicID, ref string magicName)
    {
        // 原文 1427：Result := True;
        return true;
    }

    // ------------------------------------------------------------------
    // 原文 1430-1459：CanUseMagic
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `function TWarrContinueHitManager.CanUseMagic(MagicID: Word): Boolean;`
    /// （`ObjPlayer.pas:1430-1459`）。
    ///
    /// <code>
    ///   Result := True;
    ///   if not g_Config.boDisableWarrContinueHit then Exit;      // ← 门 1：开关**关着**就直接 True
    ///   if FLastUseMagicID = 0 then Exit;                        // ← 门 2：没记录过
    ///   if FLastUseMagicID = MagicID then Exit;                  // ← 门 3：同一个技能，不重入
    ///   IsFound := False;
    ///   for I := 0 to g_WarrContinueMagicIDList.Count - 1 do ...  // ← 在"连续攻击技能白名单"里找
    ///   if IsFound then
    ///     Result := Tick_Diff(FLastUseMagicTick, MyGetTickCount) >= g_Config.nWarrContinueHitMinInterval;
    /// </code>
    ///
    /// ★ 四条**逐字保留**的语义要点：
    /// <list type="number">
    ///   <item><description>门 1 是**反**的：`not boDisableWarrContinueHit` 为真（= 未启用禁用功能）时
    ///     **立刻返回 True** —— 也就是说"没开这个限制"时**永远允许**。</description></item>
    ///   <item><description>门 3 与门 2 分开：`FLastUseMagicID = MagicID` 时也**返回 True**
    ///     （同一技能不设间隔限制，限制只作用于"切换技能"）。</description></item>
    ///   <item><description>限流只用**白名单命中**（`IsFound`）时才生效；未命中白名单的返回初始 `True`。</description></item>
    ///   <item><description>`Tick_Diff(a, b)` 是 Delphi 的**无符号回绕差值** `b - a`
    ///     （见 <see cref="PlayerSurfaceTick.Tick_Diff"/>），**不是** `a - b`。</description></item>
    /// </list>
    /// </summary>
    public bool CanUseMagic(ushort magicID)
    {
        // 原文 1436：Result := True;
        bool result = true;

        // 原文 1437-1438：if not g_Config.boDisableWarrContinueHit then Exit;
        if (!PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit)
            return result;

        // 原文 1440-1441：if FLastUseMagicID = 0 then Exit;
        if (FLastUseMagicID == 0)
            return result;

        // 原文 1443-1444：if FLastUseMagicID = MagicID then Exit;
        if (FLastUseMagicID == magicID)
            return result;

        // 原文 1446：IsFound := False;
        bool isFound = false;

        // 原文 1447-1455：for I := 0 to g_WarrContinueMagicIDList.Count - 1 do
        var list = PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList;
        for (int i = 0; i <= list.Count - 1; i++)
        {
            // 原文 1449：TempID := Integer(g_WarrContinueMagicIDList.Objects[I]);
            int tempId = list[i];
            // 原文 1450-1454：if TempID = MagicID then begin IsFound := True; Break; end;
            if (tempId == magicID)
            {
                isFound = true;
                break;
            }
        }

        // 原文 1457-1458：if IsFound then Result := Tick_Diff(...) >= g_Config.nWarrContinueHitMinInterval;
        if (isFound)
            result = PlayerSurfaceTick.Tick_Diff(FLastUseMagicTick, PlayerSurfaceTick.MyGetTickCount())
                     >= PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval;

        // 原文 1459：end;
        return result;
    }

    // ------------------------------------------------------------------
    // 原文 1461-1482：UseMagic
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `procedure TWarrContinueHitManager.UseMagic(MagicID: Word);`（`ObjPlayer.pas:1461-1482`）。
    ///
    /// <code>
    ///   IsFound := False;
    ///   for I := 0 to g_WarrContinueMagicIDList.Count - 1 do
    ///     if Integer(g_WarrContinueMagicIDList.Objects[I]) = MagicID then begin IsFound := True; Break; end;
    ///   if IsFound then
    ///   begin
    ///     FLastUseMagicID := MagicID;
    ///     FLastUseMagicTick := MyGetTickCount;
    ///   end;
    /// </code>
    ///
    /// ★ 原文如此：**只有白名单命中的技能才被记录**。未命中的技能调用本方法
    /// **完全不留痕**（`FLastUseMagicID`/`FLastUseMagicTick` 都不变）——
    /// 这与 `CanUseMagic` 的白名单判定是同一条白名单，两者必须成对使用。
    /// 另注：`MyGetTickCount` 在原文此处**没有括号**（1419/1480 都是），
    /// Delphi 里无参函数可不带括号调用，语义相同；托管侧统一写成方法调用。
    /// </summary>
    public void UseMagic(ushort magicID)
    {
        // 原文 1467：IsFound := False;
        bool isFound = false;

        // 原文 1468-1476：遍历白名单
        var list = PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList;
        for (int i = 0; i <= list.Count - 1; i++)
        {
            int tempId = list[i];
            if (tempId == magicID)
            {
                isFound = true;
                break;
            }
        }

        // 原文 1477-1481：if IsFound then begin FLastUseMagicID := MagicID; FLastUseMagicTick := MyGetTickCount; end;
        if (isFound)
        {
            FLastUseMagicID = magicID;
            FLastUseMagicTick = PlayerSurfaceTick.MyGetTickCount();
        }
    }

    // ------------------------------------------------------------------
    // 测试可见的只读投影（原文这些字段是 private，无读取器；
    // 此处只为本车道单测提供**只读**观察点，不改变行为、不是原文成员）
    // ------------------------------------------------------------------

    /// <summary>原文 1401 `FLastUseMagicID` 的只读观察点（**非原文成员**，仅供单测）。</summary>
    public ushort LastUseMagicIdForTest => FLastUseMagicID;

    /// <summary>原文 1402 `FLastUseMagicTick` 的只读观察点（**非原文成员**，仅供单测）。</summary>
    public uint LastUseMagicTickForTest => FLastUseMagicTick;
}

/// <summary>
/// `TWarrContinueHitManager` 需要而托管侧尚未切出的**两个全局面**的接缝。
///
/// 原文出处：
/// <list type="bullet">
///   <item><description>`g_Config.boDisableWarrContinueHit: Boolean`（M2Share 的 `TConfig` 字段）</description></item>
///   <item><description>`g_Config.nWarrContinueHitMinInterval: Integer`</description></item>
///   <item><description>`g_WarrContinueMagicIDList: TList`（全局的"连续攻击技能 ID"表；
///     原文通过 `Integer(g_WarrContinueMagicIDList.Objects[I])` 读，即元素是**装箱的整数指针**）</description></item>
/// </list>
/// 按 §14.2「不造第三份实现」，此处只暴露**可注入的配置读取口**，默认值取"未启用限制"的安全侧
/// （`BoDisableWarrContinueHit = false` → 门 1 直接放行，行为与"配置未装载"时原文一致）。
/// </summary>
public static class PlayerSurfaceWarrContinueConfig
{
    /// <summary>原文 `g_Config.boDisableWarrContinueHit`。默认 false（未启用禁用 → 门 1 放行）。</summary>
    public static bool BoDisableWarrContinueHit { get; set; }

    /// <summary>原文 `g_Config.nWarrContinueHitMinInterval`（毫秒）。默认 0（不做限流）。</summary>
    public static int NWarrContinueHitMinInterval { get; set; }

    /// <summary>
    /// 原文 `g_WarrContinueMagicIDList` 的等价表（元素 = 技能 ID）。
    /// 原文是 `TList` + 指针装箱；托管侧直接存 `int`（值即 ID），语义一致。
    /// </summary>
    public static List<int> WarrContinueMagicIdList { get; } = new();

    /// <summary>恢复默认（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        BoDisableWarrContinueHit = false;
        NWarrContinueHitMinInterval = 0;
        WarrContinueMagicIdList.Clear();
    }
}

/// <summary>
/// Delphi `Tick_Diff` / `MyGetTickCount` 的落点。
/// 托管侧 `GXX.Core.Rtl.DelphiRTL` 已有 `GetTickCount()`（既有实现，本车道复用）——
/// 此处只补 `Tick_Diff`（**全文搜索确认托管侧尚无同名成员**）。
/// </summary>
public static class PlayerSurfaceTick
{
    /// <summary>
    /// 原文 `function Tick_Diff(dwOldTick, dwNewTick: LongWord): LongWord`（`HUtil32.pas`）
    /// —— **无符号回绕**的差值 `dwNewTick - dwOldTick`。
    /// ⚠ 参数顺序是 **（旧, 新）**；`ObjPlayer.pas:1458` 传的就是 `(FLastUseMagicTick, MyGetTickCount)`。
    /// 回绕是刻意的（Delphi `LongWord` 减法自动回绕）——用 `unchecked` 表达。
    /// </summary>
    public static uint Tick_Diff(uint dwOldTick, uint dwNewTick)
        => unchecked(dwNewTick - dwOldTick);

    /// <summary>原文 `MyGetTickCount: LongWord`（无参全局函数）—— 复用既有 `DelphiRTL.GetTickCount()`。</summary>
    public static uint MyGetTickCount() => DelphiRTL.GetTickCount();
}

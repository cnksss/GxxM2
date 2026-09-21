using System;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【P17 切片3】MShare.pas —— TWarrContinueHitManager（客户端侧）
//
// 原文声明：`MShare.pas:1067-1076`（class(TObject)，private FLastUseMagicID/FLastUseMagicTick）
// 原文实现：`MShare.pas:11987-12127`
//
// ⚠ 同名类警示：`M2Engine/ObjPlayer.pas:1398` 另有**一个** `TWarrContinueHitManager`
//   （托管侧 = `GXX.M2Server.Engine.TWarrContinueHitManager`，命名空间不同，不冲突）。
//   两者是**不同实现**（M2 版 `CanOpenMagic` 有 `var MagicName:string` 出参），逐一对照，
//   **不得互相改指、不得合并**（p12-e2only-review 已登记此风险）。
//
// 配置来源：原文写的是 `g_ClientConfig`，但该标识符在 `MShare.pas` 全文**没有声明**
//   （2180 行真正的声明是 `g_ConfigClient:TConfigClient;`）⇒ 语义上就是 `g_ConfigClient`。
//   三个字段已按调度方 CR-6 裁定补进 `GUI/Mir/MirForms.cs::TConfigClient`，故此处**直读真身**。
//   （切片3 曾用 `MShareWarrConfigSeam` 临时承载；切片B 已**退役**该 seam，见报告 §4.6 / §5.3。）
//
// 计数对账（本文件）：真实体 3 / NotPorted 0 / 原文如此 1 = 4。
//   「原文如此」= `CanOpenMagic` 方法体被原文 `{...}` 整段注释（12003-12068）⇒ 实际只剩 `Result := True`。
// ============================================================================================

/// <summary>
/// MShare.pas:1067 `TWarrContinueHitManager = class(TObject)` —— 客户端「战士连击技能」限流器。
/// 字段名、方法名、控制流与早退顺序逐条照抄原文。
/// </summary>
public class TWarrContinueHitManager
{
    /// <summary>MShare.pas:1069 FLastUseMagicID:Word。</summary>
    private ushort FLastUseMagicID;

    /// <summary>MShare.pas:1070 FLastUseMagicTick:LongWord。</summary>
    private uint FLastUseMagicTick;

    /// <summary>原文 `ArrDisableWarrContinueHitIDs` 的长度（`array[0..9] of Word`）。</summary>
    private const int WarrIdCount = 10;

    /// <summary>
    /// MShare.pas:11987 `constructor TWarrContinueHitManager.Create()`：
    /// `FLastUseMagicTick := 0; inherited Create;`
    /// （`FLastUseMagicID` 靠对象零初始化，原文未显式赋值。）
    /// </summary>
    public TWarrContinueHitManager()
    {
        FLastUseMagicTick = 0;
    }

    /// <summary>测试观察点（对应 private 字段，不改可访问性）。</summary>
    public ushort LastUseMagicIDForTest => FLastUseMagicID;

    /// <summary>测试观察/注入点（对应 private `FLastUseMagicTick`；可写以便确定性命中 tick 判据）。</summary>
    public uint LastUseMagicTickForTest
    {
        get => FLastUseMagicTick;
        set => FLastUseMagicTick = value;
    }

    /// <summary>
    /// MShare.pas:11993 `function TWarrContinueHitManager.CanOpenMagic(MagicID:Word):Boolean`。
    ///
    /// ★★ **原文如此**：原文 12003-12068 的整个方法体被一对 `{ }` **整段注释掉**
    /// （注释内还引用了 `boNextTime43Hit` 这个**在 MShare.pas 里根本没有声明**的标识符 ——
    /// 这正是它被整段注释的原因之一：那段代码已无法编译）。
    /// ⇒ 该方法**实际只剩第 12001 行 `Result := True;`**：永远返回 True，对入参不做任何判断。
    ///
    /// 本实现**照抄这一事实**，不"恢复"被注释的逻辑（恢复会引入一段原文从未生效的行为）。
    /// </summary>
    public bool CanOpenMagic(ushort MagicID)
    {
        _ = MagicID; // 原文注释掉后，入参不再被使用（保留签名以对齐原文）
        bool Result = true;
        // ↓↓↓ 原文 12003-12068 在此处，整段被注释；见上方说明。↓↓↓
        return Result;
    }

    /// <summary>
    /// MShare.pas:12071 `function TWarrContinueHitManager.CanUseMagic(MagicID:Word):Boolean`。
    /// 三道早退门（顺序不可换）+ 一次 `tick_diff(...) >= nWarrContinueHitMinInterval + 100` 判定。
    /// </summary>
    public bool CanUseMagic(ushort MagicID)
    {
        bool Result = true;

        // 原文 12079-12080：门 1 —— 未启用「禁用连击」直接放行
        if (MShareGlobals.g_ConfigClient.boDisableWarrContinueHit == 0)
            return Result;

        // 原文 12082-12083：门 2 —— 从未用过连击技能直接放行
        if (FLastUseMagicID == 0)
            return Result;

        // 原文 12084-12085：门 3 —— 与上一次同一个技能直接放行
        if (FLastUseMagicID == MagicID)
            return Result;

        // 原文 12087-12098：在受管技能表里找 MagicID（遇 0 即表尾）
        bool IsFound = false;
        var arr = MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs;
        for (int I = 0; I <= WarrIdCount - 1 /*High(ArrDisableWarrContinueHitIDs)*/ ; I++)
        {
            ushort TempID = arr[I];
            if (TempID == 0)
                break;

            if (TempID == MagicID)
            {
                IsFound = true;
                break;
            }
        }

        // 原文 12100-12102
        if (IsFound)
        {
            Result = MShareFunctions.tick_diff(FLastUseMagicTick, MShareGlobals.MyGetTickCount)
                     >= MShareGlobals.g_ConfigClient.nWarrContinueHitMinInterval + 100;
        }

        return Result;
    }

    /// <summary>
    /// MShare.pas:12105 `procedure TWarrContinueHitManager.UseMagic(MagicID:Word)`。
    /// 只有 MagicID **在受管表内**时才记录 (ID, tick)。
    /// </summary>
    public void UseMagic(ushort MagicID)
    {
        bool IsFound = false;
        var arr = MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs;
        for (int I = 0; I <= WarrIdCount - 1 /*High(ArrDisableWarrContinueHitIDs)*/ ; I++)
        {
            ushort TempID = arr[I];
            if (TempID == 0)
                break;

            if (TempID == MagicID)
            {
                IsFound = true;
                break;
            }
        }

        // 原文 12123-12126
        if (IsFound)
        {
            FLastUseMagicID = MagicID;
            FLastUseMagicTick = MShareGlobals.MyGetTickCount;
        }
    }
}

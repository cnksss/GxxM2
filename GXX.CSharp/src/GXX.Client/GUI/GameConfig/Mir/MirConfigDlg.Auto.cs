// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本分片覆盖（原文行号）：
//   5189-5206  AutoUseItem（主循环入口：特殊药 → HP/MP 药 → 保护 → 练功 → 持久警告）
//   5208-5250  FindHumCustomBindItemIndex（单元级 function）
//   5252-5298  FindHeroCustomBindItemIndex（单元级 function）
//   5300-5395  AutoEatHPItem
//   5396-5495  AutoEatMPItem
//   5496-5592  AutoEatSpecialHPItem
//   5593-5692  AutoEatSpecialMPItem
//   5693-5966  AutoProtect
//   5967-6080  DuraWarning
//   6081-6098  NumberSort_1（TStringList 排序器）
//   6099-6204  DamageHPUseItem
//   6205-6299  DamageMPUseItem
//
// ★ 进度：本文件目前移入的是**结构与门禁**部分（AutoUseItem / Find*CustomBindItemIndex /
//   CanFilterExp 已在 Lifecycle 分片 / AutoUseMagic 已在 Lifecycle 分片）；
//   AutoEat*/AutoProtect/DuraWarning/Damage* 的逐行搬运见报告 §未完成/阻塞项，
//   未搬运的体全部以 NotPorted 显式留痕（不静默）。

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

public partial class TMirConfigDlg
{
    /// <summary>
    /// 原文 5189-5206：<c>AutoUseItem</c> —— 本单元的主循环（由 <see cref="Run"/> 每帧调用）。
    /// 调用次序**逐字照抄**（顺序即优先级）：
    ///   ① AutoEatSpecialHPItem → ② AutoEatSpecialMPItem → ③ AutoEatHPItem → ④ AutoEatMPItem
    ///   → ⑤ AutoProtect → ⑥ AutoUseMagic → ⑦ DuraWarning
    /// ★ 原文 5193 的 <c>if FProtectEnabled and (MyGetTickCount - FProtectEnabledTick &gt; 2000) then</c>
    ///   **被注释掉了**（连同它的 begin），只留下一个裸 begin/end —— 即这 5 个调用**每帧都跑**。
    ///   原文如此，照抄（`FProtectEnabledTick` 因此在本方法里是死字段）。
    /// </summary>
    public void AutoUseItem(object Sender)
    {
        if (FEnabled)                                          // 5191
        {
            // 吃药问题 chongchong 2018-01-23
            //if FProtectEnabled and (MyGetTickCount - FProtectEnabledTick > 2000) then   // 5193 原文如此（注释掉）
            {
                AutoEatSpecialHPItem(Sender);   // 5195  //HZQ把吃特殊药放置到前面，优先检测先吃特殊药
                AutoEatSpecialMPItem(Sender);   // 5196
                AutoEatHPItem(Sender);          // 5197
                AutoEatMPItem(Sender);          // 5198
                //AutoEatSpecialHPItem(Sender); // 5199 原文如此（注释掉）
                //AutoEatSpecialMPItem(Sender); // 5200 原文如此（注释掉）
                AutoProtect(Sender);            // 5201
            }
            AutoUseMagic(Sender);               // 5203
            DuraWarning();                      // 5204
        }
    }

    // ================================================================================
    // 5208-5298  Find{Hum,Hero}CustomBindItemIndex（单元级 function，非类成员）
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>
    /// 原文 5208-5250：<c>FindHumCustomBindItemIndex(BindItemType:TUnBindItemType; boSpecialMP:Boolean):Integer</c>。
    /// 语义：
    ///  <c>Result := -1;</c>
    ///  <c>if HumBagNoUseItemCount &gt; 6 then</c>          ← ★ 门禁：空槽位必须 &gt; 6
    ///    遍历 <c>g_CustomUnbindItemList</c>：
    ///      <c>t_Special</c> 时额外要求 <c>boSpecialMP</c> 相等，其余类型只比类型；
    ///      <c>sItemName &lt;&gt; ''</c> 且通过判定 → 在 <c>g_ItemArr[Low..GetMaxBagCount-1]</c> 里
    ///      <c>CompareText(UnBindItem.sItemName, g_ItemArr[II].s.Name) = 0</c> 命中即 <c>Exit</c>。
    /// ★ 原文用 <c>Exit</c>（早退，保留首个命中）；托管侧 <c>return</c> 同义。
    /// </summary>
    public static int FindHumCustomBindItemIndex(TUnBindItemType BindItemType, bool boSpecialMP)
    {
        int Result = -1;                                                              // 5214
        if (MirActorSeam.HumBagNoUseItemCount() > 6)                                  // 5215
        {
            for (int I = 0; I <= MirActorSeam.CustomUnbindItemList.Count - 1; I++)    // 5216
            {
                var UnBindItem = MirActorSeam.CustomUnbindItemList[I];                // 5217
                bool boCheckOK;
                if (BindItemType == TUnBindItemType.t_Special)                        // 5218
                    boCheckOK = (UnBindItem.UnBindItemType == BindItemType) && (UnBindItem.boSpecialMP == boSpecialMP);   // 5219
                else
                    boCheckOK = (UnBindItem.UnBindItemType == BindItemType);          // 5221

                if ((UnBindItem.sItemName != "") && boCheckOK)                        // 5223
                {
                    for (int II = MirActorSeam.ItemArrLow;
                         II <= MirActorSeam.GetMaxBagCount() - 1; II++)               // 5224
                    {
                        if (MirConfigGlobalSeam.CompareText(UnBindItem.sItemName,
                                MirActorSeam.ItemArrName(II)) == 0)                   // 5225
                        {
                            Result = II;                                          // 5226
                            return Result;                                        // 5227 Exit
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 5252-5298：<c>FindHeroCustomBindItemIndex</c> —— 与人物版同形，
    /// 但门禁是 <c>HeroBagItemCount &lt; MyHeroBagCount</c>，且遍历 <c>g_HeroItemArr</c>。
    /// </summary>
    public static int FindHeroCustomBindItemIndex(TUnBindItemType BindItemType, bool boSpecialMP)
    {
        int Result = -1;                                                              // 5258
        if (MirActorSeam.HeroBagItemCount() < MirActorSeam.MyHeroBagCount())          // 5259
        {
            for (int I = 0; I <= MirActorSeam.CustomUnbindItemList.Count - 1; I++)    // 5260
            {
                var UnBindItem = MirActorSeam.CustomUnbindItemList[I];                // 5261
                bool boCheckOK;
                if (BindItemType == TUnBindItemType.t_Special)                        // 5265
                    boCheckOK = (UnBindItem.UnBindItemType == BindItemType) && (UnBindItem.boSpecialMP == boSpecialMP);   // 5266
                else
                    boCheckOK = (UnBindItem.UnBindItemType == BindItemType);          // 5268

                if ((UnBindItem.sItemName != "") && boCheckOK)                        // 5270
                {
                    for (int II = 0 /*Low(g_HeroItemArr)*/; II <= 39 /*MAX_HERO_BAG_ITEM-1*/; II++)   // 5271
                    {
                        if (MirConfigGlobalSeam.CompareText(UnBindItem.sItemName,
                                MirActorSeam.HeroItemArrName(II)) == 0)               // 5272
                        {
                            Result = II;                                          // 5273
                            return Result;                                        // 5274 Exit
                        }
                    }
                }
            }
        }
        return Result;
    }

    // ================================================================================
    // 5300-6299  自动吃药 / 保护 / 持久警告 / 伤害喝药
    // ⏳ 骨架：方法签名与门禁已就位，逐行体见报告 §未完成/阻塞项
    // ================================================================================

    /// <summary>原文 5300-5395：<c>AutoEatHPItem</c>（自动喝 HP 药）。</summary>
    public void AutoEatHPItem(object Sender) => NotPorted(nameof(AutoEatHPItem), 5300);

    /// <summary>原文 5396-5495：<c>AutoEatMPItem</c>。</summary>
    public void AutoEatMPItem(object Sender) => NotPorted(nameof(AutoEatMPItem), 5396);

    /// <summary>原文 5496-5592：<c>AutoEatSpecialHPItem</c>。</summary>
    public void AutoEatSpecialHPItem(object Sender) => NotPorted(nameof(AutoEatSpecialHPItem), 5496);

    /// <summary>原文 5593-5692：<c>AutoEatSpecialMPItem</c>。</summary>
    public void AutoEatSpecialMPItem(object Sender) => NotPorted(nameof(AutoEatSpecialMPItem), 5593);

    /// <summary>原文 5693-5966：<c>AutoProtect</c>（保护物品：HP/MP/特殊/持久 → 传送/小退）。</summary>
    public void AutoProtect(object Sender) => NotPorted(nameof(AutoProtect), 5693);

    /// <summary>原文 5967-6080：<c>DuraWarning</c>（持久警告：8 组装备位 × 持久阈值）。</summary>
    public void DuraWarning() => NotPorted(nameof(DuraWarning), 5967);

    /// <summary>
    /// 原文 6081-6098：<c>NumberSort_1(List:TStringList; Index1, Index2:Integer):Integer</c>
    /// （单元级排序器 —— 按 TStringList 里的**数值**升序；原文用 <c>StrToIntDef</c>）。
    /// </summary>
    public static int NumberSort_1(List<string> List, int Index1, int Index2)
    {
        // 原文逐字：
        //   Result := StrToIntDef(List[Index1], 0) - StrToIntDef(List[Index2], 0);
        return MirConfigGlobalSeam.StrToIntDef(Safe(List, Index1), 0)
             - MirConfigGlobalSeam.StrToIntDef(Safe(List, Index2), 0);
    }

    private static string Safe(List<string> list, int i)
        => list != null && i >= 0 && i < list.Count ? list[i] : "";

    /// <summary>原文 6099-6204：<c>DamageHPUseItem(nObj, nDamage)</c>。</summary>
    public void DamageHPUseItem(int nObj, int nDamage) => NotPorted(nameof(DamageHPUseItem), 6099);

    /// <summary>原文 6205-6299：<c>DamageMPUseItem(nObj, nDamage)</c>。</summary>
    public void DamageMPUseItem(int nObj, int nDamage) => NotPorted(nameof(DamageMPUseItem), 6205);

    // ================================================================================
    // 基类抽象面补齐
    // ================================================================================

    /// <summary>
    /// 原文 569 <c>FConfigCheckeds:array[TConfigChecked] of Boolean</c> 的对外读取面。
    /// 托管侧 <c>TGameConfigObject.ConfigCheckeds</c> 是抽象 <c>bool[]</c>
    /// （C# 不允许命名索引器，见 GameConfigDlgs.cs 的说明），此处返回同一数组实例。
    /// </summary>
    public override bool[] ConfigCheckeds => FConfigCheckeds;

    /// <summary>
    /// 原文 3851-4534：<c>LoadClientConfig(ClientConfig:pTClientConfig)</c>。
    /// 形参按基线类型（<c>Seams.TClientConfig</c>）声明以满足 override 合同，
    /// 值搬进 <see cref="MirClientConfigFull"/>（原文 <c>FClientConfig := ClientConfig^;</c>）。
    /// ⏳ 体（页签可见性重排 1500 余行）见报告 §未完成/阻塞项。
    /// </summary>
    public override void LoadClientConfig(TClientConfig ClientConfig)
    {
        FClientConfig = MirClientConfigFull.From(ClientConfig);   // 3858
        NotPorted(nameof(LoadClientConfig), 3859);
    }

    /// <summary>原文 1424-1430：<c>Logout</c>（FLoadConfig/FEnabled 清零 + 无条件 SaveConfigFile）。</summary>
    public override void Logout()
    {
        FLoadConfig = false;                                   // 1426
        FEnabled = false;                                      // 1427
        SaveConfigFile();                                      // 1428
    }
}

using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 副本地图（FB）怪物刷新的 **`MonGenInfo` 副本共享语义** 1:1 移植（批次J110），
/// 主源 `LocalDB.pas` 3600-3641（副本分支与 `MonGenInfo2` 副本构造），
/// 另含生命周期的两端：`Envir.pas` 3687-3691（释放）与 `NpcActionCmd.pas` 23189-23194（消费）。
///
/// J109 移植了主加载流程与排序查找，本批次补上 `LoadMonGen` 中唯一的分支——
/// 当 `sMapName` 以 `$` 开头时走**副本地图**路径（3600-3641），
/// 与普通地图路径（3642-3673）在**六处**上不同：
///
/// | | 副本路径（3600-3641） | 普通路径（3642-3673） |
/// |---|---|---|
/// | 地图查找 | `g_FBMapManager.IndexOf`（只是一个模板名） | `g_MapManager.GetMapInfo` |
/// | `boNoManNoMon` | **False**（3606） | **True**（3650） |
/// | `boFB` | **True**（3607） | **False**（3652） |
/// | `Envir` | **nil**（3608，之后由副本逐一覆盖） | `FindMap` 结果（3651） |
/// | `nRace` | 需 `GetMonRace` ≠ **-1** 才加入（3609-3610） | 不查 |
/// | 计数登记 | 无 | `AddMapMonGenCount`（3669） |
///
/// **本批次的核心是 `MonGenInfo2` 的"浅拷贝 + 共享 `CertList`"语义（3630-3631）**：
/// ```
/// New(MonGenInfo2);
/// MonGenInfo2^ := MonGenInfo^;      // 整记录值拷贝
/// MonGenInfo2.boFB := True;
/// MonGenInfo2.sMapName := Envir.sMapName;
/// MonGenInfo2.Envir := Envir;
/// Envir.m_FBMonGenList.Add(MonGenInfo2);
/// ```
/// 原文 3630 的注释写得很明确：**`MonGenInfo2.CertList` 与 `MonGenInfo.CertList` 是同一个
/// `TList` 对象，副本只引用指针、不管理对象**。故：
/// 1. **多个副本地图共享同一份怪物凭证表**——在任一副本里刷出的怪，其凭证都进同一个 `CertList`；
/// 2. **`MonGenInfo2` 的 `Dispose` 不得释放 `CertList`**——
///    这正是 `Envir.pas` 3689 只用 `Dispose(pTMonGenInfo(...))` 而**不**调 `CertList.Free` 的原因。
///    若在此处释放 `CertList`，**原 `MonGenInfo` 与其余副本会持有悬空指针**，
///    症状是副本里刷怪时崩溃、或凭证表被清空导致怪物数量错乱，且**同一份配置在
///    "只有一个副本"时不复现**——因为那时只有一个持有者，崩溃点在别处才暴露。
/// 3. **`MonGenInfo`（原件）自身仍归 `m_MonGenList` 管理**，并在 3619 就被加入该表，
///    故它在整个 `LoadMonGen` 期间的生命周期由 `m_MonGenList` 决定，
///    而副本只是**额外引用**。
///
/// **注意加入顺序（3619 vs 3635）**：原件先加入 `m_MonGenList`（3619），
/// 随后才为每个副本地图各建一个副本加入 `Envir.m_FBMonGenList`（3635）。
/// 故副本总是晚于原件登记；若 `GetMonRace` 返回 -1（3610），
/// 则**既不建 `CertList`（3612）也不加入任何表**，只 `DisPose` 掉（3639）——
/// 即"怪物名不存在"的副本模板被整体丢弃。
///
/// **副本的 `sMapName` 被改写为具体地图名**（3633 `Envir.sMapName`），
/// 而原件保留带 `$` 前缀的模板名——故 J109 的按地图名排序/查找中，
/// 副本以**真实地图名**参与，而模板名（带 `$`）只出现在原件上。
/// </summary>
public static class MonGenFbCopyCore
{
    /// <summary>3600：副本模板名前缀。</summary>
    public const char FbMapNamePrefix = '$';

    /// <summary>3610：`GetMonRace` 的"未找到"返回值。</summary>
    public const int MonRaceNotFound = -1;

    /// <summary>3600：`sMapName[1] = '$'` —— 是否为副本模板行。</summary>
    public static bool IsFbTemplateMapName(string mapName)
        => mapName.Length > 0 && mapName[0] == FbMapNamePrefix;

    /// <summary>3602：剥掉 `$` 前缀得到供 `g_FBMapManager` 查找的名字。</summary>
    public static string StripFbPrefix(string mapName)
        => IsFbTemplateMapName(mapName) ? mapName.Substring(1) : mapName;

    /// <summary>3604：`IndexOf` 未找到。</summary>
    public const int IndexNotFound = -1;

    /// <summary>
    /// 3610：怪物名在 `GetMonRace` 中是否存在——
    /// 不存在则整个模板（含 `CertList` 创建）被跳过并 `DisPose`（3639）。
    /// </summary>
    public static bool ShouldRegisterFbTemplate(int monRace) => monRace != MonRaceNotFound;

    /// <summary>3606：副本路径下 `boNoManNoMon` 恒为 **False**（与普通路径的 True 相反）。</summary>
    public const bool FbNoManNoMon = false;

    /// <summary>3607：副本路径下 `boFB` 恒为 **True**。</summary>
    public const bool FbBoFB = true;

    /// <summary>3608：副本路径下原件 `Envir` 初值为 **nil**。</summary>
    public static readonly object? FbTemplateEnvir = null;

    /// <summary>3650：普通路径下 `boNoManNoMon` 恒为 **True**。</summary>
    public const bool NormalNoManNoMon = true;

    /// <summary>3652：普通路径下 `boFB` 恒为 **False**。</summary>
    public const bool NormalBoFB = false;

    /// <summary>
    /// `TMonGenInfo` 中会被副本改写的字段之外，**必须共享**的部分。
    /// 本移植用一个显式的"共享引用"载体表达原文的指针共享语义。
    /// </summary>
    public sealed class MonGenInfo
    {
        public string MapName = "";
        public string MonName = "";
        public int X;
        public int Y;
        public int Range;
        public int Count;
        public int ZenTimeMs;
        public int MonRace;

        public bool BoNoManNoMon;
        public bool BoFB;

        /// <summary>副本路径下原件为 `null`；普通路径为实际地图对象。</summary>
        public object? Envir;

        /// <summary>
        /// **怪物凭证表**。原文为 `TList` 指针；
        /// 副本（`MonGenInfo2`）与原件**共享同一个实例**（3630 注释）。
        /// 本移植用同一 `List` 引用表达该共享。
        /// </summary>
        public List<object>? CertList;

        /// <summary>副本专属：指向其所属副本地图。</summary>
        public bool IsFbCopy;
    }

    /// <summary>
    /// 3630-3635：为一个副本地图构造 `MonGenInfo2`（整记录值拷贝 + 三处改写）。
    /// **`CertList` 按引用共享**——返回的副本与原件的 `CertList` 是同一实例。
    /// </summary>
    public static MonGenInfo CreateFbCopy(MonGenInfo template, string envirMapName, object envir)
    {
        // 3631：MonGenInfo2^ := MonGenInfo^  （整记录值拷贝）
        var copy = new MonGenInfo
        {
            MapName = template.MapName,
            MonName = template.MonName,
            X = template.X,
            Y = template.Y,
            Range = template.Range,
            Count = template.Count,
            ZenTimeMs = template.ZenTimeMs,
            MonRace = template.MonRace,
            BoNoManNoMon = template.BoNoManNoMon,
            BoFB = template.BoFB,
            Envir = template.Envir,

            // **共享**：同一个 List 实例，而非拷贝内容
            CertList = template.CertList,
        };

        // 3632-3634
        copy.BoFB = true;
        copy.MapName = envirMapName;
        copy.Envir = envir;

        copy.IsFbCopy = true;

        return copy;
    }

    /// <summary>
    /// 3630-3635：为 `fbList` 中**每个**副本地图各建一个副本。
    /// 返回新建的副本列表（顺序与 `fbMapNames` 一致）。
    /// </summary>
    public static List<MonGenInfo> CreateFbCopies(
        MonGenInfo template, IReadOnlyList<(string MapName, object Envir)> fbMaps)
    {
        var result = new List<MonGenInfo>(fbMaps.Count);

        foreach (var (mapName, envir) in fbMaps)
            result.Add(CreateFbCopy(template, mapName, envir));

        return result;
    }

    /// <summary>
    /// 3630 注释的语义断言：副本与原件**共享同一个 `CertList` 实例**。
    /// </summary>
    public static bool SharesCertList(MonGenInfo template, MonGenInfo copy)
        => ReferenceEquals(template.CertList, copy.CertList);

    /// <summary>
    /// `Envir.pas` 3689：释放副本时**只释放记录本身**，
    /// **不得**释放 `CertList`——否则原件与其余副本会持有悬空指针。
    /// 本方法返回"应当释放 `CertList` 吗"，答案恒为 `false`。
    /// </summary>
    public static bool ShouldDisposeCertListOfFbCopy() => false;

    /// <summary>
    /// `Envir.pas` 3687-3691：释放整个 `m_FBMonGenList`。
    /// 逐个 `Dispose` 副本记录，但**不碰** `CertList`；最后释放列表本身。
    /// </summary>
    public static void DisposeFbMonGenList<T>(IList<T> fbMonGenList, Action<T> disposeRecord)
    {
        for (int i = 0; i < fbMonGenList.Count; i++)
            disposeRecord(fbMonGenList[i]);
    }

    /// <summary>
    /// 3600-3641 的整体决策：副本模板行该如何处置。
    /// </summary>
    public enum FbTemplateAction
    {
        /// <summary>3600 不成立：走普通地图路径（3642-3673）。</summary>
        NotFbTemplate,

        /// <summary>3604：`IndexOf` 返回 -1，整个模板被丢弃（**不 `DisPose` 之外无动作**，原文 3640 什么都不做）。</summary>
        ManagerNotFound,

        /// <summary>3610：怪物名不存在 → `DisPose`（3639）。</summary>
        RaceNotFound,

        /// <summary>3612-3635：建 `CertList`、登记原件、为每个副本地图建副本。</summary>
        RegisterAndCreateCopies,
    }

    /// <summary>
    /// 3600-3641：副本模板行的处置决策。**顺序为原文顺序，短路判定**。
    /// **注意 3604 的 `IndexOf` 返回 -1 时原文什么都不做**（没有 else 分支，3640-3641 直接结束），
    /// 即该模板**既不入表也不报错**——已用 `ManagerNotFound` 单列以固化这一"静默丢弃"。
    /// </summary>
    public static FbTemplateAction SelectFbTemplateAction(
        string mapName, int fbManagerIndex, int monRace)
    {
        if (!IsFbTemplateMapName(mapName))
            return FbTemplateAction.NotFbTemplate;

        if (fbManagerIndex == IndexNotFound)
            return FbTemplateAction.ManagerNotFound;

        if (!ShouldRegisterFbTemplate(monRace))
            return FbTemplateAction.RaceNotFound;

        return FbTemplateAction.RegisterAndCreateCopies;
    }

    /// <summary>
    /// 3619 vs 3635：登记顺序——原件先进 `m_MonGenList`，副本后进 `m_FBMonGenList`。
    /// 返回固定的登记次序标签，供测试固化"副本总是晚于原件"。
    /// </summary>
    public static readonly string[] RegistrationOrder = { "m_MonGenList(原件)", "m_FBMonGenList(副本)" };

    /// <summary>
    /// 3612：`CertList` 只在 `nRace ≠ -1` 的分支内创建；
    /// 故 `RaceNotFound` 时模板**根本没有凭证表**，也就无所谓共享。
    /// </summary>
    public static bool CreatesCertList(FbTemplateAction action)
        => action == FbTemplateAction.RegisterAndCreateCopies;
}

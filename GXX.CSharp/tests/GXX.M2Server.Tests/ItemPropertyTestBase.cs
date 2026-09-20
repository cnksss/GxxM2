using GXX.M2Server.Engine;
using GXX.M2Server.Forms.ItemProperty;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc：uFrmCustomItemProperty.pas 窗体与接缝层的**测试基类**。
///
/// <para>
/// 集合 <c>ItemPropertyLane</c> 声明 <c>DisableParallelization = true</c>：本车道的被测代码大量读写
/// 静态接缝量（CustomItemPropertyGlobals / M2ShareState.ConfigIni），按《并行派发台账》§19.5
/// 「测试间静态全局污染」的既定处置，与其它集合并行时会互相踩，故串行。
/// </para>
/// <para>照搬 p5-m2-custommagic 车道 CustomMagicTestBase.cs 的做法。</para>
/// </summary>
[CollectionDefinition("ItemPropertyLane", DisableParallelization = true)]
public sealed class ItemPropertyLaneCollection
{
}

/// <summary>
/// 窗体测试基类：注入 60 项接缝数组 / 文本列表 / 4 个委托，并把 Config INI 指向临时目录。
/// </summary>
public abstract class ItemPropertyTestBase : IDisposable
{
    /// <summary>本次用例的临时目录（Config !Setup.txt 落盘处）。</summary>
    protected readonly string Dir;

    /// <summary>原文 g_CustomItemPropertyChecks（下标 1..60；下标 0 不用）。</summary>
    protected readonly bool[] Checks = new bool[61];

    /// <summary>原文 g_CustomItemPropertyBindNames（下标 1..60；下标 0 不用）。</summary>
    protected readonly string[] BindNames = new string[61];

    /// <summary>原文 g_CustomItemPropertyTextVarList。</summary>
    protected readonly GXX.Core.Util.TStringList TextVarList = new();

    /// <summary>RebuildCustomItemPropertyConfig 调用次数。</summary>
    protected int RebuildCalls;

    /// <summary>SaveCustomItemPropertyTextVarList 调用次数。</summary>
    protected int SaveCalls;

    /// <summary>UserEngine.SendCustomItemPropertyConfig 调用次数。</summary>
    protected int SendConfigCalls;

    /// <summary>UserEngine.SendCustomItemPropertyTextVarList 调用次数。</summary>
    protected int SendTextVarListCalls;

    protected ItemPropertyTestBase()
    {
        Dir = Path.Combine(Path.GetTempPath(), "p8itemprop_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Dir);
        M2ShareState.ResetForTests(Dir);
        CustomItemPropertyGlobals.Reset();
        CustomItemPropertyMessageBoxSeam.UiEnabled = false;      // ★ 无头：否则 testhost 挂死

        for (int i = 0; i < BindNames.Length; i++)
            BindNames[i] = "";

        CustomItemPropertyGlobals.g_CustomItemPropertyChecks = Checks;
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames = BindNames;
        CustomItemPropertyGlobals.g_CustomItemPropertyTextVarList = TextVarList;
        CustomItemPropertyGlobals.RebuildCustomItemPropertyConfig = () => RebuildCalls++;
        CustomItemPropertyGlobals.SaveCustomItemPropertyTextVarList = () => SaveCalls++;
        CustomItemPropertyGlobals.SendCustomItemPropertyConfig = () => SendConfigCalls++;
        CustomItemPropertyGlobals.SendCustomItemPropertyTextVarList = () => SendTextVarListCalls++;
    }

    /// <summary>按原文 M2Share.pas:4054 的 typed const 默认表（60 项全 True）填充。</summary>
    protected void FillChecksAllTrue()
    {
        for (int i = 1; i <= 60; i++)
            Checks[i] = true;
    }

    /// <summary>给 60 个名称填可区分值（"名称NN"），用于捕捉下标错位。</summary>
    protected void FillBindNamesDistinct()
    {
        for (int i = 1; i <= 60; i++)
            BindNames[i] = "名称" + i.ToString("00");
    }

    public void Dispose()
    {
        CustomItemPropertyGlobals.Reset();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(Dir, true); } catch { /* 临时目录清理失败不影响门禁 */ }
    }
}

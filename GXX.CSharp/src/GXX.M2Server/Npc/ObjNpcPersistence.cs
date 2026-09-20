// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：**脚本/图标装载**方法族 1:1 移植（TNormNpc 与 TMerchant 共 4 个方法）。
//   · TNormNpc.LoadNpcScript      9575-9591
//   · TNormNpc.LoadNpcIconFile    9593-9602
//   · TMerchant.LoadNpcScript     3180-3204
//   · TMerchant.LoadNpcIconFile   3206-3226
//
// 全部走 FrmDB（LocalDB.pas 全局，整单元未移植）读盘 → 经 NpcSeams 转发；
// 值得 1:1 照抄的是**文件名的分支矩阵**：`IsAddMapName` × `m_boFB` 四组合，
// 其中 `IsAddMapName = False` 的两支**都返回 `m_sScript`**（原文 3197/3199 与 3221/3223
// 两个分支体完全相同 —— 原文冗余，照抄保留）。
//
// `FrmDB.LoadIconFile` 原文第三参是 `@m_ActorIcons`（TBaseObject 的图标数组，托管侧
// Engine.TCreature 尚无该字段）—— 接缝签名把这层吞掉，见 NpcSeams.LoadIconFile 的说明。
// ============================================================================

using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TNormNpc
{
    /// <summary>
    /// 原文 `procedure LoadNpcScript;`（ObjNpc.pas:9575-9591）。
    /// <para>`m_boIsQuest` 为真：`m_sPath := sNpc_def`（不是 `m_sFilePath`！）、
    /// 文件名 `m_sCharName + '-' + m_sMapName`，并且**额外**装载一次图标；
    /// 为假：`m_sPath := m_sFilePath`、文件名就是 `m_sCharName`，**不装图标**。</para>
    /// </summary>
    public void LoadNpcScript()
    {
        if (m_boIsQuest)
        {
            m_sPath = NpcSeams.sNpc_def;
            string s08 = m_sCharName + '-' + m_sMapName;
            NpcSeams.LoadNpcScriptFile(this, m_sFilePath, s08);
            NpcSeams.LoadIconFile(this, NpcSeams.sNpcIcons, s08);
        }
        else
        {
            m_sPath = m_sFilePath;
            NpcSeams.LoadNpcScriptFile(this, m_sFilePath, m_sCharName);
        }
    }

    /// <summary>
    /// 原文 `procedure LoadNpcIconFile;`（ObjNpc.pas:9593-9602）。
    /// <para><b>照抄的原文细节</b>：`if m_boIsQuest` 之后**没有 else 分支** ——
    /// `m_boIsQuest` 为假时本过程是空操作（与 <see cref="LoadNpcScript"/> 的"为假时也装一次"不同，勿混）。</para>
    /// </summary>
    public void LoadNpcIconFile()
    {
        if (m_boIsQuest)
        {
            string s08 = m_sCharName + '-' + m_sMapName;
            NpcSeams.LoadIconFile(this, NpcSeams.sNpcIcons, s08);
        }
    }
}

public partial class TMerchant
{
    /// <summary>
    /// 原文 `procedure LoadNpcScript(IsAddMapName: Boolean = True);`（ObjNpc.pas:3180-3204）。
    /// <para><b>与 TNormNpc 版的差异（照抄）</b>：先 `m_ItemTypeList.Clear`；`m_sPath := sMarket_Def`（不是 sNpc_def）；
    /// 用 `FrmDB.LoadScriptFile`（带 `True`）而非 `LoadNpcScript`；且 `IsAddMapName = False` 时
    /// **两个 `m_boFB` 分支体完全相同**（都是 `SC := m_sScript`，原文 3197/3199 冗余）。</para>
    /// </summary>
    public void LoadNpcScript(bool IsAddMapName = true)
    {
        string SC;
        m_ItemTypeList.Clear();
        m_sPath = NpcSeams.sMarket_Def;
        // 副本地图 -- 副本地图NPC对应脚本 chongchong 2013-09-11
        if (IsAddMapName)
        {
            if (m_boFB)
                SC = m_sScript + '-' + m_sFBName;
            else
                SC = m_sScript + '-' + m_sMapName;
        }
        else
        {
            if (m_boFB)
                SC = m_sScript;
            else
                SC = m_sScript;
        }
        NpcSeams.LoadScriptFile(this, NpcSeams.sMarket_Def, SC);
        NpcSeams.LoadIconFile(this, NpcSeams.sNpcIcons, SC);
        // 原文 3203：`// call    sub_49ABE0` 注释保留
    }

    /// <summary>
    /// 原文 `procedure LoadNpcIconFile(IsAddMapName: Boolean = True);`（ObjNpc.pas:3206-3226）。
    /// 文件名矩阵与 <see cref="LoadNpcScript"/> 完全一致（含 `IsAddMapName = False` 的冗余双分支），
    /// 但**不碰 `m_ItemTypeList` / `m_sPath`**，只装图标。原文 3221/3223 冗余照抄。
    /// </summary>
    public void LoadNpcIconFile(bool IsAddMapName = true)
    {
        string SC;
        // 副本地图 -- 副本地图NPC对应脚本 chongchong 2013-09-11
        if (IsAddMapName)
        {
            if (m_boFB)
                SC = m_sScript + '-' + m_sFBName;
            else
                SC = m_sScript + '-' + m_sMapName;
        }
        else
        {
            if (m_boFB)
                SC = m_sScript;
            else
                SC = m_sScript;
        }
        NpcSeams.LoadIconFile(this, NpcSeams.sNpcIcons, SC);
    }
}

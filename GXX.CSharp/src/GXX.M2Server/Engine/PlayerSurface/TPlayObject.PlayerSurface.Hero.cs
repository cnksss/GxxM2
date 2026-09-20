// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK，49,232 LF）
// 本文件：**英雄 / 副将片**（切片 4 / 4）。
// 覆盖原文行号范围：
//   ObjPlayer.pas:372   m_MyHero: TBaseObject;
//   ObjPlayer.pas:374   m_sHeroName: string;         // 主英雄名
//   ObjPlayer.pas:375   m_sTempHeroName: string;
//   ObjPlayer.pas:380   m_sDeputyHeroName: string;   // 副将英雄名字
//   ObjPlayer.pas:275   m_boWaitHeroDate: Boolean;
//   ObjPlayer.pas:1698-1702 ClearData 内的初始化
//   （原文其余英雄字段 376-379/381-... 依赖 ObjHero.pas 的 THeroObject，未覆盖，见报告）
//
// ⚠ 不重复声明的既有成员（git grep 证据见交付报告）：
//   · m_MyGamePet → Engine/RecalcBonus.cs:248
// ============================================================================

namespace GXX.M2Server.Engine;

/// <summary>
/// ObjPlayer.pas 的**英雄 / 副将**面。
/// </summary>
public partial class TPlayObject
{
    /// <summary>
    /// 原文 `m_MyHero: TBaseObject;`（ObjPlayer.pas:372）——当前召唤的主英雄。
    /// 托管侧 `THeroObject`（ObjHero.pas）与 `TBaseObject` 都未切出，用最薄的
    /// <see cref="TCreature"/> 代表（同 ObjNpc 车道报告 §6.3 的口径）。
    /// ⚠ 原文在 :1698 初始化为 `nil`。**注意类型**：原文声明是 `TBaseObject` 而**不是**
    /// `THeroObject`（使用时到处 `THeroObject(m_MyHero)` 强转，见 :2700/:2713/:2728）。
    /// </summary>
    public TCreature? m_MyHero;

    /// <summary>原文 `m_sHeroName: string; // [ACTORNAMELEN]; //主英雄名`（ObjPlayer.pas:374）。</summary>
    public string m_sHeroName = "";

    /// <summary>原文 `m_sTempHeroName: string; // [ACTORNAMELEN];`（ObjPlayer.pas:375）。</summary>
    public string m_sTempHeroName = "";

    /// <summary>原文 `m_sDeputyHeroName: string; // [ACTORNAMELEN]; //副将英雄名字`（ObjPlayer.pas:380）。</summary>
    public string m_sDeputyHeroName = "";

    /// <summary>
    /// 原文 `m_boWaitHeroDate: Boolean;`（ObjPlayer.pas:275）——「等待英雄数据（LoadHeroData）」标志。
    /// 用途见 :34523 / :34636 / :34717 / :35949 / :36327 / :36574：为 True 时**禁止**召回/锁定英雄。
    /// </summary>
    public bool m_boWaitHeroDate;
}

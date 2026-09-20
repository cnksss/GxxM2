// ObjPlayer.pas 的三个脚本会话字段 1:1 补齐（集成方代补，2026-09-21）
//
// 背景：车道 p4-m2-objnpc 报出 `TNormNpc.Click`(ObjNpc.pas:4431) 被这三个字段阻塞，
// 而它们在托管侧**只存在于** `Engine.TScriptPlayer`（`NpcScriptState.cs:31/32`，且缺第三个），
// `TPlayObject` 上没有。回读原文确认它们**确实是 `TPlayObject` 的成员**：
//
//   ObjPlayer.pas:149  m_nScriptGotoCount: Integer;    // 脚本跳转计数（防死循环）
//   ObjPlayer.pas:356  m_sRandomString: string;        // RANDOMNO 抽签结果
//   ObjPlayer.pas:357  m_sNpcSelectItemName: string;   // NPC 选物名
//
// 初始化点（原文，供比对）：
//   :1647 / :2816 / :2820  m_nScriptGotoCount := 0;
//   :1781                  m_sRandomString := '';
//   :1782                  m_sNpcSelectItemName := '';
//   —— C# 的字段默认值恰好等价（int→0、string 此处显式给 "" 以与原文的 ShortString 空串语义一致，
//      注意本类其它 string 数组成员的默认值由 `PlayerSurfaceVarDefaults` 在构造函数里归一）。
//
// 放在 `Engine/PlayerSurface/` 下用 `partial` 追加，**不改任何既有文件**（见台账 §19.6/§22.1）。
// 补齐后 ObjNpc 车道的 `TNormNpc.Click` 可从"虚外壳"升级为真实现，
// 进而使 `TMerchant.Click`/`TGuildOfficial.Click`/`TCastleOfficial.Click` 三处覆写的
// `inherited` 真正落到原文逻辑。

namespace GXX.M2Server.Engine;

public partial class TPlayObject
{
    /// <summary>原文 ObjPlayer.pas:149 `m_nScriptGotoCount: Integer`（脚本跳转计数，防死循环）。</summary>
    public int m_nScriptGotoCount;

    /// <summary>原文 ObjPlayer.pas:356 `m_sRandomString: string`（RANDOMNO 抽签结果；原文 :1781 置 ''）。</summary>
    public string m_sRandomString = "";

    /// <summary>原文 ObjPlayer.pas:357 `m_sNpcSelectItemName: string`（NPC 选物名；原文 :1782 置 ''）。</summary>
    public string m_sNpcSelectItemName = "";
}

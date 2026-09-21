// ============================================================================
// 源单元：Source/M2Engine/ItemEvent.pas（1:1 移植，389 行 / 14 个过程函数）
//
// 「文件头即证据」：本注释是 tools/audit-coverage.ps1 的 E2 规则用到的 header 提及；
// 同目录另有 `ItemEvent.cs`（文件名与源单元同名 ⇒ E1）承载类主体，本文件承载
//   · ItemEvent.pas 的**结构体/类本体**（TItemObject / TItemManager）
//   · 以及它所必需的 ObjGame.pas 基类 TGameObject（见下）
//
// 【为什么本文件里有 TGameObject】
//   ItemEvent.pas:9 写的是 `TItemObject = class(TGameObject)`，而 `TGameObject`
//   定义在 ObjGame.pas:9-17（4 个字段 + Create/Destroy）。托管侧**没有这个类**
//   （`git grep -n "class TGameObject" -- GXX.CSharp/src` 零命中；既有的
//   `GXX.M2Server.IGameObject`（MapObjectScanCore.cs:24）只是 `m_ObjGame { get; }` 只读接口，
//   **无任何实现类**，也不构成类层次）。要让 TItemObject 的基类关系 1:1（台账 §18.8
//   「虚分派必须保留」要求层次真实存在，而不是把基类字段平铺进子类），必须补上它。
//   本车道只补 **TGameObject 本身**（ObjGame.pas:9-17、51-62），
//   **不**顺手移植同单元的 TGateObject/TDoorObject（与本三单元无关，且已被别处以接缝表达）。
//   这是一个**有意登记的跨单元依赖**（报告 D-P9-01）。
// ============================================================================

using System;
using GXX.Core.Protocol;

namespace GXX.M2Server.Sweep9.DataLayer;

/// <summary>
/// ObjGame.pas:9-17 的 <c>TGameObject</c> 1:1（字段名与顺序逐字保留）。
/// <para>ObjGame.pas:51-57 的构造：`m_ObjGame := Obj_None; m_dwAddTime := MyGetTickCount;
/// m_nMapX := 0; m_nMapY := 0;`；:59-62 的析构只有 `inherited`（无实体）⇒ 托管侧为空析构。</para>
/// <para>`m_ObjGame` 取既有 <see cref="TObjGame"/>（MapObjectScanCore.cs:9）；`m_dwAddTime` 是
/// `LongWord` ⇒ `uint`。</para>
/// </summary>
public class TGameObject
{
    /// <summary>原文 `m_ObjGame: TObjGame;`（ObjGame.pas:10）。</summary>
    public TObjGame m_ObjGame;

    /// <summary>原文 `m_dwAddTime: LongWord;`（ObjGame.pas:11）。</summary>
    public uint m_dwAddTime;

    /// <summary>原文 `m_nMapX: Integer;`（ObjGame.pas:12）。</summary>
    public int m_nMapX;

    /// <summary>原文 `m_nMapY: Integer;`（ObjGame.pas:13）。</summary>
    public int m_nMapY;

    /// <summary>
    /// 原文 `constructor Create(); virtual`（ObjGame.pas:15/51-57）——
    /// **原文就是 `virtual`**，故托管侧保留 `virtual`（台账 §18.8）。
    /// </summary>
    public TGameObject()
    {
        m_ObjGame = TObjGame.Obj_None;
        m_dwAddTime = Sweep9DataLayerSeam.MyGetTickCount();
        m_nMapX = 0;
        m_nMapY = 0;
    }
}

/// <summary>
/// ItemEvent.pas:9-36 的 <c>TItemObject</c> 1:1。
///
/// <para><b>字段（19 个，逐字保留原文名与类型）</b>：
/// 1-4 个是基类 `TGameObject` 的 `m_ObjGame`/`m_dwAddTime`/`m_nMapX`/`m_nMapY`（见上），
/// 本类另有 `m_wLooks` … `m_dwRunTick` 共 15 个（ItemEvent.pas:10-30）。</para>
///
/// <para><b>虚分派</b>：原文 `constructor Create(); override`（:32）、
/// `destructor Destroy; override`（:33）—— 两处都是 `override`，
/// 托管侧 `TItemObject()` 覆盖 <see cref="TGameObject"/> 的构造语义（`m_ObjGame := Obj_Item` 等），
/// 故基类构造必须是 `virtual`（已保留）。`Run`/`MakeGhost` 原文**不是** virtual（:34/:35），
/// 托管侧同样**不加** `virtual`。</para>
///
/// <para><b>原文缺陷（照抄 + 差异断言锁定，见 Sweep9DataLayerItemEventTests）</b>：
/// ① :102/105/113 同一表达式里 <c>MyGetTickCount</c> 被调用**两次**（判据用旧值、赋值用新值）；
/// ② :102 的注释 `{60 * 60 * 1000}` 与实际读的 `g_Config.dwClearDropOnFloorItemTime` 不一致
///    （注释是过期常量，**不据此改写代码**）；
/// ③ :143 的 `if not DeleteFromMap(...) then begin end;` 是**空 then 块**（返回值被丢弃）；
/// ④ :220-221 的多行布尔表达式在断行处被 Delpi 解析为连续条件（无运算符丢失）；
/// ⑤ `m_boDieDrop`/`m_boHumDrop`/`m_boNpcThrowItem`（:25-27）在**整个单元内零引用**
///    （构造不初始化、无任何读取点）—— 逐字保留。</para>
/// </summary>
public class TItemObject : TGameObject, IItemGameItem
{
    // ---- ItemEvent.pas:10-30 的 15 个自有字段（名字/顺序逐字保留）----

    /// <summary>原文 `m_wLooks: Word;`（:10）。</summary>
    public ushort m_wLooks;

    /// <summary>原文 `m_wAniCount: Word;`（:11）。</summary>
    public ushort m_wAniCount;

    /// <summary>原文 `m_btReserved: Byte;`（:12）——`// 队友能不能捡 0队友可以捡  1队友不能捡`（:13）。</summary>
    public byte m_btReserved;

    /// <summary>原文 `m_btColor: Byte;`（:14）。</summary>
    public byte m_btColor;

    /// <summary>原文 `m_nCount: Integer;`（:15）。</summary>
    public int m_nCount;

    /// <summary>原文 `m_OfBaseObject: TObject;`（:16）——`// 哪个能捡`。</summary>
    public object? m_OfBaseObject;

    /// <summary>原文 `m_DropBaseObject: TObject;`（:17）——`// 哪个掉的`。</summary>
    public object? m_DropBaseObject;

    /// <summary>原文 `m_dwCanPickUpTick: LongWord;`（:18）。</summary>
    public uint m_dwCanPickUpTick;

    /// <summary>原文 `m_dwFloorItemCanPickUpTime: LongWord;`（:19）。</summary>
    public uint m_dwFloorItemCanPickUpTime;

    /// <summary>原文 `m_UserItem: TUserItem;`（:20）。托管侧是既有 packed 值类型（Grobal2.Types6.cs:13）。</summary>
    public TUserItem m_UserItem;

    /// <summary>原文 `m_sName: string;`（:21）。</summary>
    public string m_sName = "";

    /// <summary>原文 `m_sDBName: string;`（:22）。</summary>
    public string m_sDBName = "";

    /// <summary>原文 `m_PEnvir: TObject;`（:23）——原文类型是 `TObject`（**不是** `TEnvirnoment`，故可有非地图对象）。</summary>
    public object? m_PEnvir;

    /// <summary>原文 `m_boGhost: Boolean;`（:24）——`// 0x2FC`。</summary>
    public bool m_boGhost;

    /// <summary>原文 `m_boDieDrop: Boolean;`（:25）——`// 是否死亡掉落`。**单元内零引用（原文如此）**。</summary>
    public bool m_boDieDrop;

    /// <summary>原文 `m_boHumDrop: Boolean;`（:26）——`// 是否人物/英雄掉落`。**单元内零引用（原文如此）**。</summary>
    public bool m_boHumDrop;

    /// <summary>原文 `m_boNpcThrowItem: Boolean;`（:27）——`// Npc命令 ThrowItem搞的物品 2020-04-06 00:29:33`。**单元内零引用（原文如此）**。</summary>
    public bool m_boNpcThrowItem;

    /// <summary>原文 `m_dwGhostTick: LongWord;`（:29）——`// 0x300`。</summary>
    public uint m_dwGhostTick;

    /// <summary>原文 `m_dwRunTick: LongWord;`（:30）——`// 0x300`。</summary>
    public uint m_dwRunTick;

    // ---- 对 IItemGameItem 的**显式**实现（不新增任何原文没有的状态）----
    //
    // Delphi 侧这些是**公有字段**（`m_UserItem`/`m_nMapX`/`m_nMapY`），托管侧已按字段逐字落地；
    // 但 C# 的 interface **不能声明字段**，只能声明属性。为避免"为凑接口把公有字段改成属性"
    // 这种会改变原文形态的改动，这里用**显式接口实现**（`IItemGameItem.m_XXX`）——它只在
    // 通过接口访问时可见，不参与 `TItemObject` 的成员查找，公有字段仍是唯一真源。
    TUserItem IItemGameItem.m_UserItem => m_UserItem;
    int IItemGameItem.m_nMapX => m_nMapX;
    int IItemGameItem.m_nMapY => m_nMapY;

    /// <summary>
    /// ItemEvent.pas:62-83 的 `constructor TItemObject.Create();`（原文 `override`）。
    /// <para>逐行对应：`inherited`(:64) → 基类构造；`m_ObjGame := Obj_Item`(:65)；
    /// `m_sName := ''`(:66)；`m_sDBName := ''`(:67)；`m_wLooks := 0`(:68)；`m_wAniCount := 0`(:69)；
    /// `m_btReserved := 0`(:70)；`m_nCount := 0`(:71)；`m_OfBaseObject := nil`(:72)；
    /// `m_DropBaseObject := nil`(:73)；`m_dwCanPickUpTick := 0`(:74)；`m_PEnvir := nil`(:76)；
    /// `m_boGhost := False`(:77)；`m_dwGhostTick := 0`(:78)；`m_dwRunTick := MyGetTickCount`(:79)；
    /// `m_btColor := 255`(:80)；`FillChar(m_UserItem, SizeOf(TUserItem), #0)`(:81) →
    /// 托管侧值类型 `default(TUserItem)` 等价（等价性由测试用**逐字节**比较取证）；
    /// `m_dwFloorItemCanPickUpTime := g_Config.dwFloorItemCanPickUpTime`(:82)。</para>
    /// <para>**注意原文顺序**：`m_btColor := 255`(:80) 在 `m_dwRunTick`(:79) 之后 —— 逐行保留，
    /// 不"整理"成字段声明序。</para>
    /// </summary>
    public TItemObject()
    {
        m_ObjGame = TObjGame.Obj_Item;
        m_sName = "";
        m_sDBName = "";
        m_wLooks = 0;
        m_wAniCount = 0;
        m_btReserved = 0;
        m_nCount = 0;
        m_OfBaseObject = null;
        m_DropBaseObject = null;
        m_dwCanPickUpTick = 0;

        m_PEnvir = null;
        m_boGhost = false;
        m_dwGhostTick = 0;
        m_dwRunTick = Sweep9DataLayerSeam.MyGetTickCount();
        m_btColor = 255;
        m_UserItem = default;   // FillChar(m_UserItem, SizeOf(TUserItem), #0)（:81）
        m_dwFloorItemCanPickUpTime = Sweep9DataLayerSeam.DwFloorItemCanPickUpTime();
    }

    /// <summary>
    /// ItemEvent.pas:85-93 的 `destructor TItemObject.Destroy;`（原文 `override`）。
    /// <para>原文：若 `m_PEnvir &lt;&gt; nil` 则 `TEnvirnoment(m_PEnvir).DeleteFromMap(m_nMapX, m_nMapY, Self)`
    /// 并把 `m_PEnvir := nil`；随后 `inherited`（无实体）。</para>
    /// <para>托管侧无确定性析构，故提供与原文同名同序的 <see cref="Destroy"/> 显式方法（1:1），
    /// 并由测试直接调用；**不**实现 `IDisposable`（原文析构不是契约，只是释放钩子）。</para>
    /// </summary>
    public void Destroy()
    {
        if (m_PEnvir != null)
        {
            DeleteFromMap(m_nMapX, m_nMapY, this);
            m_PEnvir = null;
        }
    }

    /// <summary>
    /// `TEnvirnoment(m_PEnvir).DeleteFromMap(...)` 的落地口径：
    /// ① 若 `m_PEnvir` 实现了本车道的 <see cref="IItemGameEnvir"/>，走该接口；
    /// ② 否则若它就是既有的 <see cref="GXX.M2Server.Engine.TEnvirnoment"/>，直接转调其实例方法
    ///    （`Engine/Envir.cs:250`，**转调既有实现，不另造**）；
    /// ③ 都不是 ⇒ 抛 <see cref="InvalidCastException"/>，对齐原文硬转换失败的等价行为
    ///    （原文 `TEnvirnoment(非地图对象)` 会读错内存 / AV，托管侧取"响亮失败"）。
    /// </summary>
    private void DeleteFromMap(int nX, int nY, object obj)
    {
        if (m_PEnvir is IItemGameEnvir envir)
        {
            envir.DeleteFromMap(nX, nY, obj);
            return;
        }
        if (m_PEnvir is GXX.M2Server.Engine.TEnvirnoment engineEnvir)
        {
            engineEnvir.DeleteFromMap(nX, nY, obj);
            return;
        }
        throw new InvalidCastException(
            "TItemObject.m_PEnvir 既不是 IItemGameEnvir 也不是 TEnvirnoment（原文 TEnvirnoment(m_PEnvir) 硬转换，原文如此）");
    }

    /// <summary>`TEnvirnoment(m_PEnvir).sMapName`（原文 :150 直接读字段）。</summary>
    private string EnvirMapName()
        => m_PEnvir is IItemGameEnvir e ? e.sMapName
         : m_PEnvir is GXX.M2Server.Engine.TEnvirnoment t ? t.sMapName
         : throw new InvalidCastException(
             "TItemObject.m_PEnvir 既不是 IItemGameEnvir 也不是 TEnvirnoment（原文 TEnvirnoment(m_PEnvir) 硬转换，原文如此）");

    /// <summary>
    /// ItemEvent.pas:95-157 的 `procedure TItemObject.Run();` 1:1。
    /// <para>三个顶层块，逐行对应：
    /// <b>①</b> :100-115 「非幽灵」块：到期（:102）置幽灵 + 记 `m_dwGhostTick`（:104-105）；
    /// 副本地图（`m_boFB and (not m_boFBCreate)`，:110）立即置幽灵（:112-113）。
    /// <b>②</b> :117-138 「仍非幽灵」块：`m_OfBaseObject`/`m_DropBaseObject` 超时可捡期
    /// （:121）则双清（:124-125）；再各自若基对象已幽灵则单独清空（:127-136）。
    /// <b>③</b> :139-156 「已幽灵」块：`m_PEnvir &lt;&gt; nil` 时从地图删除（:143）、
    /// 若标准物品 `NeedIdentify = 1` 则写消失日志（:150-151）、最后 `m_PEnvir := nil`（:154）。</para>
    /// <para><b>原文缺陷 ①（照抄）</b>：:102 与 :105 各调一次 `MyGetTickCount`，:113 同理；
    /// 判据用的是**本块最早那次**的值，赋值用的是**当次新值**。托管侧保留双调用
    /// （测试用「每次调用递增的假时钟」把该差异暴露成可断言的整数差）。</para>
    /// <para><b>原文缺陷 ③（照抄）</b>：:143 的 `if not DeleteFromMap(...) then begin end;`
    /// —— `then` 块为空，**返回值被完全丢弃**，删除失败也照样往下写日志、照样 `m_PEnvir := nil`。
    /// 托管侧同样不检查返回值。</para>
    /// </summary>
    public void Run()
    {
        // ---- ① ItemEvent.pas:100-115 ----
        if (!m_boGhost)
        {
            if ((Sweep9DataLayerSeam.MyGetTickCount() - m_dwAddTime) > Sweep9DataLayerSeam.DwClearDropOnFloorItemTime())
            {
                // 原文 :103-104 注释：{ 删除到期装备 }
                m_boGhost = true;
                m_dwGhostTick = Sweep9DataLayerSeam.MyGetTickCount();
            }

            // 原文 :108 注释：{ 副本地图 -- 清除地面物品 chongchong 2013-09-14 }
            // 原文 :109 是**无保护**硬转换 `TEnvirnoment(m_PEnvir)`：
            // 若 m_PEnvir 为 nil，:110 读 m_boFB 即 AV。托管侧把该"未判空"显式化为
            // NullReferenceException（不是"顺手加个 if nil then skip"——那会偏离 1:1）。
            if (m_PEnvir == null)
                throw new NullReferenceException(
                    "TItemObject.Run: m_PEnvir = nil 时原文 TEnvirnoment(nil).m_boFB 会 AV（原文如此）");
            if (Sweep9DataLayerSeam.EnvirBoFB(m_PEnvir) &&
                !Sweep9DataLayerSeam.EnvirBoFBCreate(m_PEnvir))
            {
                m_boGhost = true;
                m_dwGhostTick = Sweep9DataLayerSeam.MyGetTickCount();
            }
        }

        // ---- ② ItemEvent.pas:117-138 ----
        if (!m_boGhost)
        {
            if ((m_OfBaseObject != null) || (m_DropBaseObject != null))
            {
                if ((Sweep9DataLayerSeam.MyGetTickCount() - m_dwCanPickUpTick) > m_dwFloorItemCanPickUpTime)
                {
                    // 原文 :123 注释：// g_Config.dwFloorItemCanPickUpTime
                    m_OfBaseObject = null;
                    m_DropBaseObject = null;
                }

                if (m_OfBaseObject != null)
                {
                    if (Sweep9DataLayerSeam.IsBaseObjectGhost(m_OfBaseObject))
                        m_OfBaseObject = null;
                }
                if (m_DropBaseObject != null)
                {
                    if (Sweep9DataLayerSeam.IsBaseObjectGhost(m_DropBaseObject))
                        m_DropBaseObject = null;
                }
            }
        }
        // ---- ③ ItemEvent.pas:139-156 ----
        else
        {
            if (m_PEnvir != null)
            {
                // 原文 :143-145：`if not ... then begin end;` —— 空 then 块，返回值被丢弃（原文如此）
                DeleteFromMap(m_nMapX, m_nMapY, this);

                TStdItem? StdItem = Sweep9DataLayerSeam.GetStdItem(m_UserItem.wIndex);
                if ((StdItem != null) && (StdItem.Value.NeedIdentify == 1))
                {
                    Sweep9DataLayerSeam.AddGameDataLog(new ItemEventGameDataLogArgs
                    {
                        LogAction1 = ItemEventLogTypes.LOG_ItemDisappear,
                        LogAction2 = ItemEventLogTypes.LOG_ActionNone,
                        LogActorType = ItemEventLogTypes.latNone,
                        MapName = EnvirMapName(),
                        PointX = m_nMapX,
                        PointY = m_nMapY,
                        ItemName = StdItem.Value.NameStr,
                        ItemMakeIndex = m_UserItem.MakeIndex,
                        ActorName = "0",
                        TargetName = "0",
                        Data1 = 0,
                        Data2 = 0,
                        LogDesc = "到时清理",
                    });
                }

                m_PEnvir = null;
            }
        }
    }

    /// <summary>
    /// ItemEvent.pas:159-164 的 `procedure TItemObject.MakeGhost;` 1:1。
    /// <para>置 `m_boGhost := True`（:161）、`m_dwGhostTick := MyGetTickCount`（:162）；
    /// :163 的 `// m_PEnvir := nil;` 是**被注释掉的行**，逐字保留为注释。</para>
    /// </summary>
    public void MakeGhost()
    {
        m_boGhost = true;
        m_dwGhostTick = Sweep9DataLayerSeam.MyGetTickCount();
        // m_PEnvir := nil;
    }
}

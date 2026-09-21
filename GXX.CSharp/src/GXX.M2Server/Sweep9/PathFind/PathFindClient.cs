// ============================================================================
// 车道 p9-m2-pathfind —— 切片 3
// 源单元：Source/M2Engine/PathFindClient.pas（811 行，
//          镜像 _analysis/utf8_mirror/M2Engine/PathFindClient.pas）
// 1:1 托管移植。
//
// ⚠ 文件名 vs 单元名：原文 `PathFindClient.pas:36` 里写的是 **`unit PathFind;`**
//   （复制自 Common\PathFind.pas 的残留），而磁盘文件名是 `PathFindClient.pas`。
//   工程内**同名单元共有三份**（实测 sha256 前 16 位 / 行数）：
//     Source/Common/PathFind.pas        509 行  3690E569CF132CF5   ← 已由 GXX.Core/Util/PathFind.cs 移植
//     Source/Client-HGE/PathFind.pas    817 行  981A0BADF8EC41FD
//     Source/M2Engine/PathFind.pas      732 行  53127E8E9F5A088F
//     Source/M2Engine/PathFindClient.pas 811 行 4C5E3F24167B2771   ← 本文件
//     Source/M2Engine/PathFind_Hero.pas  693 行 31212A4294A7BC1D   ← 同车道切片 4
//   ⇒ 三者**互不相同**，本文件只对 `PathFindClient.pas` 负责。
//
// ---------------------------------------------------------------------------
// 【与既有 GXX.Core/Util/PathFind.cs（= Common\PathFind.pas）的对照结论】
//   二者**同源但不同单元**：波扩散（Dijkstra 变体）算法与方向布局
//   （7 0 1 / 6 X 2 / 5 4 3）、`DirToDX`/`DirToDY`、`TWave`/`TWaveCell`、
//   `GetCost` 的"先掩码再越界判 -1"骨架完全一致；但**容器类不同形**：
//
//   | PathFindClient.pas（本文件）                  | Common\PathFind.pas（既有 Core 产物）        |
//   |----------------------------------------------|---------------------------------------------|
//   | `TPathMap` 用 `ClientRect: TRect` 定边界       | `TPathMap` 用 `MapWidth/MapHeight` 定边界     |
//   | `FWidth/FHeight` + `SetWidth/SetHeight` + `FRealSize` | 无                                    |
//   | `GetClientRect(X1,Y1,X2,Y2)`（**忽略入参**）    | 无（尺寸由 `FindPath` 的入参直接吸收）        |
//   | `FFillPathMap`/`FFindPathOnMap` 两个互斥标志位 | **无**（是靠 `TLegendMap.FindPath` 自旋等待） |
//   | `FillPathMap` 全程查 `StartFind`（可中途放弃）  | 无 `StartFind` 概念                           |
//   | `FindPathOnMap` 是 **procedure**（写 `FPath`）  | 是 **function**（返回 `TPath`）               |
//   | `WalkToRun()` 把 `FPath` 合成 `FRunPath`（含骑马一步三格支） | **无** WalkToRun              |
//   | `Path`/`RunPath`/`Width`/`Height` 四个属性      | 无                                            |
//   | `TLegendMap` 用 `TMapInfo` 表 + `NewCanWalkEx`  | `TLegendMap` 用 `TCellParams` 表 + `CanWalkEx` + `LoadMap` |
//   | `TFindPathThread`（异步寻路线程）               | 无                                            |
//   | `TerrainParams` 常量表（含 TColor/CellLabel）   | 只有 `PathFindConst.TerrainMoveCost`          |
//
//   ⇒ 按任务书"**只移植差异部分**"：本文件**不重写**波扩散算法本身，
//     而是**复用**既有的 `GXX.Core.Util.TWave` / `TWaveCell` / `TPathMapCell` /
//     `PathPoint`（`TPath` 的元素类型）四个数据容器，只把上表右列"无"的部分
//     按原文 1:1 落下来。差异断言 `TerrainParams_MoveCostMatchesExistingCore` 用
//     既有 `PathFindConst.TerrainMoveCost` **逐项交叉验证** TerrainParams 的 6 个 MoveCost，
//     保证两份产物不会静默分叉。
//
// 【托管侧类型映射】
//   * `TPathMapArray = array of array of TPathMapCell` → `TPathMapCell[][]`（首维=Y、次维=X，与原文一致）
//   * `TPath = array of TPoint` → `PathPoint[]`（复用 Core 的 `PathPoint`）
//   * `TRect`（Windows 单元）→ `System.Drawing.Rectangle`；`Bounds(0,0,W,H)` → `new Rectangle(0,0,W,H)`
//   * `TColor`（Graphics 单元）→ `int`（Delphi TColor 就是 Integer，$00BBGGRR 布局）
//   * `string[16]` → `string`（见偏离 D-P9-06：不强制 16 字节截断）
//
// ⚠ **值语义陷阱（必须在移植里显式处理）**：Delphi 的 `TPoint` 是**记录**，
//   `WalkPath[I] := Path[I]` 是**值拷贝**；托管侧 `PathPoint` 是 **class**（Core 的既有选择），
//   直接赋值会**共享实例** ⇒ 后面 `WalkPath[I-1].x := -1` 会**连带改写 `FPath`**。
//   本文件在每一处"记录拷贝"都显式 `new PathPoint(x, y)`，并有差异断言
//   `WalkToRun_DoesNotAliasInputPath` 把该语义钉死。
//
// 【原文缺陷（照抄 + 差异断言锁定）】
//   ① `TLegendMap.Find` 先 `TFindPathThread.Create(...)`（构造里就 `Resume` 起了线程），
//      **之后**才给 `FExcludeMonster` 赋值 ⇒ 与线程体存在竞态。逐字保留该顺序。
//   ② `TPathMap.FindPathOnMap` 的 `while` 里若 `StartFind` 变假会 `Break`，
//      但**仍然**执行 `FPath[0] := Point(nX,nY)` 与整表 `MapX/MapY` 回填 ⇒
//      中途放弃时 `FPath` 是一个**长度按 Distance 分配、后半段为 null** 的数组。
//   ③ `WalkToRun` 的收集条件是 `(x <> -1) and (y <> -1)`（**与**，不是"或"）⇒
//      坐标为 -1 的点一律被判为"已合并掉"。标记处总是同时置 `x`、`y` 为 -1，故正常路径下
//      与"或"不可区分；但**当 `ClientRect.Left/Top` 为负**时（`MapX/MapY = X + Left`），
//      合法坐标也可能等于 -1 而被**静默丢弃**。差异断言锁死该条件形状。
//   ④ `WalkToRun` 收集时**从下标 1 开始**，起点 `WalkPath[0]` 永不进入 `FRunPath`。
//   ⑤ `TPathMap.SetWidth/SetHeight` 只在值变化时才重算 `FRealSize`，
//      而 `FRealSize` **在本单元内从不被读取**（死字段，照抄保留）。
//   ⑥ `TLegendMap.FindPath(StopX, StopY, ...)` 与
//      `TLegendMap.FindPath(StartX, StartY, StopX, StopY, ...)` 的**前几步顺序不同**
//      （`Inc(FindCount)` 与 `FExcludeMonster := ...` 互换），逐一保留。
//   ⑦ `GetClientRect` 有四个入参但**全部不使用**（函数体只有一句 `ClientRect := Bounds(0,0,FWidth,FHeight)`），
//      那段按 ScopeValue 收窄 ClientRect 的算法**整段被 `{ }` 注释掉**，逐字保留为注释。
// ============================================================================

using System;
using System.Drawing;
using System.Threading;
// 复用既有 GXX.Core 产物的四个数据容器（不重写波扩散算法，见文件头"只移植差异部分"）。
// 这里一律用**别名**而非 `using GXX.Core.Util;`：本单元自己声明了 TPathMap / TCellParams /
// TGetCostFunc 等同名类型，带 using-namespace 会引入难以察觉的解析歧义风险。
using TPathMapCell = GXX.Core.Util.TPathMapCell;
using TWave = GXX.Core.Util.TWave;
using TWaveCell = GXX.Core.Util.TWaveCell;
using PathPoint = GXX.Core.Util.PathPoint;
using PathFindConst = GXX.Core.Util.PathFindConst;
using TerrainType = GXX.Core.Util.TerrainType;

namespace GXX.M2Server.Sweep9.PathFind.Client;

// ---------------------------------------------------------------- 单元级类型

/// <summary>原文 <c>PathFindClient.pas:45 TTerrainTypes</c>（地图元素分类）。</summary>
public enum TTerrainTypes
{
    ttNormal,
    ttSand,
    ttForest,
    ttRoad,
    ttObstacle,
    ttPath,
}

/// <summary>
/// 原文 <c>:46-50 TTerrainParam</c>（record）。
/// <para><c>CellColor: TColor</c>（= Integer，$00BBGGRR）、<c>CellLabel: string[16]</c>、<c>MoveCost: Integer</c>。</para>
/// </summary>
public struct TTerrainParam
{
    public int CellColor;
    public string CellLabel;
    public int MoveCost;
}

/// <summary>
/// 原文 <c>:93 TGetCostFunc = function(X, Y, Direction: Integer; PathWidth: Integer = 0): Integer;</c>。
/// <para>⚠ 与既有 <c>GXX.Core.Util.TGetCostFunc</c>（3 参、Common\PathFind.pas 版）**签名不同**，
/// 故本单元独立声明，不复用。</para>
/// </summary>
public delegate int TGetCostFunc(int X, int Y, int Direction, int PathWidth = 0);

/// <summary>
/// 原文 <c>:62-68 TMapHeader = packed record</c>（52 字节：2+2+17+8+23）。
/// <para><b>本单元未使用</b>（原文只在 Common\PathFind.pas 的 <c>LoadMap</c> 里用），
/// 按"1:1 完整"要求照抄声明，并用 <c>StructBytes.SizeOf</c> 的等价断言锁布局。</para>
/// </summary>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMapHeader
{
    public ushort wWidth;
    public ushort wHeight;

    /// <summary><c>sTitle: string[16]</c> = 1 个长度字节 + 16 字节字符 ⇒ 拆成两个字段。</summary>
    public byte sTitleLen;

    public fixed byte sTitle[16];
    public double UpdateDate;
    public fixed byte Reserved[23];
}

/// <summary>原文 <c>:70-80 TMapInfo = packed record</c>（12 字节）。</summary>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
public struct TMapInfo
{
    public ushort wBkImg;
    public ushort wMidImg;
    public ushort wFrImg;

    /// <summary>原文注释「$80 (巩娄) …」（GBK 原文乱码，逐字保留语义：门索引）。</summary>
    public byte btDoorIndex;

    /// <summary>原文注释「摧腮 巩狼 弊覆狼 惑措 困摹, $80 (凯覆/摧塞(扁夯))」。</summary>
    public byte btDoorOffset;

    /// <summary>原文注释「$80(Draw Alpha) + 橇贰烙 荐」。</summary>
    public byte btAniFrame;

    public byte btAniTick;
    public byte btArea;
    public byte btLight;
}

/// <summary>
/// <c>TMapInfoArray = array of array of TMapInfo</c> → <c>TMapInfo[][]</c>。
/// <para>原文 <c>:83-84</c> 另有 <c>TMapInfoArr = array[0..MaxListSize] of TMapInfo</c> 与
/// <c>pTMapInfoArr</c>（巨型定长数组 + 指针类型，本单元未使用，且 <c>MaxListSize</c> 是 Delphi 私有常量）
/// ⇒ 登记为不移植（D-P9-07）。</para>
/// </summary>

/// <summary>
/// 原文 <c>:87-90 TCellParams = record</c>（<b>本单元未使用</b>；既有
/// <c>GXX.Core.Util.TCellParams</c> 是 Common\PathFind.pas 那份的移植，二者不同单元）。
/// </summary>
public struct TCellParams
{
    public TTerrainTypes TerrainType;
    public bool OnPath;
}

// ------------------------------------------------------------------ TPathMap

/// <summary>
/// 原文 <c>PathFindClient.pas:95-131 TPathMap = class</c>（**客户端版**寻路类）。
/// <para>与既有 <c>GXX.Core.Util.TPathMap</c>（Common 版）的差异见文件头对照表。</para>
/// </summary>
public class TPathMap
{
    // ---- private（原文 private 段；同单元内 TLegendMap 也要访问，见下） ----

    /// <summary>原文 <c>:97 FWidth</c>。</summary>
    private int FWidth;

    /// <summary>原文 <c>:98 FHeight</c>。</summary>
    private int FHeight;

    /// <summary>原文 <c>:99 FRealSize: TRect</c>（**死字段**：只写不读，原文缺陷 ⑤）。</summary>
    private Rectangle FRealSize;

    /// <summary>
    /// 原文 <c>:101 FPath: TPath</c>（private）。
    /// <para>⚠ 原文是 <c>private</c>，但 Delphi 的 private 在**同一单元内**对其它类可见，
    /// 而 <c>TLegendMap</c>（同单元）确实读写它 ⇒ 托管侧落为 <c>protected</c> 以保留同一可见性。</para>
    /// </summary>
    protected PathPoint[] FPath;

    /// <summary>原文 <c>:102 FRunPath: TPath</c>（同上：同单元可见）。</summary>
    protected PathPoint[] FRunPath;

    /// <summary>原文 <c>:103 FFillPathMap</c>（同单元可见：<c>TLegendMap.FindPath</c> 自旋等它）。</summary>
    protected bool FFillPathMap;

    /// <summary>原文 <c>:104 FFindPathOnMap</c>（同单元可见：<c>TLegendMap.FindPath</c> 自旋等它）。</summary>
    protected bool FFindPathOnMap;

    // ---- public ----

    /// <summary>原文 <c>:110 PathMapArray: TPathMapArray</c>（首维 Y、次维 X）。</summary>
    public TPathMapCell[][] PathMapArray = Array.Empty<TPathMapCell[]>();

    /// <summary>原文 <c>:111 GetCostFunc: TGetCostFunc</c>。</summary>
    public TGetCostFunc GetCostFunc;

    /// <summary>原文 <c>:112 PathWidth: Integer</c>（传给 <c>GetCostFunc</c> 的第 4 参）。</summary>
    public int PathWidth;

    /// <summary>原文 <c>:113 ClientRect: TRect</c>（本地坐标系的矩形，Left/Top 一般为 0）。</summary>
    public Rectangle ClientRect;

    /// <summary>原文 <c>:114 ScopeValue: Integer</c>（注释「寻找范围」）。</summary>
    public int ScopeValue;

    /// <summary>原文 <c>:115 StartFind: Boolean</c>（寻路开关；变假即尽快放弃）。</summary>
    public bool StartFind;

    /// <summary>原文 <c>:116-281 constructor TPathMap.Create</c>（<c>ScopeValue := 120</c>；<c>GetCostFunc := nil</c>）。</summary>
    public TPathMap()
    {
        ScopeValue = 120;   // 原文注释「寻路范围」
        GetCostFunc = null;
    }

    /// <summary>原文 <c>:283-290 procedure TPathMap.SetWidth</c>（值不变则不重算 <c>FRealSize</c>）。</summary>
    private void SetWidth(int Value)
    {
        if (FWidth != Value)
        {
            FWidth = Value;
            FRealSize = new Rectangle(0, 0, FWidth, FHeight);   // 原文 Bounds(0, 0, FWidth, FHeight)
        }
    }

    /// <summary>原文 <c>:292-299 procedure TPathMap.SetHeight</c>。</summary>
    private void SetHeight(int Value)
    {
        if (FHeight != Value)
        {
            FHeight = Value;
            FRealSize = new Rectangle(0, 0, FWidth, FHeight);
        }
    }

    /// <summary>原文 <c>:126 property Width: Integer read FWidth write SetWidth</c>。</summary>
    public int Width
    {
        get => FWidth;
        set => SetWidth(value);
    }

    /// <summary>原文 <c>:127 property Height: Integer read FHeight write SetHeight</c>。</summary>
    public int Height
    {
        get => FHeight;
        set => SetHeight(value);
    }

    /// <summary>原文 <c>:124 property Path: TPath read FPath write FPath</c>。</summary>
    public PathPoint[] Path
    {
        get => FPath;
        set => FPath = value;
    }

    /// <summary>原文 <c>:125 property RunPath: TPath read FRunPath write FRunPath</c>。</summary>
    public PathPoint[] RunPath
    {
        get => FRunPath;
        set => FRunPath = value;
    }

    // ---- 方向换算 ----

    /// <summary>
    /// 原文 <c>:308-316 function TPathMap.DirToDX</c>。
    /// <code>
    /// 7  0  1
    /// 6  X  2
    /// 5  4  3
    /// </code>
    /// </summary>
    private int DirToDX(int Direction)
    {
        switch (Direction)
        {
            case 0:
            case 4:
                return 0;
            case 1:
            case 2:
            case 3:
                return 1;
            default:
                return -1;
        }
    }

    /// <summary>原文 <c>:318-326 function TPathMap.DirToDY</c>。</summary>
    private int DirToDY(int Direction)
    {
        switch (Direction)
        {
            case 2:
            case 6:
                return 0;
            case 3:
            case 4:
            case 5:
                return 1;
            default:
                return -1;
        }
    }

    // ---- 坐标换算 ----

    /// <summary>原文 <c>:530-533 function TPathMap.MapX</c>（<c>Result := X + ClientRect.Left</c>）。</summary>
    public int MapX(int X)
    {
        return X + ClientRect.Left;
    }

    /// <summary>原文 <c>:535-538 function TPathMap.MapY</c>。</summary>
    public int MapY(int Y)
    {
        return Y + ClientRect.Top;
    }

    /// <summary>原文 <c>:540-543 function TPathMap.LoaclX</c>（原文拼写就是 <c>Loacl</c>，非 Local）。</summary>
    public int LoaclX(int X)
    {
        return X - ClientRect.Left;
    }

    /// <summary>原文 <c>:545-548 function TPathMap.LoaclY</c>。</summary>
    public int LoaclY(int Y)
    {
        return Y - ClientRect.Top;
    }

    /// <summary>
    /// 原文 <c>:551-564 procedure TPathMap.GetClientRect(X1, Y1, X2, Y2: Integer)</c>。
    /// <para><b>四个入参全部未使用</b>（原文缺陷 ⑦）：函数体只有 <c>ClientRect := Bounds(0,0,FWidth,FHeight)</c>，
    /// 原本按 <c>ScopeValue</c> 收窄可视区的算法**整段被花括号注释掉**，逐字保留在下方注释里。</para>
    /// </summary>
    public void GetClientRect(int X1, int Y1, int X2, int Y2)
    {
        ClientRect = new Rectangle(0, 0, FWidth, FHeight);

        // 原文 :554-563（整段被 { } 注释掉，逐字保留）：
        //
        // { ScopeValue := Max(Abs(X1 - X2), Abs(Y1 - Y2));
        //   ScopeValue := ScopeValue + ScopeValue div 2;
        //   if Width > ScopeValue then begin
        //     ClientRect.Left := Max(0, X1 - ScopeValue div 2);
        //     ClientRect.Right := ClientRect.Left + Min(Width, X1 + ScopeValue div 2);
        //   end;
        //   if Height > ScopeValue then begin
        //     ClientRect.Top := Max(0, Y1 - ScopeValue div 2);
        //     ClientRect.Bottom := ClientRect.Top + Min(Height, Y1 + ScopeValue div 2);
        //   end; }
    }

    /// <summary>
    /// 原文 <c>:681-690 function TPathMap.GetCost</c>（<c>virtual</c> ⇒ 托管侧保留虚分派，台账 §18.8）。
    /// </summary>
    protected virtual int GetCost(int X, int Y, int Direction)
    {
        Direction &= 7;
        if (X < 0 || X >= ClientRect.Right - ClientRect.Left || Y < 0 || Y >= ClientRect.Bottom - ClientRect.Top)
            return -1;
        if (GetCostFunc != null)
            return GetCostFunc(X, Y, Direction, PathWidth);
        return -1;
    }

    // ---- 寻路主流程 ----

    /// <summary>
    /// 原文 <c>:332-373 procedure TPathMap.FindPathOnMap(X, Y: Integer)</c>。
    /// <para><b>procedure</b>（不是 function）：结果写进 <c>FPath</c>，随后 <c>WalkToRun()</c> 生成 <c>FRunPath</c>。</para>
    /// <para>两条早退都先把 <c>StartFind</c> 置假再 <c>Exit</c>（原文缺陷 ②见文件头）。</para>
    /// </summary>
    public void FindPathOnMap(int X, int Y)
    {
        int nX = LoaclX(X);
        int nY = LoaclY(Y);
        if (nX < 0 || nY < 0 ||
            nX >= ClientRect.Right - ClientRect.Left || nY >= ClientRect.Bottom - ClientRect.Top)
        {
            StartFind = false;
            return;
        }

        if (PathMapArray == null || PathMapArray.Length <= 0 || PathMapArray[nY][nX].Distance < 0)
        {
            StartFind = false;
            return;
        }

        FFindPathOnMap = true;

        FPath = new PathPoint[PathMapArray[nY][nX].Distance + 1];
        while (PathMapArray[nY][nX].Distance > 0)
        {
            if (!StartFind) break;
            FPath[PathMapArray[nY][nX].Distance] = new PathPoint(nX, nY);
            int Direction = PathMapArray[nY][nX].Direction;
            nX -= DirToDX(Direction);
            nY -= DirToDY(Direction);
        }

        // 原文缺陷 ②：中途 Break 时下半段仍是 null，这行与下面的回填照旧执行。
        FPath[0] = new PathPoint(nX, nY);
        for (int I = 0; I <= FPath.Length - 1; I++)
            FPath[I] = new PathPoint(MapX(FPath[I].X), MapY(FPath[I].Y));

        WalkToRun();

        FFindPathOnMap = false;
        StartFind = false;
    }

    /// <summary>
    /// 原文 <c>:375-522 procedure TPathMap.WalkToRun()</c>（注释「把WALK合并成RUN」）。
    /// <para>把 <c>FPath</c> 里连续同向的中间点标成 <c>(-1,-1)</c>，再收集进 <c>FRunPath</c>。</para>
    /// <para><b>骑马支</b>（<c>g_MySelf.m_btHorse &lt;&gt; 0</c> 且 <c>g_ClientConfig.boHorseRun3Grid</c>）
    /// 一次合并 3 格，普通支合并 2 格。</para>
    /// </summary>
    public void WalkToRun()
    {
        // 原文 :377-418 的嵌套函数 GetNextDirection（局部常量 DR_UP..DR_UPLEFT）。
        int GetNextDirection(int sx, int sy, int dx, int dy)
        {
            const int DR_UP = 0;
            const int DR_UPRIGHT = 1;
            const int DR_RIGHT = 2;
            const int DR_DOWNRIGHT = 3;
            const int DR_DOWN = 4;
            const int DR_DOWNLEFT = 5;
            const int DR_LEFT = 6;
            const int DR_UPLEFT = 7;

            int Result = DR_DOWN;
            int flagx;
            int flagy;

            if (sx < dx)
                flagx = 1;
            else if (sx == dx)
                flagx = 0;
            else
                flagx = -1;
            if (Math.Abs(sy - dy) > 2)
                if (sx >= dx - 1 && sx <= dx + 1) flagx = 0;

            if (sy < dy)
                flagy = 1;
            else if (sy == dy)
                flagy = 0;
            else
                flagy = -1;
            if (Math.Abs(sx - dx) > 2)
                if (sy > dy - 1 && sy <= dy + 1) flagy = 0;

            if (flagx == 0 && flagy == -1) Result = DR_UP;
            if (flagx == 1 && flagy == -1) Result = DR_UPRIGHT;
            if (flagx == 1 && flagy == 0) Result = DR_RIGHT;
            if (flagx == 1 && flagy == 1) Result = DR_DOWNRIGHT;
            if (flagx == 0 && flagy == 1) Result = DR_DOWN;
            if (flagx == -1 && flagy == 1) Result = DR_DOWNLEFT;
            if (flagx == -1 && flagy == 0) Result = DR_LEFT;
            if (flagx == -1 && flagy == -1) Result = DR_UPLEFT;
            return Result;
        }

        FRunPath = null;
        PathPoint[] WalkPath = null;
        if (FPath != null && FPath.Length > 1)
        {
            WalkPath = new PathPoint[FPath.Length];

            // ⚠ 值拷贝（原文是记录赋值）：见文件头"值语义陷阱"。
            for (int I = 0; I <= FPath.Length - 1; I++)
                WalkPath[I] = new PathPoint(FPath[I].X, FPath[I].Y);

            int nStep = 0;
            int nI = 0;

            // 骑马一步三格 chongchong 2013-10-17
            if (PathFindClientSeam.MySelfHorse() != 0 && PathFindClientSeam.ClientConfigHorseRun3Grid())
            {
                while (true)
                {
                    if (!StartFind) break;
                    if (nI >= WalkPath.Length) break;
                    if (nStep >= 3)
                    {
                        int nDir0 = GetNextDirection(WalkPath[nI - 3].X, WalkPath[nI - 3].Y,
                            WalkPath[nI - 2].X, WalkPath[nI - 2].Y);
                        int nDir1 = GetNextDirection(WalkPath[nI - 2].X, WalkPath[nI - 2].Y,
                            WalkPath[nI - 1].X, WalkPath[nI - 1].Y);
                        int nDir2 = GetNextDirection(WalkPath[nI - 1].X, WalkPath[nI - 1].Y,
                            WalkPath[nI].X, WalkPath[nI].Y);

                        if (nDir0 == nDir1 && nDir1 == nDir2)
                        {
                            WalkPath[nI - 2].X = -1;
                            WalkPath[nI - 2].Y = -1;

                            WalkPath[nI - 1].X = -1;
                            WalkPath[nI - 1].Y = -1;
                            nStep = 0;
                        }
                        else
                        {
                            // 原文注释「需要转向不能合并」
                            nI--;
                            nStep = 0;
                            continue;
                        }
                    }

                    nStep++;
                    nI++;
                }
            }
            else
            {
                while (true)
                {
                    if (!StartFind) break;
                    if (nI >= WalkPath.Length) break;
                    if (nStep >= 2)
                    {
                        int nDir1 = GetNextDirection(WalkPath[nI - 2].X, WalkPath[nI - 2].Y,
                            WalkPath[nI - 1].X, WalkPath[nI - 1].Y);
                        int nDir2 = GetNextDirection(WalkPath[nI - 1].X, WalkPath[nI - 1].Y,
                            WalkPath[nI].X, WalkPath[nI].Y);
                        if (nDir1 == nDir2)
                        {
                            WalkPath[nI - 1].X = -1;
                            WalkPath[nI - 1].Y = -1;
                            nStep = 0;
                        }
                        else
                        {
                            // 原文注释「需要转向不能合并」
                            nI--;
                            nStep = 0;
                            continue;
                        }
                    }

                    nStep++;
                    nI++;
                }
            }

            int n01 = 0;
            for (int I = 1; I <= WalkPath.Length - 1; I++)
            {
                // 原文条件是 `(x <> -1) and (y <> -1)`：任一坐标为 -1 即视为"已合并"（见文件头缺陷 ③）。
                if (WalkPath[I].X != -1 && WalkPath[I].Y != -1)
                {
                    n01++;
                    // 原文 SetLength(FRunPath, n01)：保留已有序元素、只增长长度。
                    Array.Resize(ref FRunPath, n01);
                    FRunPath[n01 - 1] = WalkPath[I];
                }
            }

            return;
        }

        if (FPath != null && FPath.Length > 0)
        {
            FRunPath = new PathPoint[FPath.Length - 1];
            // ⚠ 值拷贝：原文是记录赋值，托管侧必须显式 new（见文件头"值语义陷阱"）。
            for (int I = 1; I <= FPath.Length - 1; I++)
                FRunPath[I - 1] = new PathPoint(FPath[I].X, FPath[I].Y);
        }
        else
        {
            FRunPath = null;   // 原文 SetLength(FRunPath, 0); FRunPath := nil;
        }
    }

    // ---- 波扩散核心 ----

    /// <summary>
    /// 原文 <c>:566-679 function TPathMap.FillPathMap(X1, Y1, X2, Y2): TPathMapArray</c>。
    /// <para>三个嵌套过程（<c>PreparePathMap</c> / <c>TestNeighbours</c> / <c>ExchangeWaves</c>）
    /// 在托管侧落为私有方法（C# 无嵌套函数），共享状态经参数/字段传递。</para>
    /// <para>与既有 Core 版 <c>FillPathMap</c> 的差异：本版全程查 <c>StartFind</c> 且边界由
    /// <c>ClientRect</c> 决定（见文件头对照表）。</para>
    /// </summary>
    protected TPathMapCell[][] FillPathMap(int X1, int Y1, int X2, int Y2)
    {
        FFillPathMap = true;
        GetClientRect(X1, Y1, X2, Y2);

        int nX1 = LoaclX(X1);
        int nY1 = LoaclY(Y1);
        int nX2 = LoaclX(X2);
        int nY2 = LoaclY(Y2);

        if (X2 < 0) nX2 = X2;
        if (Y2 < 0) nY2 = Y2;

        if (X2 >= 0 && Y2 >= 0)
        {
            if (Math.Abs(nX1 - nX2) > ClientRect.Right - ClientRect.Left ||
                Math.Abs(nY1 - nY2) > ClientRect.Bottom - ClientRect.Top)
            {
                FFillPathMap = false;
                return Array.Empty<TPathMapCell[]>();   // 原文 SetLength(Result, 0, 0)
            }
        }

        TPathMapCell[][] Result = PreparePathMap();      // 初始化 PathMapArray, Distance := -1

        TWave OldWave = new TWave();
        TWave NewWave = new TWave();
        Result[nY1][nX1].Distance = 0;                   // 起点 Distance := 0
        OldWave.Add(nX1, nY1, 0, 0);                     // 将起点加入 OldWave
        TestNeighbours(Result, OldWave, NewWave);

        bool Finished = nX1 == nX2 && nY1 == nY2;        // 检验是否到达终点
        while (!Finished)
        {
            ExchangeWaves(ref OldWave, ref NewWave);
            if (!StartFind) break;
            if (!OldWave.Start()) break;
            do
            {
                if (!StartFind) break;
                TWaveCell I = OldWave.Item;
                I.Cost -= OldWave.MinCost;               // 如果大于 MinCost 则更新 Cost = cost - MinCost
                if (I.Cost > 0)
                {
                    NewWave.Add(I.X, I.Y, I.Cost, I.Direction);
                }
                else
                {
                    // 处理最小 COST 的点
                    if (Result[I.Y][I.X].Distance >= 0)
                        continue;

                    Result[I.Y][I.X].Distance =
                        Result[I.Y - DirToDY(I.Direction)][I.X - DirToDX(I.Direction)].Distance + 1;

                    Result[I.Y][I.X].Direction = I.Direction;
                    Finished = I.X == nX2 && I.Y == nY2;  // 检验是否到达终点
                    if (Finished) break;
                    TestNeighbours(Result, OldWave, NewWave);
                }
            }
            while (OldWave.Next());
        }

        FFillPathMap = false;
        return Result;
    }

    /// <summary>
    /// 原文 <c>:573-587</c> 的嵌套过程 <c>PreparePathMap</c>：按 <c>ClientRect</c> 尺寸分配并把
    /// <c>Distance</c> 全置 <c>-1</c>；**每一步都查 <c>StartFind</c>，变假即就地返回**
    /// （未初始化的格子保持默认 0 —— 原文如此）。
    /// </summary>
    private TPathMapCell[][] PreparePathMap()
    {
        int rows = ClientRect.Bottom - ClientRect.Top;
        int cols = ClientRect.Right - ClientRect.Left;
        var Result = new TPathMapCell[rows][];
        for (int Y = 0; Y <= rows - 1; Y++)
        {
            if (!StartFind) return Result;
            Result[Y] = new TPathMapCell[cols];
            for (int X = 0; X <= cols - 1; X++)
            {
                if (!StartFind) return Result;
                Result[Y][X].Distance = -1;
            }
        }

        return Result;
    }

    /// <summary>
    /// 原文 <c>:592-604</c> 的嵌套过程 <c>TestNeighbours</c>：
    /// 计算相邻 8 个节点的权 cost，合法点（cost &gt;= 0 且未访问）加入 <c>NewWave</c>。
    /// <para>注意原文**只判 <c>C &gt;= 0</c>**，没有额外的坐标越界判断
    /// （依赖 <c>GetCost</c> 对越界返回 -1，以及 Delphi 的短路求值）。</para>
    /// </summary>
    private void TestNeighbours(TPathMapCell[][] Result, TWave OldWave, TWave NewWave)
    {
        for (int D = 0; D <= 7; D++)
        {
            int X = OldWave.Item.X + DirToDX(D);
            int Y = OldWave.Item.Y + DirToDY(D);
            int C = GetCost(X, Y, D);
            // 原文 {$B-} 短路：C < 0 时不访问 Result（越界保护就靠这一条）。
            if (C >= 0 && Result[Y][X].Distance < 0)
                NewWave.Add(X, Y, C, D);
        }
    }

    /// <summary>原文 <c>:606-614</c> 的嵌套过程 <c>ExchangeWaves</c>（交换两个波对象并清空新波）。</summary>
    private static void ExchangeWaves(ref TWave OldWave, ref TWave NewWave)
    {
        TWave W = OldWave;
        OldWave = NewWave;
        NewWave = W;
        NewWave.Clear();
    }
}

// ------------------------------------------------------------ TFindPathThread

/// <summary>
/// 原文 <c>PathFindClient.pas:176-185/201-220 TFindPathThread = class(TThread)</c>。
/// <para><c>Create</c> 里 <c>FreeOnTerminate := True</c> → 赋四个字段 → <c>inherited Create(True)</c>（挂起创建）
/// → <c>Resume</c>（立即开跑）。<c>Execute</c> 里对**全局** <c>LegendMap</c> 调四次参的 <c>FindPath</c>。</para>
/// <para>托管侧不继承 <see cref="Thread"/>（<c>Thread.Run</c> 非虚、无法保留 <c>Execute</c> 的
/// <c>override</c> 语义），改为**组合**一个后台线程，但 <c>Execute</c> 仍是
/// <c>protected virtual</c> —— 保留原文的虚分派（台账 §18.8）。</para>
/// <para><c>FreeOnTerminate</c> 在托管侧由 GC 承担；<c>WaitFor</c> 是原文从 <c>TThread</c>
/// 继承来的能力，这里显式补回，供宿主/测试同步。</para>
/// </summary>
public class TFindPathThread
{
    /// <summary>原文 <c>:203 FreeOnTerminate := True</c>。</summary>
    public bool FreeOnTerminate = true;

    private readonly Thread FThread;

    /// <summary>原文 <c>:179 FStartX, FStartY, FStopX, FStopY: Integer</c>（private）。</summary>
    private int FStartX;
    private int FStartY;
    private int FStopX;
    private int FStopY;

    /// <summary>
    /// 原文 <c>:201-210 constructor TFindPathThread.Create(StartX, StartY, StopX, StopY)</c>。
    /// <para>顺序逐字保留：<c>FreeOnTerminate</c> → 四个字段 → <c>inherited Create(True)</c> → <c>Resume</c>。</para>
    /// </summary>
    public TFindPathThread(int StartX, int StartY, int StopX, int StopY)
    {
        FreeOnTerminate = true;
        FStartX = StartX;
        FStartY = StartY;
        FStopX = StopX;
        FStopY = StopY;

        // inherited Create(True)：挂起创建
        FThread = new Thread(ExecuteRunner) { IsBackground = true };
        FThread.Start();   // 原文 Resume
    }

    private void ExecuteRunner()
    {
        Execute();
    }

    /// <summary>
    /// 原文 <c>:217-220 procedure TFindPathThread.Execute</c>（<c>override</c> ⇒ 托管侧 <c>virtual</c>）。
    /// <para>对**全局** <c>LegendMap</c> 调用四个参数的 <c>FindPath</c>（无 <c>PathSpace</c>、无 <c>ExcludeMonster</c>）。</para>
    /// </summary>
    protected virtual void Execute()
    {
        PathFindClientSeam.LegendMap.FindPath(FStartX, FStartY, FStopX, FStopY);
    }

    /// <summary>
    /// 原文 <c>:212-215 destructor TFindPathThread.Destroy</c>（只 <c>inherited Destroy</c>）。
    /// 托管侧无析构时机，落为空操作占位。
    /// </summary>
    public void Destroy()
    {
    }

    /// <summary>
    /// 原文从 <c>TThread</c> 继承的 <c>WaitFor</c>（本类是 <c>TThread</c> 子类 ⇒ 原文确有此能力）。
    /// 托管侧组合线程后必须显式补回。
    /// </summary>
    public void WaitFor()
    {
        FThread.Join();
    }

    /// <summary><c>WaitFor</c> 的超时版本（返回是否已结束）。</summary>
    public bool WaitFor(int millisecondsTimeout)
    {
        return FThread.Join(millisecondsTimeout);
    }
}

// --------------------------------------------------------------- TLegendMap

/// <summary>
/// 原文 <c>PathFindClient.pas:133-149/692-809 TLegendMap = class(TPathMap)</c>
/// （注释「传奇地图读取及寻路类」）。
/// <para>⚠ 与既有 <c>GXX.Core.Util.TLegendMap</c>（Common 版）**不同**：本版没有 <c>LoadMap</c>，
/// 却多了标题/计数/起止点一族公开字段与 <c>Find</c>/<c>Stop</c>。见文件头对照表。</para>
/// </summary>
public class TLegendMap : TPathMap
{
    /// <summary>原文 <c>:135 FExcludeMonster: Boolean</c>（private）。</summary>
    private bool FExcludeMonster;

    /// <summary>原文 <c>:137 Title: string</c>。</summary>
    public string Title = "";

    /// <summary>原文 <c>:138 FindCount, BeginX, BeginY, EndX, EndY, FindX, FindY, PathPoisonIndex: Integer</c>。</summary>
    public int FindCount;
    public int BeginX;
    public int BeginY;
    public int EndX;
    public int EndY;
    public int FindX;
    public int FindY;
    public int PathPoisonIndex;

    /// <summary>
    /// 原文 <c>:139 MapData: TMapInfoArray</c>（<c>array of array of TMapInfo</c>）。
    /// <para><b>本单元从不读它</b>（读取与填充属其它单元），照抄声明。</para>
    /// </summary>
    public TMapInfo[][] MapData = Array.Empty<TMapInfo[]>();

    /// <summary>原文 <c>:692-702 constructor TLegendMap.Create</c>（六个字段逐一初始化）。</summary>
    public TLegendMap()
        : base()
    {
        StartFind = false;
        FFillPathMap = false;
        FFindPathOnMap = false;
        PathPoisonIndex = 0;
        FindCount = 0;

        FExcludeMonster = false;
    }

    /// <summary>
    /// 原文 <c>:704-720 procedure TLegendMap.Stop</c>（清空路径与全部计数/起止点，<c>PathMapArray := nil</c>）。
    /// </summary>
    public void Stop()
    {
        FPath = null;
        FRunPath = null;
        StartFind = false;
        BeginX = -1;
        BeginY = -1;
        EndX = -1;
        EndY = -1;

        FindX = -1;
        FindY = -1;
        PathPoisonIndex = 0;
        FindCount = 0;
        PathMapArray = Array.Empty<TPathMapCell[]>();   // 原文 SetLength(PathMapArray, 0, 0); PathMapArray := nil;
    }

    /// <summary>
    /// 原文 <c>:142 procedure TLegendMap.Find(StartX, StartY, StopX, StopY; ExcludeMonster: Boolean = False)</c>。
    /// <para><b>原文缺陷 ①</b>：<c>TFindPathThread.Create</c> 内部就 <c>Resume</c> 起了线程，
    /// 而 <c>FExcludeMonster</c> 是**之后**才赋的（Delphi 的 <c>with</c> 里
    /// <c>FExcludeMonster</c> 落到外层 <c>Self</c>）⇒ 与线程体存在竞态。顺序逐字保留。</para>
    /// </summary>
    public void Find(int StartX, int StartY, int StopX, int StopY, bool ExcludeMonster)
    {
        var finder = new TFindPathThread(StartX, StartY, StopX, StopY);
        // 原文如此：线程已开跑，这里才写 FExcludeMonster。
        FExcludeMonster = ExcludeMonster;
    }

    /// <summary>原文 <c>ExcludeMonster</c> 默认 <c>False</c> 的调用形式。</summary>
    public void Find(int StartX, int StartY, int StopX, int StopY) => Find(StartX, StartY, StopX, StopY, false);

    /// <summary>
    /// 原文 <c>:722-740 procedure TLegendMap.FindPath(StopX, StopY; ExcludeMonster = False)</c>。
    /// <para>先自旋等 <c>FFillPathMap</c>/<c>FFindPathOnMap</c> 落下，再清空、置起点、调 <c>FindPathOnMap</c>。</para>
    /// <para><b>注意与九参版的前后顺序不同</b>（原文缺陷 ⑥）：本版是 <c>Inc(FindCount)</c> 在前、
    /// <c>FExcludeMonster := ...</c> 在后。</para>
    /// </summary>
    public void FindPath(int StopX, int StopY, bool ExcludeMonster)
    {
        StartFind = false;
        while (FFillPathMap || FFindPathOnMap)
            Thread.Sleep(1);

        FindCount++;

        FExcludeMonster = ExcludeMonster;

        PathMapArray = Array.Empty<TPathMapCell[]>();
        FindX = StopX;
        FindY = StopY;
        FPath = null;
        FRunPath = null;
        StartFind = true;
        FindPathOnMap(StopX, StopY);
    }

    /// <summary>原文 <c>FindPath(StopX, StopY)</c>（<c>ExcludeMonster</c> 默认 <c>False</c>）。</summary>
    public void FindPath(int StopX, int StopY) => FindPath(StopX, StopY, false);

    /// <summary>
    /// 原文 <c>:750-769 procedure TLegendMap.FindPath(StartX, StartY, StopX, StopY; PathSpace: Integer = 0; ExcludeMonster: Boolean = False)</c>。
    /// <para>与上一版的不同：多 <c>PathWidth := PathSpace</c>，且 <c>PathMapArray := FillPathMap(...)</c> 是显式赋值的。</para>
    /// </summary>
    public void FindPath(int StartX, int StartY, int StopX, int StopY, int PathSpace, bool ExcludeMonster)
    {
        StartFind = false;
        while (FFillPathMap || FFindPathOnMap)
            Thread.Sleep(1);

        FExcludeMonster = ExcludeMonster;

        FindCount++;
        PathMapArray = Array.Empty<TPathMapCell[]>();
        FRunPath = null;
        FPath = null;
        FindX = StopX;
        FindY = StopY;
        PathWidth = PathSpace;
        StartFind = true;
        PathMapArray = FillPathMap(StartX, StartY, StopX, StopY);
        FindPathOnMap(StopX, StopY);
    }

    /// <summary>原文 <c>FindPath(StartX, StartY, StopX, StopY)</c>（两个默认参都取默认值）。</summary>
    public void FindPath(int StartX, int StartY, int StopX, int StopY)
        => FindPath(StartX, StartY, StopX, StopY, 0, false);

    /// <summary>原文 <c>FindPath(StartX, StartY, StopX, StopY, PathSpace)</c>（只默认 <c>ExcludeMonster</c>）。</summary>
    public void FindPath(int StartX, int StartY, int StopX, int StopY, int PathSpace)
        => FindPath(StartX, StartY, StopX, StopY, PathSpace, false);

    /// <summary>
    /// 原文 <c>:771-777 procedure TLegendMap.SetStartPos(StartX, StartY, PathSpace: Integer)</c>。
    /// <para><b>不</b>触碰 <c>FindCount</c>/<c>FindX</c>/<c>FindY</c>，也<b>不</b>设 <c>StartFind</c>
    /// （原文缺陷 ⑧：它沿用调用前的开关）—— 而 <c>FillPathMap</c> 全程查 <c>StartFind</c>，
    /// 故<b>调用方必须先自己把 <c>StartFind</c> 置真</b>，否则这里会崩（见用例
    /// <c>LegendMap_SetStartPosRequiresStartFindAlreadyTrue</c>）。</para>
    /// </summary>
    public void SetStartPos(int StartX, int StartY, int PathSpace)
    {
        BeginX = StartX;
        BeginY = StartY;
        PathWidth = PathSpace;
        PathMapArray = FillPathMap(StartX, StartY, -1, -1);
    }

    /// <summary>
    /// 原文 <c>:779-809 function TLegendMap.GetCost</c>（<c>override</c> ⇒ 必须保留虚分派）。
    /// <para>把本地坐标经 <c>MapX/MapY</c> 换成地图坐标，再问客户端场景对象
    /// <c>PlayScene.NewCanWalkEx</c>（<c>FExcludeMonster = False</c>）或
    /// <c>PlayScene.NewCanWalkEx_2</c>（<c>True</c>）；可走给 4，否则 -1；
    /// 斜方向（<c>Direction and 1 = 1</c>）且成本为正时乘 1.5（<c>Result + (Result shr 1)</c>）。</para>
    /// <para>托管侧对 <c>PlayScene</c> 走接缝 <see cref="PathFindClientSeam"/>（M2Server 无客户端场景对象）。</para>
    /// </summary>
    protected override int GetCost(int X, int Y, int Direction)
    {
        int Result;
        Direction &= 7;
        if (X < 0 || X >= ClientRect.Right - ClientRect.Left || Y < 0 || Y >= ClientRect.Bottom - ClientRect.Top)
        {
            Result = -1;
        }
        else
        {
            int nX = MapX(X);
            int nY = MapY(Y);

            if (!FExcludeMonster)
            {
                if (PathFindClientSeam.NewCanWalkEx(nX, nY))
                    Result = 4;
                else
                    Result = -1;
            }
            else
            {
                if (PathFindClientSeam.NewCanWalkEx_2(nX, nY))
                    Result = 4;
                else
                    Result = -1;
            }

            // 如果是斜方向,则COST增加；应为 Result*sqt(2)，此处近似为 1.5
            if ((Direction & 1) == 1 && Result > 0)
                Result += Result >> 1;
        }

        return Result;
    }
}

// -------------------------------------------------------------------- 接缝

/// <summary>
/// 本单元用到的全部外部依赖的**最小接缝层**。
/// <para>这些能力分属未移植的客户端单元（<c>MShare.pas</c> 的全局 <c>LegendMap</c>/<c>g_MySelf</c>、
/// <c>Grobal2</c> 的 <c>g_ClientConfig</c>、<c>PlayScn.pas</c> 的 <c>PlayScene</c>），
/// 本车道**不**复制它们，只把用到的成员抽象出来；默认实现映射到"客户端未接入"的空状态。</para>
/// <para>接缝：待 GXX.Client 的 PlayScene/MShare 移植后由宿主注入。</para>
/// </summary>
public static class PathFindClientSeam
{
    /// <summary>
    /// 原文 <c>MShare.pas</c> 的全局 <c>LegendMap: TLegendMap</c>（<c>TFindPathThread.Execute</c> 用它）。
    /// <para>客户端侧是一个真实存在的全局单例，且**在载入地图时已由宿主设好 <c>Width</c>/<c>Height</c>**；
    /// M2Server 侧没有它 ⇒ 默认给一个空实例（这样 <c>Execute</c> 不会因 nil 崩溃）。</para>
    /// <para>⚠ <b>宿主必须先设 <c>Width</c>/<c>Height</c> 再用</b>：0×0 的地图会让
    /// <c>TPathMap.FillPathMap</c> 在 <c>PreparePathMap</c> 之后越界访问（原文是访问违例），
    /// 而托管侧后台线程里的未捕获异常会**直接终止进程**（比原文的 AV 更早、更硬）。
    /// 这是本单元唯一的"接缝默认值不安全"处，已登记为报告中的风险项。</para>
    /// </summary>
    public static TLegendMap LegendMap { get; set; } = new TLegendMap();

    /// <summary>
    /// 原文 <c>g_MySelf.m_btHorse</c>（<c>MShare.pas</c> 的本地玩家对象字段；
    /// <c>WalkToRun</c> 判"是否骑马"）。
    /// </summary>
    public static Func<int> MySelfHorse { get; set; } = () => 0;

    /// <summary>
    /// 原文 <c>g_ClientConfig.boHorseRun3Grid</c>（注释「骑马一步三格 chongchong 2013-10-17」）。
    /// <para>⚠ 这是**客户端**配置（<c>Grobal2</c> 的 <c>TClientConfig</c>）；M2Server 侧另有同名的
    /// <c>M2Config.boHorseRun3Grid</c>（**服务端**配置），两者是不同实体，不要混用。</para>
    /// </summary>
    public static Func<bool> ClientConfigHorseRun3Grid { get; set; } = () => false;

    /// <summary>原文 <c>PlayScene.NewCanWalkEx(nX, nY)</c>（客户端场景物件可走判定）。</summary>
    public static Func<int, int, bool> NewCanWalkEx { get; set; } = (_, _) => false;

    /// <summary>原文 <c>PlayScene.NewCanWalkEx_2(nX, nY)</c>（与上一个同体，原文重复实现保留）。</summary>
    public static Func<int, int, bool> NewCanWalkEx_2 { get; set; } = (_, _) => false;

    /// <summary>恢复全部接缝默认值（测试隔离用）。</summary>
    public static void ResetDefaults()
    {
        LegendMap = new TLegendMap();
        MySelfHorse = () => 0;
        ClientConfigHorseRun3Grid = () => false;
        NewCanWalkEx = (_, _) => false;
        NewCanWalkEx_2 = (_, _) => false;
    }
}

// --------------------------------------------------------------- 常量表

/// <summary>
/// 原文 <c>PathFindClient.pas:187-194 const TerrainParams: array[TTerrainTypes] of TTerrainParam</c>。
/// <para><c>TColor</c> 常量按 Delphi Graphics.pas 的 $00BBGGRR 取值：
/// <c>clWhite=$FFFFFF</c>、<c>clOlive=$008080</c>、<c>clGreen=$008000</c>、
/// <c>clSilver=$C0C0C0</c>、<c>clBlack=$000000</c>、<c>clRed=$0000FF</c>。</para>
/// </summary>
public static class PathFindTerrainParams
{
    /// <summary>原文 <c>TerrainParams</c>（下标与 <see cref="TTerrainTypes"/> 一一对应）。</summary>
    public static readonly TTerrainParam[] TerrainParams =
    {
        new TTerrainParam { CellColor = 0xFFFFFF, CellLabel = "平地", MoveCost = 4 },
        new TTerrainParam { CellColor = 0x008080, CellLabel = "沙地", MoveCost = 6 },
        new TTerrainParam { CellColor = 0x008000, CellLabel = "树林", MoveCost = 10 },
        new TTerrainParam { CellColor = 0x00C0C0, CellLabel = "马路", MoveCost = 2 },
        new TTerrainParam { CellColor = 0x000000, CellLabel = "障碍物", MoveCost = -1 },
        new TTerrainParam { CellColor = 0x0000FF, CellLabel = "路径", MoveCost = 0 },
    };

    /// <summary>按下标取一项（模拟原文 <c>TerrainParams[ttXxx]</c>）。</summary>
    public static TTerrainParam Get(TTerrainTypes t) => TerrainParams[(int)t];

    /// <summary>
    /// 交叉验证：本表的 <c>MoveCost</c> 必须与既有 <c>GXX.Core.Util.PathFindConst.TerrainMoveCost</c>
    /// （Common\PathFind.pas 那份的移植）**逐项相等**，保证两份产物不分叉。
    /// </summary>
    public static bool MoveCostsMatchExistingCore()
    {
        for (int i = 0; i < TerrainParams.Length; i++)
        {
            if (TerrainParams[i].MoveCost != PathFindConst.TerrainMoveCost((TerrainType)i))
                return false;
        }

        return true;
    }
}

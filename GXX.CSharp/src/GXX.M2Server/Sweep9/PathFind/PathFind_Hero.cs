// ============================================================================
// 车道 p9-m2-pathfind —— 切片 4
// 源单元：Source/M2Engine/PathFind_Hero.pas（693 行，
//          镜像 _analysis/utf8_mirror/M2Engine/PathFind_Hero.pas）
// 1:1 托管移植。
//
// ⚠ 原文 `PathFind_Hero.pas:36` 里写的也是 **`unit PathFind;`**（与 PathFindClient.pas 同款复制残留），
//   且它声明的 `TPathMap` / `TWave` / `TWaveCell` 与 PathFindClient.pas **同名不同形**
//   ⇒ 托管侧必须另开命名空间（`...PathFind.Hero`），否则 CS0101 重名。
//
// ---------------------------------------------------------------------------
// 【与既有 GXX.Core/Util/PathFind.cs（= Common\PathFind.pas）的对照结论】
//   三者（Common / Client / Hero）同一祖先，方向布局、`DirToDX`/`DirToDY`、`TWave` 语义完全一致；
//   Hero 版的**特征差异**（本文件只移植这些）：
//   | PathFind_Hero.pas                                   | Common（既有 Core 产物）             |
//   |-----------------------------------------------------|--------------------------------------|
//   | `Width`/`Height` 是**公开字段**，`GetClientRect` 无参 | 尺寸由 `FindPath` 入参吸收              |
//   | `FindPathOnMap(X,Y,Run): TPath` **返回**路径          | `FindPathOnMap(X,Y): TPath`           |
//   | `WalkToRun(Path): TPath` 是**纯函数**                 | **无** WalkToRun                      |
//   | `GetCost(X,Y,Dir,boFlag)` **多一个 boFlag**           | `GetCost(X,Y,Dir)`                    |
//   | `FillPathMap(...,boFlag)`；**两道 2000 次循环上限**    | 无上限                                 |
//   | `TFindPath`：`FEnvir`/`FBaseObject`/临界区/三重重载      | `TLegendMap`：`LoadMap`/`SetStartPos`   |
//   | `TWave.Clear` 多一句 `FData := nil`                   | 无该句（与 PathFindClient 版一致）      |
//   | 基类 `GetCost` 返回 **0**（不是 -1）                   | 返回 -1                                |
//
// 【托管侧类型映射与接缝（全部登记在报告中）】
//   * `TEnvirnoment` → **复用既有** `GXX.M2Server.Engine.TEnvirnoment`；
//     `FEnvir.m_nWidth/m_nHeight` → 既有字段名 `nWidth/nHeight`（D-P9-11：既有 Envir 移植的命名偏离）。
//   * `TBaseObject` → **复用既有** `GXX.M2Server.Engine.TCreature`（托管侧把原 `TGameObject`→`TBaseObject`
//     →`TAnimalObject`→`TSmartObject` 几层折叠成了 `TCreature`；`TPlayObject : TCreature`）。
//   * `TM2CriticalSection` → **复用既有** `GXX.M2Server.Sweep.TM2CriticalSection`（M2Locker.pas 的 1:1 产物）。
//   * `g_MultiThreadRun` → 接缝，**默认转发**到既有 `Forms.CustomMagicFormGlobals.g_MultiThreadRun`
//     （不另造第二份存储；台账 §34.2）。
//   * `FEnvir.CanWalkEx(x,y,boFlag)` / `FEnvir.CanWalkEx(BaseObject,x,y,boFlag)` → **接缝**
//     （Envir.pas:2296-2735 的 `CanWalkEx` 本体尚未移植；`CanWalkCore.cs` 是"证据型核心"，不含本体）。
//   * `TBaseObject(FBaseObject).m_btPermission` / `.InSafeZone` → **接缝**
//     （`m_btPermission` 在原文属 `TBaseObject`（ObjBase.pas:117，Byte），而托管侧该字段落在 `TPlayObject`
//     上、基类没有；`InSafeZone` **根本未移植**——原文是 `TBaseObject.InSafeZone()` 方法（ObjBase.pas:633））。
//
// 【原文缺陷（照抄 + 差异断言锁定）】
//   ① `FillPathMap` 的两道越界守卫**互相错位**：
//      `if nX1 >= Length(Result)` 拿**列号**比**行数**；`else if nY1 >= Length(Result[0])` 拿**行号**比**列数**。
//      正常方图下两式近似等价，但**宽高不等时会把本来合法的起点判成非法** ⇒ 直接返回空图。
//   ② 上面那两条 `Exit` 发生在 `TWave.Create` **之后** ⇒ 原文**漏了** `OldWave.Free`/`NewWave.Free`
//      （内存泄漏）。托管侧由 GC 承担（无泄漏），语义等价，但顺序逐字保留。
//   ③ `FindPathOnMap` 若在中途 `Break`（`nCount` 上限或 `StartFind` 变假），
//      `Result[1..]` 仍是 `null`，紧接着的 `Result[I] := Point(MapX(...))` 会崩（原文 AV）。
//   ④ `Stop` / `FindPath` 等**先解锁再返回**的写法依赖 `g_MultiThreadRun`：开关为假时临界区**完全不进**。
//   ⑤ `FindPath(Envir,...,boFlag): Boolean` 这一重载**自己不拿锁**，它转调 6 参重载（那一层才拿锁，编号 2）。
//   ⑥ `WalkToRun` 收集条件同样是 `(x <> -1) and (y <> -1)`；且**起点永不进入结果**。
//   ⑦ `TPathMap.GetCost` 基类返回 **0**（而非 -1）⇒ 若直接用基类，`TestNeighbours` 的 `C >= 0` 恒真。
// ============================================================================

using System;
using System.Drawing;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;
using PathPoint = GXX.Core.Util.PathPoint;
using TPathMapCell = GXX.Core.Util.TPathMapCell;
using TWaveCell = GXX.Core.Util.TWaveCell;

namespace GXX.M2Server.Sweep9.PathFind.Hero;

/// <summary>
/// 原文 `UnitPath.pas:29 TGetCostFunc = procedure(Sender: TObject; X, Y, Direction: Integer;
/// var Result: Integer) of object;`
/// <para>⚠ 工程内**有三个不同的** `TGetCostFunc`：UnitPath 的这个（方法指针 + `var Result`）、
/// Common\PathFind.pas 的三参函数、PathFindClient.pas 的四参函数（带默认参 PathWidth）。
/// 本单元用的是 **UnitPath 版**，故必须独立声明（`GXX.Core.Util.TGetCostFunc` 是另一种形状）。</para>
/// <para>本单元里 `GetCostFunc` 只在构造时被赋 <c>nil</c>、**从不被调用**（原文如此），
/// 故这里只做忠实声明，不参与任何逻辑。</para>
/// </summary>
public delegate void TGetCostFunc(object Sender, int X, int Y, int Direction, ref int Result);

/// <summary>
/// 原文 <c>PathFind_Hero.pas:101-118 TWave = class</c>（注释「路线类」）。
/// <para>与 PathFindClient.pas 的 <c>TWave</c>、既有 Core 的 <c>TWave</c> **只差 <c>Clear</c> 里的一句
/// <c>FData := nil</c>**（本版有、另两版没有）；三者是三个单元各自的独立声明，不是重复实现。</para>
/// </summary>
public class TWave
{
    private TWaveCell[] FData = Array.Empty<TWaveCell>();

    /// <summary>原文 <c>:104 FPos</c>。</summary>
    private int FPos;

    /// <summary>原文 <c>:105 FCount</c>。</summary>
    private int FCount;

    /// <summary>原文 <c>:106 FMinCost</c>。</summary>
    private int FMinCost = int.MaxValue;

    /// <summary>原文 <c>:109 property Item: TWaveCell read GetItem</c>（即 <c>FData[FPos]</c>）。</summary>
    public TWaveCell Item => FData[FPos];

    /// <summary>原文 <c>:110 property MinCost: Integer read FMinCost</c>（原文注释「Cost」）。</summary>
    public int MinCost => FMinCost;

    /// <summary>原文 <c>:125-128 constructor TWave.Create</c>（只调 <c>Clear</c>）。</summary>
    public TWave()
    {
        Clear();
    }

    /// <summary>原文 <c>:130-134 destructor TWave.Destroy</c>（<c>FData := nil</c>）。</summary>
    public void Destroy()
    {
        FData = Array.Empty<TWaveCell>();
    }

    /// <summary>原文 <c>:141-155 procedure TWave.Add</c>（容量不足时 +30；同时维护 <c>FMinCost</c>）。</summary>
    public void Add(int NewX, int NewY, int NewCost, int NewDirection)
    {
        if (FCount >= FData.Length)
            Array.Resize(ref FData, FData.Length + 30);

        FData[FCount].X = NewX;
        FData[FCount].Y = NewY;
        FData[FCount].Cost = NewCost;
        FData[FCount].Direction = NewDirection;

        if (NewCost < FMinCost)
            FMinCost = NewCost;
        FCount++;
    }

    /// <summary>
    /// 原文 <c>:157-163 procedure TWave.Clear</c>。
    /// <para><b>本版特有</b>：比另两版多一句 <c>FData := nil</c>（动态数组被真正释放）。
    /// 行为上只影响容量复用，不影响可观察结果。</para>
    /// </summary>
    public void Clear()
    {
        FPos = 0;
        FCount = 0;
        FData = Array.Empty<TWaveCell>();
        FMinCost = int.MaxValue;
    }

    /// <summary>原文 <c>:165-169 function TWave.Start</c>（<c>FPos := 0; Result := FCount &gt; 0</c>）。</summary>
    public bool Start()
    {
        FPos = 0;
        return FCount > 0;
    }

    /// <summary>原文 <c>:171-175 function TWave.Next</c>（<c>Inc(FPos); Result := FPos &lt; FCount</c>）。</summary>
    public bool Next()
    {
        FPos++;
        return FPos < FCount;
    }
}

/// <summary>
/// 原文 <c>PathFind_Hero.pas:46-71 TPathMap = class</c>（注释「寻路类」，**服务端版**）。
/// </summary>
public class TPathMap
{
    /// <summary>原文 <c>:48 PathMapArray: TPathMapArray</c>（首维 Y、次维 X）。</summary>
    public TPathMapCell[][] PathMapArray = Array.Empty<TPathMapCell[]>();

    /// <summary>原文 <c>:49 Height: Integer</c>（公开字段，不是属性）。</summary>
    public int Height;

    /// <summary>原文 <c>:50 Width: Integer</c>（公开字段）。</summary>
    public int Width;

    /// <summary>原文 <c>:51 GetCostFunc: TGetCostFunc</c>（UnitPath 版；本单元从不调用）。</summary>
    public TGetCostFunc GetCostFunc;

    /// <summary>原文 <c>:52 ClientRect: TRect</c>。</summary>
    public Rectangle ClientRect;

    /// <summary>原文 <c>:53 ScopeValue: Integer</c>（注释「寻找范围」）。</summary>
    public int ScopeValue;

    /// <summary>原文 <c>:54 StartFind: Boolean</c>。</summary>
    public bool StartFind;

    /// <summary>原文 <c>:179-184 constructor TPathMap.Create</c>（<c>ScopeValue := 24 * 4</c>）。</summary>
    public TPathMap()
    {
        ScopeValue = 24 * 4;   // = 96
        GetCostFunc = null;
    }

    /// <summary>
    /// 原文 <c>:194-202 function TPathMap.DirToDX</c>（布局 <c>7 0 1 / 6 X 2 / 5 4 3</c>）。
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

    /// <summary>原文 <c>:204-212 function TPathMap.DirToDY</c>。</summary>
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

    /// <summary>原文 <c>:388-391 function TPathMap.MapX</c>。</summary>
    public int MapX(int X)
    {
        return X + ClientRect.Left;
    }

    /// <summary>原文 <c>:393-396 function TPathMap.MapY</c>。</summary>
    public int MapY(int Y)
    {
        return Y + ClientRect.Top;
    }

    /// <summary>原文 <c>:398-401 function TPathMap.LoaclX</c>（原文拼写就是 <c>Loacl</c>）。</summary>
    public int LoaclX(int X)
    {
        return X - ClientRect.Left;
    }

    /// <summary>原文 <c>:403-406 function TPathMap.LoaclY</c>。</summary>
    public int LoaclY(int Y)
    {
        return Y - ClientRect.Top;
    }

    /// <summary>
    /// 原文 <c>:408-411 procedure TPathMap.GetClientRect</c>（**无参**；与 PathFindClient 版不同）。
    /// </summary>
    public void GetClientRect()
    {
        ClientRect = new Rectangle(0, 0, Width, Height);
    }

    /// <summary>
    /// 原文 <c>:546-549 function TPathMap.GetCost(X, Y, Direction: Integer; boFlag: Boolean): Integer</c>（<c>virtual</c>）。
    /// <para><b>基类返回 0</b>（原文缺陷 ⑦）—— 注意不是 -1。</para>
    /// </summary>
    protected virtual int GetCost(int X, int Y, int Direction, bool boFlag)
    {
        return 0;
    }

    /// <summary>
    /// 原文 <c>:218-267 function TPathMap.FindPathOnMap(X, Y: Integer; Run: Boolean): TPath</c>。
    /// <para>回溯出路径后**把 <c>PathMapArray</c> 释放掉**（<c>PathMapArray := nil</c>）；
    /// <c>Run</c> 为真时再经 <c>WalkToRun</c> 合并成 RUN 步。</para>
    /// </summary>
    public PathPoint[] FindPathOnMap(int X, int Y, bool Run)
    {
        PathPoint[] Result = null;
        int nCount = 0;

        int nX = LoaclX(X);
        int nY = LoaclY(Y);
        if (nX < 0 || nY < 0 ||
            nX >= ClientRect.Right - ClientRect.Left || nY >= ClientRect.Bottom - ClientRect.Top)
        {
            PathMapArray = Array.Empty<TPathMapCell[]>();   // 原文 SetLength(...,0,0); := nil;
            return null;
        }

        if (PathMapArray == null || PathMapArray.Length <= 0 || PathMapArray[nY][nX].Distance < 0)
        {
            PathMapArray = Array.Empty<TPathMapCell[]>();
            return null;
        }

        Result = new PathPoint[PathMapArray[nY][nX].Distance + 1];
        while (PathMapArray[nY][nX].Distance > 0)
        {
            // 原文顺序：先查"循环上限"，再查 StartFind（两者都是 Break）。
            if (nCount >= Result.Length * 2) break;
            if (!StartFind) break;
            Result[PathMapArray[nY][nX].Distance] = new PathPoint(nX, nY);
            int Direction = PathMapArray[nY][nX].Direction;
            nX -= DirToDX(Direction);
            nY -= DirToDY(Direction);
            nCount++;
        }

        PathMapArray = Array.Empty<TPathMapCell[]>();

        // 原文缺陷 ③：上面若 Break，Result[1..] 仍是 null，这两步会崩（原文 AV）。
        Result[0] = new PathPoint(nX, nY);
        for (int I = 0; I <= Result.Length - 1; I++)
            Result[I] = new PathPoint(MapX(Result[I].X), MapY(Result[I].Y));

        if (Run)
            Result = WalkToRun(Result);

        return Result;
    }

    /// <summary>
    /// 原文 <c>:269-380 function TPathMap.WalkToRun(Path: TPath): TPath</c>
    /// （注释「把WALK合并成RUN」）。
    /// <para>**纯函数**：返回新数组，不改 <c>Path</c>，也不写 <c>FRunPath</c>（与 PathFindClient 版不同）。
    /// 只有**2 格合并**一支（没有骑马一步三格支）。</para>
    /// </summary>
    public PathPoint[] WalkToRun(PathPoint[] Path)
    {
        // 原文 :271-312 的嵌套函数 GetNextDirection（局部常量 DR_UP..DR_UPLEFT）。
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

        PathPoint[] Result = null;
        PathPoint[] WalkPath = null;
        if (Path != null && Path.Length > 1)
        {
            WalkPath = new PathPoint[Path.Length];

            // ⚠ 值拷贝（原文是记录赋值）：见 PathFindClient.cs 文件头"值语义陷阱"同一条。
            for (int I = 0; I <= Path.Length - 1; I++)
                WalkPath[I] = new PathPoint(Path[I].X, Path[I].Y);

            int nStep = 0;
            int nI = 0;

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

            int n01 = 0;
            for (int I = 1; I <= WalkPath.Length - 1; I++)
            {
                // 原文条件是 `(x <> -1) and (y <> -1)`：任一坐标为 -1 即视为"已合并"（缺陷 ⑥）。
                if (WalkPath[I].X != -1 && WalkPath[I].Y != -1)
                {
                    n01++;
                    Array.Resize(ref Result, n01);       // 原文 SetLength(Result, n01)
                    Result[n01 - 1] = WalkPath[I];
                }
            }

            return Result;
        }

        if (Path != null && Path.Length > 0)
        {
            Result = new PathPoint[Path.Length - 1];
            // ⚠ 值拷贝（原文记录赋值）。
            for (int I = 1; I <= Path.Length - 1; I++)
                Result[I - 1] = new PathPoint(Path[I].X, Path[I].Y);
        }
        else
        {
            Result = null;   // 原文 SetLength(Result, 0); Result := nil;
        }

        return Result;
    }

    /// <summary>
    /// 原文 <c>:413-544 function TPathMap.FillPathMap(X1, Y1, X2, Y2: Integer; boFlag: Boolean): TPathMapArray</c>。
    /// <para>与 PathFindClient 版的差异：无 <c>StartFind</c> 初始化循环检查、
    /// 多了**两道 2000 次循环上限**、入口处两道越界守卫（**原文错位**，缺陷 ①）、
    /// 以及 `GetCost` 多传一个 <c>boFlag</c>。</para>
    /// </summary>
    protected TPathMapCell[][] FillPathMap(int X1, int Y1, int X2, int Y2, bool boFlag)
    {
        GetClientRect();

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
                return Array.Empty<TPathMapCell[]>();       // 原文 SetLength(Result, 0, 0)
            }
        }

        TPathMapCell[][] Result = PreparePathMap();

        if (Result == null || Result.Length == 0) return Result;

        TWave OldWave = new TWave();
        TWave NewWave = new TWave();

        // 原文缺陷 ①：两道守卫的维数互相错位（列号比行数、行号比列数）。
        if (nX1 >= Result.Length)
        {
            return Array.Empty<TPathMapCell[]>();
        }
        else if (Result.Length > 0 && nY1 >= Result[0].Length)
        {
            return Array.Empty<TPathMapCell[]>();
        }

        Result[nY1][nX1].Distance = 0;                      // 起点 Distance := 0
        OldWave.Add(nX1, nY1, 0, 0);                        // 将起点加入 OldWave
        TestNeighbours(Result, OldWave, NewWave, boFlag);

        int nCount = 0;
        bool Finished = nX1 == nX2 && nY1 == nY2;           // 检验是否到达终点
        while (!Finished)
        {
            nCount++;
            if (nCount >= 2000) break;                      // 原文上限一
            ExchangeWaves(ref OldWave, ref NewWave);
            int nLoopCount = 0;

            if (!StartFind) break;
            if (!OldWave.Start()) break;
            do
            {
                nLoopCount++;
                if (nLoopCount >= 2000) break;              // 原文上限二
                if (!StartFind) break;
                TWaveCell I = OldWave.Item;
                I.Cost -= OldWave.MinCost;                  // 如果大于 MinCost 则更新 Cost = cost - MinCost
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
                    Finished = I.X == nX2 && I.Y == nY2;    // 检验是否到达终点
                    if (Finished) break;
                    TestNeighbours(Result, OldWave, NewWave, boFlag);
                }
            }
            while (OldWave.Next());
        }

        // 原文 :542-543 NewWave.Free; OldWave.Free;（托管侧由 GC 承担）
        return Result;
    }

    /// <summary>
    /// 原文 <c>:422-434</c> 的嵌套过程 <c>PreparePathMap</c>：按 <c>ClientRect</c> 分配
    /// <c>[Height][Width]</c> 并把 <c>Distance</c> 全置 -1。
    /// <para><b>注意维序</b>：<c>SetLength(Result, Bottom-Top, Right-Left)</c> ⇒ 首维是 **Height（行=Y）**、
    /// 次维是 **Width（列=X）**，与 <c>Result[Y][X]</c> 的用法一致。</para>
    /// <para>与 PathFindClient 版不同：**没有 StartFind 中途检查**。</para>
    /// </summary>
    private TPathMapCell[][] PreparePathMap()
    {
        int rows = ClientRect.Bottom - ClientRect.Top;
        int cols = ClientRect.Right - ClientRect.Left;
        var Result = new TPathMapCell[rows][];
        for (int Y = 0; Y <= rows - 1; Y++)
        {
            Result[Y] = new TPathMapCell[cols];
            for (int X = 0; X <= cols - 1; X++)
            {
                Result[Y][X].Distance = -1;
            }
        }

        return Result;
    }

    /// <summary>原文 <c>:439-451</c> 的嵌套过程 <c>TestNeighbours</c>（多传 <c>boFlag</c> 给 <c>GetCost</c>）。</summary>
    private void TestNeighbours(TPathMapCell[][] Result, TWave OldWave, TWave NewWave, bool boFlag)
    {
        for (int D = 0; D <= 7; D++)
        {
            int X = OldWave.Item.X + DirToDX(D);
            int Y = OldWave.Item.Y + DirToDY(D);
            int C = GetCost(X, Y, D, boFlag);
            if (C >= 0 && Result[Y][X].Distance < 0)
                NewWave.Add(X, Y, C, D);
        }
    }

    /// <summary>原文 <c>:453-461</c> 的嵌套过程 <c>ExchangeWaves</c>。</summary>
    private static void ExchangeWaves(ref TWave OldWave, ref TWave NewWave)
    {
        TWave W = OldWave;
        OldWave = NewWave;
        NewWave = W;
        NewWave.Clear();
    }
}

/// <summary>
/// 原文 <c>PathFind_Hero.pas:73-93 TFindPath = class(TPathMap)</c>
/// （注释「传奇地图读取及寻路类」）。
/// </summary>
public class TFindPath : TPathMap, IDisposable
{
    /// <summary>原文 <c>:75 FEnvir: TEnvirnoment</c>（private）。</summary>
    private TEnvirnoment FEnvir;

    /// <summary>原文 <c>:76 FBaseObject: TBaseObject</c>（private；托管侧用既有的 <see cref="TCreature"/>）。</summary>
    private TCreature FBaseObject;

    /// <summary>原文 <c>:77 FCriticalSection: TM2CriticalSection</c>（private）。</summary>
    private TM2CriticalSection FCriticalSection;

    /// <summary>原文 <c>:79 Title: string</c>。</summary>
    public string Title = "";

    /// <summary>原文 <c>:80 BeginX, BeginY, EndX, EndY: Integer</c>。</summary>
    public int BeginX;
    public int BeginY;
    public int EndX;
    public int EndY;

    /// <summary>
    /// 原文 <c>:551-558 constructor TFindPath.Create</c>（<c>FCriticalSection := TM2CriticalSection.Create('TFindPath.FCriticalSection')</c>）。
    /// </summary>
    public TFindPath()
        : base()
    {
        FEnvir = null;
        FBaseObject = null;
        StartFind = false;
        FCriticalSection = new TM2CriticalSection("TFindPath.FCriticalSection");
    }

    /// <summary>原文 <c>:687-691 destructor TFindPath.Destroy</c>（<c>FCriticalSection.Free</c>）。</summary>
    public void Dispose()
    {
        FCriticalSection.Dispose();
    }

    /// <summary>原文 <c>:89 property Envir: TEnvirnoment read FEnvir write FEnvir</c>。</summary>
    public TEnvirnoment Envir
    {
        get => FEnvir;
        set => FEnvir = value;
    }

    /// <summary>原文 <c>:90 property BaseObject: TBaseObject read FBaseObject write FBaseObject</c>。</summary>
    public TCreature BaseObject
    {
        get => FBaseObject;
        set => FBaseObject = value;
    }

    /// <summary>
    /// 原文 <c>:560-579 procedure TFindPath.Stop</c>（锁编号 **3**；<c>g_MultiThreadRun</c> 为假时完全不进临界区）。
    /// </summary>
    public void Stop()
    {
        if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.LockW(3);
        try
        {
            StartFind = false;
            BeginX = -1;
            BeginY = -1;
            EndX = -1;
            EndY = -1;
            PathMapArray = Array.Empty<TPathMapCell[]>();   // 原文 SetLength(...,0,0); := nil;
        }
        finally
        {
            if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.UnLockW();
        }
    }

    /// <summary>
    /// 原文 <c>:581-595 function TFindPath.FindPath(StopX, StopY: Integer; Run, boFlag: Boolean): TPath</c>
    /// （锁编号 **1**；只更新终点后转调 <c>FindPathOnMap</c>，**不重建图**）。
    /// </summary>
    public PathPoint[] FindPath(int StopX, int StopY, bool Run, bool boFlag)
    {
        if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.LockW(1);
        try
        {
            EndX = StopX;
            EndY = StopY;
            return FindPathOnMap(StopX, StopY, Run);
        }
        finally
        {
            if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.UnLockW();
        }
    }

    /// <summary>
    /// 原文 <c>:597-623 function TFindPath.FindPath(Envir; StartX, StartY, StopX, StopY: Integer;
    /// Run, boFlag: Boolean): TPath</c>（锁编号 **2**；<c>Envir = nil</c> 时返回 <c>nil</c>）。
    /// </summary>
    public PathPoint[] FindPath(TEnvirnoment Envir, int StartX, int StartY, int StopX, int StopY, bool Run,
        bool boFlag)
    {
        if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.LockW(2);
        try
        {
            if (Envir != null)
            {
                FEnvir = Envir;
                // 原文 Envir.m_nWidth / Envir.m_nHeight（托管侧既有字段名是 nWidth / nHeight，见 D-P9-11）
                Width = Envir.nWidth;
                Height = Envir.nHeight;
                BeginX = StartX;
                BeginY = StartY;
                EndX = StopX;
                EndY = StopY;
                StartFind = true;
                PathMapArray = FillPathMap(StartX, StartY, StopX, StopY, boFlag);
                return FindPathOnMap(StopX, StopY, Run);
            }

            return null;
        }
        finally
        {
            if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.UnLockW();
        }
    }

    /// <summary>
    /// 原文 <c>:625-632 function TFindPath.FindPath(Envir; StartX, StartY, StopX, StopY: Integer;
    /// boFlag: Boolean): Boolean</c>（**自己不拿锁**，转调 6 参重载并只回"路径非空"）。
    /// </summary>
    public bool FindPath(TEnvirnoment Envir, int StartX, int StartY, int StopX, int StopY, bool boFlag)
    {
        PathPoint[] Path = FindPath(Envir, StartX, StartY, StopX, StopY, false, boFlag);
        bool Result = Path != null && Path.Length > 0;
        Path = null;                                         // 原文 SetLength(Path, 0)
        return Result;
    }

    /// <summary>
    /// 原文 <c>:634-648 procedure TFindPath.SetStartPos(StartX, StartY: Integer)</c>
    /// （锁编号 **5**；<c>boFlag</c> 硬编码 <c>False</c>）。
    /// </summary>
    public void SetStartPos(int StartX, int StartY)
    {
        if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.LockW(5);
        try
        {
            BeginX = StartX;
            BeginY = StartY;
            PathMapArray = FillPathMap(StartX, StartY, -1, -1, false);
        }
        finally
        {
            if (PathFindHeroSeam.MultiThreadRun()) FCriticalSection.UnLockW();
        }
    }

    /// <summary>
    /// 原文 <c>:650-685 function TFindPath.GetCost(X, Y, Direction: Integer; boFlag: Boolean): Integer</c>
    /// （<c>override</c> ⇒ 保留虚分派）。
    /// <para>可走 4 / 不可走 -1；斜方向（<c>Direction and 1 = 1</c>）且为正时乘 1.5。
    /// <c>boFlag</c> 为真走 <c>CanWalkEx(nX,nY,boFlag)</c>，为假走带"跑动权限"的那个重载，
    /// 并且**额外 or 上** <c>boSafeAreaLimited and InSafeZone</c>。</para>
    /// </summary>
    protected override int GetCost(int X, int Y, int Direction, bool boFlag)
    {
        int Result;
        if (FEnvir != null)
        {
            Direction &= 7;
            if (X < 0 || X >= ClientRect.Right - ClientRect.Left || Y < 0 || Y >= ClientRect.Bottom - ClientRect.Top)
            {
                Result = -1;
            }
            else
            {
                int nX = MapX(X);
                int nY = MapY(Y);

                if (boFlag)
                {
                    if (PathFindHeroSeam.CanWalkEx(FEnvir, nX, nY, boFlag))
                        Result = 4;
                    else
                        Result = -1;
                }
                else
                {
                    // 原文第四个实参：g_Config.boDiableHumanRun or ((TBaseObject(FBaseObject).m_btPermission > 9) and g_Config.boGMRunAll)
                    bool runFlag = M2Config.boDiableHumanRun ||
                                   (PathFindHeroSeam.GetPermission(FBaseObject) > 9 && M2Config.boGMRunAll);
                    if (PathFindHeroSeam.CanWalkExObject(FEnvir, FBaseObject, nX, nY, runFlag) ||
                        (M2Config.boSafeAreaLimited && PathFindHeroSeam.InSafeZone(FBaseObject)))
                        Result = 4;
                    else
                        Result = -1;
                }

                // 如果是斜方向,则COST增加；应为 Result*sqt(2)，此处近似为 1.5
                if ((Direction & 1) == 1 && Result > 0)
                    Result += Result >> 1;
            }
        }
        else
        {
            Result = -1;
        }

        return Result;
    }
}

/// <summary>
/// 本单元用到的全部外部依赖的**最小接缝层**。
/// <para>这些能力分属未移植/异形移植的单元：<c>Envir.pas</c> 的 <c>CanWalkEx</c>（2296-2735）与
/// <c>ObjBase.pas</c> 的 <c>m_btPermission</c>（:117，属 <c>TBaseObject</c>）/ <c>InSafeZone</c>（:633）、
/// <c>M2Threads.pas</c> 的 <c>g_MultiThreadRun</c>。本车道不复制它们，只抽象出用到的成员。</para>
/// </summary>
public static class PathFindHeroSeam
{
    /// <summary>
    /// 原文 <c>M2Threads.pas:1648 g_MultiThreadRun: Boolean</c>（<c>{$IF MULTI_THREAD = 1}</c> 的总开关）。
    /// <para><b>默认转发</b>到既有的 <c>GXX.M2Server.Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun</c>
    /// —— 那是本仓对该全局的既有唯一存储，**不另造第二份**（台账 §34.2）。接缝：待 M2Threads.pas 正式移植后改指向它。</para>
    /// </summary>
    public static Func<bool> MultiThreadRun { get; set; } =
        () => GXX.M2Server.Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun;

    /// <summary>
    /// 原文 <c>Envir.pas</c> 的 <c>function TEnvirnoment.CanWalkEx(nX, nY: Integer; boFlag: Boolean): Boolean</c>。
    /// <para>接缝：**该本体尚未移植**（<c>CanWalkCore.cs</c> 是证据型核心，不含可调用实现）⇒ 默认假。</para>
    /// </summary>
    public static Func<TEnvirnoment, int, int, bool, bool> CanWalkEx { get; set; } = (_, _, _, _) => false;

    /// <summary>
    /// 原文 <c>Envir.pas</c> 的
    /// <c>function TEnvirnoment.CanWalkEx(BaseObject: TBaseObject; nX, nY: Integer; boFlag: Boolean): Boolean</c>。
    /// <para>接缝：同上（未移植）⇒ 默认假。</para>
    /// </summary>
    public static Func<TEnvirnoment, TCreature, int, int, bool, bool> CanWalkExObject { get; set; } =
        (_, _, _, _, _) => false;

    /// <summary>
    /// 原文 <c>TBaseObject(FBaseObject).m_btPermission</c>（ObjBase.pas:117，<c>Byte</c>，属 <c>TBaseObject</c>）。
    /// <para>托管侧该字段落在 <c>TPlayObject</c> 上、<c>TCreature</c> 基类没有（既有移植的分层结果）
    /// ⇒ 默认实现按"是人物就取该字段、否则 0"转发，**不再新增字段**（避免 §34.2 同名字段隐藏）。</para>
    /// </summary>
    public static Func<TCreature, int> GetPermission { get; set; } =
        obj => obj is TPlayObject play ? play.m_btPermission : 0;

    /// <summary>
    /// 原文 <c>TBaseObject.InSafeZone(): Boolean</c>（ObjBase.pas:633；原文在 PathFind_Hero 里以
    /// <c>TBaseObject(FBaseObject).InSafeZone</c> 的无括号形式调用）。接缝：**该本体尚未移植** ⇒ 默认假。
    /// </summary>
    public static Func<TCreature, bool> InSafeZone { get; set; } = _ => false;

    /// <summary>恢复全部接缝默认值（测试隔离用）。</summary>
    public static void ResetDefaults()
    {
        MultiThreadRun = () => GXX.M2Server.Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun;
        CanWalkEx = (_, _, _, _) => false;
        CanWalkExObject = (_, _, _, _, _) => false;
        GetPermission = obj => obj is TPlayObject play ? play.m_btPermission : 0;
        InSafeZone = _ => false;
    }
}

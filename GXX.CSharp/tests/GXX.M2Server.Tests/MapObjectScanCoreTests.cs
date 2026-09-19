using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J100：ObjBase.pas GetMapBaseObjects(30866-30922) / GetMapBaseObjectCount(30924-30978) 1:1 测试。
/// </summary>
public sealed class MapObjectScanCoreTests
{
    /// <summary>测试用地图格。</summary>
    private sealed class Cell : IMapCell
    {
        public List<object>? ObjList { get; set; } = new();
    }

    /// <summary>测试用扫描环境：按坐标登记格子，未登记的坐标返回 false。</summary>
    private sealed class Envir : IMapScanEnvir
    {
        public readonly Dictionary<(int, int), Cell> Cells = new();
        public int GetCalls;
        public bool ThrowOnGet;

        public Cell At(int x, int y)
        {
            if (!Cells.TryGetValue((x, y), out var c))
            {
                c = new Cell();
                Cells[(x, y)] = c;
            }
            return c;
        }

        public bool GetMapCellInfo(int nX, int nY, out IMapCell? cell)
        {
            GetCalls++;
            if (ThrowOnGet) throw new InvalidOperationException("boom");
            if (Cells.TryGetValue((nX, nY), out var c)) { cell = c; return true; }
            cell = null;
            return false;
        }
    }

    /// <summary>测试用游戏对象。</summary>
    private sealed class Obj : IGameObject
    {
        public TObjGame m_ObjGame { get; set; } = TObjGame.Obj_Actor;
        public bool Dead;
        public bool Ghost;
        public bool Proper = true;
        public string Name = "";
        public override string ToString() => Name;
    }

    private static readonly Func<object, bool> Dead = o => ((Obj)o).Dead;
    private static readonly Func<object, bool> Ghost = o => ((Obj)o).Ghost;
    private static readonly Func<object, bool> Proper = o => ((Obj)o).Proper;

    private static List<object> List_(Envir e, int x, int y, int rage, Action<string>? ex = null)
        => MapObjectScanCore.GetMapBaseObjects(e, x, y, rage, Proper, Dead, Ghost, ex);

    private static int Count_(Envir e, int x, int y, int rage, Action<string>? ex = null)
        => MapObjectScanCore.GetMapBaseObjectCount(e, x, y, rage, Proper, Dead, Ghost, ex);

    // ===================== 枚举与常量 =====================

    [Fact]
    public void ObjGameEnumMatchesSourceOrder()
    {
        // M2Definition.pas: (Obj_None, Obj_Actor, Obj_Item, Obj_Event, Obj_Gate,
        //                    Obj_Switch, Obj_MapEvent, Obj_Door, Obj_Roon, Obj_MapEffect)
        Assert.Equal(0, (int)TObjGame.Obj_None);
        Assert.Equal(1, (int)TObjGame.Obj_Actor);
        Assert.Equal(2, (int)TObjGame.Obj_Item);
        Assert.Equal(9, (int)TObjGame.Obj_MapEffect);
    }

    [Fact]
    public void ExceptionMessageKeepsOriginalText()
    {
        // 30933 在计数函数里也写作 GetMapBaseObjects（原文遗留）
        Assert.Equal("[Exception] TBaseObject.GetMapBaseObjects", MapObjectScanCore.ExceptionMsg);
    }

    // ===================== 扫描范围（闭区间） =====================

    [Fact]
    public void CellCountIsSquareOfSide()
    {
        Assert.Equal(1, MapObjectScanCore.CellCountForRage(0));
        Assert.Equal(9, MapObjectScanCore.CellCountForRage(1));
        Assert.Equal(25, MapObjectScanCore.CellCountForRage(2));
    }

    [Fact]
    public void RangeIsInclusiveSoRageZeroVisitsOneCell()
    {
        var e = new Envir();
        e.At(5, 5).ObjList!.Add(new Obj { Name = "a" });

        Assert.Single(List_(e, 5, 5, 0));
        Assert.Equal(1, e.GetCalls);
    }

    [Fact]
    public void RangeIsInclusiveSoRageOneVisitsNineCells()
    {
        var e = new Envir();
        List_(e, 10, 10, 1);
        Assert.Equal(9, e.GetCalls);      // (2*1+1)^2
    }

    [Fact]
    public void RangeIsInclusiveSoRageTwoVisitsTwentyFiveCells()
    {
        var e = new Envir();
        List_(e, 10, 10, 2);
        Assert.Equal(25, e.GetCalls);     // (2*2+1)^2
    }

    [Fact]
    public void ObjectOutsideRangeIsNotSeen()
    {
        var e = new Envir();
        e.At(10, 10).ObjList!.Add(new Obj { Name = "a" });
        e.At(12, 10).ObjList!.Add(new Obj { Name = "b" });

        // rage 1 → 覆盖 x ∈ 9..11，故 x=12 不可见
        var got = List_(e, 10, 10, 1);
        Assert.Single(got);
        Assert.Equal("a", got[0].ToString());
    }

    [Fact]
    public void BoundaryCellsAreIncluded()
    {
        var e = new Envir();
        e.At(9, 9).ObjList!.Add(new Obj { Name = "corner" });

        var got = List_(e, 10, 10, 1);
        Assert.Single(got);
        Assert.Equal("corner", got[0].ToString());
    }

    // ===================== obj_Actor 过滤 =====================

    [Fact]
    public void OnlyActorObjectsAreConsidered()
    {
        var e = new Envir();
        var cell = e.At(1, 1);
        cell.ObjList!.Add(new Obj { Name = "actor" });
        cell.ObjList!.Add(new Obj { Name = "item", m_ObjGame = TObjGame.Obj_Item });
        cell.ObjList!.Add(new Obj { Name = "gate", m_ObjGame = TObjGame.Obj_Gate });
        cell.ObjList!.Add(new Obj { Name = "none", m_ObjGame = TObjGame.Obj_None });

        var got = List_(e, 1, 1, 0);
        Assert.Single(got);
        Assert.Equal("actor", got[0].ToString());
    }

    [Fact]
    public void NullEntriesInObjListAreSkipped()
    {
        var e = new Envir();
        var cell = e.At(1, 1);
        cell.ObjList!.Add(null!);
        cell.ObjList!.Add(new Obj { Name = "actor" });

        Assert.Single(List_(e, 1, 1, 0));
    }

    [Fact]
    public void NullCellIsSkipped()
    {
        var e = new Envir();
        e.Cells[(1, 1)] = new Cell { ObjList = null };

        Assert.Empty(List_(e, 1, 1, 0));
        Assert.Equal(0, Count_(e, 1, 1, 0));
    }

    [Fact]
    public void MissingCellIsSkipped()
    {
        var e = new Envir();
        Assert.Empty(List_(e, 1, 1, 0));
    }

    // ===================== 死亡 / Ghost 过滤（两函数共同） =====================

    [Fact]
    public void DeadObjectsExcludedFromBoth()
    {
        var e = new Envir();
        e.At(1, 1).ObjList!.Add(new Obj { Name = "dead", Dead = true });

        Assert.Empty(List_(e, 1, 1, 0));
        Assert.Equal(0, Count_(e, 1, 1, 0));
    }

    [Fact]
    public void GhostObjectsExcludedFromBoth()
    {
        var e = new Envir();
        e.At(1, 1).ObjList!.Add(new Obj { Name = "ghost", Ghost = true });

        Assert.Empty(List_(e, 1, 1, 0));
        Assert.Equal(0, Count_(e, 1, 1, 0));
    }

    [Fact]
    public void AliveNonGhostIncludedInBoth()
    {
        var e = new Envir();
        e.At(1, 1).ObjList!.Add(new Obj { Name = "ok" });

        Assert.Single(List_(e, 1, 1, 0));
        Assert.Equal(1, Count_(e, 1, 1, 0));
    }

    // ===================== 关键差异：IsProperTarget =====================

    [Fact]
    public void CountExcludesNonProperTargets()
    {
        // **30959-30960 的关键差异**：计数函数**会**过滤 IsProperTarget
        var e = new Envir();
        e.At(1, 1).ObjList!.Add(new Obj { Name = "improper", Proper = false });

        Assert.Equal(0, Count_(e, 1, 1, 0));
    }

    [Fact]
    public void ListIncludesNonProperTargets()
    {
        // **30903**：列表函数**不判** IsProperTarget，故非法目标仍被收入
        var e = new Envir();
        e.At(1, 1).ObjList!.Add(new Obj { Name = "improper", Proper = false });

        var got = List_(e, 1, 1, 0);
        Assert.Single(got);
        Assert.Equal("improper", got[0].ToString());
    }

    [Fact]
    public void TwoFunctionsAgreeWhenAllTargetsProper()
    {
        var e = new Envir();
        var cell = e.At(1, 1);
        for (int i = 0; i < 4; i++) cell.ObjList!.Add(new Obj { Name = $"m{i}" });

        Assert.Equal(List_(e, 1, 1, 0).Count, Count_(e, 1, 1, 0));
    }

    [Fact]
    public void TwoFunctionsDifferOnMixedPropriety()
    {
        // 2 合法 + 3 非法：列表得 5（不过滤），计数得 2（过滤）
        var e = new Envir();
        var cell = e.At(1, 1);
        cell.ObjList!.Add(new Obj { Name = "p1", Proper = true });
        cell.ObjList!.Add(new Obj { Name = "p2", Proper = true });
        cell.ObjList!.Add(new Obj { Name = "n1", Proper = false });
        cell.ObjList!.Add(new Obj { Name = "n2", Proper = false });
        cell.ObjList!.Add(new Obj { Name = "n3", Proper = false });

        Assert.Equal(5, List_(e, 1, 1, 0).Count);
        Assert.Equal(2, Count_(e, 1, 1, 0));
    }

    [Fact]
    public void ProperTargetGateIsNotAppliedToList()
    {
        // 冗余保护：若有人把两个函数"统一"成同一谓词，本测试会失败
        var e = new Envir();
        e.At(1, 1).ObjList!.Add(new Obj { Name = "x", Proper = false });

        Assert.NotEqual(List_(e, 1, 1, 0).Count, Count_(e, 1, 1, 0));
    }

    // ===================== 异常处理差异 =====================

    [Fact]
    public void ListReportsExceptionAndReturnsEmpty()
    {
        var e = new Envir { ThrowOnGet = true };
        string? msg = null;

        var got = List_(e, 1, 1, 0, m => msg = m);

        Assert.Empty(got);
        Assert.Equal(MapObjectScanCore.ExceptionMsg, msg);
    }

    [Fact]
    public void ListStillReturnsCollectedOnLateException()
    {
        // 30921：GetMapBaseObjects 末尾无条件返回 True，异常被吞
        var e = new Envir();
        e.At(0, 0).ObjList!.Add(new Obj { Name = "first" });
        string? msg = null;

        // 在扫描到 (1,0) 之后抛异常：用只对特定坐标抛的 envir
        var partial = new PartialThrowEnvir(new Obj { Name = "first" });
        var got = MapObjectScanCore.GetMapBaseObjects(partial, 0, 0, 1, Proper, Dead, Ghost, m => msg = m);

        Assert.Single(got);                       // 异常前已收集的仍在
        Assert.Equal(MapObjectScanCore.ExceptionMsg, msg);
    }

    [Fact]
    public void CountReportsException()
    {
        var e = new Envir { ThrowOnGet = true };
        string? msg = null;

        Assert.Equal(0, Count_(e, 1, 1, 0, m => msg = m));
        Assert.Equal(MapObjectScanCore.ExceptionMsg, msg);
    }

    [Fact]
    public void CountReturnsPartialTotalOnLateException()
    {
        var partial = new PartialThrowEnvir(new Obj { Name = "first" });

        int n = MapObjectScanCore.GetMapBaseObjectCount(partial, 0, 0, 1, Proper, Dead, Ghost);

        Assert.Equal(1, n);      // 异常时返回已累计值（非 0）
    }

    [Fact]
    public void NoExceptionMeansNoCallback()
    {
        var e = new Envir();
        bool called = false;
        List_(e, 1, 1, 0, _ => called = true);
        Assert.False(called);
    }

    /// <summary>在第一个格子之后抛异常，用于验证部分结果保留。</summary>
    private sealed class PartialThrowEnvir : IMapScanEnvir
    {
        private readonly Obj _obj;
        private int _n;

        public PartialThrowEnvir(Obj obj) => _obj = obj;

        public bool GetMapCellInfo(int nX, int nY, out IMapCell? cell)
        {
            _n++;
            if (_n > 1) throw new InvalidOperationException("late boom");

            var c = new Cell();
            c.ObjList!.Add(_obj);
            cell = c;
            return true;
        }
    }
}

// ============================================================================
// FileSearchPool.pas（558 行）→ Pool/FileSearchPool.cs 的逐成员用例。
// 对账：原文 7 个类型（TCustomMemoryStreamEx / TMemoryStreamEx / TSearchTask /
//       TSearchThread / TSearchManager / TTaskCompleteEvent）+ 1 个自由函数 GetSearch。
// ============================================================================

using System;
using System.IO;
using System.Threading;
using Xunit;

// 命名空间见 P10PoolTestKit.cs 顶部说明（接缝与真实现同名 ⇒ 必须落在 ...Pool.Tests）。
namespace GXX.LogDataServer.Pool.Tests;

[Collection("P10PoolSerial")]
public sealed class P10FileSearchPoolTests : IDisposable
{
    private readonly string _dir;

    public P10FileSearchPoolTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p10_fsp_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    // ---------------- TCustomMemoryStreamEx ----------------

    private sealed class P10RawStream : TCustomMemoryStreamEx
    {
        public void Set(byte[]? ptr, int size) => SetPointer(ptr, size);
    }

    [Fact]
    public void TCustomMemoryStreamEx_SetPointer_SetsMemoryAndSize()
    {
        var s = new P10RawStream();
        Assert.Null(s.Memory);
        Assert.Equal(0, s.Size);

        var buf = new byte[] { 1, 2, 3, 4, 5 };
        s.Set(buf, 3);
        Assert.Same(buf, s.Memory);
        Assert.Equal(3, s.Size);
    }

    [Fact]
    public void TCustomMemoryStreamEx_Read_ReturnsRemainingClampedToCount()
    {
        var s = new P10RawStream();
        s.Set(new byte[] { 10, 20, 30, 40, 50 }, 5);

        var b = new byte[8];
        Assert.Equal(3, s.Read(b, 0, 3));
        Assert.Equal(10, b[0]);
        Assert.Equal(20, b[1]);
        Assert.Equal(30, b[2]);
        Assert.Equal(3, s.Position);

        // 剩余 2，读 8 ⇒ 返回 2（短读不抛）
        Assert.Equal(2, s.Read(b, 0, 8));
        Assert.Equal(5, s.Position);
    }

    [Fact]
    public void TCustomMemoryStreamEx_Read_AtEndOrNegativeCount_ReturnsZero()
    {
        var s = new P10RawStream();
        s.Set(new byte[] { 1 }, 1);
        s.Position = 1;
        Assert.Equal(0, s.Read(new byte[4], 0, 4));   // FPosition >= FSize ⇒ 0

        s.Position = 0;
        Assert.Equal(0, s.Read(new byte[4], 0, -1));  // Count < 0 ⇒ 0
    }

    [Fact]
    public void TCustomMemoryStreamEx_Read_OffsetOverload_WritesAtOffset()
    {
        var s = new P10RawStream();
        s.Set(new byte[] { 9, 8 }, 2);
        var b = new byte[4];
        Assert.Equal(2, s.Read(b, 2, 2));
        Assert.Equal(0, b[0]);
        Assert.Equal(9, b[2]);
        Assert.Equal(8, b[3]);
    }

    [Theory]
    [InlineData(TCustomMemoryStreamEx.soFromBeginning, 3, 3)]
    [InlineData(TCustomMemoryStreamEx.soFromCurrent, 3, 3)]
    [InlineData(TCustomMemoryStreamEx.soFromEnd, -2, 3)]
    public void TCustomMemoryStreamEx_Seek_FollowsDelphiOriginSemantics(ushort origin, int offset, int expected)
    {
        var s = new P10RawStream();
        s.Set(new byte[5], 5);
        s.Position = 0;
        Assert.Equal(expected, s.Seek(offset, origin));
    }

    [Fact]
    public void TCustomMemoryStreamEx_Seek_UnknownOrigin_LeavesPositionUnchanged()
    {
        // 原文 `case Origin of` 无 else ⇒ 未知 Origin 时 FPosition 不变
        var s = new P10RawStream();
        s.Set(new byte[5], 5);
        s.Position = 4;
        Assert.Equal(4, s.Seek(100, 99));
        Assert.Equal(4, s.Position);
    }

    [Fact]
    public void TCustomMemoryStreamEx_SaveToStream_ZeroSizeWritesNothing()
    {
        var s = new P10RawStream();
        s.Set(new byte[0], 0);
        using var ms = new MemoryStream();
        s.SaveToStream(ms);
        Assert.Equal(0, ms.Length);
    }

    [Fact]
    public void TCustomMemoryStreamEx_SaveToStream_WritesFSizeBytes()
    {
        var s = new P10RawStream();
        s.Set(new byte[] { 1, 2, 3, 4 }, 3);   // 只写前 3 字节（FSize=3）
        using var ms = new MemoryStream();
        s.SaveToStream(ms);
        Assert.Equal(new byte[] { 1, 2, 3 }, ms.ToArray());
    }

    [Fact]
    public void TCustomMemoryStreamEx_SaveToFile_WritesFile()
    {
        var s = new P10RawStream();
        s.Set(new byte[] { 7, 7, 7 }, 3);
        string path = Path.Combine(_dir, "out.bin");
        s.SaveToFile(path);
        Assert.Equal(new byte[] { 7, 7, 7 }, File.ReadAllBytes(path));
    }

    // ---------------- TMemoryStreamEx ----------------

    [Fact]
    public void TMemoryStreamEx_Capacity_IsRoundedUpToMemoryDelta()
    {
        var s = new TMemoryStreamEx();
        Assert.Equal(0, s.Capacity);
        Assert.Equal(0, s.Size);

        s.SetSize(10);
        Assert.Equal(10, s.Size);
        Assert.Equal(0x2000, s.Capacity);   // (10 + 8191) and not 8191

        s.SetSize(0x2001);
        Assert.Equal(0x4000, s.Capacity);
    }

    [Fact]
    public void TMemoryStreamEx_SetSize_ZeroFillsNewBytes()
    {
        var s = new TMemoryStreamEx();
        s.SetSize(4);
        Assert.Equal(new byte[4], ReadAll(s, 4));
    }

    [Fact]
    public void TMemoryStreamEx_SetSize_ShrinkingClampsPositionToEnd()
    {
        // 原文 `if OldPosition > NewSize then Seek(0, soFromEnd);`
        var s = new TMemoryStreamEx();
        s.SetSize(10);
        s.Position = 8;
        s.SetSize(3);
        Assert.Equal(3, s.Position);
        Assert.Equal(3, s.Size);
    }

    [Fact]
    public void TMemoryStreamEx_Write_GrowsSizeAndPosition()
    {
        var s = new TMemoryStreamEx();
        Assert.Equal(3, s.Write(new byte[] { 1, 2, 3 }, 0, 3));
        Assert.Equal(3, s.Size);
        Assert.Equal(3, s.Position);
        Assert.Equal(0x2000, s.Capacity);

        // 追加：Pos = 3 + 2 = 5 > FSize(3) ⇒ FSize := 5
        Assert.Equal(2, s.Write(new byte[] { 4, 5 }, 0, 2));
        Assert.Equal(5, s.Size);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, ReadAll(s, 5));
    }

    [Fact]
    public void TMemoryStreamEx_Write_NegativeCount_ReturnsZero()
    {
        var s = new TMemoryStreamEx();
        Assert.Equal(0, s.Write(new byte[2], 0, -1));
    }

    [Fact]
    public void TMemoryStreamEx_Clear_ResetsCapacitySizeAndPosition()
    {
        var s = new TMemoryStreamEx();
        s.Write(new byte[] { 1, 2, 3 }, 0, 3);
        s.Clear();
        Assert.Equal(0, s.Capacity);
        Assert.Equal(0, s.Size);
        Assert.Equal(0, s.Position);
        Assert.Null(s.Memory);
    }

    [Fact]
    public void TMemoryStreamEx_LoadFromStream_ReadsWholeStreamFromPositionZero()
    {
        var src = new MemoryStream(new byte[] { 5, 6, 7, 8 });
        src.Position = 3;                       // 原文强制回卷到 0（否则只能读到 1 字节，ReadBuffer 会抛）
        var s = new TMemoryStreamEx();
        s.LoadFromStream(src);
        Assert.Equal(4, s.Size);
        Assert.Equal(new byte[] { 5, 6, 7, 8 }, ReadAll(s, 4));
    }

    [Fact]
    public void TMemoryStreamEx_LoadFromFile_RoundTripsBytes()
    {
        string path = Path.Combine(_dir, "round.bin");
        File.WriteAllBytes(path, new byte[] { 1, 2, 3, 4, 5 });

        var s = new TMemoryStreamEx();
        s.LoadFromFile(path);
        Assert.Equal(5, s.Size);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, ReadAll(s, 5));
    }

    [Fact]
    public void TMemoryStreamEx_LoadFromStream_EmptyStream_SizeZero()
    {
        var s = new TMemoryStreamEx();
        s.LoadFromStream(new MemoryStream());
        Assert.Equal(0, s.Size);
        Assert.Equal(0, s.Capacity);
    }

    [Fact]
    public void TMemoryStreamEx_Dispose_Clears()
    {
        var s = new TMemoryStreamEx();
        s.Write(new byte[] { 1 }, 0, 1);
        s.Dispose();
        Assert.Equal(0, s.Size);
        Assert.Equal(0, s.Capacity);
    }

    private static byte[] ReadAll(TMemoryStreamEx s, int count)
    {
        s.Seek(0, TCustomMemoryStreamEx.soFromBeginning);   // 原文 Read 不自动回卷
        var b = new byte[count];
        Assert.Equal(count, s.Read(b, 0, count));
        return b;
    }

    // ---------------- TSearchTask ----------------

    [Fact]
    public void TSearchTask_Properties_AreReadWrite()
    {
        var t = new TSearchTask();
        Assert.Equal(0, t.TaskID);
        Assert.Equal("", t.FileName);
        Assert.Null(t.ShowPanel);

        var lbl = new System.Windows.Forms.ToolStripStatusLabel();
        t.TaskID = 12;
        t.FileName = "a.dat";
        t.ShowPanel = lbl;
        Assert.Equal(12, t.TaskID);
        Assert.Equal("a.dat", t.FileName);
        Assert.Same(lbl, t.ShowPanel);
    }

    [Fact]
    public void TSearchTask_CompareTask_AlwaysFalse()
    {
        // 原文如此：函数体只有 `Result := False`（比较逻辑被注释掉）
        var a = new TSearchTask { TaskID = 1 };
        var b = new TSearchTask { TaskID = 1 };
        Assert.False(a.CompareTask(b));
        Assert.False(a.CompareTask(a));
    }

    [Fact]
    public void TSearchTask_Assign_SearchToSearch_CopiesPanelOnly()
    {
        // 原文如此：AssignTo 只搬 FPanel（TaskID / FileName 不搬）
        var lbl = new System.Windows.Forms.ToolStripStatusLabel();
        var src = new TSearchTask { TaskID = 5, FileName = "x.dat", ShowPanel = lbl };
        var dst = new TSearchTask { TaskID = 9, FileName = "y.dat" };

        dst.Assign(src);

        Assert.Same(lbl, dst.ShowPanel);
        Assert.Equal(9, dst.TaskID);
        Assert.Equal("y.dat", dst.FileName);
    }

    [Fact]
    public void TSearchTask_Assign_NonSearchDest_FallsBackToInheritedError()
    {
        var src = new TSearchTask();
        var dst = new P10ProbeTask(1);
        Assert.Throws<EConvertError>(() => dst.Assign(src));
    }

    [Fact]
    public void TSearchTask_Dispose_IsNoOp()
    {
        var t = new TSearchTask { TaskID = 3 };
        t.Dispose();
        Assert.Equal(3, t.TaskID);
    }

    // ---------------- GetSearch（自由函数 / pTLogData） ----------------

    private static TLogData MakeLog() => new()
    {
        nAct = 5,
        sMapName = "0",
        nX = 10,
        nY = 20,
        sObjectName = "HeroName",
        ObjectType = TLogActorType.latHuman,
        sItemName = "ItemA",
        nItemIndex = 42,
        sActObjectName = "MonsterX",
        LogDesc = "d",
    };

    [Fact]
    public void GetSearch_WhereZero_ReturnsTrue()
    {
        // 原文如此：`Result := True; if nWhere <= 0 then Exit;`
        Assert.True(FileSearchPool.GetSearch(0, "zzz", "zzz", "zzz", 99, 99, MakeLog()));
        Assert.True(FileSearchPool.GetSearch(-1, "zzz", "zzz", "zzz", 99, 99, MakeLog()));
    }

    [Theory]
    // bit1 物体名（大小写不敏感子串）
    [InlineData(1, "hero", "x", "x", 99, 99, true)]
    [InlineData(1, "nope", "x", "x", 99, 99, false)]
    // bit2 类型精确等值
    [InlineData(2, "x", "x", "x", 1, 99, true)]
    [InlineData(2, "x", "x", "x", 2, 99, false)]
    // bit4 动作对象名
    [InlineData(4, "x", "monsterx", "x", 99, 99, true)]
    [InlineData(4, "x", "nope", "x", 99, 99, false)]
    // bit8 物品名
    [InlineData(8, "x", "x", "itema", 99, 99, true)]
    [InlineData(8, "x", "x", "nope", 99, 99, false)]
    // bit16 物品序号
    [InlineData(16, "x", "x", "x", 99, 42, true)]
    [InlineData(16, "x", "x", "x", 99, 41, false)]
    // 多位组合（逐位"与"短路）
    [InlineData(3, "hero", "x", "x", 1, 99, true)]
    [InlineData(3, "hero", "x", "x", 2, 99, false)]
    [InlineData(31, "hero", "monsterx", "itema", 1, 42, true)]
    [InlineData(31, "nope", "monsterx", "itema", 1, 42, false)]
    public void GetSearch_BitTests(int where, string objName, string actObjName, string itemName,
        int actObjType, int itemId, bool expected)
    {
        Assert.Equal(expected,
            FileSearchPool.GetSearch(where, objName, actObjName, itemName, actObjType, itemId, MakeLog()));
    }

    [Fact]
    public void GetSearch_EmptySubText_MatchesEverything()
    {
        // 原文 AnsiContainsText(s, '') ⇒ Pos('', s) = 1 > 0 ⇒ True
        Assert.True(FileSearchPool.GetSearch(1, "", "x", "x", 99, 99, MakeLog()));
    }

    [Fact]
    public void AnsiContainsText_IsCaseInsensitiveAndEmptyMatches()
    {
        Assert.True(FileSearchPool.AnsiContainsText("HeroName", "hero"));
        Assert.True(FileSearchPool.AnsiContainsText("hero", ""));
        Assert.False(FileSearchPool.AnsiContainsText("hero", "xyz"));
        Assert.True(FileSearchPool.AnsiContainsText("英雄名字", "英雄"));
    }

    // ---------------- TSearchThread / TSearchManager（端到端，临时目录） ----------------

    [Fact]
    public void TSearchManager_AddTask_IgnoresNonSearchTask()
    {
        using var mgr = new TSearchManager(1);
        mgr.AddTask(new P10ProbeTask(1));   // 原文 `if not (Task is TSearchTask) then Exit;`
        Assert.Equal(0, mgr.TaskCount);
    }

    [Fact]
    public void TSearchManager_GetPoolThreadClass_IsSearchThread()
    {
        using var mgr = new TSearchManager(2);
        Assert.IsType<TSearchThread>(mgr.GetThreadAt(0));
        Assert.Equal(2, mgr.ThreadCount);
    }

    [Fact]
    public void TSearchManager_Search_HappyPath_PushesMatchingRecord()
    {
        string path = P10LogFile.WriteSingle(_dir, nAction: 5, objectName: "英雄");

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions[5] = true;
        mgr.SearchWhere = 0;      // 不筛选 ⇒ GetSearch 恒 True

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();

        var panel = new System.Windows.Forms.ToolStripStatusLabel();
        var task = new TSearchTask { TaskID = 1, FileName = path, ShowPanel = panel };
        mgr.AddTask(task);

        Assert.True(done.Wait(15000), "搜索任务未在 15s 内完成");

        var list = mgr.SearchDataList!.LockList();
        try
        {
            var rec = Assert.IsType<TLogData>(Assert.Single(list));
            Assert.Equal(5u, rec.nAct);
            Assert.Equal("0", rec.sMapName);
            Assert.Equal(100, rec.nX);
            Assert.Equal(200, rec.nY);
            Assert.Equal("英雄", rec.sObjectName);
            Assert.Equal(TLogActorType.latHuman, rec.ObjectType);
            Assert.Equal("itemA", rec.sItemName);
            Assert.Equal(42, rec.nItemIndex);
            Assert.Equal("monster", rec.sActObjectName);
            Assert.Equal(7, rec.nData1);
            Assert.Equal(8, rec.nData2);
            Assert.Equal("desc", rec.LogDesc);

            // ★ 原文如此：nServerNumber / nServerIndex 读出后从未写进记录
            Assert.Equal(0, rec.nServerNumber);
            Assert.Equal(0, rec.nServerIndex);

            // nIndx := TaskID * 1000000 + nIndex（第 1 条）
            Assert.Equal(1000001, rec.nIndx);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }

        // RefreshLabel 已把文件名写进 TStatusPanel 接缝
        Assert.Equal(path, panel.Text);
    }

    [Fact]
    public void TSearchManager_GetActionChecked_HighByteOfLowWordAlsoGates()
    {
        // 原文 `GetActionChecked`: SearchActions[LoByte(W1)] or SearchActions[HiByte(W1)]
        //   nAction = 5 ⇒ W1 = 5 ⇒ B1 = 5, B2 = 0 ⇒ SearchActions[0] 也能让动作通过
        string path = P10LogFile.WriteSingle(_dir, nAction: 5);

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions[0] = true;      // 只开 B2（高位字节）这一支
        mgr.SearchActions[5] = false;
        mgr.SearchWhere = 0;

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();
        mgr.AddTask(new TSearchTask
        {
            TaskID = 2,
            FileName = path,
            ShowPanel = new System.Windows.Forms.ToolStripStatusLabel(),
        });
        Assert.True(done.Wait(15000));

        var list = mgr.SearchDataList!.LockList();
        try
        {
            Assert.Single(list);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }
    }

    [Fact]
    public void TSearchManager_Search_NoActionChecked_PushesNothing()
    {
        string path = P10LogFile.WriteSingle(_dir, nAction: 5);

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions = P10LogFile.NoActions();
        mgr.SearchWhere = 0;

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();
        mgr.AddTask(new TSearchTask
        {
            TaskID = 3,
            FileName = path,
            ShowPanel = new System.Windows.Forms.ToolStripStatusLabel(),
        });
        Assert.True(done.Wait(15000));

        var list = mgr.SearchDataList!.LockList();
        try
        {
            Assert.Empty(list);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }
    }

    [Fact]
    public void TSearchManager_Search_MissingFile_IsSilentlySwallowed()
    {
        // ★ 原文如此：DoSearch 整体包在 `try ... except end` 里 ⇒ 打不开文件也照常回调完成
        string path = Path.Combine(_dir, "does_not_exist.dat");

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions[5] = true;

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();
        mgr.AddTask(new TSearchTask
        {
            TaskID = 4,
            FileName = path,
            ShowPanel = new System.Windows.Forms.ToolStripStatusLabel(),
        });

        Assert.True(done.Wait(15000));
        var list = mgr.SearchDataList!.LockList();
        try
        {
            Assert.Empty(list);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }
    }

    [Fact]
    public void TSearchManager_Search_WhereFilterRejectsMismatch()
    {
        string path = P10LogFile.WriteSingle(_dir, nAction: 5, objectName: "英雄");

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions[5] = true;
        mgr.SearchWhere = 1;
        mgr.SearchObjName = "不存在";

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();
        mgr.AddTask(new TSearchTask
        {
            TaskID = 5,
            FileName = path,
            ShowPanel = new System.Windows.Forms.ToolStripStatusLabel(),
        });
        Assert.True(done.Wait(15000));

        var list = mgr.SearchDataList!.LockList();
        try
        {
            Assert.Empty(list);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }
    }

    [Fact]
    public void TSearchManager_Search_CanceledTaskStopsBeforePushing()
    {
        string path = P10LogFile.WriteSingle(_dir, nAction: 5);

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions[5] = true;
        mgr.SearchWhere = 0;

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();

        var task = new TSearchTask
        {
            TaskID = 6,
            FileName = path,
            ShowPanel = new System.Windows.Forms.ToolStripStatusLabel(),
        };
        task.Cancel();                 // 原文在读完一条后 `if Task.Canceled then Break;`
        mgr.AddTask(task);

        Assert.True(done.Wait(15000));
        var list = mgr.SearchDataList!.LockList();
        try
        {
            Assert.Empty(list);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }
    }

    [Fact]
    public void TSearchManager_Search_TwoRecords_IndexesAreTaskIdTimesMillion()
    {
        string path = Path.Combine(_dir, "two.dat");
        using (var w = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
        {
            P10LogFile.WriteRecord(w, 5, "0", "A", 1, "i", 1, "m", "d");
            P10LogFile.WriteRecord(w, 5, "1", "B", 1, "i", 2, "m", "d");
        }

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions[5] = true;
        mgr.SearchWhere = 0;

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();
        mgr.AddTask(new TSearchTask
        {
            TaskID = 7,
            FileName = path,
            ShowPanel = new System.Windows.Forms.ToolStripStatusLabel(),
        });
        Assert.True(done.Wait(15000));

        var list = mgr.SearchDataList!.LockList();
        try
        {
            Assert.Equal(2, list.Count);
            Assert.Equal(7000001, ((TLogData)list[0]).nIndx);
            Assert.Equal(7000002, ((TLogData)list[1]).nIndx);
            Assert.Equal("A", ((TLogData)list[0]).sObjectName);
            Assert.Equal("B", ((TLogData)list[1]).sObjectName);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }
    }

    [Fact]
    public void TSearchManager_Search_ActorTypeOutOfRange_ClampsToLatNone()
    {
        string path = Path.Combine(_dir, "actor.dat");
        using (var w = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
        {
            P10LogFile.WriteRecord(w, 5, "0", "A", 200, "i", 1, "m", "d");   // 200 > High(TLogActorType)
        }

        using var mgr = new TSearchManager(1);
        mgr.SearchDataList = new TThreadList();
        mgr.SearchActions[5] = true;
        mgr.SearchWhere = 0;

        var done = new ManualResetEventSlim(false);
        mgr.OnTaskComplete = _ => done.Set();
        mgr.AddTask(new TSearchTask
        {
            TaskID = 8,
            FileName = path,
            ShowPanel = new System.Windows.Forms.ToolStripStatusLabel(),
        });
        Assert.True(done.Wait(15000));

        var list = mgr.SearchDataList!.LockList();
        try
        {
            Assert.Equal(TLogActorType.latNone, ((TLogData)Assert.Single(list)).ObjectType);
        }
        finally
        {
            mgr.SearchDataList!.UnlockList();
        }
    }

    [Fact]
    public void TSearchTask_Create_DoesNotCallInheritedCtor_CanceledStillFalse()
    {
        // 原文 `constructor TSearchTask.Create; begin end;`（未调 inherited）⇒ 语义无差异
        var t = new TSearchTask();
        Assert.False(t.Canceled);
    }
}

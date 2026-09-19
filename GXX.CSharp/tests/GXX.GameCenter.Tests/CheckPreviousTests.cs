using System;
using System.Collections.Generic;
using GXX.GameCenter;
using Xunit;

namespace GXX.GameCenter.Tests;

/// <summary>
/// CheckPrevious.pas（93 行）1:1 移植测试。
/// 共享内存以内存 <see cref="IInstanceInfoStore"/> 注入，完整覆盖
/// <c>ERROR_ALREADY_EXISTS</c> / <c>RunCounter >= MaxInstances</c> / 窗口还原三支。
/// </summary>
[Collection("GameCenterSequential")]
public sealed class CheckPreviousTests : GameCenterTestBase
{
    /// <summary>内存共享内存实现：命名对象 + TInstanceInfo 视图。</summary>
    private sealed class MemoryStore : IInstanceInfoStore
    {
        private readonly Dictionary<string, TInstanceInfo> _map = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _handles = new(StringComparer.Ordinal);
        private int _next = 1;

        public int OpenOrCreateCount { get; private set; }
        public int OpenExistingCount { get; private set; }

        public IntPtr OpenOrCreate(string name, out bool alreadyExists)
        {
            OpenOrCreateCount++;
            alreadyExists = _map.ContainsKey(name);
            if (!alreadyExists) _map[name] = default;
            if (!_handles.TryGetValue(name, out int h))
            {
                h = _next++;
                _handles[name] = h;
            }
            return new IntPtr(h);
        }

        public IntPtr OpenExisting(string name)
        {
            OpenExistingCount++;
            return _handles.TryGetValue(name, out int h) ? new IntPtr(h) : IntPtr.Zero;
        }

        private string NameOf(IntPtr handle)
        {
            foreach (var kv in _handles)
                if (kv.Value == handle.ToInt32()) return kv.Key;
            throw new InvalidOperationException("unknown handle");
        }

        public TInstanceInfo Read(IntPtr handle) => _map[NameOf(handle)];
        public void Write(IntPtr handle, TInstanceInfo info) => _map[NameOf(handle)] = info;

        /// <summary>测试直读（不走句柄）。</summary>
        public TInstanceInfo Peek(string name) => _map.TryGetValue(name, out var v) ? v : default;
        public void Poke(string name, TInstanceInfo info) => _map[name] = info;

        /// <summary>测试预置：同时登记句柄，使 <see cref="OpenExisting"/> 能命中（模拟外部实例已建立映射）。</summary>
        public void PokeWithHandle(string name, TInstanceInfo info)
        {
            _map[name] = info;
            if (!_handles.ContainsKey(name)) _handles[name] = _next++;
        }

        public void Dispose() { }
    }

    private sealed class MemoryWindowService : IForegroundWindowService
    {
        public bool Iconic;
        public readonly List<(uint hwnd, int cmd)> ShowCalls = new();
        public readonly List<uint> ForegroundCalls = new();

        public bool IsIconic(uint hWnd) => Iconic;
        public bool ShowWindow(uint hWnd, int nCmdShow) { ShowCalls.Add((hWnd, nCmdShow)); return true; }
        public bool SetForegroundWindow(uint hWnd) { ForegroundCalls.Add(hWnd); return true; }
    }

    private MemoryStore _store = null!;
    private MemoryWindowService _windows = null!;

    private void Setup(string appPath = @"D:\App\GameCenter.exe")
    {
        _store = new MemoryStore();
        _windows = new MemoryWindowService();
        CheckPrevious.ResetForTests(_store);
        CheckPrevious.AppPathProvider = () => appPath;
        CheckPrevious.WindowService = _windows;
    }

    // ---------------- 映射名 ----------------

    [Fact]
    public void RestoreIfRunning_MappingNameStripsAllBackslashes()
    {
        Setup(@"D:\App\GameCenter.exe");
        CheckPrevious.RestoreIfRunning(1);
        Assert.Equal(@"D:AppGameCenter.exe", CheckPrevious.MappingName);
    }

    [Fact]
    public void RestoreIfRunning_MappingNameIgnoresCaseAndKeepsDriveColon()
    {
        Setup(@"c:\X\y.EXE");
        CheckPrevious.RestoreIfRunning(1);
        Assert.Equal("c:Xy.EXE", CheckPrevious.MappingName);
    }

    // ---------------- 首次实例 ----------------

    [Fact]
    public void RestoreIfRunning_FirstInstance_ReturnsFalseAndSetsCounterOne()
    {
        Setup();
        Assert.False(CheckPrevious.RestoreIfRunning(1234, 1));

        Assert.Equal((uint)1234, CheckPrevious.InstanceInfo.PreviousHandle);
        Assert.Equal(1, CheckPrevious.InstanceInfo.RunCounter);
        Assert.Equal((uint)1234, _store.Peek(CheckPrevious.MappingName).PreviousHandle);
        Assert.Equal(1, _store.Peek(CheckPrevious.MappingName).RunCounter);
        Assert.True(CheckPrevious.RemoveMe);
        Assert.NotEqual(IntPtr.Zero, CheckPrevious.MappingHandle);
    }

    // ---------------- 已存在且达到 MaxInstances ----------------

    [Fact]
    public void RestoreIfRunning_AlreadyRunningAtMax_ReturnsTrueAndBringsToFront()
    {
        Setup();
        // 模拟外部已有实例（RunCounter=1, MaxInstances=1）
        _store.Poke(@"D:AppGameCenter.exe", new TInstanceInfo { PreviousHandle = 4321, RunCounter = 1 });

        Assert.True(CheckPrevious.RestoreIfRunning(9999, 1));

        Assert.False(CheckPrevious.RemoveMe);
        Assert.Equal(new[] { 4321u }, _windows.ForegroundCalls);
        Assert.Empty(_windows.ShowCalls);   // 非最小化则不 ShowWindow
    }

    [Fact]
    public void RestoreIfRunning_AlreadyRunningAtMax_IconicWindowIsRestored()
    {
        Setup();
        _store.Poke(@"D:AppGameCenter.exe", new TInstanceInfo { PreviousHandle = 4321, RunCounter = 5 });
        _windows.Iconic = true;

        Assert.True(CheckPrevious.RestoreIfRunning(9999, 1));

        Assert.Equal(new[] { (4321u, IForegroundWindowService.SW_RESTORE) }, _windows.ShowCalls);
        Assert.Equal(new[] { 4321u }, _windows.ForegroundCalls);
        Assert.Equal(4321u, CheckPrevious.InstanceInfo.PreviousHandle);
        Assert.Equal(5, CheckPrevious.InstanceInfo.RunCounter);
    }

    [Fact]
    public void RestoreIfRunning_RunCounterOneBelowMax_IncrementsInstead()
    {
        Setup();
        _store.Poke(@"D:AppGameCenter.exe", new TInstanceInfo { PreviousHandle = 4321, RunCounter = 2 });

        // MaxInstances = 3 → 2 < 3，走自增分支（Result := False）
        Assert.False(CheckPrevious.RestoreIfRunning(777, 3));

        Assert.True(CheckPrevious.RemoveMe);
        Assert.Equal((uint)777, CheckPrevious.InstanceInfo.PreviousHandle);
        Assert.Equal(3, CheckPrevious.InstanceInfo.RunCounter);
        Assert.Empty(_windows.ForegroundCalls);
    }

    [Fact]
    public void RestoreIfRunning_WithoutWindowService_DoesNotThrow()
    {
        Setup();
        CheckPrevious.WindowService = null;
        _store.Poke(@"D:AppGameCenter.exe", new TInstanceInfo { PreviousHandle = 1, RunCounter = 1 });

        Assert.True(CheckPrevious.RestoreIfRunning(2, 1));   // 无接缝时静默等效
    }

    [Fact]
    public void RestoreIfRunning_OpenOrCreateReportsAlreadyExists()
    {
        Setup();
        CheckPrevious.RestoreIfRunning(1);
        Assert.Equal(1, _store.OpenOrCreateCount);

        CheckPrevious.RestoreIfRunning(2);
        Assert.Equal(2, _store.OpenOrCreateCount);
    }

    // ---------------- finalization ----------------

    [Fact]
    public void FinalizeUnit_WhenRemoveMe_DecrementsRunCounter()
    {
        Setup();
        CheckPrevious.RestoreIfRunning(1);                     // RunCounter = 1, RemoveMe = true
        Assert.True(CheckPrevious.RemoveMe);

        CheckPrevious.FinalizeUnit();

        Assert.Equal(0, CheckPrevious.InstanceInfo.RunCounter);
        Assert.Equal(1, _store.OpenExistingCount);
    }

    [Fact]
    public void FinalizeUnit_WhenRemoveMeFalse_DoesNotTouchCounter()
    {
        Setup();
        _store.Poke(@"D:AppGameCenter.exe", new TInstanceInfo { PreviousHandle = 4321, RunCounter = 1 });
        CheckPrevious.RestoreIfRunning(2, 1);                  // 走前台分支 → RemoveMe = false

        CheckPrevious.FinalizeUnit();

        Assert.Equal(1, _store.Peek(@"D:AppGameCenter.exe").RunCounter);
        Assert.Equal(0, _store.OpenExistingCount);
    }

    [Fact]
    public void FinalizeUnit_MissingMapping_ThrowsWin32Exception()
    {
        Setup();
        CheckPrevious.SetRemoveMeForTests(true);
        CheckPrevious.SetMappingNameForTests("no_such_mapping");

        Assert.Throws<System.ComponentModel.Win32Exception>(() => CheckPrevious.FinalizeUnit());
    }

    [Fact]
    public void FinalizeUnit_RemovingMoreThanAdded_StillDecrements()
    {
        // 原文不夹紧负值：RunCounter 可为负（逐字保留）。
        Setup();
        _store.PokeWithHandle("m", new TInstanceInfo { RunCounter = 0 });
        CheckPrevious.SetMappingNameForTests("m");

        CheckPrevious.FinalizeUnit();

        Assert.Equal(-1, _store.Peek("m").RunCounter);
    }

    [Fact]
    public void CloseFileMap_IsEmptyImplementation()
    {
        Setup();
        CheckPrevious.CloseFileMap();
        Assert.Equal(IntPtr.Zero, CheckPrevious.MappingHandle);
    }
}

using System;
using System.Collections.Generic;
using GXX.Core.Util;

namespace GXX.GameCenter;

/// <summary>
/// Delphi <c>try/finally TStringList.Free</c> 的托管等效垫片。
/// <para>
/// GMain.pas 多处形如 <c>SaveList := TStringList.Create; try ... finally SaveList.Free; end;</c>
/// 是"延迟释放"语义：列表要活到方法尾部（甚至跨越 <c>ini.Free</c>）才释放。
/// C# 的 <c>using var</c> 会把释放点提前到内层作用域末尾，因此本车道用本垫片把释放
/// 注册到方法级 <c>using var scope = new CleanupScope()</c> 上，逐字保留原文释放时机。
/// </para>
/// </summary>
public sealed class CleanupScope : IDisposable
{
    private readonly List<Action> _actions = new();

    /// <summary>注册一个"方法结束时执行"的释放动作（按注册顺序的逆序执行）。</summary>
    public void Register(Action action)
    {
        if (action != null) _actions.Add(action);
    }

    /// <summary>注册一个 TStringList 的释放（GXX.Core.TStringList 无 IDisposable，托管侧等价于清空）。</summary>
    public void Register(TStringList list) => Register(() => list.Clear());

    public void Dispose()
    {
        for (int i = _actions.Count - 1; i >= 0; i--)
        {
            try { _actions[i](); } catch { /* 原文 Free 不抛异常 */ }
        }
        _actions.Clear();
    }
}

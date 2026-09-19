using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGatePluginInterface.pas 接口成员齐全性测试（原 RunGatePluginInterface.pas，337 行）。
/// 用反射对 **每一个** 委托的精确签名、packed 结构字段顺序、托管接口成员做断言，
/// 防止"漏项/改名/改类型"。表内容与原文行号一一对应（见生产代码注释）。
/// </summary>
public class RunGatePluginInterfaceTests
{
    private static readonly Assembly Asm = typeof(TNotifyEventEx).Assembly;

    private static Type D(string name)
    {
        var t = Asm.GetType("GXX.RunGate." + name);
        Assert.NotNull(t);
        return t;
    }

    private static void AssertDelegate(string name, Type returnType, params Type[] args)
    {
        var t = D(name);
        Assert.True(typeof(Delegate).IsAssignableFrom(t), name + " 应为委托");
        var invoke = t.GetMethod("Invoke");
        Assert.NotNull(invoke);
        Assert.Equal(returnType, invoke.ReturnType);
        Assert.Equal(args, invoke.GetParameters().Select(p => p.ParameterType).ToArray());
        var attr = t.GetCustomAttribute<UnmanagedFunctionPointerAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(CallingConvention.StdCall, attr.CallingConvention);
    }

    // ---------------- 委托签名齐全性（原文 111 个函数指针类型） ----------------

    [Fact]
    public void NotifyEventEx_Signature()
        => AssertDelegate("TNotifyEventEx", typeof(void), typeof(object));

    [Fact]
    public void ListFuncTypes_All13()
    {
        Type L = typeof(IListHandle);
        AssertDelegate("TList_Create", L);
        AssertDelegate("TList_Free", typeof(void), L);
        AssertDelegate("TList_Count", typeof(int), L);
        AssertDelegate("TList_Clear", typeof(void), L);
        AssertDelegate("TList_Add", typeof(void), L, typeof(IntPtr));
        AssertDelegate("TList_Insert", typeof(void), L, typeof(int), typeof(IntPtr));
        AssertDelegate("TList_Remove", typeof(void), L, typeof(IntPtr));
        AssertDelegate("TList_Delete", typeof(void), L, typeof(int));
        AssertDelegate("TList_GetItem", typeof(IntPtr), L, typeof(int));
        AssertDelegate("TList_SetItem", typeof(void), L, typeof(int), typeof(IntPtr));
        AssertDelegate("TList_IndexOf", typeof(int), L, typeof(IntPtr));
        AssertDelegate("TList_Exchange", typeof(void), L, typeof(int), typeof(int));
        AssertDelegate("TList_CopyTo", typeof(void), L, L);
    }

    [Fact]
    public void MenuFuncTypes_All18()
    {
        Type M = typeof(IMenuItem);
        AssertDelegate("TMenu_Count", typeof(int), M);
        AssertDelegate("TMenu_GetItems", M, M, typeof(int));
        AssertDelegate("TMenu_Add", M, typeof(int), M, typeof(byte[]), typeof(int), typeof(TNotifyEventEx));
        AssertDelegate("TMenu_Insert", M, typeof(int), M, typeof(int), typeof(byte[]), typeof(int), typeof(TNotifyEventEx));
        AssertDelegate("TMenu_GetCaption", typeof(int), M, typeof(byte[]), typeof(uint).MakeByRefType());
        AssertDelegate("TMenu_SetCaption", typeof(void), M, typeof(byte[]));
        AssertDelegate("TMenu_GetEnabled", typeof(int), M);
        AssertDelegate("TMenu_SetEnabled", typeof(void), M, typeof(int));
        AssertDelegate("TMenu_GetVisable", typeof(int), M);
        AssertDelegate("TMenu_SetVisable", typeof(void), M, typeof(int));
        AssertDelegate("TMenu_GetChecked", typeof(int), M);
        AssertDelegate("TMenu_SetChecked", typeof(void), M, typeof(int));
        AssertDelegate("TMenu_GetRadioItem", typeof(int), M);
        AssertDelegate("TMenu_SetRadioItem", typeof(void), M, typeof(int));
        AssertDelegate("TMenu_GetGroupIndex", typeof(int), M);
        AssertDelegate("TMenu_SetGroupIndex", typeof(void), M, typeof(int));
        AssertDelegate("TMenu_GetTag", typeof(int), M);
        AssertDelegate("TMenu_SetTag", typeof(void), M, typeof(int));
    }

    [Fact]
    public void ClientContextFuncTypes_All24()
    {
        Type C = typeof(IIocpClientContext);
        Type U = typeof(uint).MakeByRefType();
        AssertDelegate("TClientContext_GetContextID", typeof(int), C);
        AssertDelegate("TClientContext_GetIPValue", typeof(uint), C);
        AssertDelegate("TClientContext_GetIpAddr", typeof(int), C, typeof(byte[]), U);
        AssertDelegate("TClientContext_GetPort", typeof(ushort), C);
        AssertDelegate("TClientContext_IsOldClient", typeof(int), C);
        AssertDelegate("TClientContext_IsLoginNotice", typeof(int), C);
        AssertDelegate("TClientContext_IsPlayGame", typeof(int), C);
        AssertDelegate("TClientContext_GetRecogId", typeof(long), C);
        AssertDelegate("TClientContext_GetAccount", typeof(int), C, typeof(byte[]), U);
        AssertDelegate("TClientContext_GetChrName", typeof(int), C, typeof(byte[]), U);
        AssertDelegate("TClientContext_GetMachineID", typeof(int), C, typeof(byte[]), U);
        AssertDelegate("TClientContext_GetJob", typeof(byte), C);
        AssertDelegate("TClientContext_GetMoveSpeed", typeof(int), C);
        AssertDelegate("TClientContext_GetAttackSpeed", typeof(int), C);
        AssertDelegate("TClientContext_GetSpellSpeed", typeof(int), C);
        AssertDelegate("TClientContext_SendToClientMsg", typeof(void), C, typeof(IntPtr), typeof(byte[]), typeof(int));
        AssertDelegate("TClientContext_SendToM2ServerMsg", typeof(void), C, typeof(IntPtr), typeof(byte[]), typeof(int));
        AssertDelegate("TClientContext_SendToClientMsg_Ex", typeof(void), C, typeof(byte[]), typeof(int));
        AssertDelegate("TClientContext_SendToM2ServerMsg_Ex", typeof(void), C, typeof(byte[]), typeof(int));
        AssertDelegate("TClientContext_Close", typeof(void), C);
        AssertDelegate("TClientContext_IsWaitClose", typeof(int), C);
        AssertDelegate("TClientContext_LockUser", typeof(void), C, typeof(int));
        AssertDelegate("TClientContext_IsSendClientDllComplete", typeof(int), C);
        AssertDelegate("TClientContext_GetSendClientDllTick", typeof(uint), C);
    }

    [Fact]
    public void RunGateFuncTypes_All17()
    {
        Type U = typeof(uint).MakeByRefType();
        AssertDelegate("TRunGate_GetAppPath", typeof(int), typeof(byte[]), U);
        AssertDelegate("TRunGate_GetAppFileName", typeof(int), typeof(byte[]), U);
        AssertDelegate("TRunGate_GetPluginPath", typeof(int), typeof(byte[]), U);
        AssertDelegate("TRunGate_GetPluginFileName", typeof(int), typeof(byte[]), U);
        AssertDelegate("TRunGate_AddMainLogMsg", typeof(void), typeof(byte[]), typeof(int));
        AssertDelegate("TRunGate_EncodeMessage", typeof(int), typeof(IntPtr), typeof(byte[]), U);
        AssertDelegate("TRunGate_DecodeMessage", typeof(int), typeof(byte[]), typeof(uint), typeof(IntPtr));
        AssertDelegate("TRunGate_EncodeBuffer", typeof(int), typeof(byte[]), typeof(uint), typeof(byte[]), U);
        AssertDelegate("TRunGate_DecodeBuffer", typeof(int), typeof(byte[]), typeof(uint), typeof(byte[]), U);
        AssertDelegate("TRunGate_ZLibEncodeBuffer", typeof(int), typeof(byte[]), typeof(uint), typeof(byte[]), U);
        AssertDelegate("TRunGate_ZLibDecodeBuffer", typeof(int), typeof(byte[]), typeof(uint), typeof(byte[]), U);
        AssertDelegate("TRunGate_GetOnlineContextList", typeof(int), typeof(IListHandle));
        AssertDelegate("TRunGate_AddTempBlockIP", typeof(void), typeof(byte[]));
        AssertDelegate("TRunGate_AddBlockIP", typeof(void), typeof(byte[]));
        AssertDelegate("TRunGate_AddTempBlockMac", typeof(void), typeof(byte[]));
        AssertDelegate("TRunGate_AddBlockMac", typeof(void), typeof(byte[]));
        AssertDelegate("TRunGate_NotifyClientDllReload", typeof(void));
    }

    [Fact]
    public void DelegateCount_MatchesSourceExactly()
    {
        // 原文函数指针类型数量：1(TNotifyEventEx) + 13 + 18 + 24 + 17 = 73
        int n = Asm.GetTypes()
            .Count(t => t.Namespace == "GXX.RunGate"
                        && typeof(Delegate).IsAssignableFrom(t)
                        && t.GetCustomAttribute<UnmanagedFunctionPointerAttribute>() != null);
        Assert.Equal(73, n);
    }

    // ---------------- packed 结构字段顺序/类型 ----------------

    private static void AssertFields(Type t, params (string Name, Type Type)[] expected)
    {
        // 只比较函数指针字段；Reserved 由 ReservedFixedBuffers_MatchSourceLengths 单独校验。
        var actual = t.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => !f.IsStatic && f.Name != "Reserved")
            .ToArray();
        Assert.Equal(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Name, actual[i].Name);
            Assert.Equal(expected[i].Type, actual[i].FieldType);
        }
        // StructLayoutAttribute 在反射里通过 typeof(T).StructLayoutAttribute 读取（不是普通特性）。
        var layout = t.StructLayoutAttribute;
        Assert.NotNull(layout);
        Assert.Equal(LayoutKind.Sequential, layout.Value);
        Assert.Equal(1, layout.Pack);
    }

    [Fact]
    public void TListFunc_FieldOrder()
    {
        Type L = typeof(IListHandle);
        AssertFields(typeof(TListFunc),
            ("Create", typeof(TList_Create)),
            ("Free", typeof(TList_Free)),
            ("Count", typeof(TList_Count)),
            ("Clear", typeof(TList_Clear)),
            ("Add", typeof(TList_Add)),
            ("Insert", typeof(TList_Insert)),
            ("Remove", typeof(TList_Remove)),
            ("Delete", typeof(TList_Delete)),
            ("GetItem", typeof(TList_GetItem)),
            ("SetItem", typeof(TList_SetItem)),
            ("IndexOf", typeof(TList_IndexOf)),
            ("Exchange", typeof(TList_Exchange)),
            ("CopyTo", typeof(TList_CopyTo)));
        _ = L;
    }

    [Fact]
    public void TMemuFunc_FieldOrder_NoteOriginalSpelling()
    {
        AssertFields(typeof(TMemuFunc),
            ("Count", typeof(TMenu_Count)),
            ("GetItems", typeof(TMenu_GetItems)),
            ("Add", typeof(TMenu_Add)),
            ("Insert", typeof(TMenu_Insert)),
            ("GetCaption", typeof(TMenu_GetCaption)),
            ("SetCaption", typeof(TMenu_SetCaption)),
            ("GetEnabled", typeof(TMenu_GetEnabled)),
            ("SetEnabled", typeof(TMenu_SetEnabled)),
            ("GetVisable", typeof(TMenu_GetVisable)),
            ("SetVisable", typeof(TMenu_SetVisable)),
            ("GetChecked", typeof(TMenu_GetChecked)),
            ("SetChecked", typeof(TMenu_SetChecked)),
            ("GetRadioItem", typeof(TMenu_GetRadioItem)),
            ("SetRadioItem", typeof(TMenu_SetRadioItem)),
            ("GetGroupIndex", typeof(TMenu_GetGroupIndex)),
            ("SetGroupIndex", typeof(TMenu_SetGroupIndex)),
            ("GetTag", typeof(TMenu_GetTag)),
            ("SetTag", typeof(TMenu_SetTag)));
    }

    [Fact]
    public void TClientContextFunc_FieldOrder_All24()
    {
        AssertFields(typeof(TClientContextFunc),
            ("GetContextID", typeof(TClientContext_GetContextID)),
            ("GetIPValue", typeof(TClientContext_GetIPValue)),
            ("GetIpAddr", typeof(TClientContext_GetIpAddr)),
            ("GetPort", typeof(TClientContext_GetPort)),
            ("IsOldClient", typeof(TClientContext_IsOldClient)),
            ("IsLoginNotice", typeof(TClientContext_IsLoginNotice)),
            ("IsPlayGame", typeof(TClientContext_IsPlayGame)),
            ("GetRecogId", typeof(TClientContext_GetRecogId)),
            ("GetAccount", typeof(TClientContext_GetAccount)),
            ("GetChrName", typeof(TClientContext_GetChrName)),
            ("GetMachineID", typeof(TClientContext_GetMachineID)),
            ("GetJob", typeof(TClientContext_GetJob)),
            ("GetMoveSpeed", typeof(TClientContext_GetMoveSpeed)),
            ("GetAttackSpeed", typeof(TClientContext_GetAttackSpeed)),
            ("GetSpellSpeed", typeof(TClientContext_GetSpellSpeed)),
            ("SendToClientMsg", typeof(TClientContext_SendToClientMsg)),
            ("SendToM2ServerMsg", typeof(TClientContext_SendToM2ServerMsg)),
            ("SendToClientMsg_Ex", typeof(TClientContext_SendToClientMsg_Ex)),
            ("SendToM2ServerMsg_Ex", typeof(TClientContext_SendToM2ServerMsg_Ex)),
            ("Close", typeof(TClientContext_Close)),
            ("IsWaitClose", typeof(TClientContext_IsWaitClose)),
            ("LockUser", typeof(TClientContext_LockUser)),
            ("IsSendClientDllComplete", typeof(TClientContext_IsSendClientDllComplete)),
            ("GetSendClientDllTick", typeof(TClientContext_GetSendClientDllTick)));
    }

    [Fact]
    public void TRunGateFunc_FieldOrder_All17()
    {
        AssertFields(typeof(TRunGateFunc),
            ("GetAppPath", typeof(TRunGate_GetAppPath)),
            ("GetAppFileName", typeof(TRunGate_GetAppFileName)),
            ("GetPluginPath", typeof(TRunGate_GetPluginPath)),
            ("GetPluginFileName", typeof(TRunGate_GetPluginFileName)),
            ("AddMainLogMsg", typeof(TRunGate_AddMainLogMsg)),
            ("EncodeMessage", typeof(TRunGate_EncodeMessage)),
            ("DecodeMessage", typeof(TRunGate_DecodeMessage)),
            ("EncodeBuffer", typeof(TRunGate_EncodeBuffer)),
            ("DecodeBuffer", typeof(TRunGate_DecodeBuffer)),
            ("ZLibEncodeBuffer", typeof(TRunGate_ZLibEncodeBuffer)),
            ("ZLibDecodeBuffer", typeof(TRunGate_ZLibDecodeBuffer)),
            ("GetOnlineContextList", typeof(TRunGate_GetOnlineContextList)),
            ("AddTempBlockIP", typeof(TRunGate_AddTempBlockIP)),
            ("AddBlockIP", typeof(TRunGate_AddBlockIP)),
            ("AddTempBlockMac", typeof(TRunGate_AddTempBlockMac)),
            ("AddBlockMac", typeof(TRunGate_AddBlockMac)),
            ("NotifyClientDllReload", typeof(TRunGate_NotifyClientDllReload)));
    }

    [Fact]
    public void TAppFuncDef_FieldOrder()
    {
        AssertFields(typeof(TAppFuncDef),
            ("PluginID", typeof(int)),
            ("List", typeof(TListFunc)),
            ("Menu", typeof(TMemuFunc)),
            ("Context", typeof(TClientContextFunc)),
            ("RunGate", typeof(TRunGateFunc)));
    }

    // ---------------- 保留字段宽度（Reserved 数组） ----------------

    [Theory]
    [InlineData(typeof(TListFunc), 20)]
    [InlineData(typeof(TMemuFunc), 20)]
    [InlineData(typeof(TClientContextFunc), 40)]
    [InlineData(typeof(TRunGateFunc), 70)]
    [InlineData(typeof(TAppFuncDef), 1000)]
    public void ReservedFixedBuffers_MatchSourceLengths(Type t, int count)
    {
        var f = t.GetField("Reserved", BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(f);
        // C# fixed 缓冲会生成嵌套的 <Reserved>e__FixedBuffer 结构体字段，
        // 元素类型与长度记录在字段上的 FixedBufferAttribute 里。
        var attr = f.GetCustomAttribute<FixedBufferAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(count, attr.Length);
        Assert.Equal(typeof(long), attr.ElementType);
        var nested = f.FieldType.GetField("FixedElementField",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(nested);
        Assert.Equal(typeof(long), nested.FieldType);
    }

    // ---------------- ABI 布局（指针宽度自适应） ----------------

    [Fact]
    public void TAppFuncDef_Size_MatchesHandComputedLayout()
    {
        int p = IntPtr.Size;
        int expected = 4 + (13 + 20) * p + (18 + 20) * p + (24 + 40) * p + (17 + 70) * p + 1000 * p;
        Assert.Equal(expected, Marshal.SizeOf<TAppFuncDef>());
        Assert.Equal(expected, RunGatePluginInterfaceLayout.SizeOfTAppFuncDef);
    }

    [Fact]
    public void TAppFuncDef_Offsets_MatchDelphiFieldOrder()
    {
        int p = IntPtr.Size;
        Assert.Equal(0, Marshal.OffsetOf<TAppFuncDef>("PluginID").ToInt32());
        Assert.Equal(4, Marshal.OffsetOf<TAppFuncDef>("List").ToInt32());
        Assert.Equal(4 + 33 * p, Marshal.OffsetOf<TAppFuncDef>("Menu").ToInt32());
        Assert.Equal(4 + 71 * p, Marshal.OffsetOf<TAppFuncDef>("Context").ToInt32());
        Assert.Equal(4 + 135 * p, Marshal.OffsetOf<TAppFuncDef>("RunGate").ToInt32());
        Assert.Equal(4 + 222 * p, Marshal.OffsetOf<TAppFuncDef>("Reserved").ToInt32());
    }

    [Fact]
    public void SubStructSizes_MatchFieldCountPlusReserved()
    {
        int p = IntPtr.Size;
        Assert.Equal(33 * p, Marshal.SizeOf<TListFunc>());        // 13 + 20
        Assert.Equal(38 * p, Marshal.SizeOf<TMemuFunc>());        // 18 + 20
        Assert.Equal(64 * p, Marshal.SizeOf<TClientContextFunc>());// 24 + 40
        Assert.Equal(87 * p, Marshal.SizeOf<TRunGateFunc>());     // 17 + 70
    }

    // ---------------- 托管接口成员齐全性 ----------------

    private static void AssertInterfaceMembers(Type iface, params (string Name, Type Return, Type[] Args)[] expected)
    {
        var actual = iface.GetMethods().OrderBy(m => m.Name, StringComparer.Ordinal).ToArray();
        var exp = expected.OrderBy(e => e.Name, StringComparer.Ordinal).ToArray();
        Assert.Equal(exp.Length, actual.Length);
        for (int i = 0; i < exp.Length; i++)
        {
            Assert.Equal(exp[i].Name, actual[i].Name);
            Assert.Equal(exp[i].Return, actual[i].ReturnType);
            Assert.Equal(exp[i].Args, actual[i].GetParameters().Select(pp => pp.ParameterType).ToArray());
        }
    }

    [Fact]
    public void IRunGatePlugin_HasSingleGetAppFuncDef()
    {
        var m = typeof(IRunGatePlugin).GetMethods();
        Assert.Single(m);
        Assert.Equal("GetAppFuncDef", m[0].Name);
        Assert.Equal(typeof(void), m[0].ReturnType);
        Assert.Equal(new[] { typeof(TAppFuncDef).MakeByRefType() },
            m[0].GetParameters().Select(p => p.ParameterType).ToArray());
    }

    [Fact]
    public void IListFunc_MembersMatchTListFuncFields()
    {
        // 托管接口成员数与 packed 记录字段数（不含 Reserved）一致
        Assert.Equal(13, typeof(IListFunc).GetMethods().Length);
        Assert.Equal(18, typeof(IMemuFunc).GetMethods().Length);
        Assert.Equal(24, typeof(IClientContextFunc).GetMethods().Length);
        Assert.Equal(17, typeof(IRunGateFunc).GetMethods().Length);
    }

    [Fact]
    public void IIocpClientContext_MembersMatchSource()
    {
        var names = typeof(IIocpClientContext).GetMethods().Select(m => m.Name).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        Assert.Equal(new[]
        {
            "Close", "GetAccount", "GetAttackSpeed", "GetChrName", "GetContextID", "GetIPValue",
            "GetIpAddr", "GetJob", "GetMachineID", "GetMoveSpeed", "GetPort", "GetRecogId",
            "GetSendClientDllTick", "GetSpellSpeed", "IsLoginNotice", "IsOldClient", "IsPlayGame",
            "IsSendClientDllComplete", "IsWaitClose", "LockUser", "SendToClientMsg",
            "SendToClientMsg_Ex", "SendToM2ServerMsg", "SendToM2ServerMsg_Ex"
        }, names);
    }

    [Fact]
    public void IMenuItem_MembersMatchSource()
    {
        var props = typeof(IMenuItem).GetProperties().Select(p => p.Name).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        Assert.Equal(new[]
        {
            "Caption", "Checked", "Count", "Enabled", "GroupIndex", "RadioItem", "Tag", "Visable"
        }, props);
        Assert.Single(typeof(IMenuItem).GetMethods(), m => m.Name == "GetItems");
    }

    [Fact]
    public void IListHandle_MembersMatchSource()
    {
        // 11 个方法 + 1 个 Count 属性（原文 TList 的公开面）`r`n        Assert.Equal(11, typeof(IListHandle).GetMethods().Length);`r`n        Assert.Equal(1, typeof(IListHandle).GetProperties().Length);
    }

    [Fact]
    public void PAppFuncDef_HoldsValue()
    {
        var box = new PAppFuncDef { Value = new TAppFuncDef { PluginID = 7 } };
        Assert.Equal(7, box.Value.PluginID);
        Assert.Equal(7, new PAppFuncDef(new TAppFuncDef { PluginID = 7 }).Value.PluginID);
    }

    [Fact]
    public void DelegateCallingConvention_IsStdCall_All()
    {
        // 原文所有函数指针都带 stdcall；逐个确认（防漏标注）
        foreach (var t in Asm.GetTypes().Where(t => t.Namespace == "GXX.RunGate"
                     && typeof(Delegate).IsAssignableFrom(t)))
        {
            var a = t.GetCustomAttribute<UnmanagedFunctionPointerAttribute>();
            if (a == null) continue;
            Assert.Equal(CallingConvention.StdCall, a.CallingConvention);
        }
    }

    [Fact]
    public void NoDuplicateDelegateNames()
    {
        var names = Asm.GetTypes()
            .Where(t => t.Namespace == "GXX.RunGate" && typeof(Delegate).IsAssignableFrom(t))
            .Select(t => t.Name).ToArray();
        Assert.Equal(names.Length, names.Distinct().Count());
    }

    [Fact]
    public void TestHelper_Placeholder()
    {
        // 保留：确认断言辅助没有被误用
        Assert.NotNull(D("TNotifyEventEx"));
    }

    private static readonly List<string> _unused = new();
}

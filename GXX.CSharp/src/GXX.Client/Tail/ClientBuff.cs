// 源单元：Source/Client-HGE/ClientBuff.pas（原文 17 行；本文件 1:1 对应）
// 原文 uses：Windows, Messages, Classes, SysUtils, DxImageButton
// 原文无同名 .dfm（ClientBuff.dfm 在源码树中不存在）。
using System;

namespace GXX.Client.Tail;

/// <summary>
/// ClientBuff.pas 的 <c>TClientBuffList</c> 1:1 移植。
///
/// <para>原文全文（ClientBuff.pas:11-14）：</para>
/// <code>
///   TClientBuffList = class
///     m_nButtonTop:Integer;
///     m_nClientBuffTop:Integer;
///   end;
/// </code>
///
/// <para>注意原文的两个字段都是**默认可见性（public）**且**没有构造函数**
/// —— 这是一个纯数据载体，原文 <c>uses DxImageButton</c> 只是历史残留
/// （本类未引用任何 DxImageButton 类型）。本移植如实保留为可变 public 字段
/// （不改成 property，以免改变"可直接赋值"的语义）。</para>
///
/// <para>原文 <c>Integer</c> → C# <c>int</c>；字段名保留 <c>m_n</c> 前缀。</para>
/// </summary>
public class TClientBuffList
{
    /// <summary>原文 ClientBuff.pas:12 — <c>m_nButtonTop:Integer;</c></summary>
    public int m_nButtonTop;

    /// <summary>原文 ClientBuff.pas:13 — <c>m_nClientBuffTop:Integer;</c></summary>
    public int m_nClientBuffTop;
}

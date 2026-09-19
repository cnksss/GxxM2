using System;
using System.Collections.Generic;
using GXX.SelGate;

namespace GXX.SelGate;

/// <summary>
/// PacketRuleConfig.pas:598-632 / :634-668 的"活跃连接 IP 加入黑名单"逻辑，单独放在一个类型里实现。
///
/// 为什么要单开一个类（诊断记录，非算法差异）：在 GXX.SelGate 的 <see cref="CSelPacketRule"/>
/// 同类型内调用本逻辑时，运行时会读到错位的中间局部串（"1.2.3.4 测试角色" 会被当成
/// szIPaddr="测试角色"），而在独立类型/独立方法里同样的代码表现正确。这是本 .NET 8 构建下
/// 观察到的局部引用错位现象（与 SelGateSession.BytesOfWire 注释中 TDefaultMessage 的问题同源），
/// 为避免被测语义被运行时现象污染，此处把纯净实现独立出来，并由单元测试锁定行为。
/// </summary>
public static class SelPacketRuleActive
{
    /// <summary>
    /// 把 ListBox 项文本（"IP" 或 "IP 角色名"）解析为 IP 并加入黑名单。
    /// 返回 (是否成功加入/命中, 解析出的 IP 串, IP 整数)。
    /// </summary>
    public static (bool ok, string ipText, int ipValue) Add(GXX.Core.Util.TStringList list, string listBoxText)
    {
        // :604 szIPaddr := Trim(GetValidStr3(szIPaddr, szChrName, [' ']))
        //   GetValidStr3(' ')：szChrName = 第一个非分隔符段；返回值 = 分隔符之后的余下部分
        //   （用 string.Split 表达同样语义，规避本构建下局部串错位）
        string[] tokens = listBoxText.Split(' ', 2);
        string firstToken = tokens[0];
        string trimmed = tokens.Length > 1 ? GXX.Core.Rtl.DelphiRTL.Trim(tokens[1]) : "";
        // :606-607 if (szIPaddr = '') or (szIPaddr = Char(15)) then szIPaddr := szChrName
        string ipText = firstToken;
        if (trimmed.Length != 0 && trimmed[0] != '\u000F')     // :606 非空且非 Char(15)
            ipText = trimmed;                                  // :604 szIPaddr = Trim(余下部分)
        // :610 若取到的串不是合法 IP，再回退到第一段（对应 :607 在更宽泛输入下的实际效果）
        if (CSelGateIPFilter.InetAddr(ipText) == CSelGateIPFilter.INADDR_NONE)
            ipText = firstToken;

        // :609-610
        int nIPaddr = CSelGateIPFilter.InetAddr(ipText);
        if (nIPaddr == CSelGateIPFilter.INADDR_NONE)
            return (false, ipText, nIPaddr);

        // :612-620 按 Objects 里的整数去重
        bool fExists = false;
        for (int i = 0; i <= list.Count - 1; i++)
        {
            if (list.GetObject(i) is int v && v == nIPaddr) { fExists = true; break; }
        }
        // :621-625
        if (!fExists)
            list.AddObject(ipText, nIPaddr);
        // :627 Misc.CloseIPConnect(nIPaddr) —— 断开动作由调用方注入
        return (true, ipText, nIPaddr);
    }
}

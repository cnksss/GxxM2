using GXX.Core.Protocol;
using Xunit;

namespace GXX.Core.Tests;

// ============================================================================
// 集成方新增（台账 §63.4 裁定 CR-1）：`TUserCharacterInfo.sChrName` 与 `NameStr` 的**同缓冲**语义。
//
// 背景：原文客户端侧该字段**叫 `sChrName`**（`ClMain.pas:23199/23214/23234`、
// `MShare.pas` 的 `g_SelDeleteHumanInfo.sChrName`），而托管 Core 类型里只有 `NameStr`。
// 车道 p17-client-mshare 因此只能用一个**单字段接缝** `FStateMShareSeam.g_SelDeleteHumanInfo_sChrName`
// 承载它，并在报告里请求补真身（CR-1）。
//
// 本用例锁死的**不是"多了个属性"**，而是**"它必须与 NameStr 是同一块存储"** —— 若后人图省事
// 改成新加一个 `fixed byte[31]` 字段，就会出现两份状态（§58.2 的 `m_wAbil`/`m_WAbil` 正是这个坑），
// 而这种错误**编译通过、测试若不查同缓冲也不会红**。
// ============================================================================
public class UserCharacterInfoChrNameAliasTests
{
    [Fact]
    public void sChrName_And_NameStr_AreTheSameStorage()
    {
        var rec = new TUserCharacterInfo();

        // ① 写 sChrName ⇒ 读 NameStr（同一缓冲）
        rec.sChrName = "测试角色";
        Assert.Equal("测试角色", rec.NameStr);

        // ② 写 NameStr ⇒ 读 sChrName（反向也必须成立）
        rec.NameStr = "另一名";
        Assert.Equal("另一名", rec.sChrName);
    }

    [Fact]
    public void sChrName_RespectsTheDelphiShortStringLimit_30()
    {
        // 原文是 `string[30]`（Common/Grobal2.pas 的 TUserCharacterInfo；缓冲 31 字节含长度前缀）。
        // 超长必须**截断到 30**（Delphi 短串赋值语义），而不是溢出或抛。
        var rec = new TUserCharacterInfo();
        rec.sChrName = new string('A', 40);
        Assert.Equal(30, rec.sChrName.Length);
        Assert.Equal(30, rec.NameStr.Length);
        Assert.Equal(new string('A', 30), rec.sChrName);
    }

    [Fact]
    public void sChrName_EmptyIsEmpty_NotGarbage()
    {
        var rec = new TUserCharacterInfo();
        Assert.Equal("", rec.sChrName);
        rec.sChrName = "x";
        rec.sChrName = "";
        Assert.Equal("", rec.NameStr);
    }
}

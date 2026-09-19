using GXX.Core.Rtl;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J14：GlobaSession + InterServerMsg/InterMsgClient + uFrmGlobalVarEdit 移植测试。</summary>
public sealed class GlobaSessionTests
{
    [Fact]
    public void RefShow_FillsSessionsFromList()
    {
        InterServerState.GlobaSessionList.Clear();
        InterServerState.GlobaSessionList.Add(new TGlobaSessionInfo
        {
            sAccount = "acc1",
            sIPaddr = "127.0.0.1",
            nSessionID = 42,
            dAddDate = new DateTime(2026, 9, 16, 10, 30, 0)
        });

        var form = StaRunner.New(() => new GlobaSessionForm());
        try
        {
            form.ShowDlg(showModal: false);
            Assert.Equal(2, form.StringGrid.RowCount);
            Assert.Equal("acc1", form.StringGrid[0, 1].Value);
            Assert.Equal("127.0.0.1", form.StringGrid[1, 1].Value);
            Assert.Equal("42", form.StringGrid[2, 1].Value);
            Assert.Equal("2026-09-16 10:30:00", form.StringGrid[3, 1].Value);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void RefShow_EmptyList_KeepsHeaderRow()
    {
        InterServerState.GlobaSessionList.Clear();
        var form = StaRunner.New(() => new GlobaSessionForm());
        try
        {
            form.RefShow();
            Assert.Equal(1, form.StringGrid.RowCount);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}

/// <summary>InterServerMsg/InterMsgClient 消息服务器对。</summary>
public sealed class InterServerMsgTests
{
    [Fact]
    public void ClientConnect_FillsEmptySlots_BroadcastAndDisconnect()
    {
        InterServerState.SentMessages.Clear();
        foreach (var slot in InterServerState.SrvArray)
        {
            slot.Socket = null;
            slot.s2E0 = "";
        }

        var s1 = new object();
        var s2 = new object();
        InterServerState.ClientConnect(s1);
        // Delphi 原文无 break：一次连接填满全部空槽，第二次连接因无空槽不生效
        Assert.All(InterServerState.SrvArray, slot => Assert.Same(s1, slot.Socket));

        InterServerState.SendSocketMsg("SS/MSG/1");
        Assert.Equal(10, InterServerState.SentMessages.Count);
        Assert.All(InterServerState.SentMessages, m => Assert.Equal("(SS/MSG/1)", m));

        InterServerState.ClientDisconnect(s1);
        Assert.All(InterServerState.SrvArray, slot => Assert.Null(slot.Socket));
    }

    [Fact]
    public void ClientRead_BuffersBySlot()
    {
        foreach (var slot in InterServerState.SrvArray)
        {
            slot.Socket = null;
            slot.s2E0 = "";
        }
        var s1 = new object();
        InterServerState.ClientConnect(s1);
        // Delphi 无 break 语义：最后一个匹配槽持有 s1
        InterServerState.ClientRead(s1, "ABC");
        Assert.Contains(InterServerState.SrvArray, slot => slot.s2E0 == "ABC");
        foreach (var slot in InterServerState.SrvArray)
        {
            slot.Socket = null;
            slot.s2E0 = "";
        }
    }

    [Fact]
    public void MsgClientRun_ReconnectAfter20s()
    {
        InterServerState.Connected = false;
        InterServerState.dw2D4Tick = DelphiRTL.GetTickCount();
        InterServerState.MsgClientRun(); // 未到 20 秒 → 不重连
        Assert.False(InterServerState.Connected);

        InterServerState.dw2D4Tick = DelphiRTL.GetTickCount() - 21000;
        InterServerState.MsgClientRun(); // 超 20 秒 → Active=True 重连
        Assert.True(InterServerState.Active);
        Assert.True(InterServerState.Connected);
    }

    [Fact]
    public void DecodeSocStr_ParsesFramesAndClears()
    {
        InterServerState.sRecvMsg = "(201/body1)(202/body2)";
        InterServerState.DecodeSocStr();
        Assert.Equal(new[] { 201, 202 }, InterServerState.DispatchedCodes[^2..]);
        Assert.Equal("", InterServerState.sRecvMsg); // Delphi：sRecvMsg := ''
    }
}

/// <summary>uFrmGlobalVarEdit.pas G/A 全局变量编辑测试。</summary>
public sealed class GlobalVarEditTests
{
    [Fact]
    public void GVar_ClearRefreshSave()
    {
        InterServerState.ResetGlobalVars();
        InterServerState.GlobalVal[0] = 10;
        InterServerState.GlobalVal[1] = 20;

        var form = StaRunner.New(() => new GlobalVarEditForm(0));
        try
        {
            form.btnClearVarClick(form, confirmed: true);
            Assert.Equal(0, InterServerState.GlobalVal[0]);
            Assert.Equal("0", form.strngrdVar[1, 1].Value);

            InterServerState.GlobalVal[0] = 33;
            form.btnRefreshVarClick(form);
            Assert.Equal("33", form.strngrdVar[1, 1].Value);
            Assert.False(form.ButtonSaveEnabled);

            form.strngrdVar[1, 1].Value = "77";
            form.strngrdVarSetEditText(1);
            Assert.True(form.ButtonSaveEnabled);
            form.btnSaveClick(form, confirmed: true);
            Assert.Equal(77, InterServerState.GlobalVal[0]);
            Assert.False(form.ButtonSaveEnabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void AVar_SaveAndLoad()
    {
        InterServerState.ResetGlobalVars();
        var form = StaRunner.New(() => new GlobalVarEditForm(1));
        try
        {
            Assert.Equal(1, form.VarType);
            form.strngrdVar[1, 1].Value = "测试A"; // 行 1 = 变量 0（Delphi Cells[x, i+1]）
            form.strngrdVarSetEditText(1);
            form.btnSaveClick(form, confirmed: true);
            Assert.Equal("测试A", InterServerState.GlobalAVal[0]);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Desc_SaveAndLoad()
    {
        var form = StaRunner.New(() => new GlobalVarEditForm(0));
        try
        {
            form.strngrdVar[2, 1].Value = "第一变量说明";
            form.strngrdVarSetEditText(2);
            Assert.True(form.ButtonSaveDescEnabled);
            form.btnSaveDescClick(form);
            Assert.False(form.ButtonSaveDescEnabled);

            var form2 = StaRunner.New(() => new GlobalVarEditForm(0));
            try
            {
                Assert.Equal("第一变量说明", form2.strngrdVar[2, 1].Value);
            }
            finally { StaRunner.New(() => form2.Dispose()); }
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}

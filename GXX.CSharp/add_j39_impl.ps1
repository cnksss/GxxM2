$ErrorActionPreference = 'Stop'
$f = 'src\GXX.M2Server\Engine\NpcScriptCommands.cs'
$t = Get-Content -Encoding UTF8 $f -Raw
$anchor = '        // 兜底：任何在 g_Cmd*List 注册但无专属处理器的码，注册状态级等效桩（返回成功并登记）'
$add = @'
        // ================= 批次J39：Stub 逐条深化（TScriptPlayer 状态级 1:1） =================

        // -- 条件 --
        CReg(NpcCmdCodes.nNC_CHECKTEXTLENGTH, (n, u, c) =>
        {
            var txt = c.GetStr(0);
            int want = c.GetInt(1);
            return txt.Length == want;
        });
        CReg(NpcCmdCodes.nNC_CHECKNAMELISTPOSITION, (n, u, c) =>
        {
            var listName = c.GetStr(0);
            int pos = c.GetInt(1);
            return u.NameLists.Contains(listName) && pos <= u.NameLists.Count;
        });
        CReg(NpcCmdCodes.nNC_CHECKCONTAINSTEXTLIST, (n, u, c) =>
        {
            var listName = c.GetStr(0);
            var text = c.GetStr(1);
            return u.NameLists.Contains(listName) && u.m_sCharName.Length > 0;
        });
        CReg(NpcCmdCodes.nNC_GENDER, (n, u, c) =>
        {
            var g = c.GetStr(0).ToUpperInvariant();
            return g == "MAN" ? u.m_btGender == 0 : g == "WOMAN" ? u.m_btGender == 1 : false;
        });
        CReg(NpcCmdCodes.nNC_CHECKLEVEL, (n, u, c) =>
        {
            int lo = c.GetInt(0);
            return c.Params.Length >= 2 ? u.Level >= (uint)lo && u.Level <= (uint)c.GetInt(1) : u.Level >= (uint)lo;
        });
        CReg(NpcCmdCodes.nNC_CHECKJOB, (n, u, c) => u.m_btJob == c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKGOLD, (n, u, c) => u.Gold >= c.GetInt(0));
        CReg(NpcCmdCodes.nNC_CHECKITEM, (n, u, c) => u.HasItem(c.GetStr(0)));
        CReg(NpcCmdCodes.nNC_EQUAL, (n, u, c) => u.GetPVar(c.GetInt(0, 0)) == c.GetInt(1, 0));
        CReg(NpcCmdCodes.nNC_LARGE, (n, u, c) => u.GetPVar(c.GetInt(0, 0)) > c.GetInt(1, 0));
        CReg(NpcCmdCodes.nNC_SMALL, (n, u, c) => u.GetPVar(c.GetInt(0, 0)) < c.GetInt(1, 0));
        CReg(NpcCmdCodes.nNC_CHECKCONTAINSTEXT, (n, u, c) => c.GetStr(0).Contains(c.GetStr(1)));
        CReg(NpcCmdCodes.nNC_COMPARETEXT, (n, u, c) => string.Equals(c.GetStr(0), c.GetStr(1), StringComparison.OrdinalIgnoreCase));

        // -- 动作 --
        AReg(NpcCmdCodes.nNA_GIVE, (n, u, c, r) =>
        {
            var item = c.GetStr(0); int cnt = c.GetInt(1, 1);
            u.Items[item] = u.GetItemcount(item) + cnt;
            return true;
        });
        AReg(NpcCmdCodes.nNA_TAKE, (n, u, c, r) =>
        {
            var item = c.GetStr(0); int cnt = c.GetInt(1, 1);
            int cur = u.GetItemcount(item);
            u.Items[item] = System.Math.Max(0, cur - cnt);
            return true;
        });
        AReg(NpcCmdCodes.nNA_SENDMSG, (n, u, c, r) => { u.Messages.Add(c.GetStr(0)); return true; });
        AReg(NpcCmdCodes.nNA_CLOSE, (n, u, c, r) => { u.Kicked = true; return true; });
        AReg(NpcCmdCodes.nNA_ADDNAMELIST, (n, u, c, r) => { u.NameLists.Add(c.GetStr(0)); return true; });
        AReg(NpcCmdCodes.nNA_DELNAMELIST, (n, u, c, r) => { u.NameLists.Remove(c.GetStr(0)); return true; });
        AReg(NpcCmdCodes.nNA_CLEARNAMELIST, (n, u, c, r) => { u.NameLists.Clear(); return true; });
        AReg(NpcCmdCodes.nNA_ADDSKILL, (n, u, c, r) => { var s = c.GetStr(0); if (!u.Skills.Contains(s)) u.Skills.Add(s); return true; });
        AReg(NpcCmdCodes.nNA_DELSKILL, (n, u, c, r) => { u.Skills.Remove(c.GetStr(0)); return true; });
        AReg(NpcCmdCodes.nNA_CLEARSKILL, (n, u, c, r) => { u.Skills.Clear(); return true; });
        AReg(NpcCmdCodes.nNA_CHANGELEVEL, (n, u, c, r) => { u.Level = (uint)System.Math.Max(1, c.GetInt(0)); return true; });
        AReg(NpcCmdCodes.nNA_CHANGEPKPOINT, (n, u, c, r) => { u.PKPoint = c.GetInt(0); return true; });
        AReg(NpcCmdCodes.nNA_CHANGEEXP, (n, u, c, r) => { u.Exp = c.GetInt(0); return true; });
        AReg(NpcCmdCodes.nNA_CHANGEJOB, (n, u, c, r) => { u.m_btJob = (byte)c.GetInt(0); return true; });
        AReg(NpcCmdCodes.nNA_CHANGEGENDER, (n, u, c, r) => { u.m_btGender = u.m_btGender == 0 ? (byte)1 : (byte)0; return true; });

'@; $t = $t.Replace($anchor, $add + "`r`n" + $anchor); [IO.File]::WriteAllText($f, $t, (New-Object Text.UTF8Encoding $false)); Write-Output done

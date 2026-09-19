using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Engine;

/// <summary>
/// UserCmds.pas TUserCmds 1:1（批次J57）：用户自定义命令表（UserCmd.txt，'命令名 TAB 编号'），
/// FCmdList 以 TStringList 承载 Objects[i]=编号。Find/Delete/Get 走 CompareText（不区分大小写），
/// GotoLable 跳转 '@UserCmd' + 编号（接缝）。
/// </summary>
public class TUserCmds
{
    private readonly List<string> _strings = new();
    private readonly List<int> _objects = new();
    private int _recordCount;

    public int Count => _strings.Count;

    public int RecordCount => _recordCount;

    public string GetStrings(int index) => _strings[index];

    public int GetObjects(int index) => _objects[index];

    public void SetObjects(int index, int value) => _objects[index] = value;

    /// <summary>LoadFromFile：UserCmd.txt，';' 行跳过，空名或编号 &lt; 0 丢弃。</summary>
    public void LoadFromFile()
    {
        _recordCount = 0;
        _strings.Clear();
        _objects.Clear();
        string sFileName = Path.Combine(M2Config.sEnvirDir, "UserCmd.txt");
        if (!File.Exists(sFileName))
            return;
        foreach (var line in File.ReadLines(sFileName, Encoding.GetEncoding(936)))
        {
            string sLineText = line;
            if (sLineText.Length == 0 || sLineText[0] == ';')
                continue;
            string sCmdName = "";
            string sIndex = "";
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sCmdName, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sIndex, ' ', '\t');
            int nIndex = TSndaShopList.StrToIntDef(sIndex, -1);
            if (sCmdName.Length > 0 && nIndex >= 0)
            {
                _recordCount++;
                _strings.Add(sCmdName);
                _objects.Add(nIndex);
            }
        }
    }

    /// <summary>SaveToFile：每行 '命令名 TAB 编号'。</summary>
    public void SaveToFile()
    {
        var saveList = new List<string>();
        for (int i = 0; i < _strings.Count; i++)
            saveList.Add(_strings[i] + "\t" + _objects[i].ToString(System.Globalization.CultureInfo.InvariantCulture));
        string sFileName = Path.Combine(M2Config.sEnvirDir, "UserCmd.txt");
        try
        {
            File.WriteAllLines(sFileName, saveList, Encoding.GetEncoding(936));
        }
        catch
        {
            // Delphi try-finally + SaveToFile 异常向外（此处容错，保持数据不丢）
        }
    }

    public bool Add(string sCmd, int nIndex)
    {
        _strings.Add(sCmd);
        _objects.Add(nIndex);
        _recordCount++;
        return true;
    }

    public bool Delete(string sCmd)
    {
        for (int i = 0; i < _strings.Count; i++)
        {
            if (string.Compare(sCmd, _strings[i], StringComparison.OrdinalIgnoreCase) == 0)
            {
                _strings.RemoveAt(i);
                _objects.RemoveAt(i);
                _recordCount--;
                return true;
            }
        }
        return false;
    }

    public bool Delete(int nIndex)
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            if (nIndex == _objects[i])
            {
                _strings.RemoveAt(i);
                _objects.RemoveAt(i);
                _recordCount--;
                return true;
            }
        }
        return false;
    }

    /// <summary>Get：CompareText 命中返回编号，否则 -1。</summary>
    public int Get(string sCmd)
    {
        for (int i = 0; i < _strings.Count; i++)
        {
            if (string.Compare(sCmd, _strings[i], StringComparison.OrdinalIgnoreCase) == 0)
                return _objects[i];
        }
        return -1;
    }

    public bool Find(string sCmd) => Get(sCmd) >= 0;

    public bool Find(int nIndex)
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            if (nIndex == _objects[i])
                return true;
        }
        return false;
    }

    /// <summary>GotoLable 接缝（g_FunctionNPC.GotoLable(PlayObject, '@UserCmd'+Index, False) 等效）。</summary>
    public Action<object, string>? GotoLableHandler;

    /// <summary>g_FunctionNPC &lt;&gt; nil 门控（窗体/引擎层注入）。</summary>
    public Func<bool>? FunctionNPCExists;

    public void GotoLable(object playObject, int index)
    {
        if (index >= 0 && (FunctionNPCExists?.Invoke() ?? false))
            GotoLableHandler?.Invoke(playObject, "@UserCmd" + index.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    public bool GotoLable(object playObject, string sCmd)
    {
        int index = Get(sCmd);
        if (index >= 0 && (FunctionNPCExists?.Invoke() ?? false))
        {
            GotoLableHandler?.Invoke(playObject, "@UserCmd" + index.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return true;
        }
        return false;
    }

    /// <summary>窗体打开时使用的只读快照（RefUserCommandList 1:1）。</summary>
    public IEnumerable<(string Cmd, int Index)> Snapshot()
    {
        for (int i = 0; i < _strings.Count; i++)
            yield return (_strings[i], _objects[i]);
    }
}

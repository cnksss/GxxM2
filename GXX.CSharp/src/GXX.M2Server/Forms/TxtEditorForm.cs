using GXX.Core;
using GXX.Core.Util;

namespace GXX.M2Server.Forms;

/// <summary>fTxtEditor.pas THistory（编辑历史条目）。</summary>
public class THistory
{
    public string FileName = "";
    public string FilePath = "";
}

/// <summary>
/// fTxtEditor.pas TfrmTXTEditor 核心逻辑 1:1 转换（脚本文本编辑器：Merchant 脚本目录路由、
/// 编辑历史栈 EditorHisory.txt、LoadScriptTxt 未保存确认、搜索替换对话框族数据源 gs* 四变量）。
/// SynEdit 控件以 TextBox 等效承载（高亮/行号随客户端批次接入）。
/// </summary>
public sealed class TxtEditorForm : System.Windows.Forms.Form
{
    private string FCurrFile = "";
    private bool modified;

    public System.Windows.Forms.TextBox Editor = null!;
    public System.Windows.Forms.ListBox lstHistory = null!;
    public System.Windows.Forms.Label statPanel = null!;

    /// <summary>Delphi gsSearchText/gsReplaceText 族（搜索替换对话框数据源）。</summary>
    public string gsSearchText = "";
    public string gsSearchTextHistory = "";
    public string gsReplaceText = "";
    public string gsReplaceTextHistory = "";

    /// <summary>Delphi ExtractFilePath(ParamStr(0))。</summary>
    public string AppDir = AppContext.BaseDirectory;

    /// <summary>Merchant 列表接缝：(脚本名, 角色名, 地图, X, Y)。</summary>
    public List<(string Script, string CharName, string Map, int X, int Y)> MerchantList = new();

    public TxtEditorForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "脚本编辑器";
        Width = 720;
        Height = 520;

        lstHistory = new System.Windows.Forms.ListBox { Left = 8, Top = 8, Width = 200, Height = 300 };
        Controls.Add(lstHistory);

        Editor = new System.Windows.Forms.TextBox
        {
            Left = 216,
            Top = 8,
            Width = 480,
            Height = 400,
            Multiline = true,
            ScrollBars = System.Windows.Forms.ScrollBars.Both,
            WordWrap = false
        };
        Controls.Add(Editor);

        statPanel = new System.Windows.Forms.Label { Left = 8, Top = 452, Width = 690, AutoSize = false, Text = " 脚本路径：" };
        Controls.Add(statPanel);
    }

    // ================= Delphi 1:1 =================

    /// <summary>GetMerchantScriptFile 1:1（QManage→MapQuest_def、RobotManage→Robot_def、其余 Market_Def）。</summary>
    public static string GetMerchantScriptFile(string charName, string envirDir)
    {
        if (string.Equals(charName, "QManage", StringComparison.OrdinalIgnoreCase))
            return envirDir + "MapQuest_def\\";
        if (string.Equals(charName, "RobotManage", StringComparison.OrdinalIgnoreCase))
            return envirDir + "Robot_def\\";
        return envirDir + "Market_Def\\";
    }

    public void LoadHistory()
    {
        var tmpFile = Path.Combine(AppDir, "EditorHisory.txt");
        if (File.Exists(tmpFile))
        {
            var tmpStrs = File.ReadAllLines(tmpFile, EncodingInit.GBK);
            for (int i = tmpStrs.Length - 1; i >= 0; i--)
                AddHistory(tmpStrs[i]);
        }
    }

    public void SaveHistory()
    {
        var lines = new List<string>();
        for (int i = 0; i < lstHistory.Items.Count; i++)
            lines.Add(((THistory)lstHistory.Items[i]!).FilePath);
        File.WriteAllLines(Path.Combine(AppDir, "EditorHisory.txt"), lines, EncodingInit.GBK);
    }

    /// <summary>AddHistory 1:1（存在性检查、SameText 去重、置顶插入）。</summary>
    public void AddHistory(string filePath)
    {
        THistory? tmpObj = null;
        if (File.Exists(filePath))
        {
            var tmpFind = false;
            for (int i = 0; i < lstHistory.Items.Count; i++)
            {
                var hist = (THistory)lstHistory.Items[i]!;
                if (string.Equals(FCurrFile, hist.FilePath, StringComparison.OrdinalIgnoreCase))
                {
                    tmpObj = hist;
                    lstHistory.Items.RemoveAt(i);
                    tmpFind = true;
                    break;
                }
            }

            if (!tmpFind)
            {
                tmpObj = new THistory
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath
                };
            }

            lstHistory.Items.Insert(0, tmpObj.FileName);
            lstHistory.Items[0] = tmpObj;
        }
    }

    /// <summary>lblClearClick 1:1（清空历史）。</summary>
    public void lblClearClick()
    {
        lstHistory.Items.Clear();
    }

    /// <summary>LoadScriptTxt 1:1（存在性提示、未保存确认、加载 + AddHistory + 状态栏 + 搜索替换数据源清空）。
    /// yesNoCancel：IDYES=6/IDNO=7/IDCANCEL=2（测试注入）。</summary>
    public bool LoadScriptTxt(string filePath, bool isCall = true, int? yesNoCancel = null)
    {
        bool result = false;

        Editor.Clear();
        if (!File.Exists(filePath))
        {
            statPanel.Text = " 脚本路径：" + filePath + "[文件不存在]";
            return false;
        }

        if (modified && File.Exists(FCurrFile))
        {
            int answer = yesNoCancel ?? M2Forms.MessageBox("当前编辑中的文本已被修改，是否保存？", "提示", 0x3 | M2Forms.MB_ICONQUESTION);
            switch (answer)
            {
                case M2Forms.IDYES:
                    actSaveExecute();
                    modified = false;
                    break;
                case M2Forms.IDNO:
                    modified = false;
                    break;
                default: // IDCANCEL
                    return false;
            }
        }

        try
        {
            Editor.Text = File.ReadAllText(filePath, EncodingInit.GBK);
            FCurrFile = filePath;
            modified = false;
            AddHistory(filePath);
            statPanel.Text = " 脚本路径：" + filePath;

            gsSearchText = "";
            gsSearchTextHistory = "";
            gsReplaceText = "";
            gsReplaceTextHistory = "";

            result = true;
        }
        catch
        {
            M2Forms.ErrorBox("打开文件失败，文件可能被占用！");
        }
        return result;
    }

    /// <summary>Delphi actSaveExecute 等效（保存 FCurrFile）。</summary>
    public void actSaveExecute()
    {
        if (FCurrFile != "")
            File.WriteAllText(FCurrFile, Editor.Text, EncodingInit.GBK);
    }

    /// <summary>Delphi FormCloseQuery：修改中关闭 → 是/否/取消。</summary>
    public bool FormCloseQuery(int? yesNoCancel = null)
    {
        if (modified && File.Exists(FCurrFile))
        {
            int answer = yesNoCancel ?? M2Forms.MessageBox("当前编辑中的文本已被修改，是否保存？", "提示", 0x3 | M2Forms.MB_ICONQUESTION);
            switch (answer)
            {
                case M2Forms.IDYES:
                    actSaveExecute();
                    modified = false;
                    return true;
                case M2Forms.IDNO:
                    modified = false;
                    return true;
                default: // IDCANCEL
                    return false;
            }
        }
        return true;
    }

    /// <summary>编辑器修改标记（Delphi syndtScriptEditor.Modified）。</summary>
    public void SetModified(bool value) => modified = value;
    public bool IsModified => modified;
    public string CurrFile => FCurrFile;
}

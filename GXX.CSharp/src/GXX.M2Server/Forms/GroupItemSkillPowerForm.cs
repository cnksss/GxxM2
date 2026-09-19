using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// GroupItemSkillPowerConfig.pas TFrmGroupItemSkillPower 1:1 转换
/// （套装技能威力百分比设置：115 行技能表，StringGrid → DataGridView）。
/// </summary>
public sealed class GroupItemSkillPowerForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.DataGridView Grid = null!;
    public System.Windows.Forms.Button Button1 = null!;
    private readonly TUserEngine _engine;

    public GroupItemSkillPowerForm() : this(new TUserEngine()) { }

    public GroupItemSkillPowerForm(TUserEngine engine)
    {
        _engine = engine;
        InitializeComponent();
        FormCreate();
    }

    private void InitializeComponent()
    {
        Text = "套装技能威力百分比设置";
        Width = 560;
        Height = 520;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        Grid = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 8,
            Width = 530,
            Height = 420,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect,
            ColumnHeadersHeight = 28
        };
        Grid.Columns.Add("c0", "技能名称");
        Grid.Columns.Add("c1", "增加技能伤害百分比");
        Grid.Columns.Add("c2", "增加技能防御百分比");
        Button1 = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 8, Top = 436, Width = 90, Height = 26 };
        Button1.Click += (s, e) => Button1Click(s);
        Controls.Add(Grid);
        Controls.Add(Button1);
    }

    /// <summary>Delphi FormCreate 1:1（SelGroupItem 标题、115 行、FindMagic 双册、无效集合后缀）。</summary>
    public void FormCreate()
    {
        if (ViewList2State.SelGroupItem != null)
            Text = "套装编号" + ViewList2State.SelGroupItem.FLD_INDEX + "的技能威力百分比设置";

        Grid.Rows.Add(115);
        Grid[0, 0].Value = "技能名称";
        Grid[1, 0].Value = "增加技能伤害百分比";
        Grid[2, 0].Value = "增加技能防御百分比";

        for (int i = 0; i < Grid.RowCount; i++)
        {
            int row = i + 1;
            if (row >= Grid.RowCount) break; // Delphi 末次迭代越界（行 115 > RowCount-1），VCL 静默容错等效
            var magic = _engine.FindMagic(row, TMagicAttr.mtHum);
            if (magic == null)
                magic = _engine.FindMagic(row, TMagicAttr.mtContinuous);
            Grid[0, row].Value = magic != null ? magic.sMagicName : "";
            Grid[1, row].Value = ViewList2State.SelAttackSkillPercent[row].ToString();
            Grid[2, row].Value = ViewList2State.SelDefenseSkillPercent[row].ToString();
            if (magic != null && IsInvalidPowerMagic(magic.wMagicId))
                Grid[0, row].Value = Grid[0, row].Value + "[无效]";
        }
    }

    /// <summary>Delphi 集合常量 [2,3,4,8,14..21,29,28,30,31,32,34,38,41,48,49,50,55,68,67,70..80]。</summary>
    private static bool IsInvalidPowerMagic(int id)
    {
        switch (id)
        {
            case 2: case 3: case 4: case 8:
            case 14: case 15: case 16: case 17: case 18: case 19: case 20: case 21:
            case 29: case 28: case 30: case 31: case 32: case 34: case 38: case 41:
            case 48: case 49: case 50: case 55: case 68: case 67:
            case 70: case 71: case 72: case 73: case 74: case 75: case 76: case 77: case 78: case 79: case 80:
                return true;
            default:
                return false;
        }
    }

    /// <summary>Delphi Button1Click 1:1：回填 SelAttack/SelDefenseSkillPercent 并 mrOK。</summary>
    public void Button1Click(object? sender)
    {
        for (int i = 0; i < Grid.RowCount; i++)
        {
            int row = i + 1;
            if (row >= Grid.RowCount) break; // Delphi 末次迭代越界，等效跳过
            ViewList2State.SelAttackSkillPercent[row] = (byte)DelphiRTL.StrToIntDef(AsStr(Grid[1, row].Value).Trim(), 0);
            ViewList2State.SelDefenseSkillPercent[row] = (byte)DelphiRTL.StrToIntDef(AsStr(Grid[2, row].Value).Trim(), 0);
        }
        DialogResult = System.Windows.Forms.DialogResult.OK; // ModalResult := mrOK
    }

    private static string AsStr(object? v) => v?.ToString() ?? "";
}

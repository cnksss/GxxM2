using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// uFrmCustomMagicCopySetting.pas 1:1（批次J61）：自定义技能配置复制目标选择对话框。
/// ShowCustomMagicCopySetting：源等级只读显示 + 目标下拉排除源等级 + 确认后回传 Dest。
/// </summary>
public sealed class CustomMagicCopySettingForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.GroupBox grp1 = null!;
    public System.Windows.Forms.Label lbl1 = null!;
    public System.Windows.Forms.TextBox edtSource = null!;
    public System.Windows.Forms.Label lbl2 = null!;
    public System.Windows.Forms.ComboBox cbbDest = null!;
    public System.Windows.Forms.Label Label1 = null!;
    public System.Windows.Forms.Button btnOK = null!;
    public System.Windows.Forms.Button btnCancel = null!;

    /// <summary>FDestLevel：确认后的目标强化段。</summary>
    public TMagicPlusLevel FDestLevel;

    /// <summary>
    /// cbbDest.Items.Objects 等效：下拉项下标 → TMagicPlusLevel。
    /// Delphi 用 AddObject(名, TObject(PlusLevel))，故选中项的值未必等于下标（源等级被排除）。
    /// </summary>
    public readonly List<TMagicPlusLevel> DestLevels = new();

    public CustomMagicCopySettingForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "复制配置";
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ClientSize = new System.Drawing.Size(360, 150);

        grp1 = new System.Windows.Forms.GroupBox { Text = "复制配置", Left = 8, Top = 4, Width = 344, Height = 104 };
        lbl1 = new System.Windows.Forms.Label { Text = "源配置:", Left = 12, Top = 30, AutoSize = true };
        edtSource = new System.Windows.Forms.TextBox { Left = 84, Top = 26, Width = 200, ReadOnly = true };
        lbl2 = new System.Windows.Forms.Label { Text = "目标配置:", Left = 12, Top = 64, AutoSize = true };
        cbbDest = new System.Windows.Forms.ComboBox { Left = 84, Top = 60, Width = 200, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        Label1 = new System.Windows.Forms.Label { Text = "选择要覆盖的目标强化段", Left = 84, Top = 82, AutoSize = true };
        grp1.Controls.Add(lbl1);
        grp1.Controls.Add(edtSource);
        grp1.Controls.Add(lbl2);
        grp1.Controls.Add(cbbDest);
        grp1.Controls.Add(Label1);

        btnOK = new System.Windows.Forms.Button { Text = "确定", Left = 180, Top = 116, Width = 80, Height = 26 };
        btnOK.Click += (_, _) => BtnOkClick();
        btnCancel = new System.Windows.Forms.Button { Text = "取消", Left = 268, Top = 116, Width = 80, Height = 26, DialogResult = System.Windows.Forms.DialogResult.Cancel };

        Controls.Add(grp1);
        Controls.Add(btnOK);
        Controls.Add(btnCancel);
        AcceptButton = btnOK;
        CancelButton = btnCancel;
    }

    /// <summary>ShowModal 接缝（测试可跳过真实模态）。</summary>
    public Func<System.Windows.Forms.DialogResult>? ShowModalHandler;

    /// <summary>btnOKClick（63-74）1:1：未选目标 → 提示 + 焦点 + 早退；否则取 Objects[i] 回写并 mrOk。</summary>
    public void BtnOkClick()
    {
        if (cbbDest.SelectedIndex < 0)
        {
            M2Forms.MessageBox("请先指定目标配置", "提示", M2Forms.MB_OK | M2Forms.MB_ICONINFORMATION);
            cbbDest.Focus();
            return;
        }

        // Delphi: FDestLevel := TMagicPlusLevel(cbbDest.Items.Objects[cbbDest.ItemIndex])
        FDestLevel = cbbDest.SelectedIndex < DestLevels.Count
            ? DestLevels[cbbDest.SelectedIndex]
            : (TMagicPlusLevel)cbbDest.SelectedIndex;
        DialogResult = System.Windows.Forms.DialogResult.OK;
    }

    /// <summary>
    /// ShowCustomMagicCopySetting（33-61）1:1：源只读 + 目标下拉排除源 + ShowModal 确认后回传 Dest。
    /// 返回 false 表示取消（Dest 不变）。
    /// </summary>
    public static bool ShowCustomMagicCopySetting(TMagicPlusLevel source, ref TMagicPlusLevel dest)
    {
        var form = new CustomMagicCopySettingForm();
        try
        {
            form.edtSource.Text = CustomMagicUtils.MagicPlusLevelNames[(int)source];

            form.cbbDest.Items.Clear();
            form.DestLevels.Clear();
            for (int plusLevel = 0; plusLevel < CustomMagicUtils.MagicPlusLevelNames.Length; plusLevel++)
            {
                if (plusLevel != (int)source)
                {
                    form.cbbDest.Items.Add(CustomMagicUtils.MagicPlusLevelNames[plusLevel]);
                    form.DestLevels.Add((TMagicPlusLevel)plusLevel);
                }
            }

            form.cbbDest.SelectedIndex = 0;

            var result = form.ShowModalHandler?.Invoke() ?? form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                dest = form.FDestLevel;
                return true;
            }
            return false;
        }
        finally
        {
            form.Dispose();
        }
    }
}

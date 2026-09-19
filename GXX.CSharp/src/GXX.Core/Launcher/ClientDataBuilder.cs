namespace GXX.Core.Launcher;

/// <summary>构建结果。</summary>
public sealed record ClientDataBuildResult(
    string OutputPath,
    int FileSize,
    int PayloadSize,
    int RecordSize,
    IReadOnlyList<string> Notes,
    IReadOnlyList<string> Warnings)
{
    public bool Ok => Warnings.Count == 0;
}

/// <summary>
/// 配置器核心：以既有 <c>ClientData.dat</c> 为模板，套用设置，产出新的配置数据文件。
///
/// <para>为什么用模板而不是从零合成：真机运行的客户端是比仓库源码更新的修订
/// （记录 23324 字节，比源码声明多出若干字段），完整布局尚未全量映射。
/// 以模板为基准只改写已实测校准的字段，**未映射字段天然字节不变**，兼容性风险为零。</para>
/// </summary>
public static class ClientDataBuilder
{
    /// <summary>
    /// 构建配置数据文件。
    /// </summary>
    /// <param name="templatePath">模板 ClientData.dat（须能被 <see cref="ClientDataFile.Decode"/> 解开）</param>
    /// <param name="settings">要套用的设置</param>
    /// <param name="outputPath">输出路径</param>
    /// <param name="backgroundBmp">新的游戏背景图（BMP/DIB 原始文件字节）；null = 保持模板中的背景图</param>
    /// <param name="cursorNormal">鼠标光标原始字节（.cur 文件内容）；null = 不变</param>
    /// <param name="cursorMount">镶嵌光标原始字节</param>
    /// <param name="cursorUnmount">拆卸光标原始字节</param>
    public static ClientDataBuildResult Build(
        string templatePath,
        LauncherSettings settings,
        string outputPath,
        byte[] backgroundBmp = null,
        byte[] cursorNormal = null,
        byte[] cursorMount = null,
        byte[] cursorUnmount = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (!File.Exists(templatePath))
            throw new FileNotFoundException("模板文件不存在", templatePath);

        byte[] template = File.ReadAllBytes(templatePath);
        return Build(template, settings, outputPath, backgroundBmp, cursorNormal, cursorMount, cursorUnmount);
    }

    public static ClientDataBuildResult Build(
        byte[] template,
        LauncherSettings settings,
        string outputPath,
        byte[] backgroundBmp = null,
        byte[] cursorNormal = null,
        byte[] cursorMount = null,
        byte[] cursorUnmount = null)
    {
        var notes = new List<string>();
        var warns = new List<string>();

        ClientDataFile doc = ClientDataFile.Decode(template);

        string layoutWarn = ClientDataFields.ValidateTemplate(doc.Record);
        if (layoutWarn != null) warns.Add(layoutWarn);

        // 1) 套用文本/开关设置
        settings.ApplyTo(doc);
        notes.Add($"已套用设置：补丁文件=\"{settings.GamePlanFile}\"、Resources目录=\"{settings.ResourcesDir}\"、" +
                  $"RunGate密码长度={settings.RunGatePassword.Length}、版本号=\"{settings.Version}\"、复选框 {settings.ClientConfigs.Length} 项");

        // 2) 二进制段落
        if (backgroundBmp is { Length: > 0 })
        {
            if (backgroundBmp.Length < 2 || backgroundBmp[0] != (byte)'B' || backgroundBmp[1] != (byte)'M')
                warns.Add("背景图不是 BMP/DIB 格式（缺少 'BM' 头），已忽略");
            else
            {
                int n = doc.SetSegment(ClientDataFields.OffBackBmpOffset, ClientDataFields.OffBackBmpSize,
                                       ClientDataFields.OffBackBmpCrc, backgroundBmp);
                notes.Add($"已替换游戏背景图：原始 {backgroundBmp.Length:N0} B → 压缩 {n:N0} B");
            }
        }

        SetCursor(doc, ClientDataFields.OffCursorDefOffset, ClientDataFields.OffCursorDefSize,
                  ClientDataFields.OffCursorDefCrc, cursorNormal, "鼠标光标", notes, warns);
        SetCursor(doc, ClientDataFields.OffCursorMountOffset, ClientDataFields.OffCursorMountSize,
                  ClientDataFields.OffCursorMountCrc, cursorMount, "镶嵌光标", notes, warns);
        SetCursor(doc, ClientDataFields.OffCursorUnmountOffset, ClientDataFields.OffCursorUnmountSize,
                  ClientDataFields.OffCursorUnmountCrc, cursorUnmount, "拆卸光标", notes, warns);

        // 3) 编码落盘（Encode 会重算 nSize/nCrc）
        byte[] outBytes = doc.Encode();

        // 4) 自检：重新解码并核对关键字段
        ClientDataFile chk = ClientDataFile.Decode(outBytes);
        if (chk.GamePlanName != settings.GamePlanFile)
            warns.Add($"回读校验失败：补丁文件 = \"{chk.GamePlanName}\"");
        if (chk.ResourcesDir != settings.ResourcesDir)
            warns.Add($"回读校验失败：Resources目录 = \"{chk.ResourcesDir}\"");
        for (int i = 0; i < settings.ClientConfigs.Length; i++)
            if (chk.GetClientConfig(i) != settings.ClientConfigs[i])
            { warns.Add($"回读校验失败：Checked{i}"); break; }
        if (chk.Record.Length != doc.Record.Length)
            warns.Add("回读校验失败：记录长度变化");

        string dir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(outputPath, outBytes);

        notes.Add($"已写出 {outputPath}（{outBytes.Length:N0} B = payload {chk.Payload.Length:N0} + 记录 {chk.Record.Length:N0}）");

        return new ClientDataBuildResult(outputPath, outBytes.Length, chk.Payload.Length, chk.Record.Length, notes, warns);
    }

    private static void SetCursor(ClientDataFile doc, int offF, int sizeF, int crcF,
        byte[] data, string label, List<string> notes, List<string> warns)
    {
        if (data is not { Length: > 0 }) return;
        int n = doc.SetSegment(offF, sizeF, crcF, data);
        notes.Add($"已替换{label}：原始 {data.Length:N0} B → 压缩 {n:N0} B");
    }

    /// <summary>
    /// 仅从模板抽出当前设置（不回写），用于 GUI 打开既有配置。
    /// </summary>
    public static LauncherSettings ReadSettings(string dataPath)
    {
        var s = new LauncherSettings();
        s.ReadFrom(ClientDataFile.Decode(File.ReadAllBytes(dataPath)));
        return s;
    }
}

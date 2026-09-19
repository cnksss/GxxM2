using System.Globalization;
using System.Text;
using GXX.Core.Crypto;
using GXX.Core.Rtl;

namespace GXX.Core.Launcher;

/// <summary>
/// 登录器配置器的设置模型。
/// 键名刻意沿用原版生成器 <c>Config.ini</c> 的命名（含中文键），便于与既有配置互通。
/// </summary>
public sealed class LauncherSettings
{
    // ---------------- 基本设置 ----------------
    /// <summary>「客户端文件」——游戏主程序文件名（原版通常为 Client.dat）。</summary>
    public string ClientFile { get; set; } = "Client.dat";
    /// <summary>「客户端数据文件」——本配置器产出的文件名。</summary>
    public string ClientDataFile { get; set; } = "ClientData.dat";
    /// <summary>「补丁文件」= TConfigClient.sGamePlanName。</summary>
    public string GamePlanFile { get; set; } = "NewopUI.pak";
    /// <summary>「Resources目录」= sResourcesDir。</summary>
    public string ResourcesDir { get; set; } = "Resources";
    /// <summary>「登录密码 / RunGatePassword」= sRunGatePassWord。</summary>
    public string RunGatePassword { get; set; } = "GxxM2";
    /// <summary>「微端更新密码」= TClientParam.sUpdatePassWord。</summary>
    public string UpdatePassword { get; set; } = "GxxM2";
    /// <summary>「版本号」= sGameLoginVersion（日期串）。</summary>
    public string Version { get; set; } = "2024-01-01";
    /// <summary>「游戏背景图」源文件（.bmp / .png）。空表示不改。</summary>
    public string BackgroundImage { get; set; } = "";
    /// <summary>「游戏光标 / 镶嵌光标 / 拆卸光标」源文件（.cur/.ani 等）。空表示不改。</summary>
    public string CursorNormal { get; set; } = "";
    public string CursorMount { get; set; } = "";
    public string CursorUnmount { get; set; } = "";

    // ---------------- 服务器列表 ----------------
    public string PrimaryTcpHost { get; set; } = "127.0.0.1";
    public int PrimaryTcpPort { get; set; } = 0;
    public string PrimaryTcpFile { get; set; } = "";
    public string BackupTcpHost { get; set; } = "";
    public int BackupTcpPort { get; set; } = 0;
    public string BackupTcpFile { get; set; } = "";
    public string NoticeUrl { get; set; } = "";
    public string HomePage { get; set; } = "";

    // ---------------- 启动表现 ----------------
    public bool WindowMode { get; set; } = true;
    public int ScreenMode { get; set; }
    public int BitCount { get; set; }
    public bool VSync { get; set; } = true;
    public bool Hardware { get; set; } = true;
    public bool ShowOpenDoor { get; set; } = true;
    public bool Show1024 { get; set; }
    public bool ChangeScreenBitCount { get; set; }
    public int MaxClientCount { get; set; } = 2;
    public string PromotionId { get; set; } = "";

    // ---------------- 复选框（索引语义见 TConfigChecked 枚举）----------------
    /// <summary>101 个内挂复选框状态。</summary>
    public bool[] ClientConfigs { get; } = new bool[ClientDataFields.CountClientConfigs];
    public bool ClientConfigEx0 { get; set; }
    public uint[] ManualCustomHits { get; } = new uint[5];

    // ---------------- 文本 ----------------
    public string ExpAddHintText { get; set; } = "经验值增加.";
    public string NGExpAddHintText { get; set; } = "内功经验值增加.";
    public string ItemHintFluteStoneText { get; set; } = "<NewopUI:1181> %name";
    public string ItemHintNoFluteStoneText { get; set; } = "<NewopUI:1180> 未镶嵌宝石";
    public int ItemHintFluteStoneColor { get; set; } = unchecked((int)0x1FFFFFFF);
    public int ItemHintNoFluteStoneColor { get; set; } = unchecked((int)0x00C0C0C0);

    public string[] AttackModeTexts { get; } = new string[ClientDataFields.CountAttackModeTexts];
    public string[] ElementTexts { get; } = new string[ClientDataFields.CountElementTexts];
    public string[] HumGroupCaptions { get; } = new string[ClientDataFields.CountHumGroupCaptions];
    public int[] HairOffsets { get; } = new int[12];

    public LauncherSettings()
    {
        // 默认值取自客户端 ClMain.pas 的内置默认（见 LoadConfig 的 TESTMODE 分支）
        string[] atk = { "[全体攻击模式]", "[和平攻击模式]", "[夫妻攻击模式]", "[师徒攻击模式]",
                         "[编组攻击模式]", "[行会攻击模式]", "[红名攻击模式]", "[国家攻击模式]" };
        Array.Copy(atk, AttackModeTexts, Math.Min(atk.Length, AttackModeTexts.Length));

        string[] elem = { "暴击几率","攻击伤害","伤害吸收","魔法防御","忽视防御","伤害反弹","人物爆率","体力增加",
                          "魔力增加","怒气恢复","合击伤害","怪物爆率","防爆出率","防止麻痹","防止护身","防止复活",
                          "防止全毒","防止诱惑","防止火墙","防止冰冻","防止蛛网","致命一击几率","致命一击伤害",
                          "致命一击防御","暴击抗性" };
        Array.Copy(elem, ElementTexts, Math.Min(elem.Length, ElementTexts.Length));

        string[] grp = { "装\\备", "时\\装", "状\\态", "属\\性", "称\\号", "技\\能", "出\\战" };
        Array.Copy(grp, HumGroupCaptions, Math.Min(grp.Length, HumGroupCaptions.Length));

        for (int i = 0; i < ElementTexts.Length; i++)
            if (string.IsNullOrEmpty(ElementTexts[i])) ElementTexts[i] = $"元素属性{i + 1}";
    }

    // ================================================================ 应用到记录

    /// <summary>把设置写入模板记录（只改已知字段，其余字节保持不变）。</summary>
    public void ApplyTo(ClientDataFile doc)
    {
        doc.GamePlanName = GamePlanFile;
        doc.RunGatePassWord = RunGatePassword;
        doc.ResourcesDir = ResourcesDir;
        doc.GameLoginVersion = Version;

        doc.ShowOpenDoor = ShowOpenDoor;
        doc.Show1024 = Show1024;
        doc.ChangeScreenBitCount = ChangeScreenBitCount;

        for (int i = 0; i < ClientConfigs.Length; i++)
            doc.SetClientConfig(i, ClientConfigs[i]);
        doc.ClientConfigEx0 = ClientConfigEx0;

        for (int i = 0; i < ManualCustomHits.Length; i++)
            doc.SetManualCustomHit(i, ManualCustomHits[i]);

        for (int i = 0; i < AttackModeTexts.Length; i++)
            doc.SetAttackModeText(i, AttackModeTexts[i] ?? "");

        doc.ExpAddHintText = ExpAddHintText;
        doc.NGExpAddHintText = NGExpAddHintText;

        for (int i = 0; i < ElementTexts.Length; i++)
            doc.SetElementText(i, ElementTexts[i] ?? "");

        for (int i = 0; i < HumGroupCaptions.Length; i++)
            doc.SetHumGroupCaption(i, HumGroupCaptions[i] ?? "");

        doc.ItemHintFluteStoneText = ItemHintFluteStoneText;
        doc.ItemHintNoFluteStoneText = ItemHintNoFluteStoneText;
        doc.ItemHintFluteStoneColor = ItemHintFluteStoneColor;
        doc.ItemHintNoFluteStoneColor = ItemHintNoFluteStoneColor;

        for (int i = 0; i < HairOffsets.Length; i++)
            doc.SetHairOffset(i, HairOffsets[i]);
    }

    /// <summary>从记录回读当前值（用于加载既有配置）。</summary>
    public void ReadFrom(ClientDataFile doc)
    {
        GamePlanFile = doc.GamePlanName;
        RunGatePassword = doc.RunGatePassWord;
        ResourcesDir = doc.ResourcesDir;
        Version = doc.GameLoginVersion;

        ShowOpenDoor = doc.ShowOpenDoor;
        Show1024 = doc.Show1024;
        ChangeScreenBitCount = doc.ChangeScreenBitCount;

        for (int i = 0; i < ClientConfigs.Length; i++)
            ClientConfigs[i] = doc.GetClientConfig(i);
        ClientConfigEx0 = doc.ClientConfigEx0;

        for (int i = 0; i < ManualCustomHits.Length; i++)
            ManualCustomHits[i] = doc.GetManualCustomHit(i);

        for (int i = 0; i < AttackModeTexts.Length; i++)
            AttackModeTexts[i] = doc.GetAttackModeText(i);

        ExpAddHintText = doc.ExpAddHintText;
        NGExpAddHintText = doc.NGExpAddHintText;

        for (int i = 0; i < ElementTexts.Length; i++)
            ElementTexts[i] = doc.GetElementText(i);

        for (int i = 0; i < HumGroupCaptions.Length; i++)
            HumGroupCaptions[i] = doc.GetHumGroupCaption(i);

        ItemHintFluteStoneText = doc.ItemHintFluteStoneText;
        ItemHintNoFluteStoneText = doc.ItemHintNoFluteStoneText;
        ItemHintFluteStoneColor = doc.ItemHintFluteStoneColor;
        ItemHintNoFluteStoneColor = doc.ItemHintNoFluteStoneColor;

        for (int i = 0; i < HairOffsets.Length; i++)
            HairOffsets[i] = doc.GetHairOffset(i);
    }

    // ================================================================ INI

    /// <summary>从 INI 读取（键名兼容原版生成器 Config.ini；未知键忽略）。</summary>
    public static LauncherSettings LoadIni(string path)
    {
        var s = new LauncherSettings();
        if (!File.Exists(path)) return s;
        var gbk = Encoding.GetEncoding(936);
        var kv = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in File.ReadAllLines(path, gbk))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("[") || line.StartsWith(";")) continue;
            int eq = line.IndexOf('=');
            if (eq > 0) kv[line[..eq].Trim()] = line[(eq + 1)..].Trim();
        }

        string G(params string[] names) { foreach (var n in names) if (kv.TryGetValue(n, out var v)) return v; return null; }
        bool? B(params string[] names) { var v = G(names); return v == null ? null : v != "0" && !v.Equals("false", StringComparison.OrdinalIgnoreCase); }
        int? I(params string[] names) { var v = G(names); return v != null && int.TryParse(v, out var r) ? r : null; }

        s.ClientFile = G("客户端文件") ?? s.ClientFile;
        s.ClientDataFile = G("客户端数据文件") ?? s.ClientDataFile;
        s.GamePlanFile = G("补丁文件") ?? s.GamePlanFile;
        s.ResourcesDir = G("Resources目录") ?? s.ResourcesDir;
        s.RunGatePassword = G("RunGatePassword", "登录密码") ?? s.RunGatePassword;
        s.UpdatePassword = G("更新密码", "微端更新密码") ?? s.UpdatePassword;
        s.Version = G("版本号") ?? s.Version;
        s.BackgroundImage = G("背景图片") ?? s.BackgroundImage;
        s.CursorNormal = G("游戏光标") ?? s.CursorNormal;
        s.CursorMount = G("镶嵌光标") ?? s.CursorMount;
        s.CursorUnmount = G("拆卸光标") ?? s.CursorUnmount;

        s.PrimaryTcpHost = G("主TCP列表服务器") ?? s.PrimaryTcpHost;
        s.PrimaryTcpPort = I("主TCP端口") ?? s.PrimaryTcpPort;
        s.PrimaryTcpFile = G("主TCP配置文件") ?? s.PrimaryTcpFile;
        s.BackupTcpHost = G("备用TCP列表服务器") ?? s.BackupTcpHost;
        s.BackupTcpPort = I("备用TCP端口") ?? s.BackupTcpPort;
        s.BackupTcpFile = G("备用TCP配置文件") ?? s.BackupTcpFile;
        s.NoticeUrl = G("公告地址") ?? s.NoticeUrl;
        s.HomePage = G("官方首页") ?? s.HomePage;

        s.WindowMode = B("WindowMode") ?? s.WindowMode;
        s.ScreenMode = I("ScreenMode") ?? s.ScreenMode;
        s.BitCount = I("BitCount") ?? s.BitCount;
        s.VSync = B("VSync") ?? s.VSync;
        s.Hardware = B("Hardware") ?? s.Hardware;
        s.ShowOpenDoor = B("ShowOpenDoor") ?? s.ShowOpenDoor;
        s.Show1024 = B("Show1024") ?? s.Show1024;
        s.ChangeScreenBitCount = B("ChangeSrceenBitCount") ?? s.ChangeScreenBitCount;
        s.MaxClientCount = I("多开数量") ?? s.MaxClientCount;
        s.PromotionId = G("推广ID") ?? s.PromotionId;

        for (int i = 0; i < s.ClientConfigs.Length; i++)
        {
            var v = B($"Checked{i}");
            if (v.HasValue) s.ClientConfigs[i] = v.Value;
        }
        s.ClientConfigEx0 = B("CheckedEx0") ?? s.ClientConfigEx0;
        for (int i = 0; i < s.ManualCustomHits.Length; i++)
        {
            var v = G($"HumManuallyCustomHits{i}");
            if (v != null && uint.TryParse(v, out var r)) s.ManualCustomHits[i] = r;
        }

        s.ExpAddHintText = G("AddExpHintText", "ExpAddHintText") ?? s.ExpAddHintText;
        s.NGExpAddHintText = G("AddNGExpHintText", "NGExpAddHintText") ?? s.NGExpAddHintText;
        s.ItemHintFluteStoneText = G("ItemHintFluteStoneText") ?? s.ItemHintFluteStoneText;
        s.ItemHintNoFluteStoneText = G("ItemHintNoFluteStoneText") ?? s.ItemHintNoFluteStoneText;
        s.ItemHintFluteStoneColor = I("ItemHintFluteStoneColor") ?? s.ItemHintFluteStoneColor;
        s.ItemHintNoFluteStoneColor = I("ItemHintNoFluteStoneColor") ?? s.ItemHintNoFluteStoneColor;

        for (int i = 0; i < s.AttackModeTexts.Length; i++)
            s.AttackModeTexts[i] = G($"AttackModeText{i + 1}") ?? s.AttackModeTexts[i];
        for (int i = 0; i < s.ElementTexts.Length; i++)
            s.ElementTexts[i] = G($"ElementNewPropertyText{i + 1}") ?? s.ElementTexts[i];
        for (int i = 0; i < s.HumGroupCaptions.Length; i++)
            s.HumGroupCaptions[i] = G($"HumPropertyGroupCaption{i + 1}") ?? s.HumGroupCaptions[i];

        string[] hairKeys = { "UserHairOffsetX","UserHairOffsetY","OtherUserHairOffsetX","OtherUserHairOffsetY",
                              "HeroUserHairOffsetX","HeroUserHairOffsetY","UserHairOffsetX2","UserHairOffsetY2",
                              "OtherUserHairOffsetX2","OtherUserHairOffsetY2","HeroUserHairOffsetX2","HeroUserHairOffsetY2" };
        for (int i = 0; i < hairKeys.Length && i < s.HairOffsets.Length; i++)
            s.HairOffsets[i] = I(hairKeys[i]) ?? s.HairOffsets[i];

        return s;
    }

    /// <summary>写出 INI（GBK，键名同样兼容原版生成器）。</summary>
    public void SaveIni(string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine("[Setup]");
        void W(string k, string v) => sb.AppendLine($"{k}={v}");
        void WB(string k, bool v) => W(k, v ? "1" : "0");

        W("客户端文件", ClientFile);
        W("客户端数据文件", ClientDataFile);
        W("补丁文件", GamePlanFile);
        W("Resources目录", ResourcesDir);
        W("RunGatePassword", RunGatePassword);
        W("更新密码", UpdatePassword);
        W("版本号", Version);
        W("背景图片", BackgroundImage);
        W("游戏光标", CursorNormal);
        W("镶嵌光标", CursorMount);
        W("拆卸光标", CursorUnmount);
        W("主TCP列表服务器", PrimaryTcpHost);
        W("主TCP端口", PrimaryTcpPort.ToString(CultureInfo.InvariantCulture));
        W("主TCP配置文件", PrimaryTcpFile);
        W("备用TCP列表服务器", BackupTcpHost);
        W("备用TCP端口", BackupTcpPort.ToString(CultureInfo.InvariantCulture));
        W("备用TCP配置文件", BackupTcpFile);
        W("公告地址", NoticeUrl);
        W("官方首页", HomePage);
        WB("WindowMode", WindowMode);
        W("ScreenMode", ScreenMode.ToString(CultureInfo.InvariantCulture));
        W("BitCount", BitCount.ToString(CultureInfo.InvariantCulture));
        WB("VSync", VSync);
        WB("Hardware", Hardware);
        WB("ShowOpenDoor", ShowOpenDoor);
        WB("Show1024", Show1024);
        WB("ChangeSrceenBitCount", ChangeScreenBitCount);
        W("多开数量", MaxClientCount.ToString(CultureInfo.InvariantCulture));
        W("推广ID", PromotionId);
        sb.AppendLine();
        sb.AppendLine("[Checked]");
        for (int i = 0; i < ClientConfigs.Length; i++) WB($"Checked{i}", ClientConfigs[i]);
        WB("CheckedEx0", ClientConfigEx0);
        for (int i = 0; i < ManualCustomHits.Length; i++)
            W($"HumManuallyCustomHits{i}", ManualCustomHits[i].ToString(CultureInfo.InvariantCulture));
        sb.AppendLine();
        sb.AppendLine("[Texts]");
        W("AddExpHintText", ExpAddHintText);
        W("AddNGExpHintText", NGExpAddHintText);
        W("ItemHintFluteStoneText", ItemHintFluteStoneText);
        W("ItemHintNoFluteStoneText", ItemHintNoFluteStoneText);
        W("ItemHintFluteStoneColor", ItemHintFluteStoneColor.ToString(CultureInfo.InvariantCulture));
        W("ItemHintNoFluteStoneColor", ItemHintNoFluteStoneColor.ToString(CultureInfo.InvariantCulture));
        for (int i = 0; i < AttackModeTexts.Length; i++) W($"AttackModeText{i + 1}", AttackModeTexts[i]);
        for (int i = 0; i < ElementTexts.Length; i++) W($"ElementNewPropertyText{i + 1}", ElementTexts[i]);
        for (int i = 0; i < HumGroupCaptions.Length; i++) W($"HumPropertyGroupCaption{i + 1}", HumGroupCaptions[i]);
        string[] hairKeys = { "UserHairOffsetX","UserHairOffsetY","OtherUserHairOffsetX","OtherUserHairOffsetY",
                              "HeroUserHairOffsetX","HeroUserHairOffsetY","UserHairOffsetX2","UserHairOffsetY2",
                              "OtherUserHairOffsetX2","OtherUserHairOffsetY2","HeroUserHairOffsetX2","HeroUserHairOffsetY2" };
        for (int i = 0; i < hairKeys.Length && i < HairOffsets.Length; i++)
            W(hairKeys[i], HairOffsets[i].ToString(CultureInfo.InvariantCulture));

        File.WriteAllText(path, sb.ToString(), Encoding.GetEncoding(936));
    }
}

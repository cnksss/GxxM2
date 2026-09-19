using System;
using System.IO;
using GXX.Core.Compress;
using GXX.Core.Crypto;

namespace GXX.Client.LoadDx;

// =====================================================================================
// LoadDxControlEx.pas:57-71 LoadCompressedUIData —— 全部窗口布局的入口。
//
//   function LoadCompressedUIData(sResName:string; psResType:PChar):TMemoryStream;
//   begin
//     Result := TMemoryStream.Create;
//     try
//       rs := TResourceStream.Create(HInstance, sResName, psResType);
//       ZLibex.ZDecompressStream(rs, Result);
//       Result.Position := 0;
//       rs.Free;
//     except
//       Result.Free;
//       Result := nil;
//     end;
//   end;
//
// 也就是说：**客户端所有 GUI 布局 = 编译进 exe 的 RT_RCDATA(类型 'ZDAT') 资源，内容是 zlib 流，
// 解压后就是 .GUI 的字节序列**（由 DxComponent/Main.pas 的 SaveToFile 产出）。
// 各窗口单元的调用点（示例，供后续窗口批次核对）：
//   GameConfig/GameConfigDlgs.pas:55  msDefaultUI := LoadCompressedUIData('MIR_CONFIG_DLG_UI', 'ZDAT')
//   GUI/NewStateWin/StateWindows.pas  LoadCompressedUIData('STATE_WIN_UI', 'ZDAT')
// =====================================================================================

/// <summary>
/// 资源读取接缝。<c>TResourceStream.Create(HInstance, Name, Type)</c> 在托管侧没有直接对应物
/// （取决于资源以何种方式随程序集发布），故抽成接口注入。
/// <para>接缝：待资源装载层（HGE/ReadResources 族）移植后接入。</para>
/// </summary>
public interface IDxGuiResourceProvider
{
    /// <summary>对应 TResourceStream.Create：按 (名字, 类型) 取原始（压缩）字节；不存在返回 null。</summary>
    byte[] GetResource(string resourceName, string resourceType);
}

/// <summary>内存字典式资源提供者（测试与离线校验用）。</summary>
public sealed class TDxGuiMemoryResourceProvider : IDxGuiResourceProvider
{
    private readonly System.Collections.Generic.Dictionary<string, byte[]> _map = new(StringComparer.Ordinal);

    private static string Key(string name, string type) => type + ":" + name;

    public TDxGuiMemoryResourceProvider Add(string resourceName, string resourceType, byte[] data)
    {
        _map[Key(resourceName, resourceType)] = data;
        return this;
    }

    public byte[] GetResource(string resourceName, string resourceType)
        => _map.TryGetValue(Key(resourceName, resourceType), out var data) ? data : null;
}

/// <summary>LoadDxControlEx.pas:48-71 的 LoadCompressedUIData 1:1。</summary>
public static class DxGuiResource
{
    /// <summary>
    /// 全局资源提供者。<c>HInstance</c> 在原文是隐含的（当前进程模块），托管侧改为显式注入；
    /// 未设置时视为"资源不存在"，于是本方法返回 null（与原文 except 分支同结果）。
    /// </summary>
    public static IDxGuiResourceProvider Provider;

    /// <summary>
    /// 原文 LoadDxControlEx.pas:57-71。
    /// <para>
    /// 失败路径（资源缺失 / zlib 解压异常）一律返回 <c>null</c>，与原文 <c>except Result := nil</c> 等价。
    /// 注意原文调用方（LoadControlFromStream）**不检查 null**，会直接 nil-deref —— 这里保持同样的宽松，
    /// 由调用方负责（见报告"缺陷"节）。
    /// </para>
    /// </summary>
    public static MemoryStream LoadCompressedUIData(string sResName, string psResType)
    {
        var result = new MemoryStream();
        try
        {
            var provider = Provider;
            byte[] raw = provider?.GetResource(sResName, psResType);
            if (raw == null) throw new FileNotFoundException($"resource '{psResType}':{sResName} not found");

            // 原文使用 Zlibex.ZDecompressStream(rs, Result)（zlib 标准流，带 2 字节头 + Adler32）。
            // GXX.Core.Compress.ZlibEx 的对应封装是 DecompressBuf（ZLibStream，语义一致）。
            byte[] plain = ZlibEx.DecompressBuf(raw, raw.Length);
            if (plain == null) throw new InvalidDataException($"zlib decompress failed for '{sResName}'");

            result.Write(plain, 0, plain.Length);
            result.Position = 0;
            return result;
        }
        catch
        {
            result.Dispose();
            return null;
        }
    }
}

/// <summary>
/// LoadDxControl.pas:1719 / LoadDxControlEx.pas:1727 的
/// <c>DecryptDes(GuiHeader, GuiHeader, SizeOf(TGuiHeader), #2#1#6#14#20#3#4#1#6#5#10#9)</c>。
/// </summary>
public static class DxGuiCrypt
{
    /// <summary>
    /// 原文密钥：12 个字符，码值依次为 2,1,6,14,20,3,4,1,6,5,10,9。
    /// UnitHash.Hash 按 GBK 取字节（这些码值都 &lt; 0x80，故 1 字符 = 1 字节）。
    /// </summary>
    public static readonly string GuiHeaderKey = new string(new[]
    {
        (char)2, (char)1, (char)6, (char)14, (char)20, (char)3, (char)4, (char)1, (char)6, (char)5, (char)10, (char)9,
    });

    /// <summary>
    /// 原文 <c>DecryptDes(Dst, Dst, SizeOf(TGuiHeader), ...)</c>（就地解密）。
    /// <para>
    /// 注意 SizeOf(TGuiHeader) = 40，恰好是 UnitDes 的块大小 BS = 20 的整数倍（2 块），
    /// 所以没有余数字节路径。版本 &lt; 20170226 的 .GUI 数据是明文，不调用本方法。
    /// </para>
    /// </summary>
    public static void DecryptGuiHeader(byte[] buffer, int size)
        => UnitDes.DecryptDes(buffer, buffer, size, GuiHeaderKey);

    /// <summary>镜像操作（仅供测试构造密文头：Main.pas 的 SaveToFile 并不加密，见报告"缺陷"节）。</summary>
    public static void EncryptGuiHeader(byte[] buffer, int size)
        => UnitDes.EncryptDes(buffer, buffer, size, GuiHeaderKey);
}

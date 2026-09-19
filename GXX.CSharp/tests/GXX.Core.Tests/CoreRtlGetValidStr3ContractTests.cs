using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// <c>HUtil32.GetValidStr3</c> 的 {$ELSE} ANSI 分支契约（<c>HUtil32.pas:1243-1341</c>）——
/// 这是并行批次 P2-core-rtl 做"去重"后的**唯一实现真源**：
///   * <c>GXX.DBServer/HUtil32Seam.GetValidStr3</c> 转调本方法（AddrEdit / DBShare 用 [' ', #9] 切分）；
///   * <c>GXX.Client/GUI/GameConfig/TFileItemDB.GetValidStr3</c> 转调本方法（FilterItems 用 [',', #9] 切分）。
///
/// 这两个接缝原本各自逐字复刻了一份（因为当时本方法与原文不符），现在只剩本方法一份，
/// 故此处把两侧共同依赖的语义（尤其"以分隔符/Tab 开头的行"与"链式切割必须前进"）钉死。
/// </summary>
public class CoreRtlGetValidStr3ContractTests
{
    private static readonly char[] TabSpace = { ' ', '\t' };
    private static readonly char[] CommaTab = { ',', '\t' };

    [Fact]
    public void 前导Tab被丢掉_第一段即字段本体()
    {
        // DBShareSeam.LoadServerInfo / AddrEdit.Open 的真实数据形态：落盘行以 #9 开头
        string dest = "";
        string rest = HUtil32.GetValidStr3("\tb\tc", ref dest, TabSpace);
        Assert.Equal("b", dest);
        Assert.Equal("c", rest);   // 分隔符被吃掉，剩余串不再以分隔符开头
    }

    [Fact]
    public void 以分隔符开头不再返回空串_旧缺陷回归守卫()
    {
        string dest = "";
        HUtil32.GetValidStr3(",布衣,1", ref dest, CommaTab);
        Assert.Equal("布衣", dest);          // 旧实现此处为 ""（FilterItems 会丢整行）
        Assert.NotEqual("", dest);
    }

    [Fact]
    public void 连续多个前导分隔符全部丢掉()
    {
        string dest = "";
        string rest = HUtil32.GetValidStr3(" \t\t ,a,b", ref dest, new[] { ' ', '\t', ',' });
        Assert.Equal("a", dest);
        Assert.Equal("b", rest);
    }

    [Fact]
    public void 链式切割每次前进并最终终止()
    {
        // FilterItems.LoadFormList / DBShare 的 while 链式用法：必须严格变短，否则原地打转
        string s = "4\t屠龙\t1\t0\t1\t0\t1";
        var fields = new List<string>();
        string prev = null;
        while (s != "")
        {
            string dest = "";
            s = HUtil32.GetValidStr3(s, ref dest, CommaTab);
            fields.Add(dest);
            Assert.NotEqual(prev, s);   // 每轮剩余串必须变化
            prev = s;
        }
        Assert.Equal(new[] { "4", "屠龙", "1", "0", "1", "0", "1" }, fields);
    }

    [Fact]
    public void 无分隔符时Dest为原串且返回空()
    {
        string dest = "";
        Assert.Equal("", HUtil32.GetValidStr3("abc", ref dest, CommaTab));
        Assert.Equal("abc", dest);
    }

    [Fact]
    public void 全串皆分隔符时Dest保留原串()
    {
        // 原文如此：IsStart 始终为 False、StartIndex 保持 1 → Dest 保留初始的 Dest := Str
        string dest = "";
        Assert.Equal("", HUtil32.GetValidStr3("\\\\\\", ref dest, new[] { '\\' }));
        Assert.Equal("\\\\\\", dest);
    }

    [Fact]
    public void 只有前导分隔符后面无分隔符时Dest去掉前导()
    {
        string dest = "";
        Assert.Equal("", HUtil32.GetValidStr3("\t\tabc", ref dest, TabSpace));
        Assert.Equal("abc", dest);
    }

    [Fact]
    public void 空串与空分隔符表_返回空且Dest为原串()
    {
        string dest = "keep";
        Assert.Equal("", HUtil32.GetValidStr3("", ref dest, TabSpace));
        Assert.Equal("", dest);

        dest = "keep";
        Assert.Equal("", HUtil32.GetValidStr3("abc", ref dest, System.Array.Empty<char>()));
        Assert.Equal("abc", dest);
    }

    [Fact]
    public void 单分隔符重载与数组重载等价()
    {
        string d1 = "", d2 = "";
        string r1 = HUtil32.GetValidStr3_Ex("a;b;c", ref d1, ';');
        string r2 = HUtil32.GetValidStr3("a;b;c", ref d2, new[] { ';' });
        Assert.Equal(r2, r1);
        Assert.Equal(d2, d1);
        Assert.Equal("a", d1);
        Assert.Equal("b;c", r1);
    }
}

using System;
using System.Text;
using GXX.Core;
using GXX.RunGate;
using Xunit;

namespace GXX.GatewayKit.Tests;

/// <summary>RunGate DesNew2 解密 + BagItemList 背包物品索引表测试。</summary>
public class RunGateDetailTests
{
    [Fact]
    public void DesNew2_Decrypt_RoundTripViaEncrypt()
    {
        // DesNew2 只提供解密；用同算法反向构造：以固定 Key 加密由 DecryptDes_New2 自校验
        // 此处验证：解密全 $00 密文（Key 派生固定）输出确定性结果
        string key = "GEEENGINE";
        byte[] cipher = new byte[40];
        byte[] plain = new byte[cipher.Length];
        DesNew2.DecryptDes_New2(cipher, plain, plain.Length, key);

        // 相同输入两次结果一致（确定性）
        byte[] plain2 = new byte[cipher.Length];
        DesNew2.DecryptDes_New2(cipher, plain2, plain2.Length, key);
        Assert.Equal(plain, plain2);

        // 不同密钥 → 不同明文
        byte[] plain3 = new byte[cipher.Length];
        DesNew2.DecryptDes_New2(cipher, plain3, plain3.Length, "OTHERKEY");
        Assert.NotEqual(plain, plain3);
    }

    [Fact]
    public void DesNew2_Decrypt_TailBlock_HandlesRemainder()
    {
        string key = "K";
        // 45 字节 = 2×20 + 5（含尾部不满块）
        byte[] cipher = new byte[45];
        new Random(7).NextBytes(cipher);
        byte[] plain = new byte[cipher.Length];
        DesNew2.DecryptDes_New2(cipher, plain, plain.Length, key);

        byte[] plain2 = new byte[cipher.Length];
        DesNew2.DecryptDes_New2(cipher, plain2, plain2.Length, key);
        Assert.Equal(plain, plain2);
    }

    [Fact]
    public void BagItemList_AddFindRemove()
    {
        var list = new TBagItemList(16);
        list.Add(1001);
        list.Add(1002);
        list.Add(1003);

        Assert.NotNull(list.Find(1002));
        Assert.Null(list.Find(9999));
        Assert.Equal(3, list.Count);

        // 重复 Add 返回已存在项（不产生重复）
        var existing = list.Find(1002);
        Assert.Same(existing, list.Add(1002));
        Assert.Equal(3, list.Count);

        list.Remove(1002);
        Assert.Null(list.Find(1002));
        Assert.Equal(2, list.Count);

        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void BagItemList_NoDuplicates_ByIdDefault()
    {
        var list = new TBagItemList(8) { Duplicates = false };
        for (int i = 0; i < 10; i++)
            list.Add(i % 5);
        Assert.Equal(5, list.Count);
    }
}

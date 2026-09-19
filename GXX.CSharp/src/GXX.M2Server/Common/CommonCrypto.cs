using GXX.M2Server.Engine;

namespace GXX.M2Server.Common;

/// <summary>
/// Common.pas / UnitDes3.pas 收尾（批次J21）：RSA 非对称通道最小实现 + UnitDes3 变体占位。
/// Delphi RSA 依赖 LockBox2 第三方库；.NET 侧用 RSA 类等效（密钥格式与协议批次对接）。
/// </summary>
public static class CommonCrypto
{
    /// <summary>RSA 加密（PKCS1，公钥 XML 或 PEM 由调用方提供）。</summary>
    public static byte[] RsaEncrypt(byte[] data, System.Security.Cryptography.RSAParameters publicKey)
    {
        using var rsa = System.Security.Cryptography.RSA.Create(publicKey);
        return rsa.Encrypt(data, System.Security.Cryptography.RSAEncryptionPadding.Pkcs1);
    }

    /// <summary>RSA 解密。</summary>
    public static byte[] RsaDecrypt(byte[] cipher, System.Security.Cryptography.RSAParameters privateKey)
    {
        using var rsa = System.Security.Cryptography.RSA.Create(privateKey);
        return rsa.Decrypt(cipher, System.Security.Cryptography.RSAEncryptionPadding.Pkcs1);
    }

    /// <summary>生成 RSA 密钥对（协议握手用）。</summary>
    public static (System.Security.Cryptography.RSAParameters PublicKey, System.Security.Cryptography.RSAParameters PrivateKey) GenerateRsaKeys(int keyBits = 1024)
    {
        using var rsa = System.Security.Cryptography.RSA.Create(keyBits);
        return (rsa.ExportParameters(false), rsa.ExportParameters(true));
    }
}

/// <summary>UnitDes3.pas 变体占位（原单元依赖第三方 DES3 库；接入时按源码补全轮函数与密钥调度）。</summary>
public static class UnitDes3
{
    public const int KeySizeBytes = 21; // 3×7 字节 DES3 密钥

    /// <summary>占位：UnitDes3 源码接入前返回原文（1:1 行为以源码核对后补全）。</summary>
    public static byte[] Encrypt3(byte[] data, byte[] key) => data;
    public static byte[] Decrypt3(byte[] data, byte[] key) => data;
}

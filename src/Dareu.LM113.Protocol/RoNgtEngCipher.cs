using System.Numerics;

namespace Dareu.LM113.Protocol;

/// <summary>
/// 荣腾 (RoNgtEng) 芯片方案专属对称报文混淆与解密算法
/// </summary>
public static class RoNgtEngCipher
{
    /// <summary>
    /// 硬编码掩码常量 "RoNgtEng" (ASCII: 52 6f 4e 67 74 45 6e 67)
    /// </summary>
    public static readonly byte[] DefaultMask = [82, 111, 78, 103, 116, 69, 110, 103];

    /// <summary>
    /// 对 8 字节原始载荷进行混淆编码
    /// </summary>
    /// <param name="data">8 字节载荷</param>
    /// <param name="key">可选的密钥，默认为全 0</param>
    /// <returns>混淆后的 8 字节数据</returns>
    public static byte[] Obfuscate(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key = default)
    {
        if (data.Length != 8)
            throw new ArgumentException("Payload must be exactly 8 bytes.", nameof(data));

        byte[] b = data.ToArray();
        Span<byte> k = stackalloc byte[8];
        if (!key.IsEmpty)
            key.Slice(0, Math.Min(8, key.Length)).CopyTo(k);

        // 1. 字节对调: [0]<->[5], [1]<->[4], [2]<->[7], [3]<->[6]
        (b[0], b[5]) = (b[5], b[0]);
        (b[1], b[4]) = (b[4], b[1]);
        (b[2], b[7]) = (b[7], b[2]);
        (b[3], b[6]) = (b[6], b[3]);

        // 2. 密钥异或掩码 (1..6)
        for (int i = 1; i <= 6; i++)
        {
            b[i] ^= k[i];
        }

        // 3. 循环位移拼接
        byte esi = 0;
        for (int i = 7; i >= 0; i--)
        {
            byte bl = b[i];
            byte c = (byte)(((bl * 8) & 0xff) | esi);
            esi = (byte)(bl >> 5);
            b[i] = c;
        }
        b[7] |= esi;

        // 4. 加掩码字节循环移位
        for (int i = 0; i < 8; i++)
        {
            byte cl = (byte)BitOperations.RotateLeft(DefaultMask[i], 4);
            b[i] = (byte)(b[i] + cl);
        }

        return b;
    }

    /// <summary>
    /// 对 8 字节混淆载荷进行解密还原
    /// </summary>
    /// <param name="data">混淆后的 8 字节载荷</param>
    /// <param name="key">可选的密钥，默认为全 0</param>
    /// <returns>还原后的 8 字节数据</returns>
    public static byte[] Deobfuscate(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key = default)
    {
        if (data.Length != 8)
            throw new ArgumentException("Payload must be exactly 8 bytes.", nameof(data));

        byte[] b = data.ToArray();
        Span<byte> k = stackalloc byte[8];
        if (!key.IsEmpty)
            key.Slice(0, Math.Min(8, key.Length)).CopyTo(k);

        // 1. 逆向减掩码
        for (int i = 0; i < 8; i++)
        {
            byte cl = (byte)BitOperations.RotateLeft(DefaultMask[i], 4);
            b[i] = (byte)(b[i] - cl);
        }

        // 2. 逆向位移还原
        byte edi = 0;
        for (int i = 0; i < 8; i++)
        {
            byte ebx = b[i];
            byte cl = (byte)((ebx >> 3) | edi);
            edi = (byte)((ebx << 5) & 0xff);
            b[i] = cl;
        }
        b[0] |= edi;

        // 3. 逆向密钥异或掩码 (1..6)
        for (int i = 1; i <= 6; i++)
        {
            b[i] ^= k[i];
        }

        // 4. 逆向字节对调
        (b[0], b[5]) = (b[5], b[0]);
        (b[1], b[4]) = (b[4], b[1]);
        (b[2], b[7]) = (b[7], b[2]);
        (b[3], b[6]) = (b[6], b[3]);

        return b;
    }
}

namespace Dareu.LM113.Protocol;

/// <summary>
/// 达尔优 LM113 报文构建与校验工具
/// </summary>
public static class PacketBuilder
{
    public const int FeatureReportLength = 9;

    /// <summary>
    /// 计算 7 字节载荷的校验和 (~sum & 0xFF)
    /// </summary>
    public static byte CalculateChecksum(ReadOnlySpan<byte> payload7)
    {
        if (payload7.Length != 7)
            throw new ArgumentException("Payload must be 7 bytes to compute checksum.", nameof(payload7));

        int sum = 0;
        foreach (byte b in payload7)
        {
            sum += b;
        }
        return (byte)(~sum & 0xff);
    }

    /// <summary>
    /// 构建通用的 9 字节 Feature Report
    /// </summary>
    public static byte[] BuildFeatureReport(PacketCommand cmd, byte p1 = 0, byte p2 = 0, byte p3 = 0, byte p4 = 0, byte p5 = 0, byte p6 = 0, bool obfuscate = false)
    {
        byte[] raw = new byte[8];
        raw[0] = (byte)cmd;
        raw[1] = p1;
        raw[2] = p2;
        raw[3] = p3;
        raw[4] = p4;
        raw[5] = p5;
        raw[6] = p6;
        raw[7] = CalculateChecksum(raw.AsSpan(0, 7));

        byte[] payload = obfuscate ? RoNgtEngCipher.Obfuscate(raw) : raw;

        // 构造 9 字节 Feature Report: [ReportID=0, Payload(8)]
        byte[] report = new byte[FeatureReportLength];
        report[0] = 0x00; // Report ID
        Array.Copy(payload, 0, report, 1, 8);
        return report;
    }

    /// <summary>
    /// 构建设置灯效颜色的报文 (CMD 0x0D)
    /// 注：根据实机反馈，该硬件 LED 引脚顺序为 R-B-G，此处自动为上层做物理映射适配
    /// </summary>
    public static byte[] BuildLightingColorPacket(byte r, byte g, byte b, bool obfuscate = false)
    {
        return BuildFeatureReport(PacketCommand.SetLightingColor, p1: r, p2: b, p3: g, obfuscate: obfuscate);
    }

    /// <summary>
    /// 构建设置灯效模式与速度的报文 (CMD 0x0E)
    /// </summary>
    public static byte[] BuildLightingModePacket(LightingMode mode, byte speedOrBrightness = 2, bool obfuscate = false)
    {
        return BuildFeatureReport(PacketCommand.SetLightingMode, p1: (byte)mode, p2: speedOrBrightness, obfuscate: obfuscate);
    }

    /// <summary>
    /// 构建设置 DPI 档位的报文 (CMD 0x03)
    /// </summary>
    public static byte[] BuildDpiPacket(byte stage, ushort dpi, byte xSensitive = 4, byte ySensitive = 4, bool obfuscate = false)
    {
        byte dpiLow = (byte)(dpi & 0xff);
        byte dpiHigh = (byte)((dpi >> 8) & 0xff);
        return BuildFeatureReport(PacketCommand.SetDpi, stage, xSensitive, ySensitive, dpiLow, dpiHigh, p6: 0, obfuscate: obfuscate);
    }

    /// <summary>
    /// 构建出厂重置报文 (CMD 0x02)
    /// </summary>
    public static byte[] BuildFactoryResetPacket(bool obfuscate = false)
    {
        return BuildFeatureReport(PacketCommand.FactoryReset, p1: 0x02, obfuscate: obfuscate);
    }

    /// <summary>
    /// 构建应用/保存配置报文 (CMD 0x01)
    /// </summary>
    public static byte[] BuildApplyConfigPacket(byte profileIndex = 0, bool obfuscate = false)
    {
        return BuildFeatureReport(PacketCommand.ApplyConfig, p1: 0x01, p2: profileIndex, obfuscate: obfuscate);
    }
}

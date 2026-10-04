using Dareu.LM113.Protocol;

namespace Dareu.LM113.Protocol.Tests;

public class PacketTests
{
    [Fact]
    public void TestChecksumCalculation()
    {
        // 验证计算：sum([0x0E, 0x02, 0x02, 0, 0, 0, 0]) = 0x12 -> ~0x12 = 0xED
        byte[] payload7 = [0x0e, 0x02, 0x02, 0x00, 0x00, 0x00, 0x00];
        byte checksum = PacketBuilder.CalculateChecksum(payload7);
        Assert.Equal(0xed, checksum);
    }

    [Fact]
    public void TestBuildLightingColorPacket()
    {
        byte[] packet = PacketBuilder.BuildLightingColorPacket(255, 128, 64);
        Assert.Equal(9, packet.Length);
        Assert.Equal(0x00, packet[0]); // Report ID
        Assert.Equal((byte)PacketCommand.SetLightingColor, packet[1]); // 0x0D
        Assert.Equal(255, packet[2]); // R
        Assert.Equal(64, packet[3]);  // 硬件 B 映射
        Assert.Equal(128, packet[4]); // 硬件 G 映射

        // 校验和验证
        byte sum = 0;
        for (int i = 1; i <= 7; i++) sum += packet[i];
        Assert.Equal((byte)(~sum & 0xff), packet[8]);
    }

    [Fact]
    public void TestBuildDpiPacket()
    {
        byte[] packet = PacketBuilder.BuildDpiPacket(stage: 1, dpi: 1600, xSensitive: 4, ySensitive: 4);
        Assert.Equal(9, packet.Length);
        Assert.Equal(0x00, packet[0]);
        Assert.Equal((byte)PacketCommand.SetDpi, packet[1]);
        Assert.Equal(1, packet[2]); // stage
        Assert.Equal(4, packet[3]); // xSensitive
        Assert.Equal(4, packet[4]); // ySensitive
        Assert.Equal((byte)(1600 & 0xff), packet[5]); // dpi low
        Assert.Equal((byte)((1600 >> 8) & 0xff), packet[6]); // dpi high
    }
}

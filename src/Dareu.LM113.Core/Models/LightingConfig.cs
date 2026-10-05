using Dareu.LM113.Protocol;

namespace Dareu.LM113.Core.Models;

/// <summary>
/// RGB 灯光效果配置
/// </summary>
public class LightingConfig
{
    public LightingMode Mode { get; set; } = LightingMode.Breathing;
    public byte Speed { get; set; } = 4; // 原厂速度默认 4
    public byte Brightness { get; set; } = 2; // 原厂亮度默认 2
    public byte Red { get; set; } = 255;
    public byte Green { get; set; } = 0;
    public byte Blue { get; set; } = 0;
}

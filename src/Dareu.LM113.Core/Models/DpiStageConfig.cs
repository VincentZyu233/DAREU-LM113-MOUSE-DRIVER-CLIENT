namespace Dareu.LM113.Core.Models;

/// <summary>
/// DPI 档位配置
/// </summary>
public class DpiStageConfig
{
    public int StageIndex { get; set; } = 1;
    public int Dpi { get; set; } = 800;
    public bool IsEnabled { get; set; } = true;
    public byte Red { get; set; } = 255;
    public byte Green { get; set; } = 0;
    public byte Blue { get; set; } = 0;
    public byte XSensitivity { get; set; } = 4;
    public byte YSensitivity { get; set; } = 4;

    public DpiStageConfig() { }

    public DpiStageConfig(int stageIndex, int dpi, byte r, byte g, byte b)
    {
        StageIndex = stageIndex;
        Dpi = dpi;
        Red = r;
        Green = g;
        Blue = b;
    }
}

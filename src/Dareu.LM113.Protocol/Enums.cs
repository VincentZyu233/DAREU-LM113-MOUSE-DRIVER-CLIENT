namespace Dareu.LM113.Protocol;

/// <summary>
/// 鼠标灯效模式 (达尔优 LM113 原厂硬件支持的三种标准模式)
/// </summary>
public enum LightingMode : byte
{
    Static = 1,     // 常亮模式 (颜色由 DPI 档位颜色决定)
    Breathing = 2,  // 呼吸模式 (颜色由 DPI 档位颜色决定)
    Neon = 3        // 霓虹模式 (全彩流转)
}

/// <summary>
/// USB 报告率 (Polling Rate)
/// </summary>
public enum PollingRate : byte
{
    Hz1000 = 1,
    Hz500 = 2,
    Hz250 = 4,
    Hz125 = 8
}

/// <summary>
/// 鼠标物理按键
/// </summary>
public enum MouseButton : byte
{
    Left = 1,
    Right = 2,
    Middle = 3,
    Forward = 4,
    Backward = 5,
    DpiLoop = 6,
    DpiUp = 7,
    DpiDown = 8
}

/// <summary>
/// 达尔优原厂协议命令字定义
/// </summary>
public enum PacketCommand : byte
{
    ApplyConfig = 0x01,
    FactoryReset = 0x02,
    SetDpi = 0x03,
    SetKeyMapping = 0x04,
    SetMacro = 0x0B,
    SetLightingMode = 0x0D,   // 原厂协议：0x0D 为 SetRGBLedEffect(mode, speed, brightness)
    SetXYSensitive = 0x0E,    // 原厂协议：0x0E 为 SetXYSensitive(x, y)
    SetLEDStageColor = 0x10,  // 原厂协议：0x10 为 SetLED8StageColor(stage, rgb)
    QueryConfig = 0x81
}

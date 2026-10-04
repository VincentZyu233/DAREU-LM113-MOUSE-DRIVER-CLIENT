namespace Dareu.LM113.Protocol;

/// <summary>
/// 鼠标灯效模式
/// </summary>
public enum LightingMode : byte
{
    Off = 0,
    Static = 1,
    Breathing = 2,
    Neon = 3,
    FingerMove = 4
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
/// 协议命令字定义
/// </summary>
public enum PacketCommand : byte
{
    ApplyConfig = 0x01,
    FactoryReset = 0x02,
    SetDpi = 0x03,
    SetKeyMapping = 0x04,
    SetMacro = 0x0B,
    SetLightingColor = 0x0D,
    SetLightingMode = 0x0E,
    SetKeyMatrix0 = 0x10,
    SetKeyMatrix1 = 0x11,
    SetKeyMatrix2 = 0x12,
    SetKeyMatrix3 = 0x13,
    QueryConfig = 0x81
}

using Dareu.LM113.Core.Models;

namespace Dareu.LM113.Core.Services;

/// <summary>
/// 鼠标硬件交互接口 (跨平台抽象)
/// </summary>
public interface IMouseDevice : IDisposable
{
    bool IsConnected { get; }
    string DevicePath { get; }
    int VendorId { get; }
    int ProductId { get; }

    event EventHandler<int>? DpiStageChanged;
    event EventHandler<bool>? ConnectionChanged;

    Task<bool> ConnectAsync();
    Task DisconnectAsync();

    Task<bool> ApplyLightingAsync(LightingConfig lighting);
    Task<bool> ApplyDpiAsync(int stageIndex, int dpi, byte xSensitive = 4, byte ySensitive = 4);
    Task<bool> ApplyProfileAsync(MouseProfile profile);
    Task<bool> FactoryResetAsync();
}

using Dareu.LM113.Core.Models;
using Dareu.LM113.Protocol;
using HidSharp;

namespace Dareu.LM113.Core.Services;

/// <summary>
/// 达尔优 LM113 硬件设备服务 (基于 HidSharp 原生跨平台驱动，无外部动态库依赖)
/// </summary>
public class DareuDevice : IMouseDevice
{
    private HidDevice? _hidDevice;
    private HidStream? _stream;
    private readonly SemaphoreSlim _ioLock = new(1, 1);
    private bool _disposed;

    // 达尔优 / 荣腾常见 VID 与 PID
    public static readonly (int Vid, int Pid)[] SupportedDevices =
    [
        (0x260D, 0x1095), // 当前目标达尔优 LM113 (Holtek HT MS)
        (0x258A, 0x010C)  // 中颖/荣腾方案兼容
    ];

    public bool IsConnected => _stream != null;
    public string DevicePath { get; private set; } = string.Empty;
    public int VendorId { get; private set; }
    public int ProductId { get; private set; }

    public event EventHandler<int>? DpiStageChanged;
    public event EventHandler<bool>? ConnectionChanged;

    protected virtual void OnDpiStageChanged(int stage) => DpiStageChanged?.Invoke(this, stage);

    /// <summary>
    /// 自动扫描并连接达尔优鼠标的 Vendor-Defined 接口
    /// </summary>
    public async Task<bool> ConnectAsync()
    {
        await _ioLock.WaitAsync();
        try
        {
            if (_stream != null)
                return true;

            foreach (var (vid, pid) in SupportedDevices)
            {
                var devices = DeviceList.Local.GetHidDevices(vid, pid);
                foreach (var dev in devices)
                {
                    // 尝试匹配 Vendor-Defined 接口或非鼠标基本指针接口
                    // 在达尔优 LM113 上，配置接口 FeatureReport 长度为 9 字节 (或最大长度 >= 9)
                    if (dev.GetMaxFeatureReportLength() >= 9)
                    {
                        try
                        {
                            if (dev.TryOpen(out _stream))
                            {
                                _hidDevice = dev;
                                DevicePath = dev.DevicePath;
                                VendorId = dev.VendorID;
                                ProductId = dev.ProductID;
                                ConnectionChanged?.Invoke(this, true);
                                return true;
                            }
                        }
                        catch
                        {
                            _stream?.Dispose();
                            _stream = null;
                        }
                    }
                }
            }

            return false;
        }
        finally
        {
            _ioLock.Release();
        }
    }

    /// <summary>
    /// 断开设备连接
    /// </summary>
    public async Task DisconnectAsync()
    {
        await _ioLock.WaitAsync();
        try
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
                _hidDevice = null;
                DevicePath = string.Empty;
                ConnectionChanged?.Invoke(this, false);
            }
        }
        finally
        {
            _ioLock.Release();
        }
    }

    /// <summary>
    /// 下发灯效设置
    /// </summary>
    public async Task<bool> ApplyLightingAsync(LightingConfig lighting)
    {
        if (!IsConnected)
            return false;

        await _ioLock.WaitAsync();
        try
        {
            if (_stream == null)
                return false;

            // 1. 设置灯效模式 (CMD 0x0E)
            byte[] modePacket = PacketBuilder.BuildLightingModePacket(lighting.Mode, lighting.Speed);
            _stream.SetFeature(modePacket);

            await Task.Delay(15);

            // 2. 设置静态/基准 RGB 颜色 (CMD 0x0D)
            byte[] colorPacket = PacketBuilder.BuildLightingColorPacket(lighting.Red, lighting.Green, lighting.Blue);
            _stream.SetFeature(colorPacket);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _ioLock.Release();
        }
    }

    /// <summary>
    /// 下发单档 DPI 设置
    /// </summary>
    public async Task<bool> ApplyDpiAsync(int stageIndex, int dpi, byte xSensitive = 4, byte ySensitive = 4)
    {
        if (!IsConnected)
            return false;

        await _ioLock.WaitAsync();
        try
        {
            if (_stream == null)
                return false;

            byte[] dpiPacket = PacketBuilder.BuildDpiPacket((byte)stageIndex, (ushort)dpi, xSensitive, ySensitive);
            _stream.SetFeature(dpiPacket);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _ioLock.Release();
        }
    }

    /// <summary>
    /// 应用完整配置 (DPI 档位、灯光、应用生效)
    /// </summary>
    public async Task<bool> ApplyProfileAsync(MouseProfile profile)
    {
        if (!IsConnected)
            return false;

        // 1. 应用所有已启用的 DPI 档位
        for (int i = 0; i < profile.DpiStages.Count; i++)
        {
            var stage = profile.DpiStages[i];
            if (stage.IsEnabled)
            {
                bool success = await ApplyDpiAsync(stage.StageIndex, stage.Dpi, stage.XSensitivity, stage.YSensitivity);
                if (!success) return false;
                await Task.Delay(15);
            }
        }

        // 2. 应用灯光设置
        bool lightSuccess = await ApplyLightingAsync(profile.Lighting);
        if (!lightSuccess) return false;

        await Task.Delay(15);

        // 3. 发送应用生效保存指令 (CMD 0x01)
        await _ioLock.WaitAsync();
        try
        {
            if (_stream == null) return false;
            byte[] applyPacket = PacketBuilder.BuildApplyConfigPacket((byte)profile.CurrentStageIndex);
            _stream.SetFeature(applyPacket);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _ioLock.Release();
        }
    }

    /// <summary>
    /// 恢复出厂设置
    /// </summary>
    public async Task<bool> FactoryResetAsync()
    {
        if (!IsConnected)
            return false;

        await _ioLock.WaitAsync();
        try
        {
            if (_stream == null) return false;
            byte[] resetPacket = PacketBuilder.BuildFactoryResetPacket();
            _stream.SetFeature(resetPacket);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _ioLock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _stream?.Dispose();
        _stream = null;
        _hidDevice = null;
        _ioLock.Dispose();
        GC.SuppressFinalize(this);
    }
}

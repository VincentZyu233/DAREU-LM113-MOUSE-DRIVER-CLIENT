using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dareu.LM113.Core.Models;
using Dareu.LM113.Core.Services;
using Dareu.LM113.Protocol;

namespace Dareu.LM113.App.ViewModels;

public partial class DpiStageItemViewModel : ObservableObject
{
    [ObservableProperty] private int _stageIndex;
    [ObservableProperty] private int _dpi;
    [ObservableProperty] private bool _isEnabled;
    [ObservableProperty] private byte _red;
    [ObservableProperty] private byte _green;
    [ObservableProperty] private byte _blue;
    [ObservableProperty] private byte _xSensitivity = 4;
    [ObservableProperty] private byte _ySensitivity = 4;

    public string StageLabel => $"第{StageIndex}档";
    public string DpiText => $"{Dpi} DPI";
    public string DisplayColor => $"#{Red:X2}{Green:X2}{Blue:X2}";
}

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DareuDevice _device = new();

    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private string _statusMessage = "正在初始化...";
    [ObservableProperty] private string _deviceInfoText = "未连接";
    [ObservableProperty] private int _currentStageIndex = 2;
    [ObservableProperty] private int _selectedPollingRateIndex = 0; // 0:1000Hz, 1:500Hz, 2:250Hz, 3:125Hz
    [ObservableProperty] private int _selectedLightingModeIndex = 2; // 0:关, 1:常亮, 2:呼吸, 3:霓虹, 4:指动
    [ObservableProperty] private byte _lightingSpeed = 2;
    [ObservableProperty] private byte _selectedRed = 255;
    [ObservableProperty] private byte _selectedGreen = 0;
    [ObservableProperty] private byte _selectedBlue = 0;
    [ObservableProperty] private bool _swapGreenAndBlue = true; // 硬件 RBG 排布适配

    public ObservableCollection<DpiStageItemViewModel> DpiStages { get; } = [];
    public ObservableCollection<string> PollingRateOptions { get; } = ["1000 Hz", "500 Hz", "250 Hz", "125 Hz"];
    public ObservableCollection<string> LightingModeOptions { get; } = ["关闭 (Off)", "常亮 (Static)", "呼吸 (Breathing)", "霓虹 (Neon)", "指动 (FingerMove)"];

    public MainWindowViewModel()
    {
        InitDefaultStages();
        _ = AutoConnectAsync();
    }

    private void InitDefaultStages()
    {
        DpiStages.Clear();
        var defaults = MouseProfile.CreateDefault().DpiStages;
        foreach (var s in defaults)
        {
            DpiStages.Add(new DpiStageItemViewModel
            {
                StageIndex = s.StageIndex,
                Dpi = s.Dpi,
                IsEnabled = s.IsEnabled,
                Red = s.Red,
                Green = s.Green,
                Blue = s.Blue,
                XSensitivity = s.XSensitivity,
                YSensitivity = s.YSensitivity
            });
        }
    }

    [RelayCommand]
    public async Task AutoConnectAsync()
    {
        StatusMessage = "正在扫描达尔优 LM113 鼠标...";
        bool ok = await _device.ConnectAsync();
        IsConnected = ok;
        if (ok)
        {
            StatusMessage = "鼠标已连接，随时可下发配置";
            DeviceInfoText = $"达尔优发光鼠标 (VID: 0x{_device.VendorId:X4} PID: 0x{_device.ProductId:X4})";
        }
        else
        {
            StatusMessage = "未发现可用鼠标，请插入 USB 接口后点击重试";
            DeviceInfoText = "未连接";
        }
    }

    [RelayCommand]
    public async Task ApplyLightingAsync()
    {
        if (!IsConnected)
        {
            StatusMessage = "设备未连接，无法下发灯光！";
            return;
        }

        byte g = SwapGreenAndBlue ? SelectedBlue : SelectedGreen;
        byte b = SwapGreenAndBlue ? SelectedGreen : SelectedBlue;

        var lighting = new LightingConfig
        {
            Mode = (LightingMode)SelectedLightingModeIndex,
            Speed = LightingSpeed,
            Brightness = 3,
            Red = SelectedRed,
            Green = g,
            Blue = b
        };

        bool ok = await _device.ApplyLightingAsync(lighting);
        StatusMessage = ok ? "灯效配置已实时下发！" : "灯效下发失败，请检查连接";
    }

    [RelayCommand]
    public async Task ApplyAllAsync()
    {
        if (!IsConnected)
        {
            StatusMessage = "设备未连接，无法应用配置！";
            return;
        }

        StatusMessage = "正在下发完整配置到鼠标芯片...";
        var profile = new MouseProfile
        {
            CurrentStageIndex = CurrentStageIndex,
            PollingRate = SelectedPollingRateIndex switch
            {
                0 => PollingRate.Hz1000,
                1 => PollingRate.Hz500,
                2 => PollingRate.Hz250,
                _ => PollingRate.Hz125
            },
            Lighting = new LightingConfig
            {
                Mode = (LightingMode)SelectedLightingModeIndex,
                Speed = LightingSpeed,
                Brightness = 3,
                Red = SelectedRed,
                Green = SwapGreenAndBlue ? SelectedBlue : SelectedGreen,
                Blue = SwapGreenAndBlue ? SelectedGreen : SelectedBlue
            },
            DpiStages = DpiStages.Select(vm => new DpiStageConfig
            {
                StageIndex = vm.StageIndex,
                Dpi = vm.Dpi,
                IsEnabled = vm.IsEnabled,
                Red = vm.Red,
                Green = vm.Green,
                Blue = vm.Blue,
                XSensitivity = vm.XSensitivity,
                YSensitivity = vm.YSensitivity
            }).ToList()
        };

        bool ok = await _device.ApplyProfileAsync(profile);
        StatusMessage = ok ? "全部设置已成功写入鼠标芯片！" : "下发失败，请重试";
    }

    [RelayCommand]
    public void SetColorPreset(string hex)
    {
        if (hex.Length == 6)
        {
            SelectedRed = Convert.ToByte(hex[..2], 16);
            SelectedGreen = Convert.ToByte(hex.Substring(2, 2), 16);
            SelectedBlue = Convert.ToByte(hex.Substring(4, 2), 16);
            _ = ApplyLightingAsync();
        }
    }

    [RelayCommand]
    public async Task FactoryResetAsync()
    {
        if (!IsConnected) return;
        bool ok = await _device.FactoryResetAsync();
        InitDefaultStages();
        StatusMessage = ok ? "鼠标已恢复出厂设置！" : "恢复出厂失败";
    }

    [RelayCommand]
    public void ImportOriginalP1()
    {
        string path = @"D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\modules\setting\p1.bin";
        if (File.Exists(path))
        {
            var p = ProfileStorageService.ImportFromOriginalIni(path);
            DpiStages.Clear();
            foreach (var s in p.DpiStages)
            {
                DpiStages.Add(new DpiStageItemViewModel
                {
                    StageIndex = s.StageIndex,
                    Dpi = s.Dpi,
                    IsEnabled = s.IsEnabled,
                    Red = s.Red,
                    Green = s.Green,
                    Blue = s.Blue,
                    XSensitivity = s.XSensitivity,
                    YSensitivity = s.YSensitivity
                });
            }
            CurrentStageIndex = p.CurrentStageIndex;
            SelectedLightingModeIndex = (int)p.Lighting.Mode;
            StatusMessage = "已从原版 p1.bin 导入全部配置！";
        }
        else
        {
            StatusMessage = "未找到原版 p1.bin 文件";
        }
    }
}

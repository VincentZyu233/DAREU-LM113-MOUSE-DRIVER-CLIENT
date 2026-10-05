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
    [ObservableProperty] private LightingMode _currentLightingMode = LightingMode.Breathing;
    [ObservableProperty] private byte _lightingSpeed = 4;
    [ObservableProperty] private int _activeTabIndex = 0; // 0: DPI, 1: RGB, 2: About
    [ObservableProperty] private bool _isLoadingTab;
    [ObservableProperty] private string _loadingTitle = "正在切换...";

    public bool IsStaticMode => CurrentLightingMode == LightingMode.Static;
    public bool IsBreathingMode => CurrentLightingMode == LightingMode.Breathing;
    public bool IsNeonMode => CurrentLightingMode == LightingMode.Neon;

    public string CurrentLightingModeText => CurrentLightingMode switch
    {
        LightingMode.Static => "常亮模式",
        LightingMode.Breathing => "呼吸模式",
        LightingMode.Neon => "霓虹模式",
        _ => "未知模式"
    };

    public string CurrentLightingModeDescription => CurrentLightingMode switch
    {
        LightingMode.Static => "保持恒定常量照明，基色由当前 DPI 档位绑定的色彩决定",
        LightingMode.Breathing => "柔和渐明渐暗律动呼吸闪烁，基色由当前 DPI 档位绑定的色彩决定",
        LightingMode.Neon => "全彩 RGB 流光平滑循环流转，由芯片内置算法自动跑彩虹流动",
        _ => ""
    };

    partial void OnCurrentLightingModeChanged(LightingMode value)
    {
        OnPropertyChanged(nameof(IsStaticMode));
        OnPropertyChanged(nameof(IsBreathingMode));
        OnPropertyChanged(nameof(IsNeonMode));
        OnPropertyChanged(nameof(CurrentLightingModeText));
        OnPropertyChanged(nameof(CurrentLightingModeDescription));
    }

    [RelayCommand]
    public async Task SelectLightingModeAsync(string modeName)
    {
        if (Enum.TryParse<LightingMode>(modeName, true, out var mode))
        {
            CurrentLightingMode = mode;
            if (IsConnected)
            {
                await ApplyLightingAsync();
            }
            else
            {
                StatusMessage = $"已选中【{CurrentLightingModeText}】(离线预览，鼠标未连接)";
            }
        }
    }

    public bool IsDpiTabActive => ActiveTabIndex == 0;
    public bool IsRgbTabActive => ActiveTabIndex == 1;
    public bool IsAboutTabActive => ActiveTabIndex == 2;

    [RelayCommand]
    public async Task SwitchTabAsync(string indexStr)
    {
        if (!int.TryParse(indexStr, out int idx) || (idx == ActiveTabIndex && !IsLoadingTab))
            return;

        // 1. 立即给药丸按钮选中状态，并开启 Loading 视图 (0ms 即时响应)
        ActiveTabIndex = idx;
        IsLoadingTab = true;
        LoadingTitle = idx switch
        {
            0 => "正在加载 DPI 灵敏度矩阵...",
            1 => "正在加载 RGB 幻彩调色系统...",
            _ => "正在加载系统与驱动信息..."
        };

        // 2. 释放 UI 线程，让按钮高亮和转圈圈能够第一时间刷新到屏幕（0ms 响应）
        await Task.Yield();
        await Task.Delay(100); // 100ms 极客电竞微动效缓冲，平滑过渡

        // 3. 完成显隐并关闭 Loading
        OnPropertyChanged(nameof(IsDpiTabActive));
        OnPropertyChanged(nameof(IsRgbTabActive));
        OnPropertyChanged(nameof(IsAboutTabActive));
        IsLoadingTab = false;
    }

    public ObservableCollection<DpiStageItemViewModel> DpiStages { get; } = [];
    public ObservableCollection<string> PollingRateOptions { get; } = ["1000 Hz", "500 Hz", "250 Hz", "125 Hz"];
    public ObservableCollection<string> LightingModeOptions { get; } = ["关闭 (Off)", "常亮 (Static)", "呼吸 (Breathing)", "霓虹 (Neon)", "指动 (FingerMove)"];

    public MainWindowViewModel()
    {
        InitDefaultStages();
        _ = Task.Run(AutoConnectAsync);
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
        bool ok = await Task.Run(() => _device.ConnectAsync());
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

        var lighting = new LightingConfig
        {
            Mode = CurrentLightingMode,
            Speed = LightingSpeed,
            Brightness = 2
        };

        bool ok = await _device.ApplyLightingAsync(lighting);
        StatusMessage = ok ? $"灯效已成功切换为【{CurrentLightingModeText}】并写入芯片！" : "灯效下发失败，请检查连接";
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
                Mode = CurrentLightingMode,
                Speed = LightingSpeed,
                Brightness = 2
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
            CurrentLightingMode = p.Lighting.Mode;
            StatusMessage = "已从原版 p1.bin 导入全部配置！";
        }
        else
        {
            StatusMessage = "未找到原版 p1.bin 文件";
        }
    }
}

using Dareu.LM113.Protocol;

namespace Dareu.LM113.Core.Models;

/// <summary>
/// 鼠标完整配置文件 (Profile)
/// </summary>
public class MouseProfile
{
    public string ProfileName { get; set; } = "默认配置";
    public int CurrentStageIndex { get; set; } = 2; // 默认第 2 档 (例如 800 或 1600)
    public PollingRate PollingRate { get; set; } = PollingRate.Hz1000;
    public LightingConfig Lighting { get; set; } = new();
    public List<DpiStageConfig> DpiStages { get; set; } = [];
    public List<KeyBindingItem> KeyBindings { get; set; } = [];

    public static MouseProfile CreateDefault()
    {
        var profile = new MouseProfile
        {
            ProfileName = "默认配置",
            CurrentStageIndex = 2,
            PollingRate = PollingRate.Hz1000,
            Lighting = new LightingConfig
            {
                Mode = LightingMode.Breathing,
                Speed = 2,
                Brightness = 2,
                Red = 255,
                Green = 0,
                Blue = 0
            },
            DpiStages =
            [
                new(1, 400, 255, 0, 0),       // 红色
                new(2, 800, 0, 255, 0),       // 绿色
                new(3, 1600, 0, 0, 255),      // 蓝色
                new(4, 3200, 255, 255, 0),    // 黄色
                new(5, 6000, 255, 0, 255),    // 紫色
                new(6, 4000, 0, 255, 255)     // 青色
            ],
            KeyBindings =
            [
                new(MouseButton.Left, "左键", KeyFunctionType.StandardMouse, 1),
                new(MouseButton.Right, "右键", KeyFunctionType.StandardMouse, 2),
                new(MouseButton.Middle, "中键", KeyFunctionType.StandardMouse, 3),
                new(MouseButton.Forward, "前进", KeyFunctionType.StandardMouse, 5),
                new(MouseButton.Backward, "后退", KeyFunctionType.StandardMouse, 6),
                new(MouseButton.DpiLoop, "DPI 切换", KeyFunctionType.DpiSwitch, 4)
            ]
        };
        return profile;
    }
}

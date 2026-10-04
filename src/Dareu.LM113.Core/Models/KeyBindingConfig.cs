using Dareu.LM113.Protocol;

namespace Dareu.LM113.Core.Models;

/// <summary>
/// 按键功能类型
/// </summary>
public enum KeyFunctionType
{
    StandardMouse = 0,
    DpiSwitch = 1,
    Multimedia = 2,
    KeyboardShortcut = 3,
    Disabled = 4,
    Macro = 5
}

/// <summary>
/// 单个按键映射设置
/// </summary>
public class KeyBindingItem
{
    public MouseButton Button { get; set; }
    public string Name { get; set; } = string.Empty;
    public KeyFunctionType FunctionType { get; set; } = KeyFunctionType.StandardMouse;
    public int ActionId { get; set; }

    public KeyBindingItem() { }

    public KeyBindingItem(MouseButton button, string name, KeyFunctionType type, int actionId)
    {
        Button = button;
        Name = name;
        FunctionType = type;
        ActionId = actionId;
    }
}

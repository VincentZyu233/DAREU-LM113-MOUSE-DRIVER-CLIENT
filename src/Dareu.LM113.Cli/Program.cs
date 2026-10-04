using Dareu.LM113.Core.Models;
using Dareu.LM113.Core.Services;
using Dareu.LM113.Protocol;

Console.WriteLine("==================================================");
Console.WriteLine("  DAREU LM113 达尔优发光鼠标 跨平台驱动 CLI 控制台");
Console.WriteLine("==================================================");

using var device = new DareuDevice();
Console.WriteLine("\n[1/3] 正在扫描并连接达尔优鼠标 HID 接口...");

bool connected = await device.ConnectAsync();
if (!connected)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("未发现可通信的达尔优鼠标 (VID: 0x260D / 0x258A)，请确认鼠标已插入 USB 接口。");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"成功连接到设备！");
Console.WriteLine($"  - 厂商 ID (VID): 0x{device.VendorId:X4}");
Console.WriteLine($"  - 产品 ID (PID): 0x{device.ProductId:X4}");
Console.WriteLine($"  - 设备端点路径: {device.DevicePath}");
Console.ResetColor();

if (args.Contains("--test-rgb"))
{
    Console.WriteLine("\n[2/3] 正在执行实机 RGB 灯效流水测试...");
    var colors = new (string Name, byte R, byte G, byte B)[]
    {
        ("纯红色 (Red)", 255, 0, 0),
        ("纯绿色 (Green)", 0, 255, 0),
        ("纯蓝色 (Blue)", 0, 0, 255),
        ("亮黄色 (Yellow)", 255, 255, 0),
        ("青色 (Cyan)", 0, 255, 255),
        ("紫色 (Purple)", 255, 0, 255)
    };

    // 先设为常亮模式
    await device.ApplyLightingAsync(new LightingConfig { Mode = LightingMode.Static, Brightness = 3, Speed = 2 });

    foreach (var (name, r, g, b) in colors)
    {
        Console.WriteLine($"  -> 切换颜色为: {name}");
        await device.ApplyLightingAsync(new LightingConfig { Mode = LightingMode.Static, Red = r, Green = g, Blue = b });
        await Task.Delay(1000);
    }

    Console.WriteLine("  -> 恢复为炫彩霓虹呼吸模式 (Mode 3)...");
    await device.ApplyLightingAsync(new LightingConfig { Mode = LightingMode.Neon, Speed = 2 });
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("RGB 变换测试完成！");
    Console.ResetColor();
}
else
{
    Console.WriteLine("\n[2/3] 设备就绪，当前可执行操作：");
    Console.WriteLine("  dotnet run --project src/Dareu.LM113.Cli -- --test-rgb     (执行实机红绿蓝RGB切换演示)");
    Console.WriteLine("  dotnet run --project src/Dareu.LM113.Cli -- --import-p1    (从原版驱动导入 p1.bin)");

    string originalP1 = @"D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\modules\setting\p1.bin";
    if (File.Exists(originalP1) && args.Contains("--import-p1"))
    {
        Console.WriteLine($"\n[3/3] 正在从原版 p1.bin 导入配置...");
        var imported = ProfileStorageService.ImportFromOriginalIni(originalP1);
        Console.WriteLine($"  - 默认档位: Stage {imported.CurrentStageIndex}");
        Console.WriteLine("  - DPI 档位列表:");
        foreach (var s in imported.DpiStages)
        {
            Console.WriteLine($"    [第 {s.StageIndex} 档] {s.Dpi} DPI, 启用={s.IsEnabled}, RGB=({s.Red},{s.Green},{s.Blue})");
        }
    }
}

Console.WriteLine("\n操作完成，连接安全关闭。");

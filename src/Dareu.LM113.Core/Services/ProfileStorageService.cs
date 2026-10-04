using System.Text.Json;
using Dareu.LM113.Core.Models;
using Dareu.LM113.Protocol;

namespace Dareu.LM113.Core.Services;

/// <summary>
/// 鼠标配置文件序列化与原版 p1.bin 兼容导入服务
/// </summary>
public static class ProfileStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// 保存为现代 JSON 格式
    /// </summary>
    public static async Task SaveToJsonAsync(MouseProfile profile, string filePath)
    {
        string dir = Path.GetDirectoryName(filePath) ?? string.Empty;
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string json = JsonSerializer.Serialize(profile, JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// 从 JSON 文件加载配置
    /// </summary>
    public static async Task<MouseProfile?> LoadFromJsonAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<MouseProfile>(json, JsonOptions);
    }

    /// <summary>
    /// 从原版驱动的 p1.bin (INI 格式) 导入配置
    /// </summary>
    public static MouseProfile ImportFromOriginalIni(string filePath)
    {
        var profile = MouseProfile.CreateDefault();
        if (!File.Exists(filePath))
            return profile;

        string[] lines = File.ReadAllLines(filePath);
        string currentSection = string.Empty;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith(';') || line.StartsWith('#'))
                continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                currentSection = line[1..^1].Trim().ToLowerInvariant();
                continue;
            }

            int eq = line.IndexOf('=');
            if (eq <= 0) continue;

            string key = line[..eq].Trim().ToLowerInvariant();
            string val = line[(eq + 1)..].Trim();

            if (currentSection == "sensor")
            {
                if (key.StartsWith("dpi_j1") || key.StartsWith("dpi_j2"))
                {
                    string[] dpis = val.Split(',', StringSplitOptions.TrimEntries);
                    for (int i = 0; i < Math.Min(profile.DpiStages.Count, dpis.Length); i++)
                    {
                        if (int.TryParse(dpis[i], out int dpiVal))
                        {
                            profile.DpiStages[i].Dpi = dpiVal;
                        }
                    }
                }
                else if (key.StartsWith("dpi_select"))
                {
                    string[] selects = val.Split(',', StringSplitOptions.TrimEntries);
                    for (int i = 0; i < Math.Min(profile.DpiStages.Count, selects.Length); i++)
                    {
                        if (int.TryParse(selects[i], out int sVal))
                        {
                            profile.DpiStages[i].IsEnabled = (sVal == 1);
                        }
                    }
                }
                else if (key == "stage_j2" && int.TryParse(val, out int stage))
                {
                    profile.CurrentStageIndex = stage;
                }
            }
            else if (currentSection == "color_j2")
            {
                if (key == "type0" || key == "type1")
                {
                    string[] colors = val.Split(',', StringSplitOptions.TrimEntries);
                    // 每 3 个为一组 RGB: R, G, B
                    for (int i = 0; i < profile.DpiStages.Count && (i * 3 + 2) < colors.Length; i++)
                    {
                        if (byte.TryParse(colors[i * 3], out byte r) &&
                            byte.TryParse(colors[i * 3 + 1], out byte g) &&
                            byte.TryParse(colors[i * 3 + 2], out byte b))
                        {
                            profile.DpiStages[i].Red = r;
                            profile.DpiStages[i].Green = g;
                            profile.DpiStages[i].Blue = b;
                        }
                    }
                }
                else if (key.StartsWith("ledeffect"))
                {
                    string[] effs = val.Split(',', StringSplitOptions.TrimEntries);
                    if (effs.Length >= 2 && byte.TryParse(effs[0], out byte m) && byte.TryParse(effs[1], out byte spd))
                    {
                        profile.Lighting.Mode = (LightingMode)m;
                        profile.Lighting.Speed = spd;
                    }
                }
            }
        }

        return profile;
    }
}

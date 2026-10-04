using Dareu.LM113.Core.Models;
using Dareu.LM113.Core.Services;

namespace Dareu.LM113.Protocol.Tests;

public class StorageTests
{
    [Fact]
    public async Task TestJsonSerializationRoundTrip()
    {
        var profile = MouseProfile.CreateDefault();
        profile.ProfileName = "电竞优化版";
        profile.DpiStages[0].Dpi = 1200;

        string tempPath = Path.Combine(Path.GetTempPath(), $"test_profile_{Guid.NewGuid():N}.json");
        try
        {
            await ProfileStorageService.SaveToJsonAsync(profile, tempPath);
            var loaded = await ProfileStorageService.LoadFromJsonAsync(tempPath);

            Assert.NotNull(loaded);
            Assert.Equal("电竞优化版", loaded.ProfileName);
            Assert.Equal(1200, loaded.DpiStages[0].Dpi);
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    [Fact]
    public void TestImportFromOriginalP1Bin()
    {
        string p1Path = @"D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\modules\setting\p1.bin";
        if (!File.Exists(p1Path))
            return;

        var profile = ProfileStorageService.ImportFromOriginalIni(p1Path);
        Assert.NotNull(profile);
        Assert.True(profile.DpiStages.Count >= 5);
        Assert.Equal(400, profile.DpiStages[0].Dpi);
        Assert.Equal(800, profile.DpiStages[1].Dpi);
        Assert.Equal(1600, profile.DpiStages[2].Dpi);
        Assert.Equal(3200, profile.DpiStages[3].Dpi);
        Assert.Equal(6000, profile.DpiStages[4].Dpi);
    }
}

using Dareu.LM113.Protocol;

namespace Dareu.LM113.Protocol.Tests;

public class CipherTests
{
    [Fact]
    public void TestObfuscateAndDeobfuscateReversibility()
    {
        var random = new Random(42);
        for (int i = 0; i < 200; i++)
        {
            byte[] original = new byte[8];
            byte[] key = new byte[8];
            random.NextBytes(original);
            random.NextBytes(key);

            byte[] obfuscated = RoNgtEngCipher.Obfuscate(original, key);
            byte[] restored = RoNgtEngCipher.Deobfuscate(obfuscated, key);

            Assert.Equal(original, restored);
        }
    }

    [Fact]
    public void TestDefaultKeyWorks()
    {
        byte[] original = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] obfuscated = RoNgtEngCipher.Obfuscate(original);
        byte[] restored = RoNgtEngCipher.Deobfuscate(obfuscated);

        Assert.Equal(original, restored);
    }
}

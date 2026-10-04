def rol8(v, n):
    return ((v << n) & 0xff) | ((v >> (8 - n)) & 0xff)

def ror8(v, n):
    return ((v >> n) & 0xff) | ((v << (8 - n)) & 0xff)

MASK = [82, 111, 78, 103, 116, 69, 110, 103] # "RoNgtEng"

def obfuscate(data, key=None):
    if key is None:
        key = [0] * 8
    b = list(data)
    # 1. 对调
    b[0], b[5] = b[5], b[0]
    b[1], b[4] = b[4], b[1]
    b[2], b[7] = b[7], b[2]
    b[3], b[6] = b[6], b[3]

    # 2. XOR (1..6)
    for i in range(1, 7):
        b[i] ^= key[i]

    # 3. 循环位移
    esi = 0
    bl_first = 0
    for i in range(7, -1, -1):
        bl = b[i]
        c = ((bl * 8) & 0xff) | esi
        esi = bl >> 5
        b[i] = c
    b[7] |= esi

    # 4. 加掩码
    for i in range(8):
        cl = rol8(MASK[i], 4)
        b[i] = (b[i] + cl) & 0xff

    return bytes(b)

def deobfuscate(data, key=None):
    if key is None:
        key = [0] * 8
    b = list(data)

    # 1. 减掩码
    for i in range(8):
        cl = rol8(MASK[i], 4)
        b[i] = (b[i] - cl) & 0xff

    # 2. 循环位移恢复
    # 汇编逻辑：
    # 0x004034e1: xor edi, edi; xor dl, dl
    # 0x004034e5: movzx ebx, byte ptr [eax + edx]
    # 0x004034e9: mov ecx, ebx; shr cl, 3; or ecx, edi
    # 0x004034f0: mov edi, ebx; mov [eax + edx], cl
    # 0x004034f8: shl edi, 5
    # 循环 8 次后: or byte ptr [eax], bl (即 edi 的最高位拼到 b[0])
    edi = 0
    for i in range(8):
        ebx = b[i]
        cl = (ebx >> 3) | edi
        edi = (ebx << 5) & 0xff
        b[i] = cl
    b[0] |= edi

    # 3. XOR (1..6)
    for i in range(1, 7):
        b[i] ^= key[i]

    # 4. 对调
    b[0], b[5] = b[5], b[0]
    b[1], b[4] = b[4], b[1]
    b[2], b[7] = b[7], b[2]
    b[3], b[6] = b[6], b[3]

    return bytes(b)

def test():
    import random
    for _ in range(100):
        orig = bytes([random.randint(0, 255) for _ in range(8)])
        key = [random.randint(0, 255) for _ in range(8)]
        enc = obfuscate(orig, key)
        dec = deobfuscate(enc, key)
        assert orig == dec, f"Mismatch: orig={orig.hex()}, dec={dec.hex()}"
    print("All 100 test vectors passed perfectly!")

if __name__ == '__main__':
    test()

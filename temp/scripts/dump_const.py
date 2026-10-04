import pefile

def dump_const():
    pe = pefile.PE(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe')
    image_base = pe.OPTIONAL_HEADER.ImageBase

    target_va = 0x42e004
    rva = target_va - image_base
    data = pe.get_data(rva, 16)
    print("Mask at 0x42e004:", list(data))
    print("Hex:", data.hex())

if __name__ == '__main__':
    dump_const()

import pefile

def dump_data():
    pe = pefile.PE(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe')
    image_base = pe.OPTIONAL_HEADER.ImageBase

    target_va = 0x65b080
    rva = target_va - image_base
    data = pe.get_data(rva, 64)
    print("Data at 0x65b080:", data.hex())
    print("Bytes:", list(data[:16]))

if __name__ == '__main__':
    dump_data()

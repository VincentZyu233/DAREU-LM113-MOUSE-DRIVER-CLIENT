import re
import struct

def find_hid_refs():
    with open(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe', 'rb') as f:
        data = f.read()

    # 查找导入表中的 hidapi 函数
    funcs = [b'hid_open', b'hid_open_path', b'hid_send_feature_report', b'hid_write', b'hid_get_feature_report', b'hid_read', b'hid_enumerate']
    for fn in funcs:
        p = 0
        while True:
            idx = data.find(fn, p)
            if idx == -1:
                break
            print(f"Found {fn.decode()} at 0x{idx:08x}")
            p = idx + len(fn)

if __name__ == '__main__':
    find_hid_refs()

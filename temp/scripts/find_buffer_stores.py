import pefile
from capstone import Cs, CS_ARCH_X86, CS_MODE_32

def find_buffer_stores():
    pe = pefile.PE(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe')
    image_base = pe.OPTIONAL_HEADER.ImageBase
    text_section = [s for s in pe.sections if b'.text' in s.Name][0]
    code = text_section.get_data()
    code_base = image_base + text_section.VirtualAddress

    md = Cs(CS_ARCH_X86, CS_MODE_32)
    stores = []
    for insn in md.disasm(code, code_base):
        # 寻找形如 mov [ebx + 0x2c], ... 或 byte ptr [ebx + 0x2c]
        if '0x2c]' in insn.op_str and 'mov' in insn.mnemonic:
            stores.append((insn.address, insn.mnemonic, insn.op_str))

    print(f"Found {len(stores)} stores to 0x2c offset:")
    for addr, mnem, op in stores:
        print(f"0x{addr:08x}: {mnem:<8} {op}")
        # 打印前后 15 条指令
        offset = addr - code_base
        start = max(0, offset - 50)
        end = min(len(code), offset + 50)
        snippet = code[start:end]
        sbase = code_base + start
        for sub_insn in md.disasm(snippet, sbase):
            mark = " > " if sub_insn.address == addr else "   "
            print(f"{mark}0x{sub_insn.address:08x}: {sub_insn.mnemonic:<8} {sub_insn.op_str}")
        print("-" * 50)

if __name__ == '__main__':
    find_buffer_stores()

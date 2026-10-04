import pefile
from capstone import Cs, CS_ARCH_X86, CS_MODE_32

def dump_403410():
    pe = pefile.PE(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe')
    image_base = pe.OPTIONAL_HEADER.ImageBase
    text_section = [s for s in pe.sections if b'.text' in s.Name][0]
    code = text_section.get_data()
    code_base = image_base + text_section.VirtualAddress

    start_addr = 0x00403410
    length = 150
    offset = start_addr - code_base
    snippet = code[offset:offset+length]
    md = Cs(CS_ARCH_X86, CS_MODE_32)
    print(f"=== Function at 0x{start_addr:08x} ===")
    for insn in md.disasm(snippet, start_addr):
        print(f"0x{insn.address:08x}: {insn.mnemonic:<8} {insn.op_str}")

if __name__ == '__main__':
    dump_403410()

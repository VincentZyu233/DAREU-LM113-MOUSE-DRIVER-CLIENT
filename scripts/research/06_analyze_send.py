import pefile
from capstone import Cs, CS_ARCH_X86, CS_MODE_32

def analyze_send():
    pe = pefile.PE(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe')
    image_base = pe.OPTIONAL_HEADER.ImageBase
    text_section = [s for s in pe.sections if b'.text' in s.Name][0]
    code = text_section.get_data()
    code_base = image_base + text_section.VirtualAddress

    thunks = {
        0x0041f678: "hid_send_feature_report",
        0x0041f680: "hid_write",
    }

    md = Cs(CS_ARCH_X86, CS_MODE_32)
    callers = []
    for insn in md.disasm(code, code_base):
        if insn.mnemonic == 'call':
            for target_addr, fn_name in thunks.items():
                if f"0x{target_addr:x}" in insn.op_str:
                    callers.append((insn.address, fn_name))

    print(f"Total write/send call sites: {len(callers)}")
    for addr, fn_name in callers:
        offset = addr - code_base
        start_offset = max(0, offset - 150)
        end_offset = min(len(code), offset + 50)
        snippet = code[start_offset:end_offset]
        snippet_base = code_base + start_offset
        print(f"\n================ Caller of {fn_name} at 0x{addr:08x} ================")
        for insn in md.disasm(snippet, snippet_base):
            mark = " >>> " if insn.address == addr else "     "
            print(f"{mark}0x{insn.address:08x}: {insn.mnemonic:<8} {insn.op_str}")

if __name__ == '__main__':
    analyze_send()

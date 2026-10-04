import pefile
from capstone import Cs, CS_ARCH_X86, CS_MODE_32

def analyze_callers():
    pe = pefile.PE(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe')
    image_base = pe.OPTIONAL_HEADER.ImageBase
    text_section = [s for s in pe.sections if b'.text' in s.Name][0]
    code = text_section.get_data()
    code_base = image_base + text_section.VirtualAddress

    thunks = {
        0x0041f660: "hid_close",
        0x0041f668: "hid_open_path",
        0x0041f670: "hid_set_nonblocking",
        0x0041f678: "hid_send_feature_report",
        0x0041f680: "hid_write",
        0x0041f688: "hid_get_feature_report",
        0x0041f690: "hid_read",
        0x0041f698: "hid_enumerate",
        0x0041f6a0: "hid_free_enumeration",
        0x0041f6a8: "hid_read_timeout"
    }

    md = Cs(CS_ARCH_X86, CS_MODE_32)
    callers = []
    for insn in md.disasm(code, code_base):
        if insn.mnemonic == 'call':
            for target_addr, fn_name in thunks.items():
                if f"0x{target_addr:x}" in insn.op_str:
                    callers.append((insn.address, fn_name))

    print(f"Total call sites found: {len(callers)}")
    for addr, fn_name in callers:
        print(f"0x{addr:08x} -> {fn_name}")

    # 反汇编调用点附近 40 条指令
    for addr, fn_name in callers:
        offset = addr - code_base
        start_offset = max(0, offset - 120)
        end_offset = min(len(code), offset + 60)
        snippet = code[start_offset:end_offset]
        snippet_base = code_base + start_offset
        print(f"\n================ Caller of {fn_name} at 0x{addr:08x} ================")
        for insn in md.disasm(snippet, snippet_base):
            mark = " >>> " if insn.address == addr else "     "
            print(f"{mark}0x{insn.address:08x}: {insn.mnemonic:<8} {insn.op_str}")

if __name__ == '__main__':
    analyze_callers()

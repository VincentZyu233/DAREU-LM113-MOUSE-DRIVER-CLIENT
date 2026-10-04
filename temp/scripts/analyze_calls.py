import pefile
from capstone import Cs, CS_ARCH_X86, CS_MODE_32

def analyze():
    pe = pefile.PE(r'D:\SSoftwareFiles\鼠标驱动\LM113达尔优发光鼠标\LM113达尔优发光鼠标.exe')
    image_base = pe.OPTIONAL_HEADER.ImageBase
    print(f"ImageBase: 0x{image_base:08x}")

    # 找到 hidapi.dll 的导入项
    hid_iat = {}
    for entry in pe.DIRECTORY_ENTRY_IMPORT:
        if b'hidapi' in entry.dll.lower():
            for imp in entry.imports:
                if imp.name:
                    hid_iat[imp.address] = imp.name.decode('utf-8')
                    print(f"Import: {imp.name.decode('utf-8')} at IAT 0x{imp.address:08x}")

    if not hid_iat:
        print("No hidapi imports found!")
        return

    # 获取 .text 节
    text_section = None
    for s in pe.sections:
        if b'.text' in s.Name:
            text_section = s
            break

    if not text_section:
        print("No .text section found!")
        return

    code = text_section.get_data()
    code_rva = text_section.VirtualAddress
    code_base = image_base + code_rva

    md = Cs(CS_ARCH_X86, CS_MODE_32)
    # 反汇编搜索 call dword ptr [IAT]
    call_sites = []
    for insn in md.disasm(code, code_base):
        for iat_addr, fn_name in hid_iat.items():
            # 搜索形如 call dword ptr [0x......] 或 jmp dword ptr [0x......]
            target_hex = f"0x{iat_addr:x}"
            if target_hex in insn.op_str:
                call_sites.append((insn.address, fn_name, insn.mnemonic, insn.op_str))

    print(f"\nFound {len(call_sites)} call sites:")
    for addr, fn, mnem, op in call_sites:
        print(f"0x{addr:08x}: {mnem} {op}  -->  {fn}")

    # 针对关键调用（比如 hid_send_feature_report 或 hid_write 或 hid_enumerate），反汇编其前后上下文
    for addr, fn, mnem, op in call_sites:
        offset = addr - code_base
        start_offset = max(0, offset - 60)
        end_offset = min(len(code), offset + 40)
        snippet = code[start_offset:end_offset]
        snippet_base = code_base + start_offset
        print(f"\n================ Context around {fn} at 0x{addr:08x} ================")
        for insn in md.disasm(snippet, snippet_base):
            mark = " >>> " if insn.address == addr else "     "
            print(f"{mark}0x{insn.address:08x}: {insn.mnemonic} {insn.op_str}")

if __name__ == '__main__':
    analyze()

# 达尔优 LM113 逆向与 PoC 验证脚本全纪录

本目录记录了从 0 逆向 Windows 原版 Qt4 驱动（Rongteng / Holtek 方案）、破解对称加密密钥、验证 USB HID 协议到实机驱动测试的完整 18 个原型验证脚本。所有脚本按照探索与验证时间先后严格编号排序：

| 序号 | 脚本文件 | 探索验证目的与产出结论 |
|:---:|---|---|
| 01 | `01_enumerate_hid.py` | 枚举当前插在 Windows 主机上的所有 USB HID 节点，锁定鼠标复合设备的各 Interface |
| 02 | `02_inspect_target_mouse.py` | 识别目标鼠标 VID (`0x260D`)、PID (`0x1095`) 及各 UsagePage，确定 Vendor 配置通道为 `UsagePage 0xFF01` |
| 03 | `03_find_hid_refs.py` | 在原版驱动 EXE 中进行 PE 扫描，定位 `HidD_GetFeature`、`HidD_SetFeature` 等关键 Win32 HID API 调用地址 |
| 04 | `04_analyze_calls.py` | 分析原版驱动对 HID API 的引用链与跨模块调用者 |
| 05 | `05_analyze_thunk_callers.py` | 追踪 IAT 导入表 Thunk 调用层级，定位底层收发核心包装函数 |
| 06 | `06_analyze_send.py` | 逆向分析数据包发送入口，发现下发固定为 9 字节 Feature Report 封包格式 |
| 07 | `07_dump_func.py` | 使用 Capstone 反汇编器提取上层业务函数的汇编指令逻辑 |
| 08 | `08_find_buffer_stores.py` | 扫描寄存器向缓冲区写入的操作，确定 9 字节封包中 ReportID 与 Payload 的排布方式 |
| 09 | `09_dump_send_func.py` | 完整提取底层发送函数汇编，定位到在调用 `HidD_SetFeature` 之前必经的加密/混淆过程 |
| 10 | `10_dump_data.py` | 扫描原驱动 `.rdata` 与 `.data` 段，提取常量数据与预设表 |
| 11 | `11_dump_403410.py` | **重大突破**：深度反编译核心混淆加解密函数 `0x403410`，彻底掌握其字节交换、位移与 XOR 混淆流程 |
| 12 | `12_dump_const.py` | **核心解密**：在数据段定位到硬编码密钥字符串 **`RoNgtEng`**（`52 6f 4e 67 74 45 6e 67`，荣腾科技） |
| 13 | `13_test_read_cmd.py` | 构造带密钥加密的 Feature Report 读取指令（Cmd `0x04`/`0x05`），测试设备响应 |
| 14 | `14_test_read_responses.py` | 监听鼠标 Interrupt In 端点，验证 DPI 物理按键切换时的 4 字节状态上报包 |
| 15 | `15_test_lighting_control.py` | 首次成功使用 Python 构造灯效指令（Cmd `0x0E`），实机控制鼠标灯光变化 |
| 16 | `16_verify_cipher.py` | 对混淆加密与解密算法进行 200+ 随机向量双向一致性回归验证，确保无损还原 |
| 17 | `17_observe_lighting.py` | 编写红、绿、蓝、霓虹实机轮流切换脚本，由用户肉眼实时观察灯光变化 |
| 18 | `18_test_rbg_order.py` | 排查物理灯珠引脚顺序，实测确定该批次硬件为 **R-B-G**（Red-Blue-Green）通道排布，为驱动加入通道校准依据 |

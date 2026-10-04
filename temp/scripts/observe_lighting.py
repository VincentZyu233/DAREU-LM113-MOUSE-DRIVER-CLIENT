import hid
import time

MASK = [82, 111, 78, 103, 116, 69, 110, 103]

def rol8(v, n):
    return ((v << n) & 0xff) | ((v >> (8 - n)) & 0xff)

def obfuscate(data):
    b = list(data)
    b[0], b[5] = b[5], b[0]
    b[1], b[4] = b[4], b[1]
    b[2], b[7] = b[7], b[2]
    b[3], b[6] = b[6], b[3]
    esi = 0
    for i in range(7, -1, -1):
        bl = b[i]
        c = ((bl * 8) & 0xff) | esi
        esi = bl >> 5
        b[i] = c
    b[7] |= esi
    for i in range(8):
        cl = rol8(MASK[i], 4)
        b[i] = (b[i] + cl) & 0xff
    return bytes(b)

def build_packet(cmd, p1=0, p2=0, p3=0, p4=0, p5=0, p6=0, do_encode=False):
    raw = [cmd, p1, p2, p3, p4, p5, p6]
    checksum = (~sum(raw)) & 0xff
    payload8 = bytes(raw + [checksum])
    if do_encode:
        payload8 = obfuscate(payload8)
    return bytes([0x00]) + payload8

def main():
    target = None
    for d in hid.enumerate(0x260d, 0x1095):
        if d['interface_number'] == 2 or d['usage_page'] >= 0xff00:
            target = d['path']
            break

    if not target:
        print("未找到设备 0x260d:0x1095")
        return

    dev = hid.device()
    dev.open_path(target)
    print(">>> 鼠标通信端口已打开，准备开始灯光切换演示！")
    print(">>> 请眼睛看着鼠标滚轮/LOGO处的发光区域！\n")

    test_colors = [
        ("【红色 RED】", 255, 0, 0),
        ("【绿色 GREEN】", 0, 255, 0),
        ("【蓝色 BLUE】", 0, 0, 255),
        ("【黄色 YELLOW】", 255, 255, 0),
        ("【紫色 PURPLE】", 255, 0, 255),
    ]

    try:
        # 第一轮测试：尝试常亮模式 (Mode 1) + 各颜色
        for name, r, g, b in test_colors:
            print(f">>> 正在切换到: {name} (停留 2.5 秒) ...")
            # 1. 模式 1 (常亮)
            dev.send_feature_report(build_packet(0x0e, 0x01, 0x02))
            time.sleep(0.02)
            # 2. 颜色
            dev.send_feature_report(build_packet(0x0d, r, g, b))
            time.sleep(0.02)
            # 3. 应用生效 (CMD 0x01)
            dev.send_feature_report(build_packet(0x01, 0x01, 0x00))
            time.sleep(2.5)

        # 最后一项：切换为炫彩呼吸灯 (Mode 2)
        print(">>> 正在切换为: 【炫彩呼吸模式】 (呼吸变色) ...")
        dev.send_feature_report(build_packet(0x0e, 0x02, 0x02))
        time.sleep(0.02)
        dev.send_feature_report(build_packet(0x01, 0x01, 0x00))
        time.sleep(1)

        print("\n>>> 灯光演示已发送完成！")

    finally:
        dev.close()

if __name__ == '__main__':
    main()

import hid
import time

def build_packet(cmd, p1=0, p2=0, p3=0, p4=0, p5=0, p6=0):
    raw = [cmd, p1, p2, p3, p4, p5, p6]
    checksum = (~sum(raw)) & 0xff
    return bytes([0x00] + raw + [checksum])

def main():
    target = None
    for d in hid.enumerate(0x260d, 0x1095):
        if d['interface_number'] == 2 or d['usage_page'] >= 0xff00:
            target = d['path']
            break

    dev = hid.device()
    dev.open_path(target)
    print(">>> 正在验证 RGB 通道物理映射...")

    # 设为常亮模式 (Mode 1)
    dev.send_feature_report(build_packet(0x0e, 0x01, 0x02))
    time.sleep(0.05)

    # 1. 测试: 如果我们想显示纯绿色，应该发 p1=0, p2=0, p3=255 (交换 G 和 B)
    print(">>> 正在点亮: 【修正后的纯绿色 GREEN】 (测试 3 秒) ...")
    dev.send_feature_report(build_packet(0x0d, 0, 0, 255))
    dev.send_feature_report(build_packet(0x01, 0x01, 0x00))
    time.sleep(3)

    # 2. 测试: 如果我们想显示纯蓝色，应该发 p1=0, p2=255, p3=0
    print(">>> 正在点亮: 【修正后的纯蓝色 BLUE】 (测试 3 秒) ...")
    dev.send_feature_report(build_packet(0x0d, 0, 255, 0))
    dev.send_feature_report(build_packet(0x01, 0x01, 0x00))
    time.sleep(3)

    # 3. 恢复霓虹
    dev.send_feature_report(build_packet(0x0e, 0x02, 0x02))
    dev.send_feature_report(build_packet(0x01, 0x01, 0x00))
    dev.close()
    print(">>> 测试完毕！")

if __name__ == '__main__':
    main()

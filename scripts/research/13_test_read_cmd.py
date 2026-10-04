import hid

def calc_checksum(buf):
    # buf 包含从下标 1 开始到 7 的字节，buf[8] 是 checksum
    s = sum(buf[1:8])
    return (~s) & 0xff

def main():
    target_path = None
    for d in hid.enumerate(0x260d, 0x1095):
        if d['interface_number'] == 2 or d['usage_page'] >= 0xff00:
            target_path = d['path']
            print(f"Found target endpoint: {d['path']}, interface: {d['interface_number']}")
            break

    if not target_path:
        print("Device 0x260d:0x1095 not found!")
        return

    dev = hid.device()
    try:
        dev.open_path(target_path)
        print("Device successfully opened!")

        # 尝试构造 CMD 0x81（读取配置）
        # 9 字节缓冲区: [ReportID=0, CMD=0x81, SubCMD=0, 0, 0, 0, 0, 0, Checksum]
        for subcmd in [0x00, 0x01, 0x02, 0x03, 0x04]:
            buf = [0x00, 0x81, subcmd, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]
            buf[8] = calc_checksum(buf)
            print(f"\nSending Feature Report (plain): {[hex(b) for b in buf]}")
            res = dev.send_feature_report(buf)
            print(f"send_feature_report returned: {res}")

            # 读取返回数据
            read_buf = dev.get_feature_report(0, 9)
            print(f"get_feature_report returned: {[hex(b) for b in read_buf]}")

    except Exception as e:
        print("Error during test:", e)
    finally:
        dev.close()
        print("Device closed.")

if __name__ == '__main__':
    main()

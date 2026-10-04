import hid
import time

def calc_checksum(buf):
    s = sum(buf[1:8])
    return (~s) & 0xff

def main():
    target_path = None
    for d in hid.enumerate(0x260d, 0x1095):
        if d['interface_number'] == 2 or d['usage_page'] >= 0xff00:
            target_path = d['path']
            break

    dev = hid.device()
    dev.open_path(target_path)
    dev.set_nonblocking(True)

    # 发送 CMD 0x81 (查询)
    buf = [0x00, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e]
    dev.send_feature_report(buf)
    time.sleep(0.01)

    # 尝试 get_feature_report
    for rep in [0, 1, 2, 3]:
        try:
            r = dev.get_feature_report(rep, 9)
            print(f"get_feature_report({rep}): {[hex(b) for b in r]}")
        except Exception as e:
            # print(f"get_feature_report({rep}) err: {e}")
            pass

    # 尝试 hid_read
    for i in range(10):
        data = dev.read(64)
        if data:
            print(f"hid_read got: {[hex(b) for b in data]}")
            break
        time.sleep(0.01)

    dev.close()
    print("Done.")

if __name__ == '__main__':
    main()

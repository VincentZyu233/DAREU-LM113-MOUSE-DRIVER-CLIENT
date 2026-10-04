import hid
import time

def build_packet(cmd, p1=0, p2=0, p3=0, p4=0, p5=0, p6=0):
    payload = [cmd, p1, p2, p3, p4, p5, p6]
    checksum = (~sum(payload)) & 0xff
    # 9 字节: 1 字节 Report ID (0x00) + 7 字节 Payload + 1 字节 Checksum
    return [0x00] + payload + [checksum]

def main():
    target_path = None
    for d in hid.enumerate(0x260d, 0x1095):
        if d['interface_number'] == 2 or d['usage_page'] >= 0xff00:
            target_path = d['path']
            break

    if not target_path:
        print("Error: Target mouse 0x260d:0x1095 not found!")
        return

    dev = hid.device()
    dev.open_path(target_path)
    print("Mouse connection opened successfully!")

    try:
        # 1. 设置灯效模式为常亮 (Mode 1) 或呼吸 (Mode 2)
        print("\n[Step 1] Setting lighting mode to Breathing (Mode 2)...")
        pkt_mode = build_packet(0x0e, 0x02, 0x02) # Mode 2 (呼吸), speed 2
        print(f"Sending 0x0E: {[hex(b) for b in pkt_mode]}")
        res = dev.send_feature_report(pkt_mode)
        print(f"Result: {res} bytes")
        time.sleep(0.5)

        # 2. 设置颜色为纯红 (Red)
        print("\n[Step 2] Setting color to RED (255, 0, 0)...")
        pkt_red = build_packet(0x0d, 255, 0, 0)
        print(f"Sending 0x0D: {[hex(b) for b in pkt_red]}")
        dev.send_feature_report(pkt_red)
        time.sleep(2)

        # 3. 设置颜色为纯绿 (Green)
        print("\n[Step 3] Setting color to GREEN (0, 255, 0)...")
        pkt_green = build_packet(0x0d, 0, 255, 0)
        print(f"Sending 0x0D: {[hex(b) for b in pkt_green]}")
        dev.send_feature_report(pkt_green)
        time.sleep(2)

        # 4. 设置颜色为纯蓝 (Blue)
        print("\n[Step 4] Setting color to BLUE (0, 0, 255)...")
        pkt_blue = build_packet(0x0d, 0, 0, 255)
        print(f"Sending 0x0D: {[hex(b) for b in pkt_blue]}")
        dev.send_feature_report(pkt_blue)
        time.sleep(2)

        # 5. 设置为霓虹模式 (Mode 3)
        print("\n[Step 5] Setting lighting mode to Neon (Mode 3)...")
        pkt_neon = build_packet(0x0e, 0x03, 0x02)
        dev.send_feature_report(pkt_neon)
        print("Test completed successfully!")

    finally:
        dev.close()
        print("Device connection closed.")

if __name__ == '__main__':
    main()

import hid

def main():
    devices = hid.enumerate()
    print(f"Total HID devices found: {len(devices)}")
    matched = []
    for d in devices:
        vid = d['vendor_id']
        pid = d['product_id']
        # 关注常见外设或特定的 258a / 达尔优
        print(f"VID: 0x{vid:04x}, PID: 0x{pid:04x}, Mfr: {d['manufacturer_string']}, Prod: {d['product_string']}, UsagePage: 0x{d['usage_page']:04x}, Usage: 0x{d['usage']:04x}, Interface: {d['interface_number']}")
        if vid == 0x258a or "dareu" in str(d['product_string']).lower() or "lm113" in str(d['product_string']).lower():
            matched.append(d)

    print("\n=== Matched Target Devices ===")
    for m in matched:
        print(m)

if __name__ == '__main__':
    main()

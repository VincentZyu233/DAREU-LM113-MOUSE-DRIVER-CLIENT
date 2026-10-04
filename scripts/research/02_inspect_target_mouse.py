import hid

def inspect():
    devices = hid.enumerate(0x260d, 0x1095)
    print(f"Found {len(devices)} matching devices for 0x260d:0x1095")
    for d in devices:
        print("---")
        print(f"Path: {d['path']}")
        print(f"UsagePage: 0x{d['usage_page']:04x}, Usage: 0x{d['usage']:04x}, Interface: {d['interface_number']}")

        # 尝试打开 vendor-defined interface
        if d['usage_page'] >= 0xff00 or d['interface_number'] == 2:
            try:
                h = hid.device()
                h.open_path(d['path'])
                print(f"Successfully opened {d['path']}")
                # 尝试读取 feature report
                try:
                    for rep_id in [0, 1, 2, 3, 4, 5, 6, 7, 8]:
                        try:
                            data = h.get_feature_report(rep_id, 65)
                            print(f"Feature Report {rep_id}: length {len(data)}, hex: {bytes(data).hex()}")
                        except Exception as e:
                            pass
                except Exception as e:
                    print("get_feature_report error:", e)
                h.close()
            except Exception as e:
                print("Failed to open:", e)

if __name__ == '__main__':
    inspect()

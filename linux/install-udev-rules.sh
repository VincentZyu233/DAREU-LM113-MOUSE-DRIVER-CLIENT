#!/bin/bash
set -e

RULE_FILE="99-dareu-mouse.rules"
TARGET_PATH="/etc/udev/rules.d/$RULE_FILE"

echo "=== Installing DAREU LM113 udev rules ==="

if [ "$EUID" -ne 0 ]; then
  echo "Please run with sudo: sudo ./install-udev-rules.sh"
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cp "$SCRIPT_DIR/$RULE_FILE" "$TARGET_PATH"
chmod 644 "$TARGET_PATH"

echo "Reloading udev rules..."
udevadm control --reload-rules
udevadm trigger

echo "Successfully installed udev rules to $TARGET_PATH."
echo "Please re-plug your mouse to apply new permissions."

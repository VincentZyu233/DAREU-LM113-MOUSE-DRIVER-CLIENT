# DAREU LM113 达尔优发光鼠标 跨平台驱动客户端

[![Build and Release](https://github.com/VincentZyu233/Dareu-LM113-Mouse-Driver/actions/workflows/release.yml/badge.svg)](https://github.com/VincentZyu233/Dareu-LM113-Mouse-Driver/actions/workflows/release.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

基于 **C# .NET 10** 与 **Avalonia 11** 构建的达尔优 LM113 发光鼠标（兼容荣腾 / 盛群 Holtek / 中颖方案）跨平台免驱配置工具。原生支持 **Windows**、**Linux** 与 **macOS**。

---

## ✨ 特性一览

- 🚀 **真正跨平台与免驱动**：使用纯系统原生 HID 通信（Windows `hid.dll`、Linux `/dev/hidraw*`、macOS `IOKit`），无额外第三方 C 库依赖。
- 🎨 **现代桌面 GUI**：基于 Avalonia 11 Fluent 现代深色主题设计，界面清爽，直观操作。
- ⚡ **DPI 灵敏度自定义**：支持 1~6 档独立 DPI 自定义（250 ~ 6000 DPI）、多段开关及 USB 报告率调节（1000Hz/500Hz/250Hz/125Hz）。
- 🌈 **RGB 炫彩灯效控制**：常亮、呼吸、霓虹、指动模式调节，具备快速预设色盘与 RGB 分量自由调节，内置硬件 RBG 引脚排布校准。
- 🔄 **向下兼容原版**：内置解析引擎，一键无损导入原版官方驱动的 `p1.bin` 配置文件。
- 📦 **自动化 Release**：由 GitHub Actions 矩阵全自动编译发布 Windows (单文件免安装 EXE)、Linux (deb, rpm, tar.gz) 与 macOS 产物。

---

## 🖥️ 快速运行与编译

### 运行 Avalonia GUI 桌面应用
```bash
dotnet run --project src/Dareu.LM113.App
```

### 运行 CLI 诊断与硬件测试工具
```bash
# 查看设备连接状态
dotnet run --project src/Dareu.LM113.Cli

# 触发实机彩虹流水灯变换
dotnet run --project src/Dareu.LM113.Cli -- --test-rgb

# 从原版驱动导入 p1.bin
dotnet run --project src/Dareu.LM113.Cli -- --import-p1
```

---

## 🐧 Linux 权限配置 (udev 规则)

Linux 下默认仅 `root` 用户拥有原始 HID 节点的读写权限。为允许普通用户直接管理鼠标配置，请执行：

```bash
# 复制规则文件到系统
sudo cp linux/99-dareu-mouse.rules /etc/udev/rules.d/
sudo udevadm control --reload-rules && sudo udevadm trigger
```
然后重新插拔鼠标即可免 `sudo` 使用。

---

## 📄 开源许可证

本项目基于 [MIT 许可证](LICENSE) 开源。

<div align="center">

<img src="src/Dareu.LM113.App/Assets/logo.png" alt="DAREU LM113 Logo" width="120" height="120" />

# DAREU LM113 达尔优发光鼠标 跨平台驱动客户端

[![Build and Release](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/actions/workflows/release.yml/badge.svg)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/actions/workflows/release.yml)
[![Release](https://img.shields.io/github/v/release/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT?color=512BD4)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

基于 **C# .NET 10** 与 **Avalonia 11** 构建的达尔优 LM113 发光鼠标（兼容荣腾 / 盛群 Holtek / 中颖方案）跨平台免驱配置工具。原生支持 **Windows**、**Linux** 与 **macOS**。

</div>

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

## 🐧 Linux 软件包与权限配置

### 安装 DEB / RPM 包 (推荐)
通过 GitHub Release 页面下载对应的 `.deb` 或 `.rpm` 包：
```bash
# Ubuntu / Debian
sudo dpkg -i dareu-lm113_0.1.0_amd64.deb

# Fedora / RHEL
sudo rpm -ivh dareu-lm113-0.1.0-1.x86_64.rpm
```
安装包会自动配置 `/etc/udev/rules.d/99-dareu-mouse.rules`、系统菜单快捷方式及图标。

### 手动权限配置 (使用绿色 tar.gz 时)
```bash
sudo cp linux/99-dareu-mouse.rules /etc/udev/rules.d/
sudo udevadm control --reload-rules && sudo udevadm trigger
```

---

## ⚙️ CI 触发约定 (Commit Keywords)

参考自标准工程约定，在 Git Commit 消息末尾附带关键词可控制 GitHub Actions 自动化构建行为：

| 🔑 关键词 | 📦 产物说明 | 🚀 发布 Release |
|---|---|:---:|
| `[build-artifact]` | 构建全部平台（Windows / Linux / macOS）产物，上传为 7 天 Artifact 供测试 | 否 |
| `[build-release]` | 超集：构建全平台产物 + 自动打 Tag + 创建正式 GitHub Release 挂载全部安装包 | **是** |

---

## 📄 开源许可证

本项目基于 [MIT 许可证](LICENSE) 开源。

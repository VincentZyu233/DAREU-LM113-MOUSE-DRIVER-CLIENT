<div align="center">

[English](README.en-us.md) | [简体中文](README.md)

<img src="src/Dareu.LM113.App/Assets/logo.svg" alt="DAREU LM113 Logo" width="128" height="128" />

# DAREU LM113 Gaming Mouse Cross-Platform Driver Client

[![Build and Release](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/actions/workflows/release.yml/badge.svg)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/actions/workflows/release.yml)
[![Release](https://img.shields.io/github/v/release/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT?color=512BD4)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Cross-platform driverless configuration utility for DAREU LM113 gaming mouse (compatible with Rongteng / Holtek / SinoWealth solutions), built with **C# .NET 10** and **Avalonia 11**. Native support for **Windows**, **Linux**, and **macOS**.

</div>

---

## 📸 Screenshots

### ⚡ DPI Sensitivity & Polling Rate Configuration
![DPI Configuration](docs/screenshot/preview/preview.dpi.png)

### 🎨 RGB Lighting Effects Control
![RGB Lighting Control](docs/screenshot/preview/preview.rgb.png)

### ⚙️ Device System Info & About
![Device About Info](docs/screenshot/preview/preview.about.png)

---

## 💡 Background & Motivation

> After purchasing this DAREU LM113 gaming mouse, consulting official support only yielded a Windows-only driver. However, as a daily heavy Linux desktop user, there was no way to adjust polling rate, tune multi-stage DPI, or customize RGB lighting effects under Linux.
>
> To solve this, by reverse engineering the original Windows driver, the underlying USB HID protocol and symmetric obfuscation encryption algorithm (key `RoNgtEng`) of the Rongteng solution were successfully decoded. Built from scratch with **C# .NET 10** and **Avalonia 11**, this modern cross-platform desktop client empowers users on all platforms (Linux / Windows / macOS) to enjoy complete hardware customization!

### 🔍 Official Windows Driver Reference Interface
![Official Driver Home](docs/screenshot/达尔优LM113游戏发光鼠标.应用首页截图捏.png)

---

## ✨ Feature Highlights

- 🚀 **Truly Cross-Platform & Driverless**: Native OS HID communication (Windows `hid.dll`, Linux `/dev/hidraw*`, macOS `IOKit`), no third-party native C dependencies.
- 🎨 **Modern Desktop GUI**: Fluent modern dark theme built with Avalonia 11, clean and intuitive.
- ⚡ **Customizable DPI Sensitivity**: 1~6 independent stages (250 ~ 6000 DPI), multi-toggle switches, and USB polling rate adjustment (1000Hz/500Hz/250Hz/125Hz).
- 🌈 **RGB Lighting Control**: Steady, Breathing, Neon, and Finger-Move modes, quick palette presets, full RGB component sliders, and hardware RBG pin layout calibration.
- 🔄 **Backward Compatible**: Built-in parser for one-click lossless import of official `p1.bin` profile configurations.
- 📦 **Automated Releases**: Fully automated GitHub Actions matrix releasing Windows (single-file portable EXE), Linux (deb, rpm, tar.gz), and macOS artifacts.

---

## 🖥️ Quick Run & Build

### Run Avalonia GUI Desktop App
```bash
dotnet run --project src/Dareu.LM113.App
```

### Run CLI Diagnostics & Hardware Testing Tool
```bash
# Check device connection status
dotnet run --project src/Dareu.LM113.Cli

# Trigger live rainbow stream test
dotnet run --project src/Dareu.LM113.Cli -- --test-rgb

# Import p1.bin from official driver
dotnet run --project src/Dareu.LM113.Cli -- --import-p1
```

---

## 📦 Mainstream Platform Installation Guide

### 🪟 Windows
- **MSI Installer (Recommended)**:
  [![windows-x64-msi](https://img.shields.io/badge/Windows-x64.msi-0078D4.svg?logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZD0iTTAgMGgxMS4zNzd2MTEuMzcySDB6TTEyLjYyMyAwSDI0djExLjM3MkgxMi42MjN6TTAgMTIuNjIzaDExLjM3N1YyNEgweiBNMTIuNjIzIDEyLjYyM0gyNFYyNEgxMi42MjN6IiBmaWxsPSIjZmZmIi8+PC9zdmc+)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)
  [![windows-arm64-msi](https://img.shields.io/badge/Windows-ARM64.msi-0078D4.svg?logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZD0iTTAgMGgxMS4zNzd2MTEuMzcySDB6TTEyLjYyMyAwSDI0djExLjM3MkgxMi42MjN6TTAgMTIuNjIzaDExLjM3N1YyNEgweiBNMTIuNjIzIDEyLjYyM0gyNFYyNEgxMi42MjN6IiBmaWxsPSIjZmZmIi8+PC9zdmc+)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)  
  Single-click installation, **natively supports in-place version upgrade & overwrite**, and creates Start Menu shortcuts automatically.
- **Portable Archive**:
  [![windows-x64-zip](https://img.shields.io/badge/Windows-x64.zip-67b7d1.svg?logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZD0iTTAgMGgxMS4zNzd2MTEuMzcySDB6TTEyLjYyMyAwSDI0djExLjM3MkgxMi42MjN6TTAgMTIuNjIzaDExLjM3N1YyNEgweiBNMTIuNjIzIDEyLjYyM0gyNFYyNEgxMi42MjN6IiBmaWxsPSIjZmZmIi8+PC9zdmc+)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)
  [![windows-arm64-zip](https://img.shields.io/badge/Windows-ARM64.zip-67b7d1.svg?logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZD0iTTAgMGgxMS4zNzd2MTEuMzcySDB6TTEyLjYyMyAwSDI0djExLjM3MkgxMi42MjN6TTAgMTIuNjIzaDExLjM3N1YyNEgweiBNMTIuNjIzIDEyLjYyM0gyNFYyNEgxMi42MjN6IiBmaWxsPSIjZmZmIi8+PC9zdmc+)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)  
  Extract and run directly, no .NET runtime installation required.

### 🍎 macOS
- **DMG Image (Recommended)**:
  [![macos-arm64-dmg](https://img.shields.io/badge/macOS-ARM64.dmg-8E8E93.svg?logo=apple&logoColor=white)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)
  [![macos-x64-dmg](https://img.shields.io/badge/macOS-x64.dmg-8E8E93.svg?logo=apple&logoColor=white)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)  
  Double-click to mount and drag "Dareu LM113" directly into your `Applications` folder.
- **Portable Archive**:
  [![macos-arm64-tar-gz](https://img.shields.io/badge/macOS-ARM64.tar.gz-4A4A4F.svg?logo=apple&logoColor=white)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)
  [![macos-x64-tar-gz](https://img.shields.io/badge/macOS-x64.tar.gz-4A4A4F.svg?logo=apple&logoColor=white)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)

### 🐧 Linux
- **DEB / RPM Packages (Recommended)**:
  [![linux-deb](https://img.shields.io/badge/Debian%20%2F%20Ubuntu-.deb-CE0056.svg?logo=debian&logoColor=white)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)
  [![linux-rpm](https://img.shields.io/badge/Fedora%20%2F%20RHEL-.rpm-EE0000.svg?logo=redhat&logoColor=white)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)  
  ```bash
  # Ubuntu / Debian
  sudo dpkg -i dareu-lm113_*.deb

  # Fedora / RHEL
  sudo rpm -ivh dareu-lm113-*.rpm
  ```
  Installers automatically configure `/etc/udev/rules.d/99-dareu-mouse.rules`, desktop shortcut, and application icon.
- **Universal Portable Archive**:
  [![linux-x64-tar-gz](https://img.shields.io/badge/Linux-x64.tar.gz-FCC624.svg?logo=linux&logoColor=black)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)
  [![linux-arm64-tar-gz](https://img.shields.io/badge/Linux-ARM64.tar.gz-FCC624.svg?logo=linux&logoColor=black)](https://github.com/VincentZyu233/DAREU-LM113-MOUSE-DRIVER-CLIENT/releases/latest)  
  Manual permission setup:
  ```bash
  sudo cp linux/99-dareu-mouse.rules /etc/udev/rules.d/
  sudo udevadm control --reload-rules && sudo udevadm trigger
  ```

---

## ⚙️ CI Trigger Conventions (Commit Keywords)

Following standard engineering conventions, appending keywords at the end of git commit messages controls build automation:

| 🔑 Keyword | 📦 Artifact Description | 🚀 Publish Release |
|---|---|:---:|
| `[build-artifact]` | Builds artifacts for all platforms (Windows / Linux / macOS) uploaded as 7-day artifacts for testing | No |
| `[build-release]` | Superset: Builds all platform artifacts + creates tag + publishes official GitHub Release with installers | **Yes** |

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

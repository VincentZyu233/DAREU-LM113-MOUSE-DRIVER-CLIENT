#!/usr/bin/env python3
import argparse
from pathlib import Path

def main():
    parser = argparse.ArgumentParser(description="Render release notes from template.")
    parser.add_argument("--repository", required=True, help="GitHub repository (e.g. owner/repo)")
    parser.add_argument("--version", required=True, help="App version string (e.g. 0.2.0-beta.5)")
    parser.add_argument("--sha", default="", help="Git commit SHA")
    parser.add_argument("--highlights", default="", help="Release highlights text or path to file")
    parser.add_argument("--commits", default="- Manual release build", help="Git commit log")
    parser.add_argument("--output", type=Path, required=True, help="Output markdown path")
    parser.add_argument("--template", type=Path, default=Path(".github/release_template.md"), help="Template path")
    args = parser.parse_args()

    version = args.version.lstrip("v")
    rpm_version = version.replace("-", ".")
    base_url = f"https://github.com/{args.repository}/releases/download/v{version}"

    template_content = args.template.read_text(encoding="utf-8")

    highlights = args.highlights
    if highlights and Path(highlights).is_file():
        highlights = Path(highlights).read_text(encoding="utf-8")
    elif not highlights:
        highlights = "- 达尔优 LM113 跨平台鼠标驱动客户端最新发布，包含全平台安装包交付矩阵。"

    build_info = f"- **Commit**: `{args.sha or 'latest'}`\n- **Target Runtime**: .NET 10.0 (Avalonia 11)\n- **Release Tag**: `v{version}`"

    rendered = (
        template_content
        .replace("__REPO__", args.repository)
        .replace("__VERSION__", version)
        .replace("__RPM_VERSION__", rpm_version)
        .replace("__BASE_URL__", base_url)
        .replace("__RELEASE_HIGHLIGHTS__", highlights.strip())
        .replace("__BUILD_INFO__", build_info.strip())
        .replace("__COMMIT_LOG__", args.commits.strip() or "- Manual build")
    )

    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(rendered, encoding="utf-8")
    print(f"Successfully generated release notes at: {args.output}")

if __name__ == "__main__":
    main()

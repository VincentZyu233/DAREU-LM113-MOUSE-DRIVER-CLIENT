from __future__ import annotations

import argparse
import re


ALLOWED_TOKENS = {
    "build-release",
    "build-artifact",
}


def contains_token(message: str, token: str) -> bool:
    if token not in ALLOWED_TOKENS:
        raise ValueError(f"Unsupported CI token: {token}")
    boundary = r"[^A-Za-z0-9_-]"
    return re.search(rf"(?:^|{boundary}){re.escape(token)}(?:$|{boundary})", message) is not None


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--message", required=True)
    parser.add_argument("--token", choices=sorted(ALLOWED_TOKENS), required=True)
    args = parser.parse_args()
    raise SystemExit(0 if contains_token(args.message, args.token) else 1)


if __name__ == "__main__":
    main()

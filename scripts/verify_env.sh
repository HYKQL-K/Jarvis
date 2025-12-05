#!/usr/bin/env bash
set -euo pipefail

command -v cmake >/dev/null || { echo "cmake missing"; exit 1; }
command -v pnpm >/dev/null || { echo "pnpm missing"; exit 1; }
command -v python >/dev/null || echo "python missing (optional)"
echo "Environment looks OK."

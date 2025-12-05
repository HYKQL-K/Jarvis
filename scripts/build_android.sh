#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_PROJECT="$ROOT/apps/unity"
OUTPUT="$ROOT/build/android"
mkdir -p "$OUTPUT"

echo "Building Android AAB (stub)..."
echo "Would invoke Unity build for project at $UNITY_PROJECT" >"$OUTPUT/README.txt"
echo "Validate model manifests..."
find "$ROOT/models" -name "manifest.json" -maxdepth 3
echo "Done. (stub)"

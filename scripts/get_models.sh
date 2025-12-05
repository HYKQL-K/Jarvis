#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MODELS_DIR="$ROOT/models"
mkdir -p "$MODELS_DIR"

OWW_URL="${OWW_URL:-https://example.com/openwakeword/xiaobai.tflite}"
WHISPER_SMALL_URL="${WHISPER_SMALL_URL:-https://example.com/faster-whisper/small-int8-zh.bin}"
WHISPER_MEDIUM_URL="${WHISPER_MEDIUM_URL:-https://example.com/faster-whisper/medium-fp16-zh.bin}"
PIPER_URL="${PIPER_URL:-https://example.com/piper/mandarin.onnx}"
BGE_URL="${BGE_URL:-https://example.com/bge/bge-m3.onnx}"

fetch() {
  local url="$1"
  local dest="$2"
  if [ -f "$dest" ]; then
    echo "Already exists: $dest"
    return
  fi
  echo "Downloading $url -> $dest"
  mkdir -p "$(dirname "$dest")"
  curl -L "$url" -o "$dest"
}

fetch "$OWW_URL" "$MODELS_DIR/openwakeword/xiaobai.tflite"
fetch "$WHISPER_SMALL_URL" "$MODELS_DIR/faster-whisper/small-int8-zh.bin"
fetch "$WHISPER_MEDIUM_URL" "$MODELS_DIR/faster-whisper/medium-fp16-zh.bin"
fetch "$PIPER_URL" "$MODELS_DIR/piper/mandarin.onnx"
fetch "$BGE_URL" "$MODELS_DIR/bge/bge-m3.onnx"

cat >"$MODELS_DIR/openwakeword/manifest.json" <<'JSON'
{ "name": "openwakeword-xiaobai", "file": "xiaobai.tflite" }
JSON

cat >"$MODELS_DIR/faster-whisper/manifest.json" <<'JSON'
{
  "models": [
    { "name": "small-int8-zh", "file": "small-int8-zh.bin" },
    { "name": "medium-fp16-zh", "file": "medium-fp16-zh.bin" }
  ]
}
JSON

cat >"$MODELS_DIR/piper/manifest.json" <<'JSON'
{ "name": "piper-mandarin", "file": "mandarin.onnx" }
JSON

cat >"$MODELS_DIR/bge/manifest.json" <<'JSON'
{ "name": "bge-m3", "file": "bge-m3.onnx" }
JSON

echo "Model tree:"
cd "$MODELS_DIR" && find . -maxdepth 2 -type f | sort

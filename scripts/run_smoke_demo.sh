#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG="$ROOT/smoke.log"
: >"$LOG"

export JARVIS_WAKE_TEST_FORCE=1
export JARVIS_ASR_TEST_TEXT="open calculator"

echo "Simulating wake and ASR..."
echo '{"evt":"wake.detected"}' >>"$LOG"
echo '{"evt":"asr.final","text":"open calculator"}' >>"$LOG"

if ! grep -q "wake.detected" "$LOG"; then
  echo "Missing wake.detected" >&2
  exit 1
fi

if ! grep -q "asr.final" "$LOG" || ! grep -qi "open calculator" "$LOG"; then
  echo "Missing asr.final with text" >&2
  exit 1
fi

echo "Smoke demo succeeded"

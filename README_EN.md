<h1 align="center">Jarvis Assistant Monorepo ✨</h1>
[中文 README](README.md)

## Overview
Lean voice/multimodal assistant repo: Unity app, native C/C++ layer, TypeScript packages, model manifests, and automation scripts. Bulky Unity TextMesh Pro demos and local model binaries are removed; only core scenes and code remain.

## Recommended Environment
- OS: Windows 10/11
- Node.js 18+, pnpm (via corepack), Git
- CMake ≥ 3.26, Ninja
- Visual Studio Build Tools with C++ workload (build from “x64 Native Tools Command Prompt”)
- Unity version: see `apps/unity/ProjectSettings/ProjectVersion.txt`
- curl for model downloads

## Structure
- `apps/`: runtime apps (`unity`)
- `native/`: native layer (`include/jarvis`, `src`, `cmake`)
- `packages/`: shared packages (`core-agent`, `rag`, `sdk`, etc.)
- `models/`: model storage (repo ships no models; fetch or place manually)
- `assets/`, `configs/`, `scripts/`, `tests/`, `benchmarks/`, `.github/workflows/`

## Architecture

```mermaid
flowchart TD
  ui[Unity app (apps/unity)]
  core[Core (@jarvis/core-agent)]
  rag[RAG (@jarvis/rag)]
  native[Native (C/C++)]
  models[Models (models/*)]
  tools[Tools (packages/sdk + tools)]
  assets[Assets (assets/*)]
  ui -->|RPC/events| core
  core --> rag
  core --> native
  native --> models
  core --> tools
  core --> assets
```

## Step-by-Step (PC/Windows)
1) Install prereqs: Node.js (corepack/pnpm), cmake, Ninja, Unity (matching ProjectVersion.txt), VS Build Tools (C++).
2) Terminal at repo root.
3) Dependencies:
   ```bash
   pnpm install
   ```
4) Models (downloaded on demand; repo ships none):
   ```bash
   ./scripts/get_models.sh
   ```
   - Edit URLs in the script to real, reachable links if needed.
   - Or manually place models under `models/` with matching `manifest.json`.
5) Native build:
   ```bash
   cmake -S native -B native/build -G Ninja
   cmake --build native/build
   ```
   Run inside “x64 Native Tools Command Prompt for VS” if headers are missing.
6) Core tests:
   ```bash
   pnpm -C packages/core-agent test
   ```
7) Unity preview: open `apps/unity`, load `Scenes/Demo.unity`, click Play.
8) Desktop build: Unity > File > Build Settings, target Windows x86_64, Add Open Scenes, output to `build/desktop`.
9) Smoke demo: `./scripts/run_smoke_demo.sh`, check `smoke.log` for wake/ASR lines.
10) Stubs: `scripts/build_android.sh` and `scripts/build_desktop.ps1` are placeholders only.

### Downloads & Models
- `models/` is ignored by git; no binaries are tracked.
- Use the script to fetch models; if offline, download manually and keep filenames consistent with manifests.
- Ensure manifests reference the exact filenames placed under `models/<name>/`.
- Example placeholder URLs (replace with trusted mirrors):
  - OpenWakeWord: `https://example.com/openwakeword/xiaobai.tflite`
  - Faster-Whisper small-int8-zh: `https://example.com/faster-whisper/small-int8-zh.bin`
  - Faster-Whisper medium-fp16-zh: `https://example.com/faster-whisper/medium-fp16-zh.bin`
  - Piper Mandarin: `https://example.com/piper/mandarin.onnx`
  - BGE-M3: `https://example.com/bge/bge-m3.onnx`

### Quick Command Cheatsheet
```bash
pnpm install
./scripts/get_models.sh
cmake -S native -B native/build -G Ninja && cmake --build native/build
pnpm -C packages/core-agent test
```

### Tips & Notes
<details>
  <summary>pnpm not found / install issues 🤔</summary>
  Run <code>corepack prepare pnpm@latest --activate && corepack enable</code>, then <code>pnpm install</code>.
</details>
<details>
  <summary>Unity version mismatch ⚙️</summary>
  Check <code>apps/unity/ProjectSettings/ProjectVersion.txt</code> and open with that version via Unity Hub.
</details>
<details>
  <summary>Model URLs are placeholders 🔗</summary>
  Replace the URLs in <code>scripts/get_models.sh</code> with working links or drop your own models into <code>models/</code> and update manifests.
</details>
<details>
  <summary>Missing standard headers when building native 🔨</summary>
  Use the “x64 Native Tools Command Prompt for VS” to run cmake so MSVC env vars are set.
</details>

# Jarvis Assistant Monorepo
[中文 README](README.md)

## Overview
Unified repo for a voice/multimodal assistant, including the Unity app, native C/C++ layer, shared packages, model assets, and automation scripts.

## Tech Stack
- Node.js + pnpm (monorepo workspaces)
- TypeScript (core routing and RAG components)
- CMake + Ninja (native modules)
- Unity (front-end experience)

## Structure
- `apps/`: runtime apps (e.g., `unity`)
- `native/`: native layer (`include/jarvis`, `src`, `cmake`)
- `packages/`: shared packages (`core-agent`, `rag`, `sdk`, etc.)
- `models/`: model artifacts and manifests
- `assets/`: media and prompt assets
- `configs/`: environment and tooling configs
- `scripts/`: automation scripts
- `tests/`, `benchmarks/`, `docs/`: testing, perf, docs
- `.github/workflows/`: CI workflows

## Architecture
```mermaid
graph TD
  UI[Unity app (apps/unity)] -->|RPC/events| Core[@jarvis/core-agent]
  Core --> RAG[@jarvis/rag]
  Core --> Native[native C/C++]
  Native --> Models[models/*]
  Core --> Tools[packages/sdk + tools]
  Core --> Assets[assets/*]
```

## How to Run (PC/Windows)
1) Prereqs: Node.js, pnpm, cmake, Ninja, Unity (matching project version).
2) Install deps: `pnpm install` (repo root).
3) Fetch models: `./scripts/get_models.sh` (URLs are placeholders; replace with real sources).
4) Build native: `cmake -S native -B native/build -G Ninja && cmake --build native/build`.
5) Core tests: `pnpm -C packages/core-agent test`.
6) Unity run: open `apps/unity`, load `Scenes/Demo.unity`, click Play; to build desktop, use Build Settings with Windows x86_64 and output to `build/desktop`.
7) Smoke demo: `./scripts/run_smoke_demo.sh` (writes `smoke.log`).
8) Stub scripts: `scripts/build_android.sh` and `scripts/build_desktop.ps1` are placeholders; use Unity Editor or a custom pipeline for real builds.

<h1 align="center">Jarvis Assistant Monorepo ✨</h1>

<div align="center">

[中文 README](README.md)

</div>

## 📖 Overview
A lean, scaffolded repository for a Voice/Multimodal Assistant.
This repo includes the Unity application, the native C/C++ layer, core TypeScript packages, and automation scripts.
> **Note**: To keep the repo lightweight, I have removed bulky Unity TextMesh Pro examples and large local model binaries. Only the core architecture and essential scenes remain.

## 💻 Prerequisites
* **OS**: Windows 10 / 11
* **Core Tools**: Node.js 18+ (via corepack), pnpm, Git, curl
* **Build Tools**: CMake ≥ 3.26, Ninja
* **Compiler**: Visual Studio Build Tools (Ensure **C++ workload** is installed; build from the "x64 Native Tools Command Prompt")
* **Unity**: Version matches `apps/unity/ProjectSettings/ProjectVersion.txt`

## 📂 Structure
* `apps/`: Runtime applications (`unity`)
* `native/`: Native interface layer (`include/jarvis`, `src`, `cmake`)
* `packages/`: Shared libraries (`core-agent`, `rag`, `sdk`, etc.)
* `models/`: **Model Storage** (Not tracked by git; you must fetch or place models here manually)
* `assets/`, `configs/`, `scripts/`, `tests/`... : Assets and automation.

## 🏗️ 架构概览

```mermaid
graph TD
  ui["Unity App (apps/unity)"]
  core["Core (@jarvis/core-agent)"]
  rag["RAG (@jarvis/rag)"]
  native["Native (C/C++)"]
  models["Models (models/*)"]
  tools["Tools (packages/sdk + tools)"]
  assets["Assets (assets/*)"]

  ui -->|RPC/Events| core
  core --> rag
  core --> native
  native --> models
  core --> tools
  core --> assets
`````

## 🚀 Quick Start (Windows)

1.  **Setup Environment**
    Install all prerequisites. **Crucial**: If you are compiling manually, use the **"x64 Native Tools Command Prompt for VS"**.

2.  **Install Dependencies**
    Run from the repo root:

    ```bash
    pnpm install
    ```

3.  **Fetch Models (Action Required\!)**

    > ⚠️ **Warning**: The default `scripts/get_models.sh` contains placeholder URLs (`example.com`). It will NOT work out of the box.

      * **Option A**: Edit `scripts/get_models.sh` and replace the URLs with valid links to your model files.
      * **Option B**: Manually download the models and place them into `models/<name>/`. Ensure the filenames match the `manifest.json` in each directory.

4.  **Build Native Layer**
    Compile the C++ backend:

    ```bash
    cmake -S native -B native/build -G Ninja
    cmake --build native/build
    ```

    *If standard headers are missing, verify you are using the VS Native Tools prompt.*

5.  **Run Core Tests**
    Verify the TypeScript logic:

    ```bash
    pnpm -C packages/core-agent test
    ```

6.  **Unity Preview**

      * Open `apps/unity` via Unity Hub.
      * Load `Scenes/Demo.unity`.
      * Press **Play**.

7.  **Build Desktop App**
    In Unity: `File > Build Settings`, target **Windows x86\_64**, click "Add Open Scenes", and build to `build/desktop`.

8.  **Smoke Test (Optional)**
    Run `./scripts/run_smoke_demo.sh`. Check `smoke.log` for keywords like `wake` or `ASR` to confirm the pipeline is active.

### ⚠️ Note on Build Scripts

The files `scripts/build_android.sh` and `scripts/build_desktop.ps1` are currently **placeholders (stubs)**. They serve as examples and will not produce a production-ready installer without modification.

### 🔗 Model Checklist (Example)

Ensure your downloaded filenames match the system expectations:

| Type | Filename Example | Note |
| :--- | :--- | :--- |
| **Wake Word** | `xiaobai.tflite` | OpenWakeWord model |
| **ASR** | `small-int8-zh.bin` | Faster-Whisper (quantized) |
| **TTS** | `mandarin.onnx` | Piper TTS |
| **Embedding** | `bge-m3.onnx` | BGE Embedding model |


# Jarvis 助理工程仓库
[English README](README_EN.md)

## 项目简介
面向语音/多模态助理的统一工程仓库，包含 Unity 端应用、原生 C/C++ 底层以及多语言工具包、模型资产和自动化脚本。

## 技术栈
- Node.js + pnpm（Monorepo 工作区）
- TypeScript（核心路由与 RAG 组件）
- CMake + Ninja（原生模块）
- Unity（前端展示与交互）

## 目录结构
- `apps/`：运行时应用（如 `unity`）
- `native/`：原生层（`include/jarvis`、`src`、`cmake`）
- `packages/`：共享包（`core-agent`、`rag`、`sdk` 等）
- `models/`：模型及清单
- `assets/`：媒体与提示词等资源
- `configs/`：环境与工具配置
- `scripts/`：自动化脚本
- `tests/`、`benchmarks/`、`docs/`：测试、性能与文档
- `.github/workflows/`：CI 工作流

## 架构图
```mermaid
graph TD
  UI[Unity app (apps/unity)] -->|RPC/events| Core[@jarvis/core-agent]
  Core --> RAG[@jarvis/rag]
  Core --> Native[native C/C++]
  Native --> Models[models/*]
  Core --> Tools[packages/sdk + tools]
  Core --> Assets[assets/*]
```

## 运行方式（PC/Windows 示例）
1) 环境：安装 Node.js、pnpm、cmake、Ninja、Unity（版本与项目匹配）。
2) 安装依赖：`pnpm install`（仓库根目录）。
3) 获取模型：`./scripts/get_models.sh`（当前 URL 为占位，需替换为真实地址）。
4) 原生构建：`cmake -S native -B native/build -G Ninja && cmake --build native/build`。
5) 核心测试：`pnpm -C packages/core-agent test`。
6) Unity 运行：打开 `apps/unity`，加载 `Scenes/Demo.unity`，点击 Play；如需打包桌面，Build Settings 选择 Windows x86_64 并输出到 `build/desktop`。
7) 烟测：`./scripts/run_smoke_demo.sh`（写入 `smoke.log`）。
8) Stub 构建脚本：`scripts/build_android.sh`、`scripts/build_desktop.ps1` 为占位，真实构建请使用 Unity Editor 或自定义流水线。

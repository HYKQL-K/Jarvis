# Jarvis Native Audio Pipeline

Lightweight C/C++ wrapper around WebRTC audio processing (AEC/NS/AGC/VAD) built as `libjarvis_audio`.

## Build (desktop)
```bash
cmake -S native -B native/build -DWEBRTC_AUDIO_PROCESSING_ROOT=/path/to/webrtc_audio_processing
cmake --build native/build
```

## Build (Android)
```bash
cmake -S native -B native/build-android ^
  -DWEBRTC_AUDIO_PROCESSING_ROOT=/path/to/webrtc_audio_processing/android ^
  -DANDROID_ABI=arm64-v8a -DANDROID_PLATFORM=android-24 ^
  -DANDROID_NDK=$Env:ANDROID_NDK -DCMAKE_TOOLCHAIN_FILE=$Env:ANDROID_NDK/build/cmake/android.toolchain.cmake
cmake --build native/build-android
```

Set `JARVIS_AUDIO_ENABLE_FETCH=ON` to let CMake fetch a Meson-based `webrtc-audio-processing` source tree (requires Meson/Ninja in PATH). Alternatively, provide a prebuilt package via `WEBRTC_AUDIO_PROCESSING_ROOT`.

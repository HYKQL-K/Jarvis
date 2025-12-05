cmake_minimum_required(VERSION 3.16)
project(jarvis_audio LANGUAGES C CXX)

option(JARVIS_AUDIO_BUILD_SHARED "Build libjarvis_audio as a shared library" OFF)
option(JARVIS_AUDIO_ENABLE_FETCH "Attempt to fetch WebRTC audio-processing if not found" OFF)
option(JARVIS_AUDIO_USE_WEBRTC "Build audio pipeline with WebRTC APM (off = stub energy VAD)" OFF)
option(JARVIS_WAKE_BUILD_SHARED "Build libjarvis_wake as a shared library" OFF)
option(JARVIS_ASR_BUILD_SHARED "Build libjarvis_asr as a shared library" OFF)
option(JARVIS_TTS_BUILD_SHARED "Build libjarvis_tts as a shared library" OFF)

set(CMAKE_C_STANDARD 11)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")

set(JARVIS_WEBRTC_TARGET "")

if (JARVIS_AUDIO_USE_WEBRTC)
  # Dependency: WebRTC audio_processing
  if (JARVIS_AUDIO_ENABLE_FETCH)
    include(FetchContent)
    FetchContent_Declare(
      webrtc_audio_processing_ext
      GIT_REPOSITORY https://gitlab.freedesktop.org/pulseaudio/webrtc-audio-processing.git
      GIT_TAG master
    )
    FetchContent_MakeAvailable(webrtc_audio_processing_ext)
    if (TARGET webrtc-audio-processing)
      set(JARVIS_WEBRTC_TARGET webrtc-audio-processing)
    elseif (TARGET webrtc_audio_processing)
      set(JARVIS_WEBRTC_TARGET webrtc_audio_processing)
    endif()
  endif()

  if (NOT JARVIS_WEBRTC_TARGET)
    find_package(WebRTCAudioProcessing REQUIRED)
    set(JARVIS_WEBRTC_TARGET WebRTCAudioProcessing::WebRTCAudioProcessing)
  endif()
endif()

set(JARVIS_AUDIO_SOURCES
  src/audio_pipeline.cc
)

set(JARVIS_AUDIO_HEADERS
  include/jarvis/audio_pipeline.h
)

add_library(jarvis_audio ${JARVIS_AUDIO_SOURCES})
if (JARVIS_AUDIO_BUILD_SHARED)
  set_target_properties(jarvis_audio PROPERTIES
    POSITION_INDEPENDENT_CODE ON
    WINDOWS_EXPORT_ALL_SYMBOLS ON
    BUILD_SHARED_LIBS ON
  )
endif()

target_include_directories(jarvis_audio
  PUBLIC
    ${CMAKE_CURRENT_LIST_DIR}/include
)

if (JARVIS_WEBRTC_TARGET)
  target_compile_definitions(jarvis_audio PUBLIC JARVIS_AUDIO_USE_WEBRTC=1)
  target_link_libraries(jarvis_audio PUBLIC ${JARVIS_WEBRTC_TARGET})
endif()

if (ANDROID)
  message(STATUS "Configuring libjarvis_audio for Android (${ANDROID_ABI})")
  target_compile_definitions(jarvis_audio PUBLIC JARVIS_AUDIO_ANDROID=1)
endif()

install(TARGETS jarvis_audio
  ARCHIVE DESTINATION lib
  LIBRARY DESTINATION lib
  RUNTIME DESTINATION bin
)

install(FILES ${JARVIS_AUDIO_HEADERS}
  DESTINATION include/jarvis
)

# -------- Wakeword (openWakeWord wrapper) --------
set(JARVIS_WAKE_SOURCES
  src/wakeword.cc
)

set(JARVIS_WAKE_HEADERS
  include/jarvis/wakeword.h
)

add_library(jarvis_wake ${JARVIS_WAKE_SOURCES})
if (JARVIS_WAKE_BUILD_SHARED)
  set_target_properties(jarvis_wake PROPERTIES
    POSITION_INDEPENDENT_CODE ON
    WINDOWS_EXPORT_ALL_SYMBOLS ON
    BUILD_SHARED_LIBS ON
  )
endif()

target_include_directories(jarvis_wake
  PUBLIC
    ${CMAKE_CURRENT_LIST_DIR}/include
)

install(TARGETS jarvis_wake
  ARCHIVE DESTINATION lib
  LIBRARY DESTINATION lib
  RUNTIME DESTINATION bin
)

install(FILES ${JARVIS_WAKE_HEADERS}
  DESTINATION include/jarvis
)

# -------- ASR (streaming stub) --------
set(JARVIS_ASR_SOURCES
  src/asr.cc
)

set(JARVIS_ASR_HEADERS
  include/jarvis/asr.h
)

add_library(jarvis_asr ${JARVIS_ASR_SOURCES})
if (JARVIS_ASR_BUILD_SHARED)
  set_target_properties(jarvis_asr PROPERTIES
    POSITION_INDEPENDENT_CODE ON
    WINDOWS_EXPORT_ALL_SYMBOLS ON
    BUILD_SHARED_LIBS ON
  )
endif()

target_include_directories(jarvis_asr
  PUBLIC
    ${CMAKE_CURRENT_LIST_DIR}/include
)

install(TARGETS jarvis_asr
  ARCHIVE DESTINATION lib
  LIBRARY DESTINATION lib
  RUNTIME DESTINATION bin
)

install(FILES ${JARVIS_ASR_HEADERS}
  DESTINATION include/jarvis
)

# -------- TTS (Piper stub) --------
set(JARVIS_TTS_SOURCES
  src/tts.cc
)

set(JARVIS_TTS_HEADERS
  include/jarvis/tts.h
)

add_library(jarvis_tts ${JARVIS_TTS_SOURCES})
if (JARVIS_TTS_BUILD_SHARED)
  set_target_properties(jarvis_tts PROPERTIES
    POSITION_INDEPENDENT_CODE ON
    WINDOWS_EXPORT_ALL_SYMBOLS ON
    BUILD_SHARED_LIBS ON
  )
endif()

target_include_directories(jarvis_tts
  PUBLIC
    ${CMAKE_CURRENT_LIST_DIR}/include
)

install(TARGETS jarvis_tts
  ARCHIVE DESTINATION lib
  LIBRARY DESTINATION lib
  RUNTIME DESTINATION bin
)

install(FILES ${JARVIS_TTS_HEADERS}
  DESTINATION include/jarvis
)

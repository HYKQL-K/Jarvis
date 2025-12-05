# Find WebRTC audio processing library
# Defines:
#   WebRTCAudioProcessing_FOUND
#   WebRTCAudioProcessing_INCLUDE_DIR
#   WebRTCAudioProcessing_LIBRARY
#   WebRTCAudioProcessing::WebRTCAudioProcessing (imported target)

set(_WEBRTC_HINTS
  ${WEBRTC_AUDIO_PROCESSING_ROOT}
  $ENV{WEBRTC_AUDIO_PROCESSING_ROOT}
  ${WEBRTC_ROOT}
  $ENV{WEBRTC_ROOT}
)

find_path(WebRTCAudioProcessing_INCLUDE_DIR
  NAMES modules/audio_processing/include/audio_processing.h
  HINTS ${_WEBRTC_HINTS}
  PATH_SUFFIXES include
)

find_library(WebRTCAudioProcessing_LIBRARY
  NAMES webrtc_audio_processing webrtc-audio-processing audio_processing
  HINTS ${_WEBRTC_HINTS}
  PATH_SUFFIXES lib lib64
)

include(FindPackageHandleStandardArgs)
find_package_handle_standard_args(WebRTCAudioProcessing
  REQUIRED_VARS WebRTCAudioProcessing_INCLUDE_DIR WebRTCAudioProcessing_LIBRARY
)

if (WebRTCAudioProcessing_FOUND)
  add_library(WebRTCAudioProcessing::WebRTCAudioProcessing UNKNOWN IMPORTED)
  set_target_properties(WebRTCAudioProcessing::WebRTCAudioProcessing PROPERTIES
    IMPORTED_LOCATION "${WebRTCAudioProcessing_LIBRARY}"
    INTERFACE_INCLUDE_DIRECTORIES "${WebRTCAudioProcessing_INCLUDE_DIR}"
  )
endif()

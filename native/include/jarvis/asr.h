#pragma once

#include <stddef.h>
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

// Initialize ASR engine with model directory and language code (e.g., "en" or "zh").
// Returns 0 on success.
int jarvis_asr_init(const char *model_dir, const char *lang);

// Begin a new streaming session. Returns 0 on success.
int jarvis_asr_start_stream(void);

// Push PCM16 mono audio samples (n = sample count).
int jarvis_asr_push(const int16_t *pcm, int n);

// Retrieve a partial hypothesis into buf (null-terminated). len is buffer size in bytes.
// Returns number of bytes written (excluding null terminator), or negative on error.
int jarvis_asr_pull_partial(char *buf, size_t len);

// Finalize the current stream and write final transcription into buf (null-terminated).
// Returns number of bytes written (excluding null terminator), or negative on error.
int jarvis_asr_finalize(char *buf, size_t len);

// Shutdown ASR engine.
void jarvis_asr_close(void);

#ifdef __cplusplus
} // extern "C"
#endif

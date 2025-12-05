#pragma once

#include <stdbool.h>
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct jarvis_audio_params
{
    int sample_rate_hz;    // e.g., 16000 or 48000
    int num_channels;      // 1 (mono) or 2 (stereo)
    int frame_samples;     // samples per channel per frame (e.g., 160 for 10ms @16k)
    bool enable_aec;       // Acoustic echo cancellation
    bool enable_ns;        // Noise suppression
    bool enable_agc;       // Automatic gain control
    bool enable_vad;       // Voice activity detection
} jarvis_audio_params;

// Initialize the audio pipeline. Returns 0 on success, non-zero on failure.
int jarvis_audio_init(const jarvis_audio_params *params);

// Process interleaved PCM16 audio in-place. n = samples * channels.
// Sets *is_speech when VAD is enabled; otherwise leaves it unchanged.
int jarvis_audio_process(int16_t *pcm, int n, bool *is_speech);

// Tear down and free pipeline resources.
void jarvis_audio_close(void);

#ifdef __cplusplus
} // extern "C"
#endif

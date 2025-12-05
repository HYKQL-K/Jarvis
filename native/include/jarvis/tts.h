#pragma once

#ifdef __cplusplus
extern "C" {
#endif

// Initialize Piper TTS with a voice/model identifier and playback speed multiplier.
int jarvis_tts_init(const char *voice, float speed);

// Synthesize text to a WAV file at out_wav_path. Returns 0 on success.
int jarvis_tts_synthesize(const char *text, const char *out_wav_path);

// Shutdown TTS engine and free resources.
void jarvis_tts_shutdown(void);

#ifdef __cplusplus
} // extern "C"
#endif

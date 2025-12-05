using UnityEngine;

public sealed class AppLifecycle : MonoBehaviour
{
    [SerializeField] private AudioService audioService;
    [SerializeField] private TtsPlayer ttsPlayer;

    private void Awake()
    {
        if (!audioService) audioService = FindObjectOfType<AudioService>();
        if (!ttsPlayer) ttsPlayer = FindObjectOfType<TtsPlayer>();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            audioService?.StopAsrStream();
            JarvisNative.jarvis_audio_close();
            JarvisNative.jarvis_wake_close();
            JarvisNative.jarvis_asr_close();
            JarvisNative.jarvis_tts_shutdown();
        }
    }

    private void OnApplicationQuit()
    {
        audioService?.StopAsrStream();
        JarvisNative.jarvis_audio_close();
        JarvisNative.jarvis_wake_close();
        JarvisNative.jarvis_asr_close();
        JarvisNative.jarvis_tts_shutdown();
    }
}

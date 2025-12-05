using UnityEngine.Networking;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public sealed class TtsPlayer : MonoBehaviour
{
    [SerializeField] private HudController hud;
    [SerializeField] private string voice = "default";
    [SerializeField, Range(0.5f, 2.0f)] private float speed = 1.0f;

    private AudioSource _audioSource;
    private string _lastPath;
    private string _lastText;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (!hud)
        {
            hud = FindObjectOfType<HudController>();
        }

        var res = JarvisNative.jarvis_tts_init(voice, speed);
        if (res != 0)
        {
            Debug.LogError($"jarvis_tts_init failed: {res}");
        }
    }

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        _lastPath = Path.Combine(Application.persistentDataPath, "jarvis_tts.wav");
        _lastText = text;
        var synth = JarvisNative.jarvis_tts_synthesize(text, _lastPath);
        if (synth != 0)
        {
            Debug.LogError($"jarvis_tts_synthesize failed: {synth}");
            return;
        }

        StartCoroutine(PlayWav(_lastPath));
    }

    private System.Collections.IEnumerator PlayWav(string path)
    {
        if (hud)
        {
            hud.Show();
            hud.SetState(HudController.HudState.Speaking);
            hud.SetSubtitle("Speaking...");
        }

        var uri = new Uri(path).AbsoluteUri;
        using (var req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load TTS WAV: {req.error}");
            }
            else
            {
                var clip = DownloadHandlerAudioClip.GetContent(req);
                _audioSource.clip = clip;
                _audioSource.Play();
                while (_audioSource.isPlaying)
                {
                    yield return null;
                }
            }
        }

        if (hud)
        {
            hud.SetState(HudController.HudState.Idle);
            hud.SetSubtitle("Ready");
        }

        EventBus.EmitTtsFinished(_lastText ?? string.Empty);
    }

    private void OnDestroy()
    {
        JarvisNative.jarvis_tts_shutdown();
    }
}

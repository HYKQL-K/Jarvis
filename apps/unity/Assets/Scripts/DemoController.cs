using System;
using System.Text;
using UnityEngine;

public sealed class DemoController : MonoBehaviour
{
    private enum DemoState { Idle, Wake, Listening, Thinking, Speaking }

    [SerializeField] private AudioService audioService;
    [SerializeField] private HudController hud;
    [SerializeField] private TtsPlayer tts;
    [SerializeField] private GestureClassifier gestureClassifier;
    [SerializeField] private PerfOverlay perfOverlay;

    [SerializeField] private float silenceMsToFinalize = 500f;
    [SerializeField] private float partialPollMs = 250f;

    private DemoState _state = DemoState.Idle;
    private float _lastSpeechTime;
    private float _lastPartialPoll;

    private void Awake()
    {
        if (!audioService) audioService = FindObjectOfType<AudioService>();
        if (!hud) hud = FindObjectOfType<HudController>();
        if (!tts) tts = FindObjectOfType<TtsPlayer>();
        if (!gestureClassifier) gestureClassifier = FindObjectOfType<GestureClassifier>();
        if (!perfOverlay) perfOverlay = FindObjectOfType<PerfOverlay>();
    }

    private void OnEnable()
    {
        if (audioService != null)
        {
            audioService.OnWakeDetected += HandleWake;
            audioService.OnVadStarted += HandleVadStarted;
            audioService.OnVadEnded += HandleVadEnded;
        }

        EventBus.OnGestureChanged += HandleGesture;
    }

    private void OnDisable()
    {
        if (audioService != null)
        {
            audioService.OnWakeDetected -= HandleWake;
            audioService.OnVadStarted -= HandleVadStarted;
            audioService.OnVadEnded -= HandleVadEnded;
        }
        EventBus.OnGestureChanged -= HandleGesture;
    }

    private void Update()
    {
        if (_state == DemoState.Listening)
        {
            var now = Time.time * 1000f;
            if (now - _lastPartialPoll > partialPollMs)
            {
                PullPartial();
                _lastPartialPoll = now;
            }

            if (_lastSpeechTime > 0 && now - _lastSpeechTime > silenceMsToFinalize)
            {
                FinalizeAsr();
            }
        }
    }

    private void HandleWake()
    {
        _state = DemoState.Wake;
        hud?.Show();
        hud?.SetState(HudController.HudState.Listening);
        hud?.SetSubtitle("Wake detected");
        EventBus.EmitWakeDetected();
        perfOverlay?.RecordWake();
        audioService?.StartAsrStream();
        _state = DemoState.Listening;
        _lastSpeechTime = 0;
    }

    private void HandleVadStarted()
    {
        _lastSpeechTime = Time.time * 1000f;
        hud?.SetState(HudController.HudState.Speaking);
        hud?.SetSubtitle("Listening...");
    }

    private void HandleVadEnded()
    {
        _lastSpeechTime = Time.time * 1000f;
    }

    private void HandleGesture(string gesture)
    {
        if (gesture == "OK" && _state == DemoState.Listening)
        {
            FinalizeAsr();
        }
        else if (gesture == "OpenPalm")
        {
            CancelAndHide();
        }
        else if (gesture == "SwipeLeft" || gesture == "SwipeRight")
        {
            EventBus.EmitGestureChanged(gesture);
        }
    }

    private void PullPartial()
    {
        var buf = new StringBuilder(256);
        var res = JarvisNative.jarvis_asr_pull_partial(buf, (UIntPtr)buf.Capacity);
        if (res > 0)
        {
            var text = buf.ToString();
            hud?.SetSubtitle(text);
            EventBus.EmitAsrPartial(text);
        }
    }

    private void FinalizeAsr()
    {
        if (_state != DemoState.Listening) return;
        _state = DemoState.Thinking;
        var buf = new StringBuilder(512);
        var res = JarvisNative.jarvis_asr_finalize(buf, (UIntPtr)buf.Capacity);
        var text = res >= 0 ? buf.ToString() : "";
        perfOverlay?.RecordAsrLatencyMs(silenceMsToFinalize);
        EventBus.EmitAsrFinal(text);
        RouteAndAct(text);
    }

    private void RouteAndAct(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Speak("I didn't catch that.");
            return;
        }

        var lower = text.ToLowerInvariant();
        if (lower.Contains("calculator"))
        {
            EventBus.EmitToolStarted("open_app:calculator");
            ToolBridge.QueueOpenApp("calculator");
            EventBus.EmitToolFinished("open_app:calculator");
            Speak("Opening calculator.");
        }
        else if (lower.Contains("browser"))
        {
            EventBus.EmitToolStarted("open_app:browser");
            ToolBridge.QueueOpenApp("browser");
            EventBus.EmitToolFinished("open_app:browser");
            Speak("Opening browser.");
        }
        else
        {
            Speak($"You said: {text}");
        }
    }

    private void Speak(string text)
    {
        _state = DemoState.Speaking;
        hud?.SetState(HudController.HudState.Speaking);
        hud?.SetSubtitle(text);
        EventBus.EmitTtsStarted(text);
        tts?.Speak(text);
    }

    private void CancelAndHide()
    {
        _state = DemoState.Idle;
        audioService?.StopAsrStream();
        hud?.SetState(HudController.HudState.Idle);
        hud?.SetSubtitle("Canceled");
        hud?.Hide();
        EventBus.EmitUiHudHide();
    }
}

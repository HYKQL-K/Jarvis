using System;

public static class EventBus
{
    public const string WakeDetected = "wake.detected";
    public const string AsrPartial = "asr.partial";
    public const string AsrFinal = "asr.final";
    public const string DialogIntent = "dialog.intent";
    public const string ToolStarted = "tool.started";
    public const string ToolFinished = "tool.finished";
    public const string TtsStarted = "tts.started";
    public const string TtsFinished = "tts.finished";
    public const string UiHudShow = "ui.hud.show";
    public const string UiHudHide = "ui.hud.hide";
    public const string GestureChanged = "gesture.changed";

    public static event Action OnWakeDetected;
    public static event Action<string> OnAsrPartial;
    public static event Action<string> OnAsrFinal;
    public static event Action<string> OnDialogIntent;
    public static event Action<string> OnToolStarted;
    public static event Action<string> OnToolFinished;
    public static event Action<string> OnTtsStarted;
    public static event Action<string> OnTtsFinished;
    public static event Action OnUiHudShow;
    public static event Action OnUiHudHide;
    public static event Action<string> OnGestureChanged;

    public static void EmitWakeDetected() => OnWakeDetected?.Invoke();
    public static void EmitAsrPartial(string text) => OnAsrPartial?.Invoke(text);
    public static void EmitAsrFinal(string text) => OnAsrFinal?.Invoke(text);
    public static void EmitDialogIntent(string intent) => OnDialogIntent?.Invoke(intent);
    public static void EmitToolStarted(string tool) => OnToolStarted?.Invoke(tool);
    public static void EmitToolFinished(string tool) => OnToolFinished?.Invoke(tool);
    public static void EmitTtsStarted(string text) => OnTtsStarted?.Invoke(text);
    public static void EmitTtsFinished(string text) => OnTtsFinished?.Invoke(text);
    public static void EmitUiHudShow() => OnUiHudShow?.Invoke();
    public static void EmitUiHudHide() => OnUiHudHide?.Invoke();
    public static void EmitGestureChanged(string label) => OnGestureChanged?.Invoke(label);
}

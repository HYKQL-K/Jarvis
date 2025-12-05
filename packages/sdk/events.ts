export const Events = {
  WakeDetected: "wake.detected",
  AsrPartial: "asr.partial",
  AsrFinal: "asr.final",
  DialogIntent: "dialog.intent",
  ToolStarted: "tool.started",
  ToolFinished: "tool.finished",
  TtsStarted: "tts.started",
  TtsFinished: "tts.finished",
  UiHudShow: "ui.hud.show",
  UiHudHide: "ui.hud.hide",
} as const;

export type EventName = (typeof Events)[keyof typeof Events];

export type EventPayloads = {
  [Events.WakeDetected]: undefined;
  [Events.AsrPartial]: { text: string };
  [Events.AsrFinal]: { text: string };
  [Events.DialogIntent]: { intent: string; slots?: Record<string, string | number | boolean> };
  [Events.ToolStarted]: { tool: string; callId?: string };
  [Events.ToolFinished]: { tool: string; callId?: string; result?: unknown; error?: string };
  [Events.TtsStarted]: { text: string };
  [Events.TtsFinished]: { text: string };
  [Events.UiHudShow]: undefined;
  [Events.UiHudHide]: undefined;
};

import { EventEmitter } from "events";

export type OpenAppInput = { app: "calculator" | "browser" };
export type OpenAppPayload = { type: "open_app"; app: OpenAppInput["app"] };
export type OpenAppResult = { ok: true; dispatched: boolean; payload: OpenAppPayload };

// Simple bridge emitter so a host can forward tool intents to a consumer (e.g., Unity).
export const bridge = new EventEmitter();

export async function run(input: OpenAppInput): Promise<OpenAppResult> {
  const payload: OpenAppPayload = { type: "open_app", app: input.app };
  bridge.emit("tool", payload);
  return { ok: true, dispatched: bridge.listenerCount("tool") > 0, payload };
}

export default { run, bridge };

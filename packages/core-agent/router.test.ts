import { describe, expect, it, afterEach } from "vitest";
import route from "./router";
import { remember, clearMemory } from "./memory";

afterEach(() => {
  clearMemory();
});

describe("route", () => {
  it("routes calculator intents to open_app:calculator", () => {
    const result = route({ utterance: "open the calculator" });
    expect(result).toEqual({ tool: { type: "open_app", app: "calculator" } });
  });

  it("extracts volume percentage when present", () => {
    const result = route({ utterance: "please set the volume to 75%" });
    expect(result).toEqual({ tool: { type: "set_volume", level: 75 } });
  });

  it("creates PDF search queries", () => {
    const result = route({ utterance: "search pdf about cats" });
    expect(result).toEqual({ tool: { type: "search_pdfs", query: "about cats" } });
  });

  it("returns remembered answers when confidence is high", () => {
    remember("how are you", "Doing great!");
    const result = route({ utterance: "how are you" });
    expect(result).toEqual({ replyDraft: "Doing great!" });
  });
});

import { InMemoryVectorStore } from "../rag/vectorstore";

const store = new InMemoryVectorStore();

export function remember(key: string, value: string) {
  store.upsert({ id: key, text: key, metadata: { key, answer: value } });
}

export function recall(query: string): { answer: string | null; score: number } {
  const results = store.search(query, 1);
  if (results.length === 0) return { answer: null, score: 0 };
  const top = results[0];
  const answer = typeof top.metadata?.answer === "string" ? top.metadata.answer : top.text;
  return { answer, score: top.score };
}

export function clearMemory() {
  store.clear();
}

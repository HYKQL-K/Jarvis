import { embed, cosine } from "./embeddings";

export type VectorDoc = {
  id: string;
  text: string;
  metadata?: Record<string, unknown>;
  embedding?: number[];
};

export class InMemoryVectorStore {
  private docs: VectorDoc[] = [];

  upsert(doc: VectorDoc) {
    const existingIdx = this.docs.findIndex((d) => d.id === doc.id);
    const emb = doc.embedding ?? embed(doc.text);
    const withEmb = { ...doc, embedding: emb };
    if (existingIdx >= 0) {
      this.docs[existingIdx] = withEmb;
    } else {
      this.docs.push(withEmb);
    }
  }

  search(query: string, k = 5): Array<VectorDoc & { score: number }> {
    const qEmb = embed(query);
    return this.docs
      .map((d) => ({ ...d, score: cosine(qEmb, d.embedding ?? []) }))
      .sort((a, b) => b.score - a.score)
      .slice(0, k);
  }

  clear() {
    this.docs = [];
  }
}

import { createHash } from "crypto";

export type EmbeddingOptions = { dim?: number };

export function embed(text: string, opts: EmbeddingOptions = {}): number[] {
  const dim = opts.dim ?? 384;
  const tokens = text
    .trim()
    .toLowerCase()
    .split(/\s+/)
    .filter(Boolean);
  const vec = new Array(dim).fill(0);
  for (const t of tokens) {
    const h = createHash("sha256").update(t).digest();
    for (let i = 0; i < 4; i++) {
      const idx = h[i] % dim;
      vec[idx] += 1;
    }
  }
  const norm = Math.sqrt(vec.reduce((s, v) => s + v * v, 0)) || 1;
  return vec.map((v) => v / norm);
}

export function cosine(a: number[], b: number[]): number {
  if (a.length !== b.length) return 0;
  let dot = 0;
  let na = 0;
  let nb = 0;
  for (let i = 0; i < a.length; i++) {
    dot += a[i] * b[i];
    na += a[i] * a[i];
    nb += b[i] * b[i];
  }
  return dot / (Math.sqrt(na) * Math.sqrt(nb) + 1e-6);
}

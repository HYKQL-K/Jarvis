import fs from "fs";
import path from "path";
import { createHash } from "crypto";

export type LocalSearchHit = {
  file: string;
  page: number;
  text: string;
  score: number;
};

export type LocalSearchResult = {
  hits: LocalSearchHit[];
  summary: string;
};

type Chunk = {
  file: string;
  page: number;
  text: string;
  tokens: string[];
  embedding?: number[];
};

const DOCS_DIR = path.resolve(process.cwd(), "assets", "docs");
const MAX_RESULTS = 5;
const CHUNK_TOKENS = 512;
const CHUNK_OVERLAP = 128;

// Optional: onnxruntime-node for bge-m3 embeddings if available locally.
async function loadOrt() {
  try {
    // eslint-disable-next-line @typescript-eslint/no-var-requires
    const ort = require("onnxruntime-node");
    return ort;
  } catch (_err) {
    return null;
  }
}

// Fallback hash-based embedding (deterministic, not semantic).
function hashEmbed(tokens: string[], dim = 384): number[] {
  const vec = new Array(dim).fill(0);
  tokens.forEach((tok) => {
    const h = createHash("sha256").update(tok).digest();
    for (let i = 0; i < 4; i++) {
      const idx = h[i] % dim;
      vec[idx] += 1;
    }
  });
  const norm = Math.sqrt(vec.reduce((s, v) => s + v * v, 0)) || 1;
  return vec.map((v) => v / norm);
}

function cosine(a: number[], b: number[]): number {
  if (a.length !== b.length || a.length === 0) return 0;
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

// Tokenize text coarsely (whitespace-based).
function tokenize(text: string): string[] {
  return text
    .replace(/\s+/g, " ")
    .trim()
    .split(" ")
    .filter(Boolean);
}

function chunkTokens(tokens: string[]): string[][] {
  const stride = CHUNK_TOKENS - CHUNK_OVERLAP;
  const chunks: string[][] = [];
  for (let i = 0; i < tokens.length; i += stride) {
    const slice = tokens.slice(i, i + CHUNK_TOKENS);
    if (slice.length > 0) {
      chunks.push(slice);
    }
    if (slice.length < CHUNK_TOKENS) break;
  }
  return chunks;
}

async function extractPdfText(pdfPath: string): Promise<{ page: number; text: string }[]> {
  // Try pdf-parse if available; otherwise return empty.
  try {
    // eslint-disable-next-line @typescript-eslint/no-var-requires
    const pdfParse = require("pdf-parse");
    const dataBuffer = fs.readFileSync(pdfPath);
    const parsed = await pdfParse(dataBuffer, { pagerender: (pageData: any) => pageData.getTextContent() });
    if (parsed && parsed.text) {
      // pdf-parse does not return page-level text directly; fallback to full text as single page.
      return [{ page: 1, text: parsed.text }];
    }
  } catch (err) {
    console.warn(`PDF parse failed for ${pdfPath}: ${err}`);
  }
  return [];
}

async function embedText(text: string, ort: any, session: any): Promise<number[]> {
  if (!ort || !session) {
    return hashEmbed(tokenize(text));
  }

  // Minimal placeholder: use hash embedding even when ORT present until tokenizer + inputs are wired.
  // Replace with proper tokenizer and bge-m3 model inputs when available.
  return hashEmbed(tokenize(text));
}

async function ensureOrtSession(modelPath?: string): Promise<any | null> {
  const ort = await loadOrt();
  if (!ort) return null;
  const resolvedModel = modelPath || path.resolve(process.cwd(), "assets", "models", "bge-m3.onnx");
  try {
    const session = await ort.InferenceSession.create(resolvedModel, { executionProviders: ["cpu"] });
    return session;
  } catch (err) {
    console.warn(`Failed to load ONNX model at ${resolvedModel}: ${err}`);
    return null;
  }
}

async function loadChunks(): Promise<Chunk[]> {
  const files = fs.existsSync(DOCS_DIR)
    ? fs.readdirSync(DOCS_DIR).filter((f) => f.toLowerCase().endsWith(".pdf"))
    : [];

  const chunks: Chunk[] = [];
  for (const file of files) {
    const full = path.join(DOCS_DIR, file);
    const pages = await extractPdfText(full);
    for (const { page, text } of pages) {
      const tokens = tokenize(text);
      const tokenChunks = chunkTokens(tokens);
      tokenChunks.forEach((tokChunk, idx) => {
        const chunkText = tokChunk.join(" ");
        chunks.push({
          file,
          page: page + idx, // approximate page offset if chunking splits pages
          text: chunkText,
          tokens: tokChunk,
        });
      });
    }
  }

  return chunks;
}

export async function searchLocalPdfs(query: string, topK = MAX_RESULTS): Promise<LocalSearchResult> {
  const ort = await loadOrt();
  const session = await ensureOrtSession();

  const [queryTokens, chunks] = await Promise.all([Promise.resolve(tokenize(query)), loadChunks()]);

  const queryEmbed = session ? await embedText(query, ort, session) : hashEmbed(queryTokens);
  for (const chunk of chunks) {
    chunk.embedding = await embedText(chunk.text, ort, session);
  }

  const scored: LocalSearchHit[] = chunks
    .map((chunk) => {
      const emb = chunk.embedding || hashEmbed(chunk.tokens);
      return {
        file: chunk.file,
        page: chunk.page,
        text: chunk.text,
        score: cosine(queryEmbed, emb),
      };
    })
    .sort((a, b) => b.score - a.score)
    .slice(0, topK);

  const summary = summarize(scored.map((h) => h.text).join(" "));
  return { hits: scored, summary };
}

export function summarize(text: string): string {
  if (!text) return "";
  const sentences = text.split(/(?<=[.!?])\s+/).filter(Boolean);
  const summary = sentences.slice(0, 2).join(" ");
  return summary.length > 240 ? summary.slice(0, 240) + "…" : summary;
}

export default { searchLocalPdfs, summarize };

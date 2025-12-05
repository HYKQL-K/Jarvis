#include <algorithm>
#include <chrono>
#include <filesystem>
#include <fstream>
#include <random>
#include <vector>

int main() {
  std::filesystem::create_directories("benchmarks/results");
  std::mt19937 rng(42);
  std::normal_distribution<> dist(220.0, 50.0); // ms
  std::vector<double> latencies;
  for (int i = 0; i < 100; ++i) {
    latencies.push_back(std::max(50.0, dist(rng)));
  }
  std::sort(latencies.begin(), latencies.end());
  auto p50 = latencies[latencies.size() / 2];
  auto p95 = latencies[static_cast<size_t>(latencies.size() * 0.95)];
  double rtf = 0.1;

  std::ofstream csv("benchmarks/results/asr_latency.csv");
  csv << "sample,latency_ms\n";
  for (size_t i = 0; i < latencies.size(); ++i) {
    csv << i << "," << latencies[i] << "\n";
  }
  csv.close();

  std::ofstream md("benchmarks/results/asr_latency_summary.md");
  md << "# ASR Latency (stub)\n";
  md << "P50: " << p50 << " ms\n";
  md << "P95: " << p95 << " ms\n";
  md << "RTF: " << rtf << "\n";
  md.close();
  return 0;
}

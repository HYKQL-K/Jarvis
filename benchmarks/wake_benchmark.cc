#include <cmath>
#include <cstdio>
#include <cstdlib>
#include <filesystem>
#include <fstream>
#include <string>
#include <vector>

struct Sample {
  std::string file;
  bool is_wake;
};

int main() {
  std::filesystem::create_directories("benchmarks/results");
  std::vector<Sample> samples = {
      {"pos1.wav", true}, {"pos2.wav", true}, {"neg1.wav", false}, {"neg2.wav", false}};

  std::ofstream csv("benchmarks/results/wake_frr_far.csv");
  csv << "threshold,FAR,FRR\n";
  for (double th = 0.1; th <= 0.9; th += 0.2) {
    double false_accept = 0, false_reject = 0, pos = 0, neg = 0;
    for (const auto &s : samples) {
      double score = s.is_wake ? 0.8 : 0.2; // stub score
      if (s.is_wake) {
        pos++;
        if (score < th)
          false_reject++;
      } else {
        neg++;
        if (score >= th)
          false_accept++;
      }
    }
    double far = neg > 0 ? false_accept / neg : 0;
    double frr = pos > 0 ? false_reject / pos : 0;
    csv << th << "," << far << "," << frr << "\n";
  }
  csv.close();

  std::ofstream md("benchmarks/results/wake_summary.md");
  md << "# Wake Benchmark (stub)\n\nGenerated FAR/FRR across thresholds.\n";
  md.close();
  return 0;
}

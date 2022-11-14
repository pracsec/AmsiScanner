using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common {
    public class ScanBenchmarkResults {
        public TimeSpan Total {
            get;
        }

        public TimeSpan Average {
            get;
        }

        public TimeSpan Best {
            get;
        }

        public TimeSpan Worst {
            get;
        }

        public ScanBenchmarkResults(TimeSpan total, TimeSpan average, TimeSpan best, TimeSpan worst) {
            this.Total = total;
            this.Average = average;
            this.Best = best;
            this.Worst = worst;
        }
    }

    public class SigfindByCharBenchmarkResults {
        public TimeSpan Total {
            get;
        }

        public TimeSpan Average {
            get;
        }

        public TimeSpan Best {
            get;
        }

        public TimeSpan Worst {
            get;
        }

        public int TotalIterations {
            get;
        }

        public float AverageIterations {
            get;
        }

        public int AmsiCalls {
            get;
        }

        public float AverageAmsiCalls {
            get;
        }

        public SigfindByCharBenchmarkResults(TimeSpan total, TimeSpan average, TimeSpan best, TimeSpan worst, int totalIterations, float avgIterstions, int amsicalls, float avgAmsiCalls) {
            this.Total = total;
            this.Average = average;
            this.Best = best;
            this.Worst = worst;
            this.TotalIterations = totalIterations;
            this.AverageIterations = avgIterstions;
            this.AmsiCalls = amsicalls;
            this.AverageAmsiCalls = avgAmsiCalls;
        }
    }

    public static class Benchmark {
        public static ScanBenchmarkResults Scan(FileInfo path, int iterations) {
            using (AmsiSession session = new AmsiSession()) {
                byte[] contents = Utility.ReadAllBytes(path.FullName);
                string text = default(string);
                AmsiResult result = default(AmsiResult);
                bool isText = Utility.TryGetText(contents, out text);

                long totalTicks = 0;
                long bestTicks = int.MaxValue;
                long worstTicks = 0;

                Stopwatch watch = new Stopwatch();
                for (int i = 0; i < iterations; i++) {
                    watch.Restart();
                    if (isText) {
                        result = session.ScanString(text);
                    } else {
                        result = session.ScanData(contents);
                    }
                    watch.Stop();

                    totalTicks += watch.ElapsedTicks;
                    bestTicks = Math.Min(bestTicks, watch.ElapsedTicks);
                    worstTicks = Math.Max(worstTicks, watch.ElapsedTicks);
                }

                TimeSpan total = new TimeSpan(totalTicks);
                TimeSpan best = new TimeSpan(bestTicks);
                TimeSpan worst = new TimeSpan(worstTicks);
                TimeSpan average = new TimeSpan((long)(totalTicks / (float)iterations));

                return new ScanBenchmarkResults(total, average, best, worst);
            }
        }

        public static SigfindByCharBenchmarkResults SigfindPerChar(DirectoryInfo directory, SignatureFinderByCharSettings settings) {
            FileInfo[] filepaths = directory.GetFiles();

            long totalTicks = 0;
            long bestTicks = int.MaxValue;
            long worstTicks = 0;
            int totalIterations = 0;
            int totalAmsiCalls = 0;
            Stopwatch watch = new Stopwatch();
            for (int i = 0; i < filepaths.Length; i++) {
                FileInfo current = filepaths[i];
                string content = Utility.ReadAllText(current.FullName);
                int currentIterations = 0;
                watch.Restart();
                SignatureFinderByChar sigfind = new SignatureFinderByChar(content, settings);
                sigfind.VerboseIterationComplete += (iterations, stepSize, steps, delta) => {
                    currentIterations = iterations;
                };
                sigfind.Start();
                sigfind.Wait(300000);
                sigfind.Stop();
                watch.Stop();

                totalTicks += watch.ElapsedTicks;
                bestTicks = Math.Min(bestTicks, watch.ElapsedTicks);
                worstTicks = Math.Max(worstTicks, watch.ElapsedTicks);
                totalIterations += currentIterations;
                totalAmsiCalls += (int)sigfind.AmsiCallCount;
            }

            TimeSpan total = new TimeSpan(totalTicks);
            TimeSpan best = new TimeSpan(bestTicks);
            TimeSpan worst = new TimeSpan(worstTicks);
            TimeSpan average = new TimeSpan((long)(totalTicks / (float)filepaths.Length));
            float averageIterations = totalIterations / (float)filepaths.Length;
            float averageAmsiCalls = totalAmsiCalls / (float)filepaths.Length;

            return new SigfindByCharBenchmarkResults(total, average, best, worst, totalIterations, averageIterations, totalAmsiCalls, averageAmsiCalls);
        }
    }
}

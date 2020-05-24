using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace neuroverkko {
    public class Population {
        private Runner[] runners;
        private Runner previousBestRunner;

        private int generation;

        public Population(int size) {
            runners = new Runner[size].Select(_ => new Runner()).ToArray();
            previousBestRunner = runners[0].Clone();
        }

        private Runner GetRandomRunner(Runner[] fromRunners) => fromRunners[SyncRandom.NextInt(0, fromRunners.Length)].Clone();

        public (Runner bestRunner, decimal avgFitness, int generation) Run() {
            generation += 1;
            Runner bestRunner = previousBestRunner.Clone();
            var fitnesses = new decimal[runners.Length];

            var currentBestFitness = decimal.MinValue;
            for (var runnerIndex = 0; runnerIndex < runners.Length; runnerIndex++) {
                var currentRunner = runners[runnerIndex];
                var (fitness, _) = currentRunner.Run();
                fitnesses[runnerIndex] = fitness;

                if (fitness > currentBestFitness) {
                    bestRunner = currentRunner;
                    currentBestFitness = fitness;
                }
            }

            previousBestRunner = bestRunner.Clone();

            var newRunners = new Runner[runners.Length];
            newRunners[0] = bestRunner.Clone();

            for (var runnerIndex = 1; runnerIndex < runners.Length; runnerIndex++) {
                var child = GetRandomRunner(runners).Crossover(GetRandomRunner(runners));
                child.Mutate();
                newRunners[runnerIndex] = child;
            }

            runners = newRunners.Clone() as Runner[];

            return (
                bestRunner,
                CalculateAverage(fitnesses),
                generation
            );
        }

        public decimal CalculateAverage(decimal[] arr) {
            int count = arr.Length;
            return ((arr.Sum(n => n % count) + (count * (count - 1))) / count) - (count - 1)
                   + arr.Sum(n => n / count);
        }
    }
}

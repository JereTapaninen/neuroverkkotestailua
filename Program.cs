using System;

namespace neuroverkko {
    public class Program {
        public static void Main() {
            Console.WriteLine("Hello World!");

            var pop = new Population(10);

            while (true) {
                Console.Clear();
                var (bestRunner, avgFitness, generation) = pop.Run();
                var (fitness, output) = bestRunner.Run();
                Console.WriteLine(
                    "Goal: {0}\n\nGeneration: {1}\nAverage fitness: {2}\n\nBest fitness: {3}\nBest output: {4}",
                    Constants.DesiredResult,
                    generation,
                    avgFitness,
                    fitness,
                    output
                );
            }
        }
    }
}

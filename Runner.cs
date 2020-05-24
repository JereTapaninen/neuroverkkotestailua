using System;
using System.Diagnostics;

namespace neuroverkko {
    public class Runner {
        private NeuralNet brain;

        public Runner() => brain = new NeuralNet(1, 16, 32, 1);

        /// <summary>
        /// Runs the neural network runner
        /// </summary>
        /// <returns>The fitness</returns>
        public (decimal fitness, int output) Run() {
            var output = brain.Output(new float[] { 0 });
            // Debug.WriteLine(output);
            var fitness = -Math.Abs((decimal)Constants.DesiredResult - (decimal)output);
            return (fitness, output);
        }

        public void Mutate() => brain.Mutate(Constants.MutationRate);

        public Runner Crossover(Runner parent) {
            var child = new Runner {
                brain = brain.Crossover(parent.brain)
            };
            return child;
        }

        public Runner Clone() {
            var clone = new Runner {
                brain = brain.Clone()
            };
            return clone;
        }
    }
}

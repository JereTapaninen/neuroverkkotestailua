using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace neuroverkko {
    public class NeuralNet {
        private readonly int inputNodeCount;
        private readonly int hiddenNodeCount;
        private readonly int outputNodeCount;
        private readonly int hiddenLayerCount;
        private readonly Matrix[] weights;

        public NeuralNet(
            int inputNodeCount,
            int hiddenNodeCount,
            int outputNodeCount,
            int hiddenLayerCount
        ) {
            this.inputNodeCount = inputNodeCount;
            this.hiddenNodeCount = hiddenNodeCount;
            this.outputNodeCount = outputNodeCount;
            this.hiddenLayerCount = hiddenLayerCount;

            weights = new Matrix[hiddenLayerCount + 1];
            weights[0] = new Matrix(hiddenNodeCount, inputNodeCount + 1);

            for (var hiddenLayerIndex = 1; hiddenLayerIndex < hiddenLayerCount; hiddenLayerIndex++) {
                weights[hiddenLayerIndex] = new Matrix(hiddenNodeCount, hiddenNodeCount + 1);
            }

            weights[^1] = new Matrix(outputNodeCount, hiddenNodeCount + 1);

            foreach (var weight in weights) {
                weight.Randomize();
            }
        }

        public void Mutate(float mutationRate) {
            foreach (var weight in weights)
                weight.Mutate(mutationRate);
        }

        public int Output(float[] inputsArr) {
            var inputs = weights[0].SingleColumnMatrixFromArray(inputsArr);
            var currBias = inputs.AddBias();

            for (var hiddenLayerIndex = 0; hiddenLayerIndex < hiddenLayerCount; hiddenLayerIndex++) {
                var hiddenIp = weights[hiddenLayerIndex].Dot(currBias);
                var hiddenOp = hiddenIp.Activate();

                currBias = hiddenOp.AddBias();
            }

            var outputIp = weights[^1].Dot(currBias);
            var outputFloatArr = outputIp.Activate();

            var tempArr = outputFloatArr.ToArray().ToList().Select(val => Convert.ToBoolean(val)).ToArray();
            return Convert.ToInt32(string.Join("", tempArr.Select(b => b ? 1 : 0)), 2);
        }

        public NeuralNet Crossover(NeuralNet partner) {
            var child = new NeuralNet(inputNodeCount, hiddenNodeCount, outputNodeCount, hiddenLayerCount);

            for (var weightIndex = 0; weightIndex < weights.Length; weightIndex++) {
                child.weights[weightIndex] = weights[weightIndex].Crossover(partner.weights[weightIndex]);
            }

            return child;
        }

        public NeuralNet Clone() {
            var clone = new NeuralNet(inputNodeCount, hiddenNodeCount, outputNodeCount, hiddenLayerCount);

            for (var weightIndex = 0; weightIndex < weights.Length; weightIndex++) {
                clone.weights[weightIndex] = weights[weightIndex].Clone();
            }

            return clone;
        }

        public void Load(Matrix[] weightArr) {
            for (var weightIndex = 0; weightIndex < weights.Length; weightIndex++) {
                weights[weightIndex] = weightArr[weightIndex];
            }
        }

        public Matrix[] Pull() {
            var model = weights.Clone() as Matrix[];
            return model;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace neuroverkko {
    public class Matrix {
        private readonly int rowCount;
        private readonly int columnCount;
        private readonly float[,] matrix;

        public Matrix(int rowCount, int columnCount) {
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            matrix = new float[rowCount, columnCount];
        }

        public Matrix Dot(Matrix partner) {
            var result = new Matrix(rowCount, partner.columnCount);

            if (columnCount == partner.rowCount) {
                for (var ownRowIndex = 0; ownRowIndex < rowCount; ownRowIndex++) {
                    for (var partnerColumnIndex = 0; partnerColumnIndex < partner.columnCount; partnerColumnIndex++) {
                        var sum = 0f;

                        for (var ownColumnIndex = 0; ownColumnIndex < columnCount; ownColumnIndex++) {
                            sum += matrix[ownRowIndex, ownColumnIndex] * partner.matrix[ownColumnIndex, partnerColumnIndex];
                        }

                        result.matrix[ownRowIndex, partnerColumnIndex] = sum;
                    }
                }
            }

            return result;
        }

        public void Randomize() {
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                for (var columnIndex = 0; columnIndex < columnCount; columnIndex++) {
                    matrix[rowIndex, columnIndex] = SyncRandom.NextFloat(-1, 1);
                }
            }
        }

        public Matrix SingleColumnMatrixFromArray(float[] arr) {
            var result = new Matrix(arr.Length, 1);

            for (var arrayIndex = 0; arrayIndex < arr.Length; arrayIndex++) {
                result.matrix[arrayIndex, 0] = arr[arrayIndex];
            }

            return result;
        }

        public float[] ToArray() {
            var result = new float[rowCount * columnCount];

            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                for (var columnIndex = 0; columnIndex < columnCount; columnIndex++) {
                    result[columnIndex + (rowIndex * columnCount)] = matrix[rowIndex, columnIndex];
                }
            }

            return result;
        }

        public Matrix AddBias() {
            var result = new Matrix(rowCount + 1, 1);

            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                result.matrix[rowIndex, 0] = matrix[rowIndex, 0];
            }
            result.matrix[rowCount, 0] = 1;

            return result;
        }

        public Matrix Activate() {
            var result = new Matrix(rowCount, columnCount);

            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                for (var columnIndex = 0; columnIndex < columnCount; columnIndex++) {
                    result.matrix[rowIndex, columnIndex] = BinaryStep(matrix[rowIndex, columnIndex]);
                }
            }

            return result;
        }

        public void Mutate(float mutationRate) {
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                for (var columnIndex = 0; columnIndex < columnCount; columnIndex++) {
                    var rand = SyncRandom.NextFloat();

                    if (rand < mutationRate) {
                        // Debug.WriteLine(string.Format("mutated {0}:{1}", rowIndex, columnIndex));
                        matrix[rowIndex, columnIndex] += SyncRandom.NextGaussian() / 5;

                        if (matrix[rowIndex, columnIndex] > 1)
                            matrix[rowIndex, columnIndex] = 1;
                        else if (matrix[rowIndex, columnIndex] < -1)
                            matrix[rowIndex, columnIndex] = -1;
                    }
                }
            }
        }

        public Matrix Crossover(Matrix partner) {
            var child = new Matrix(rowCount, columnCount);

            var randomRow = SyncRandom.NextInt(0, rowCount);
            var randomColumn = SyncRandom.NextInt(0, columnCount);

            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                for (var columnIndex = 0; columnIndex < columnCount; columnIndex++) {
                    child.matrix[rowIndex, columnIndex] = (rowIndex < randomRow) || (rowIndex == randomRow && columnIndex <= randomColumn) ?
                        matrix[rowIndex, columnIndex] :
                        partner.matrix[rowIndex, columnIndex];
                }
            }

            return child;
        }

        public Matrix Clone() {
            var result = new Matrix(rowCount, columnCount);

            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                for (var columnIndex = 0; columnIndex < columnCount; columnIndex++) {
                    result.matrix[rowIndex, columnIndex] = matrix[rowIndex, columnIndex];
                }
            }

            return result;
        }

        public float BinaryStep(float x) => x < 0 ? 0 : 1;
        public float Relu(float x) => Math.Max(0, x);
    }
}

using System;

namespace neuroverkko {
    public static class SyncRandom {
        private readonly static Random random = new Random();
        private readonly static object syncLock = new object();

        public static float NextFloat() {
            lock (syncLock) {
                return (float)random.NextDouble();
            }
        }

        public static float NextFloat(int min, int max) {
            lock (syncLock) {
                return (float)(random.NextDouble() * (max * 2)) - Math.Abs(min);
            }
        }

        public static int NextInt(int min, int max) => random.Next(min, max);

        public static float NextGaussian(int v = 1) {
            var r = 0d;

            for (var i = v; i > 0; i--) {
                lock (syncLock) {
                    r += random.NextDouble();
                }
            }

            return (float)(r / v);
        }
    }
}

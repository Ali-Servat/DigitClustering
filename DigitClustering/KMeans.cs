namespace DigitClustering
{
    public class KMeans
    {
        private static Random r = new();
        public static int[] Cluster(int[][] data, int clusterCount)
        {
            int[] output = new int[data.Length];
            double[][] centeroids = GenerateInitialCenteroids(clusterCount, data[0].Length);

            int epochCounter = 0;
            while (true)
            {
                epochCounter++;
                var temp = new int[output.Length];
                Array.Copy(output, temp, output.Length);

                var distanceMatrix = CreateDistanceMatrix(data, centeroids);
                output = UpdateAssignments(distanceMatrix);
                centeroids = UpdateCenteroids(output, clusterCount, data);

                if (temp.SequenceEqual(output)) break;
            }
            return output;
        }
        private static double[][] UpdateCenteroids(int[] assignments, int clusterCount, int[][] data)
        {
            double[][] updatedCenteroids = new double[clusterCount][];
            for (int cluster = 0; cluster < clusterCount; cluster++)
            {
                updatedCenteroids[cluster] = new double[data[0].Length];
                int[] indices = GetMatchingIndices(assignments, cluster);

                for (int matchedIndex = 0; matchedIndex < indices.Length; matchedIndex++)
                {
                    int currentIndex = indices[matchedIndex];
                    for (int k = 0; k < updatedCenteroids[cluster].Length; k++)
                    {
                        updatedCenteroids[cluster][k] += data[currentIndex][k];
                    }
                }
                for (int dimension = 0; dimension < data[0].Length; dimension++)
                {
                    updatedCenteroids[cluster][dimension] /= indices.Length;
                }
            }

            return updatedCenteroids;
        }

        private static int[] GetMatchingIndices(int[] array, int target)
        {
            return array
            .Select((value, index) => new { value, index })
            .Where(item => item.value == target)
            .Select(item => item.index)
            .ToArray();
        }
        private static int[] UpdateAssignments(double[][] distanceMatrix)
        {
            int[] output = new int[distanceMatrix.Length];

            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                int minIndex = 0;
                double minDistance = double.MaxValue;
                for (int j = 0; j < distanceMatrix[0].Length; j++)
                {
                    if (distanceMatrix[i][j] < minDistance)
                    {
                        minDistance = distanceMatrix[i][j];
                        minIndex = j;
                    }
                }
                output[i] = minIndex;
            }
            return output;
        }
        private static double[][] GenerateInitialCenteroids(int clusterCount, int size)
        {
            double[][] output = new double[clusterCount][];

            for (int i = 0; i < clusterCount; i++)
            {
                double[] newInitialCenteroid = new double[size];
                for (int j = 0; j < size; j++)
                {
                    newInitialCenteroid[j] = r.Next(2);
                }
                output[i] = newInitialCenteroid;
            }
            return output;
        }
        private static double CalculateDistanceBetweenPoints(double[] point1, double[] point2)
        {
            double sum = 0;

            for (int i = 0; i < point1.Length; i++)
            {
                sum += Math.Pow(point2[i] - point1[i], 2);
            }
            return Math.Sqrt(sum);
        }
        private static double[][] CreateDistanceMatrix(int[][] data, double[][] centeroids)
        {
            double[][] distanceMatrix = new double[data.Length][];

            for (int i = 0; i < data.Length; i++)
            {
                distanceMatrix[i] = new double[centeroids.Length];
                for (int j = 0; j < centeroids.Length; j++)
                {
                    double[] convertedData = data[i].Select(x => (double)x).ToArray();
                    var distance = CalculateDistanceBetweenPoints(convertedData, centeroids[j]);
                    distanceMatrix[i][j] = distance;
                }
            }
            return distanceMatrix;
        }
    }
}

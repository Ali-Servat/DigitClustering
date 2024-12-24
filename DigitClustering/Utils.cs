namespace DigitClustering
{
    public class Utils
    {
        public static Dictionary<int, int> MapClustersToLabels(int[] clusterAssignments, int[] trueLabels)
        {
            Dictionary<int, Dictionary<int, int>> clusterLabelCounts = new();

            // Count occurrences of each true label in each cluster
            for (int i = 0; i < clusterAssignments.Length; i++)
            {
                int cluster = clusterAssignments[i];
                int label = trueLabels[i];

                if (!clusterLabelCounts.ContainsKey(cluster))
                    clusterLabelCounts[cluster] = new Dictionary<int, int>();

                if (!clusterLabelCounts[cluster].ContainsKey(label))
                    clusterLabelCounts[cluster][label] = 0;

                clusterLabelCounts[cluster][label]++;
            }

            // Determine the majority label for each cluster
            Dictionary<int, int> clusterToLabelMap = new();
            foreach (var cluster in clusterLabelCounts)
            {
                int clusterIndex = cluster.Key;
                int majorityLabel = cluster.Value
                    .OrderByDescending(pair => pair.Value)
                    .First().Key; // Get the label with the highest count

                clusterToLabelMap[clusterIndex] = majorityLabel;
            }

            return clusterToLabelMap;
        }
        public static void MapAssignments(int[] targets, int[] clusterAssignments)
        {
            var map = Utils.MapClustersToLabels(clusterAssignments, targets);

            for (int i = 0; i < clusterAssignments.Length; i++)
            {
                clusterAssignments[i] = map[clusterAssignments[i]];
            }
        }
        public static T[,] AddMatrices<T>(T[,] matrix1, T[,] matrix2) where T : struct
        {
            var sum = new T[matrix1.GetLength(0), matrix1.GetLength(1)];
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix1.GetLength(1); j++)
                {
                    dynamic a = matrix1[i, j];
                    dynamic b = matrix2[i, j];
                    sum[i, j] = a + b;
                }
            }

            return sum;
        }
    }
}

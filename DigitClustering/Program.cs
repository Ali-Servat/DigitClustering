using DigitClustering;
using static DigitClustering.DataHandler;
using static DigitClustering.EvaluationReporter;
using static DigitClustering.Utils;

(var inputs, var targets) = GetAllData();

while (true)
{
    Console.Clear();
    Console.WriteLine("Digit Clustering Assignment");
    Console.WriteLine();

    ClusteringAlgorithm algorithm = ClusteringAlgorithm.Kmeans;
    SOM.Topology topology = SOM.Topology.Linear;

    while (true)
    {
        var selectedAlgorithm = GetInput("Enter the clustering algorithm: (k for kmeans and s for SOM) ");
        if (selectedAlgorithm == "k")
        {
            algorithm = ClusteringAlgorithm.Kmeans;
            break;
        }
        else if (selectedAlgorithm == "s")
        {
            algorithm = ClusteringAlgorithm.SOM;
            break;
        }
    }

    if (algorithm == ClusteringAlgorithm.SOM)
    {
        while (true)
        {
            var selectedTopology = GetInput("Enter the topology for SOM: (l for linear, r for rectangular and h for hexagonal) ");
            if (selectedTopology == "l")
            {
                topology = SOM.Topology.Linear;
                break;
            }
            else if (selectedTopology == "r")
            {
                topology = SOM.Topology.Rectangular;
                break;
            }
            else if (selectedTopology == "h")
            {
                topology = SOM.Topology.Hexagonal;
                break;
            }
        }
    }

    Console.WriteLine("Clustering started. please wait...");
    (var confusionMatrix, var evaluationMatrix) = ExecuteClustering(inputs, targets, algorithm, topology);
    PrintResults(confusionMatrix, evaluationMatrix);
    Console.WriteLine("Press any key to restart the program: ");
    Console.ReadKey();
}

static void CompareAvgPerformances(int[][] inputs, int[] targets)
{
    Console.WriteLine("Calculating the average performance for linear topology SOM...");
    var avgPerformance = GetAveragePerformance(inputs, targets, 10, ClusteringAlgorithm.SOM, SOM.Topology.Linear);
    PrintEvaluationTable(avgPerformance);
    PrintMacroScores(avgPerformance);

    Console.WriteLine("Calculating the average performance for rectangular topology SOM...");
    avgPerformance = GetAveragePerformance(inputs, targets, 10, ClusteringAlgorithm.SOM, SOM.Topology.Rectangular);
    PrintEvaluationTable(avgPerformance);
    PrintMacroScores(avgPerformance);

    Console.WriteLine("Calculating the average performance for hexagonal topology SOM...");
    avgPerformance = GetAveragePerformance(inputs, targets, 10, ClusteringAlgorithm.SOM, SOM.Topology.Hexagonal);
    PrintEvaluationTable(avgPerformance);
    PrintMacroScores(avgPerformance);

    Console.WriteLine("Calculating the average performance for kmeans...");
    avgPerformance = GetAveragePerformance(inputs, targets, 10, ClusteringAlgorithm.Kmeans);
    PrintEvaluationTable(avgPerformance);
    PrintMacroScores(avgPerformance);
}
static double[,] GetAveragePerformance(int[][] inputs, int[] targets, int totalIterations, ClusteringAlgorithm clusteringAlgorithm, SOM.Topology topology = SOM.Topology.Linear)
{
    double[,] avgEvaluation = new double[10, 4];
    for (int i = 0; i < totalIterations; i++)
    {
        (int[,] confusionMatrix, double[,] evaluationMatrix) = ExecuteClustering(inputs, targets, clusteringAlgorithm, topology);
        avgEvaluation = AddMatrices(avgEvaluation, evaluationMatrix);
    }

    for (int i = 0; i < avgEvaluation.GetLength(0); i++)
    {
        for (int j = 0; j < avgEvaluation.GetLength(1); j++)
        {
            avgEvaluation[i, j] /= totalIterations;
        }
    }
    return avgEvaluation;
}
static (int[,] confusionMatrix, double[,] evaluationMatrix) ExecuteClustering(int[][] inputs, int[] targets, ClusteringAlgorithm clusteringAlgorithm, SOM.Topology topology)
{
    var clusterAssignments = new int[targets.Length];

    if (clusteringAlgorithm == ClusteringAlgorithm.SOM)
    {
        SOM som = new(5, 2, 1024, topology);
        som.Train(inputs, 50, 0.5, 2.0);

        for (int i = 0; i < clusterAssignments.Length; i++)
        {
            clusterAssignments[i] = som.Classify(inputs[i]);
        }
    }
    else
    {
        clusterAssignments = KMeans.Cluster(inputs, 10);
    }

    MapAssignments(targets, clusterAssignments);

    var confusionMatrix = ConstructConfusionMatrix(targets, clusterAssignments);
    var evaluationMatrix = Evaluate(confusionMatrix);
    return (confusionMatrix, evaluationMatrix);
}
static void PrintResults(int[,] confusionMatrix, double[,] evaluationMatrix)
{
    PrintConfusionMatrix(confusionMatrix);
    PrintEvaluationTable(evaluationMatrix);
    PrintMacroScores(evaluationMatrix);
}
static string? GetInput(string message)
{
    Console.Write(message);
    return Console.ReadLine();
}
enum ClusteringAlgorithm
{
    Kmeans,
    SOM
}

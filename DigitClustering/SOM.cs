namespace DigitClustering
{
    public class SOM
    {
        public enum Topology
        {
            Linear,
            Rectangular,
            Hexagonal
        }

        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private readonly int _inputDimension;
        private readonly double[,] _mapWeights;
        private readonly Random _random;
        private readonly Topology _topology;

        public SOM(int mapWidth, int mapHeight, int inputDimension, Topology topology)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _inputDimension = inputDimension;
            _topology = topology;
            _mapWeights = new double[_mapWidth * _mapHeight, _inputDimension];
            _random = new Random();

            InitializeWeights();
        }

        private void InitializeWeights()
        {
            for (int i = 0; i < _mapWeights.GetLength(0); i++)
            {
                for (int j = 0; j < _inputDimension; j++)
                {
                    _mapWeights[i, j] = _random.NextDouble();
                }
            }
        }

        public void Train(int[][] inputVectors, int epochs, double learningRate, double initialRadius)
        {
            double[,] lastEpochWeights = new double[_mapWidth * _mapHeight, _inputDimension];

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                Copy2dArray(_mapWeights, lastEpochWeights);
                int steps = 5;
                int stepSize = epochs / steps;
                double radius = initialRadius - (epoch / stepSize) * (initialRadius / steps);

                foreach (var input in inputVectors)
                {
                    int winnerIndex = FindBestMatchingNeuron(input);

                    UpdateWeights(winnerIndex, input, learningRate, radius);
                }

                learningRate *= 0.9;
                double mse = MeanSquaredError(_mapWeights, lastEpochWeights);
                //Console.WriteLine($"Epoch {epoch + 1} ended. MSE: {mse}");
            }
        }

        private int FindBestMatchingNeuron(int[] input)
        {
            int bestIndex = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < _mapWeights.GetLength(0); i++)
            {
                double distance = EuclideanDistance(input, GetWeightVector(i));

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
        private void UpdateWeights(int winnerIndex, int[] input, double learningRate, double radius)
        {
            for (int i = 0; i < _mapWeights.GetLength(0); i++)
            {
                double distanceToWinner = TopologyDistance(winnerIndex, i);

                if (distanceToWinner <= radius)
                {
                    for (int j = 0; j < _inputDimension; j++)
                    {
                        _mapWeights[i, j] += learningRate * (input[j] - _mapWeights[i, j]);
                    }
                }
            }
        }
        private double EuclideanDistance(int[] vec1, double[] vec2)
        {
            double sum = 0;
            for (int i = 0; i < vec1.Length; i++)
            {
                sum += Math.Pow(vec1[i] - vec2[i], 2);
            }
            return Math.Sqrt(sum);
        }
        private double TopologyDistance(int index1, int index2)
        {
            switch (_topology)
            {
                case Topology.Linear:
                    return LinearGridDistance(index1, index2);
                case Topology.Rectangular:
                    return RectangularGridDistance(index1, index2);
                case Topology.Hexagonal:
                    return HexagonalGridDistance(index1, index2);
                default:
                    throw new InvalidOperationException("Unknown topology");
            }
        }
        private double RectangularGridDistance(int index1, int index2)
        {
            int x1 = index1 % _mapWidth;
            int y1 = index1 / _mapWidth;
            int x2 = index2 % _mapWidth;
            int y2 = index2 / _mapWidth;

            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        private double LinearGridDistance(int index1, int index2)
        {
            return Math.Abs(index1 - index2);
        }
        private double HexagonalGridDistance(int index1, int index2)
        {
            int x1 = index1 % _mapWidth;
            int y1 = index1 / _mapWidth;
            int x2 = index2 % _mapWidth;
            int y2 = index2 / _mapWidth;

            int dx = x2 - x1;
            int dy = y2 - y1;
            int dz = -dx - dy; // Derived from hexagonal grid properties

            return (Math.Abs(dx) + Math.Abs(dy) + Math.Abs(dz)) / 2.0;
        }
        private double[] GetWeightVector(int index)
        {
            double[] weights = new double[_inputDimension];
            for (int i = 0; i < _inputDimension; i++)
            {
                weights[i] = _mapWeights[index, i];
            }
            return weights;
        }
        private double MeanSquaredError(double[,] vector1, double[,] vector2)
        {
            double meanSquaredError = 0;

            for (int i = 0; i < _mapWeights.GetLength(0); i++)
            {
                for (int j = 0; j < _mapWeights.GetLength(1); j++)
                {
                    meanSquaredError += Math.Pow(vector1[i, j] - vector2[i, j], 2);
                }
            }

            return meanSquaredError;
        }
        private void Copy2dArray(double[,] source, double[,] destination)
        {
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    destination[i, j] = source[i, j];
                }
            }
        }
        public int Classify(int[] input)
        {
            return FindBestMatchingNeuron(input);
        }
    }
}

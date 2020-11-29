using SII5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SII5.Measures
{
    /// <summary> Класс для вычисления расстояний. </summary>
    class DistanceCalculator
    {
        private readonly Node _firstNode;
        private readonly Node _secondNode;

        public DistanceCalculator(Tree tree, string fname, string sname)
        {
            _firstNode = (tree ?? throw new ArgumentNullException(nameof(tree)))
                .GetNode(fname ?? throw new ArgumentNullException(nameof(fname))) ??
                throw new ArgumentNullException(nameof(_firstNode));
            _secondNode = tree.GetNode(sname ?? throw new ArgumentNullException(nameof(sname))) ??
                throw new ArgumentNullException(nameof(_secondNode));
        }

        public DistanceCalculator(Node node1, Node node2)
        {
            _firstNode = node1 ?? throw new ArgumentNullException(nameof(node1));
            _secondNode = node2 ?? throw new ArgumentNullException(nameof(node2));
        }

        public double CalculateDistance(MeasureType type = MeasureType.EuclideanDistance)
        {
            switch (type)
            {
                case MeasureType.EuclideanDistance:
                    return CalculateEuclideanDistance(_firstNode, _secondNode);
                case MeasureType.ManhattanDistance:
                    return CalculateManhattanDistance(_firstNode, _secondNode);
                case MeasureType.TreeDistance:
                    return CalculateTreeDistance(_firstNode, _secondNode);
                case MeasureType.Correlation:
                    return CalculateCorrelationDistance(_firstNode, _secondNode);
                default: return 0.0;
            }
        }

        public double CalculateEuclideanDistance(Node _firstNode, Node _secondNode)
        {
            double a1 = Math.Pow(_firstNode.Params.AverageCost - _secondNode.Params.AverageCost, 2);
            double a2 = Math.Pow(_firstNode.Params.MaxSpeed - _secondNode.Params.MaxSpeed, 2);
            double a3 = Math.Pow(_firstNode.Params.MaxStorageCapacity - _secondNode.Params.MaxStorageCapacity, 2);
            double a4 = Math.Pow(_firstNode.Params.ReleaseYear - _secondNode.Params.ReleaseYear, 2);
            double a5 = Math.Pow(Convert.ToDouble(_firstNode.Params.IsGeneralPurpose) -
                Convert.ToDouble(_secondNode.Params.IsGeneralPurpose), 2);

            return Math.Sqrt(a1 + a2 + a3 + a4 + a5);
        }

        public double CalculateManhattanDistance(Node _firstNode, Node _secondNode)
        {
            double a1 = Math.Abs(_firstNode.Params.AverageCost - _secondNode.Params.AverageCost);
            double a2 = Math.Abs(_firstNode.Params.MaxSpeed - _secondNode.Params.MaxSpeed);
            double a3 = Math.Abs(_firstNode.Params.MaxStorageCapacity - _secondNode.Params.MaxStorageCapacity);
            double a4 = Math.Abs(_firstNode.Params.ReleaseYear - _secondNode.Params.ReleaseYear);
            double a5 = Math.Abs(Convert.ToDouble(_firstNode.Params.IsGeneralPurpose) -
                Convert.ToDouble(_secondNode.Params.IsGeneralPurpose));

            return (a1 + a2 + a3 + a4 + a5);
        }

        public double CalculateTreeDistance(Node _firstNode, Node _secondNode)
        {
            var distances = new Dictionary<string, double>();
            distances.Add(_firstNode.Name, 0);

            var stack = new Stack<Node>();
            stack.Push(_firstNode);

            while (stack.Any())
            {
                Node node = stack.Pop();
                var nodes = (node.Parent == null) ? node.Child :
                            node.Child.Union(new[] { node.Parent });
                foreach (Node child in nodes)
                {
                    if (!distances.TryGetValue(child.Name, out _))
                    {
                        distances.Add(child.Name, distances[node.Name] + 1.0);
                        stack.Push(child);
                    }
                }
            }

            return distances[_secondNode.Name];
        }

        public double CalculateCorrelationDistance(Node _firstNode, Node _secondNode)
        {
            double avg1 = (_firstNode.Params.MaxSpeed + _firstNode.Params.MaxStorageCapacity +
                          (double)_firstNode.Params.ReleaseYear + _firstNode.Params.AverageCost +
                          Convert.ToDouble(_firstNode.Params.IsGeneralPurpose)) / 5.0;
            double avg2 = (_secondNode.Params.MaxSpeed + _secondNode.Params.MaxStorageCapacity +
                          (double)_secondNode.Params.ReleaseYear + _secondNode.Params.AverageCost +
                          Convert.ToDouble(_secondNode.Params.IsGeneralPurpose)) / 5.0;

            double[] a1 = new[]
            {
                _firstNode.Params.MaxSpeed - avg1,
                _firstNode.Params.MaxStorageCapacity - avg1,
                (double)_firstNode.Params.ReleaseYear - avg1,
                _firstNode.Params.AverageCost - avg1,
                Convert.ToDouble(_firstNode.Params.IsGeneralPurpose) - avg1,
            };
            double[] a2 = new[]
            {
                _secondNode.Params.MaxSpeed - avg2,
                _secondNode.Params.MaxStorageCapacity - avg2,
                (double)_secondNode.Params.ReleaseYear - avg2,
                _secondNode.Params.AverageCost - avg2,
                Convert.ToDouble(_secondNode.Params.IsGeneralPurpose) - avg2,
            };

            double numerator = a1.Select((a, i) => a * a2[i]).Sum();
            double denominator = a1.Select(a => a * a).Sum() * a2.Select(a => a * a).Sum();

            return numerator / Math.Sqrt(denominator);
        }
    }
}

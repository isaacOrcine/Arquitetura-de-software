using System;
using System.Collections.Generic;
using System.Linq;

namespace ModularizationOportunities.core
{
    public class KMeansAlgorithm
    {
        private readonly Graph _graph;
        private readonly int _k;
        private readonly Dictionary<int, int> _nodeToCluster = new();
        private readonly Dictionary<int, List<int>> _clusterToNodes = new();

        public KMeansAlgorithm(Graph graph, int k)
        {
            _graph = graph;
            _k = k;
        }

        public NodeList FindClusters()
        {
            InitializeClusters();
            bool changed;

            do
            {
                changed = false;
                var newClusters = new Dictionary<int, List<int>>();

                foreach (var node in _graph.Nodes)
                {
                    var bestCluster = GetBestCluster(node);
                    if (_nodeToCluster[node] != bestCluster)
                    {
                        _nodeToCluster[node] = bestCluster;
                        changed = true;
                    }

                    if (!newClusters.ContainsKey(bestCluster))
                    {
                        newClusters[bestCluster] = new List<int>();
                    }
                    newClusters[bestCluster].Add(node);
                }

                _clusterToNodes.Clear();
                foreach (var cluster in newClusters)
                {
                    _clusterToNodes[cluster.Key] = cluster.Value;
                }

            } while (changed);

            return new NodeList(_clusterToNodes.Values.ToList());
        }

        private void InitializeClusters()
        {
            var random = new Random();
            foreach (var node in _graph.Nodes)
            {
                var cluster = random.Next(_k);
                _nodeToCluster[node] = cluster;

                if (!_clusterToNodes.ContainsKey(cluster))
                {
                    _clusterToNodes[cluster] = new List<int>();
                }
                _clusterToNodes[cluster].Add(node);
            }
        }

        private int GetBestCluster(int node)
        {
            double bestDistance = double.MaxValue;
            int bestCluster = _nodeToCluster[node];

            foreach (var cluster in _clusterToNodes.Keys)
            {
                double distance = 0;
                foreach (var clusterNode in _clusterToNodes[cluster])
                {
                    distance += _graph.CalculateDistance(node, clusterNode);
                }
                distance /= _clusterToNodes[cluster].Count;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCluster = cluster;
                }
            }

            return bestCluster;
        }
    }
}
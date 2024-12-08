using System;
using System.Collections.Generic;
using System.Linq;

namespace ModularizationOportunities.core
{
    public class ElbowMethod
    {
        public static int DetermineOptimalK(Graph graph, int maxK)
        {
            var sseList = new List<double>();

            for (int k = 1; k <= maxK; k++)
            {
                var kmeans = new KMeansAlgorithm(graph, k);
                var clusters = kmeans.FindClusters();
                double sse = CalculateSSE(graph, clusters);
                sseList.Add(sse);
            }

            // Encontre o "cotovelo" na lista de SSEs
            int optimalK = FindElbowPoint(sseList);
            return optimalK;
        }

        private static double CalculateSSE(Graph graph, NodeList clusters)
        {
            double sse = 0.0;

            foreach (var cluster in clusters)
            {
                var centroid = CalculateCentroid(graph, cluster);
                foreach (var node in cluster)
                {
                    sse += Math.Pow(graph.CalculateDistance(node, centroid), 2);
                }
            }

            return sse;
        }

        private static int CalculateCentroid(Graph graph, List<int> cluster)
        {
            int centroid = cluster.First();
            double minTotalDistance = double.MaxValue;

            foreach (var candidate in cluster)
            {
                double totalDistance = 0;
                foreach (var node in cluster)
                {
                    totalDistance += graph.CalculateDistance(candidate, node);
                }

                if (totalDistance < minTotalDistance)
                {
                    minTotalDistance = totalDistance;
                    centroid = candidate;
                }
            }

            return centroid;
        }

        private static int FindElbowPoint(List<double> sseList)
        {
            if (sseList.Count < 3)
            {
                return 1; // Não há cotovelo se houver menos de 3 pontos
            }

            // Coordenadas do primeiro e do último ponto
            var firstPoint = new Point(1, sseList[0]);
            var lastPoint = new Point(sseList.Count, sseList.Last());

            double maxDistance = double.MinValue;
            int elbowPoint = 1;

            for (int i = 1; i < sseList.Count - 1; i++)
            {
                var currentPoint = new Point(i + 1, sseList[i]);
                double distance = PerpendicularDistance(firstPoint, lastPoint, currentPoint);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    elbowPoint = i + 1;
                }
            }

            return elbowPoint;
        }

        private static double PerpendicularDistance(Point lineStart, Point lineEnd, Point point)
        {
            double numerator = Math.Abs((lineEnd.Y - lineStart.Y) * point.X - (lineEnd.X - lineStart.X) * point.Y + lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X);
            double denominator = Math.Sqrt(Math.Pow(lineEnd.Y - lineStart.Y, 2) + Math.Pow(lineEnd.X - lineStart.X, 2));
            return numerator / denominator;
        }

        private struct Point
        {
            public double X { get; }
            public double Y { get; }

            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
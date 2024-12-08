namespace ModularizationOportunities.core
{
    public class Graph
    {
        public List<int> Nodes { get; private set; }
        public List<Tuple<int, int>> Edges { get; private set; }

        public Graph()
        {
            Nodes = new List<int>();
            Edges = new List<Tuple<int, int>>();
        }

        public void AddNode(int node)
        {
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);
            }
        }

        public void AddEdge(int node1, int node2)
        {
            if (!Edges.Contains(Tuple.Create(node1, node2)) && !Edges.Contains(Tuple.Create(node2, node1)))
            {
                Edges.Add(Tuple.Create(node1, node2));
            }
        }

        public List<int> GetNeighbors(int node)
        {
            return Edges.Where(e => e.Item1 == node).Select(e => e.Item2)
                .Concat(Edges.Where(e => e.Item2 == node).Select(e => e.Item1)).ToList();
        }

        // Método para calcular a distância entre dois nós
        public double CalculateDistance(int node1, int node2)
        {
            var neighbors1 = GetNeighbors(node1);
            var neighbors2 = GetNeighbors(node2);

            // Exemplo de cálculo de distância baseado na diferença no número de vizinhos
            return Math.Abs(neighbors1.Count - neighbors2.Count);
        }
    }
}
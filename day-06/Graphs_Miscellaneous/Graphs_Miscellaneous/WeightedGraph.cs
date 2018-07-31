using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs_Miscellaneous
{
    class WeightedGraph<T>
    {
        public Dictionary<T, List<Tuple<T, T>>> adjList { get; private set; }
        public int numVertices { get; private set; }

        public WeightedGraph(int n)
        {
            numVertices = n;
            adjList = new Dictionary<T, List<Tuple<T, T>>>();
        }

        public void AddEdge(T src, T dest, T wt)
        {
            if (adjList.ContainsKey(src) == false)
            {
                adjList.Add(src, new List<Tuple<T, T>>());
            }

            if (adjList.ContainsKey(dest) == false)
            {
                adjList.Add(dest, new List<Tuple<T, T>>());
            }

            adjList[src].Add(new Tuple<T, T>(dest, wt));
            adjList[dest].Add(new Tuple<T, T>(src,wt));
        }
    }

    class GraphUser
    {

        class Edge : IComparable<Edge>
        {
            public int id;
            public int weight;
            public Edge(int id, int wt)
            {
                this.id = id;
                this.weight = wt;
            }

            // COMPARETO
            public /*override*/ int CompareTo(Edge e)
            {
                if (this.weight < e.weight) return -1;
                return 1;
            }
        }


        static int[] Dijkstra(WeightedGraph<int> g, int src)
        {

            PriorityQueue<Edge> pq = new PriorityQueue<Edge>();
            int[] distFrmSrc = new int[g.numVertices];      // distFrmSrc[0]
            bool[] visited = new bool[g.numVertices];

            pq.Push(new Edge(src, 0));

            while (pq.Count() != 0)
            {
                Edge curEdge = pq.Top();
                pq.Pop();
                if (visited[curEdge.id] == true) { continue; }

                visited[curEdge.id] = true;
                distFrmSrc[curEdge.id] = curEdge.weight;

                // for all unvisited nbrs
                List<Tuple<int, int>> curNbrList = g.adjList[curEdge.id];
                for(int i = 0; i < curNbrList.Count; ++i)
                {
                    Tuple<int, int> curNbrWt = curNbrList[i];
                    int curNbr = curNbrWt.Item1;
                    int curWt = curNbrWt.Item2;

                    if (visited[curNbr] == false)
                    {
                        int wtFrmSrc = distFrmSrc[curEdge.id] + curWt;
                        pq.Push(new Edge(curNbr, wtFrmSrc));
                    }
                }
            }
            return distFrmSrc;
        }

        public static void main()
        {
            int numVertices, numEdges;
            numVertices = int.Parse(Console.ReadLine());
            numEdges = int.Parse(Console.ReadLine());
            WeightedGraph<int> g = new WeightedGraph<int>(numVertices);

            for (int curEdge = 0; curEdge < numEdges; ++curEdge)
            {
                int[] srcDest = new int[3];
                Program.InputArray(srcDest);
                g.AddEdge(srcDest[0], srcDest[1], srcDest[2]);
            }

            int src = int.Parse(Console.ReadLine());
            int[] minDist = Dijkstra(g, src);

            int minCost = MinCostUsingKruskal(g, numEdges);
            Console.WriteLine(minCost);
            foreach (int cur in minDist)
            {
                Console.Write(cur + " ");
            }
            Console.WriteLine();
        }

        class UnionSet
        {
            int[] parent;
            int[] size;

            public UnionSet(int numItem)
            {
                parent = new int[numItem];
                size = new int[numItem];

                for(int i = 0; i < numItem; ++i)
                {
                    parent[i] = i;
                    size[i] = 1;
                }
            }

            private int Root(int curIdx)
            {
                while(curIdx != parent[curIdx])
                {
                    curIdx = parent[curIdx];
                }
                return curIdx;
            }

            public bool IsSameSet(int item1, int item2)
            {
                return Root(item1) == Root(item2);
            }

            public void MakeUnion(int item1, int item2)
            {
                int root1 = Root(item1);
                int root2 = Root(item2);
                if (size[root1] > size[root2]) {
                    parent[root2] = root1;
                    size[root1] += size[root2];
                }
                else
                {
                    parent[root1] = root2;
                    size[root2] += size[root1];
                }
            }
        }

        static int MinCostUsingKruskal(WeightedGraph<int> g, int numEdges)
        {
            int minCost = 0;
            //As it is a undirected graph, each edge will be added twice, hence size will be twice the number of edges.
            Tuple<int, int, int>[] edgeList = new Tuple<int, int, int>[2 * numEdges];

            // Get Iterartor to traverse adjList
            Dictionary<int, List<Tuple<int, int>>>.Enumerator it = g.adjList.GetEnumerator();
            int index = 0;
            while (it.MoveNext())
            {
                //Get iterator to traverse neighbour list of current node
                List<Tuple<int, int>>.Enumerator nbrIterator = it.Current.Value.GetEnumerator();
                while (nbrIterator.MoveNext())
                {
                    int curVtx = it.Current.Key;
                    int curNbr = nbrIterator.Current.Item1;
                    int curWt = nbrIterator.Current.Item2;
                    edgeList[index++] = new Tuple<int, int, int>(curVtx, curNbr, curWt);
                }
            }

            Array.Sort(edgeList, delegate (Tuple<int, int, int> t1, Tuple<int, int, int> t2) {
                if(t1.Item3 < t2.Item3) { return -1; }
                else { return 1; }
            });

            UnionSet edgeSet = new UnionSet(g.numVertices);
            for (int i = 0; i < 2 * numEdges; ++i)
            {
                var curEdge = edgeList[i];
                int vtx1 = curEdge.Item1;
                int vtx2 = curEdge.Item2;
                if (edgeSet.IsSameSet(vtx1, vtx2) == false)
                {
                    // curEdge is significant
                    minCost += curEdge.Item3;
                    edgeSet.MakeUnion(vtx1, vtx2);
                }
            }
            return minCost;

        }
    }
}

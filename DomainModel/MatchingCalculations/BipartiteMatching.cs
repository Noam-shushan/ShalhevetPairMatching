using PairMatching.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PairMatching.Tools;
using HungarianAlgorithm;


namespace PairMatching.DomainModel.MatchingCalculations
{   
    public class BipartiteMatching
    {
        int[,] _matrix;
        
        bool[,] _adjMatrix;

        HashSet<Edge> _edges;

        HashSet<Verterx> _vertices;

        Verterx _source;
        
        Verterx _targert;

        readonly IEnumerable<PairSuggestion> _pairSuggestions;

        Dictionary<(int, int), PairSuggestion> _indexSuggestions;

        public BipartiteMatching(IEnumerable<PairSuggestion> pairSuggestions)
        {
            _pairSuggestions = pairSuggestions;
            InitializeGraph();
            InitializeMatrix();
        }

        #region Initialize

        private void InitializeGraph()
        {
            _source = new Verterx { Kind = Kind.Source, PartId = "0" };
            _targert = new Verterx { Kind = Kind.Target, PartId = "-1" };
            
            _vertices = new HashSet<Verterx>();
            _edges = new HashSet<Edge>();
            
            int ind = 0;
            foreach (var sp in _pairSuggestions)
            {
                var v = new Verterx { PartId = sp.FromIsrael.Id, Index = ind++, Kind = Kind.FromIsrael };
                var u = new Verterx { PartId = sp.FromWorld.Id, Index = ind++, Kind = Kind.FromWorld };
                _vertices.Add(u);
                _vertices.Add(v);
                _edges.Add(new Edge
                {
                    Weight = 1,
                    Capacity = 1,
                    Flow = 0,
                    V1 = v,
                    V2 = u
                });    
            }
            _edges.UnionWith(from v in _vertices
                      where v.Kind == Kind.FromIsrael
                      select new Edge
                      {
                          V1 = _source,
                          V2 = v,
                          Capacity = 1,
                          Flow = 0,
                          Weight = 1
                      });

            _edges.UnionWith(from v in _vertices
                             where v.Kind == Kind.FromWorld
                             select new Edge
                             {
                                 V1 = v,
                                 V2 = _targert,
                                 Capacity = 1,
                                 Flow = 0,
                                 Weight = 1
                             });
        }

        private void InitializeMatrix()
        {
            var israelIndexs = _pairSuggestions.GetIndexes(p => p.FromIsrael.Id);

            var worldIndexs = _pairSuggestions.GetIndexes(p => p.FromWorld.Id);
                
            var maxMatching = Math.Max(israelIndexs.Count, worldIndexs.Count);

            _matrix = new int[maxMatching, maxMatching];

            _indexSuggestions = new Dictionary<(int, int), PairSuggestion>();

            //foreach pair set the cell to match score 
            foreach (var p in _pairSuggestions)
            {
                var i = israelIndexs[p.FromIsrael.Id];
                var j = worldIndexs[p.FromWorld.Id];
                _matrix[i, j] = p.MatchingScore;
                _indexSuggestions.Add((i, j), p);
            }

            //for maximum problam
            var max = int.MinValue;
            for (int k = 0; k < maxMatching; k++)
                for (int l = 0; l < maxMatching; l++)
                    if (max < _matrix[k, l])
                    {
                        max = _matrix[k, l];
                    }

            //foreach cell in metrix do max - cell
            for (int k = 0; k < maxMatching; k++)
                for (int l = 0; l < maxMatching; l++)
                    _matrix[k, l] = max - _matrix[k, l];
        } 
        #endregion

        public IEnumerable<PairSuggestion> HungarianAlgo()
        {
            int[] assignments = _matrix.FindAssignments();

            var result = new List<PairSuggestion>();

            for (int i = 0; i < assignments.Length; i++)
            {
                if (assignments[i] == -1)
                    continue;

                if (_indexSuggestions.TryGetValue((i, assignments[i]), out PairSuggestion p))
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public IEnumerable<Edge> EdmoudnsKarp()
        {
            var path = FindTarget(); // First BFS
            while(path != null)
            {
                var pathToFlow = _edges.Intersect(path); // The path in the original graph

                int flow = pathToFlow.Min(e => e.Capacity - e.Flow);

                foreach (var e in pathToFlow)
                {
                    e.Flow += flow;
                }

                // get edges from the residual network that are not in the original graph
                var edgesFromResidualNetwork = path.Except(pathToFlow);
                // Reduce the flow from the reverse edges
                foreach (var e in edgesFromResidualNetwork)
                {
                    var reverseEdges = _edges.Where(e2 => e2.V1.Equals(e.V2) && e2.V2.Equals(e.V1));
                    foreach (var e2 in reverseEdges)
                    {
                        e2.Flow -= flow;
                    }
                }
                
                path = FindTarget(); // BFS agine
            }
            // return the final matching
            return _edges.Where(e => e.Flow == e.Capacity && !e.V1.Equals(_source) && !e.V2.Equals(_targert));
        }

        /// <summary>
        /// Finds the target. using BFS algorithem
        /// </summary>
        /// <returns></returns>
        private List<Edge> FindTarget()
        {
            var residualNetwork = ResidualNetwork();

            var visited = new HashSet<Verterx>();

            var queue = new Queue<Verterx>();
            queue.Enqueue(_source);

            var path = new List<Edge>();

            while (queue.Count > 0) // while the queue is not empty
            {
                var vertex = queue.Dequeue();
                visited.Add(vertex);

                // Loop for the adjustments of this vertex
                foreach (var edge in residualNetwork.Where(e => e.V1.Equals(vertex)))
                {
                    if (visited.Contains(edge.V2))
                    {
                        continue;
                    }
                    // Target is found
                    if (edge.V2.Equals(_targert))
                    {   // Build the path to the target
                        path.Add(edge);
                        var pred = edge.V1;
                        while (pred.Pred != null)
                        {
                            path.Add(pred.Pred.Item2);
                            pred = pred.Pred.Item1;
                        }
                        return path;
                    }
                    if (!queue.Contains(edge.V2))
                    {
                        edge.V2.Pred = new(edge.V1, edge);
                        queue.Enqueue(edge.V2);
                    }
                }
            }
            return null;
        }

        HashSet<Edge> ResidualNetwork()
        {
            var residualNetwork = new HashSet<Edge>();
            foreach (var edge in _edges)
            {
                if (edge.Capacity > edge.Flow)
                {
                    residualNetwork.Add(new Edge
                    {
                        V1 = edge.V1,
                        V2 = edge.V2,
                        Weight = edge.Capacity - edge.Flow
                    });             
                }
                else if (edge.Capacity == edge.Flow && edge.Flow > 0)
                {
                    residualNetwork.Add(new Edge
                    {
                        V1 = edge.V2,
                        V2 = edge.V1,
                        Weight = edge.Flow
                    });
                }
            }
            return residualNetwork;
        }

        // A DFS based recursive function
        // that returns true if a matching
        // for vertex u is possible
        bool Bpm(int u,
                 bool[] seen, int[] matchR)
        {
            // Try every job one by one
            for (int v = 0; v < _adjMatrix.GetLength(0); v++)
            {
                // If applicant u is interested
                // in job v and v is not visited
                if (_adjMatrix[u, v] && !seen[v])
                {
                    // Mark v as visited
                    seen[v] = true;

                    // If job 'v' is not assigned to
                    // an applicant OR previously assigned
                    // applicant for job v (which is matchR[v])
                    // has an alternate job available.
                    // Since v is marked as visited in the above
                    // line, matchR[v] in the following recursive
                    // call will not get job 'v' again
                    if (matchR[v] < 0 || Bpm(matchR[v],
                                             seen, matchR))
                    {
                        matchR[v] = u;
                        return true;
                    }
                }
            }
            return false;
        }

        // Returns maximum number of
        // matching from M to N
        public int MaxMatchingAsNumber()
        {
            InitializeMatrixForBpm();
            // An array to keep track of the
            // applicants assigned to jobs.
            // The value of matchR[i] is the
            // applicant number assigned to job i,
            // the value -1 indicates nobody is assigned.
            int len = _adjMatrix.GetLength(0);


            int[] matchR = new int[len];

            // Initially all jobs are available
            for (int i = 0; i < len; ++i)
                matchR[i] = -1;

            // Count of jobs assigned to applicants
            int result = 0;
            for (int u = 0; u < len; u++)
            {
                // Mark all jobs as not
                // seen for next applicant.
                bool[] seen = new bool[len];
                for (int i = 0; i < len; ++i)
                    seen[i] = false;

                // Find if the applicant
                // 'u' can get a job
                if (Bpm(u, seen, matchR))
                    result++;
            }
            return result;
        }

        private void InitializeMatrixForBpm()
        {
            _adjMatrix = new bool[_vertices.Count, _vertices.Count];

            var edges = _edges.Where(e => e.V1 != _source || e.V2 != _targert);

            foreach (var v in _vertices)
            {
                foreach (var u in _vertices)
                {
                    var v_u = edges.FirstOrDefault(e => e.V1.Equals(v) && e.V2.Equals(u));
                    if (v_u != null)
                    {
                        _adjMatrix[v.Index, u.Index] = true;
                    }
                }
            }
        }
    }
}

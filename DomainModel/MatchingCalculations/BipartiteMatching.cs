using PairMatching.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public class BipartiteMatching
    {
        int[,] _matrix;
        
        bool[,] _adjMatrix;

        HashSet<Edge> _edges;

        List<Verterx> _vertices;

        Verterx _source;
        
        Verterx _targert;

        public BipartiteMatching(IEnumerable<PairSuggestion> pairSuggestions, IEnumerable<Participant> participants)
        {
            InitializeVertices(participants);
            InitializeEdges(pairSuggestions);
        }

        private void InitializeVertices(IEnumerable<Participant> participants)
        {
            _source = new Verterx { Kind = Kind.Source, PartId = "0" };
            _targert = new Verterx { Kind = Kind.Target, PartId = "-1" };

            int ind = 0;
            _vertices = participants.Select(p => new Verterx
            {
                PartId = p.Id,
                Index = ind++,
                Kind = p.IsFromIsrael ? Kind.FromIsrael : Kind.FromWorld
            }).ToList();
        }

        private void InitializeEdges(IEnumerable<PairSuggestion> pairSuggestions)
        {
            _edges = (from v in _vertices
                      where v.Kind == Kind.FromIsrael
                      select new Edge
                      {
                          V1 = _source,
                          V2 = v,
                          Capacity = 1,
                          Flow = 0,
                          Weight = 1
                      }).ToHashSet();

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
            var realEdges = from v in _vertices
                            from u in _vertices
                            where pairSuggestions.Any(p =>
                                p.FromIsrael.Id == v.PartId
                                && p.FromWorld.Id == u.PartId)
                            select new Edge
                            {
                                Weight = 1,
                                Capacity = 1,
                                Flow = 0,
                                V1 = v,
                                V2 = u
                            };
            _edges.UnionWith(realEdges);
        }

        public IEnumerable<Edge> EdmoudnsKarp()
        {
            var path = FindTarget();
            while(path != null)
            {
                var pathToFlow = _edges.Intersect(path);
                
                int flow = pathToFlow.Min(e => e.Capacity - e.Flow);
                
                foreach (var e in pathToFlow)
                {
                    e.Flow += flow;
                }
                path = FindTarget();
            }
            return _edges.Where(e => e.Flow == e.Capacity && (e.V1 != _source || e.V2 != _targert));
        }

        /// <summary>
        /// Finds the target. using BFS algorithem
        /// </summary>
        /// <returns></returns>
        private List<Edge> FindTarget()
        {
            var queue = new Queue<Verterx>();
            queue.Enqueue(_source);

            var edges = ResidualNetwork();
            
            var path = new List<Edge>();

            while (queue.Count > 0) // while the queue is not empty
            {
                var vertex = queue.Dequeue();

                // Loop for the adjustments of this vertex
                foreach (var edge in edges.Where(e => e.V1.Equals(vertex)))
                {
                    if (edge.Weight == 0)
                    {
                        continue;
                    }
                    // Target is found
                    if (edge.V2 == _targert)
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
                    if (!queue.Contains(edge.V2)) // If the 
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
                    if (edge.Flow > 0)
                    {
                        residualNetwork.Add(new Edge
                        {
                            V1 = edge.V2,
                            V2 = edge.V1,
                            Weight = edge.Flow
                        });
                    }                   
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
        public int MaxBPM()
        {
            InitializeMatrix();
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

        public IEnumerable<Edge> HungarianAlgorithm()
        {
            InitializeMatrix();
            ReduceRows();
            ReduceColomns();
            return null; // TODO need to be done
        }

        private void ReduceColomns()
        {
            for (int i = 0; i < _matrix.GetLength(1); i++)
            {
                int maxCol = _matrix[0, i];
                for (int j = 1; j < _matrix.GetLength(0); j++)
                {
                    if (_matrix[j, i] > maxCol)
                    {
                        maxCol = _matrix[j, i];
                    }
                }
                for (int j = 0; j < _matrix.GetLength(0); j++)
                {
                    _matrix[j, i] -= maxCol;
                }
            }
        }

        private void ReduceRows()
        {
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                int maxRow = _matrix[i, 0];
                for (int j = 1; j < _matrix.GetLength(1); j++)
                {
                    if (_matrix[i, j] > maxRow)
                    {
                        maxRow = _matrix[i, j];
                    }
                }
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    _matrix[i, j] -= maxRow;
                }
            }
        }

        private void InitializeMatrix()
        {
            _matrix = new int[_vertices.Count, _vertices.Count];

            _adjMatrix = new bool[_vertices.Count, _vertices.Count];

            var edges = _edges.Where(e => e.V1 != _source || e.V2 != _targert);

            foreach (var v in _vertices)
            {
                foreach (var u in _vertices)
                {
                    var v_u = edges.FirstOrDefault(e => e.V1.Equals(v) && e.V2.Equals(u));
                    if (v_u != null)
                    {
                        _matrix[v.Index, u.Index] = v_u.Weight;
                        _adjMatrix[v.Index, u.Index] = true;
                    }
                }
            }
        }
    }
}

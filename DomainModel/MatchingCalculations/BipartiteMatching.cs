using PairMatching.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PairMatching.DomainModel.MatchingCalculations
{
    public enum Kind
    {
        FromIsrael,
        FromWorld,
        Source, 
        Target
    }
    
    public class Verterx : IEquatable<Verterx>
    {
        public string PartId { get; set; }

        public int Index { get; set; }

        public Kind Kind { get; set; }

        public Tuple<Verterx,Edge> Pred { get; set; }

        public bool Equals(Verterx other)
        {
            if (other is null) 
                return false;
            
            return PartId == other.PartId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Verterx);
        }

        public override int GetHashCode()
        {
            return PartId.GetHashCode();
        }
    }

    public class Edge : IEquatable<Edge>
    {
        public Verterx V1 { get; set; }
        public Verterx V2 { get; set; }

        public int Capacity { get; set; }

        public int Flow { get; set; }

        public int Weight { get; set; }

        public bool Equals(Edge other)
        {
            if (other is null)
                return false;

            return V1.Equals(other.V1) && V2.Equals(other.V2);
        }
    }

    public class BipartiteMatching
    {
        //readonly Edge[,] _matrix;

        readonly HashSet<Edge> _edges;

        readonly List<Verterx> _vertices;

        readonly Verterx _source;
        
        readonly Verterx _targert;

        public BipartiteMatching(IEnumerable<PairSuggestion> pairSuggestions, IEnumerable<Participant> participants)
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
            InitializeEdges(pairSuggestions);

        }

        private void InitializeEdges(IEnumerable<PairSuggestion> pairSuggestions)
        {
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
            var path = BFS();
            while(path != null)
            {
                var pathToFlow = _edges.Intersect(path);
                foreach (var e in pathToFlow)
                {
                    e.Flow++;
                }
                path = BFS();
            }
            return _edges.Where(e => e.Flow > 0 && e.V1 != _source && e.V2 != _targert);
        }

        private List<Edge> BFS()
        {
            var queue = new Queue<Verterx>();
            queue.Enqueue(_source);

            var path = new List<Edge>();

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                foreach (var edge in _edges.Where(e => e.V1.Equals(vertex)))
                {
                    if (edge.Capacity == edge.Flow)
                    {
                        continue;
                    }
                    if (edge.V2 == _targert)
                    {
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
    }
}

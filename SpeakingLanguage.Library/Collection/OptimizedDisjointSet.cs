using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library.Collection
{
    public struct OptimizedDisjointSet
    {
        private int[] _parent, _rank, _size;

        public OptimizedDisjointSet(int n)
        {
            _parent = new int[n];
            for (int i = 0; i != n; i++) _parent[i] = i;
            _rank = new int[n];
            for (int i = 0; i != n; i++) _rank[i] = 1;
            _size = new int[n];
            for (int i = 0; i != n; i++) _size[i] = 1;
        }

        public int Find(int u)
        {
            if (u == _parent[u]) return u;
            return _parent[u] = Find(_parent[u]);
        }

        public void Merge(int u, int v)
        {
            u = Find(u);
            v = Find(v);
            if (u == v) return;
            if (_rank[u] > _rank[v])
            {
                int temp = u;
                u = v;
                v = temp;
            }

            _parent[u] = v;
            _size[v] += _size[u];
            if (_rank[u] == _rank[v]) _rank[v]++;
        }

        public void Reset()
        {
            var n = _parent.Length;
            for (int i = 0; i != n; i++) _parent[i] = i;
            for (int i = 0; i != n; i++) _rank[i] = 1;
            for (int i = 0; i != n; i++) _size[i] = 1;
        }

        public void Divide(int u, int v)
        {
        }
    }
}

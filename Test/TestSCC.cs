using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    struct OptimizedDisjointSet
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

        public void Divide(int u, int v)
        {
            u = Find(u);
            v = Find(v);
            if (u != v) return;

            _parent[u] = u;
            _rank[u] = 1;
            _size[u] = 1;

            u = Find(u);
            _size[u]--;
        }
    }

    class TestSCC
    {
        static void Main()
        {
            var disjoint = new OptimizedDisjointSet(100);
            disjoint.Merge(0, 2);
            disjoint.Merge(1, 2);
            disjoint.Merge(2, 3);
            disjoint.Merge(4, 5);
            disjoint.Merge(3, 4);
            disjoint.Merge(4, 7);
            disjoint.Merge(4, 6);
            disjoint.Merge(2, 3);

            Console.ReadLine();
        }
    }
}

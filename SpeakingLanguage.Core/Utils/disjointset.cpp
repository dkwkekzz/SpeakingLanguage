#include "stdafx.h"
#include "disjointset.h"

using namespace SpeakingLanguage::Utils;

disjointset::disjointset(int n) : _parent(n), _rank(n, 1), _size(n, 1)
{
	for (int i = 0; i != n; i++) _parent[i] = i;
}

disjointset::~disjointset()
{
}

int 
disjointset::Find(int u)
{
	if (u == _parent[u]) return u;
	return _parent[u] = this->Find(_parent[u]);
}

// 나와 부모가 같은(실제 최종그룹부모) 대상을 캐싱하여 리스트에 담는다.
void 
disjointset::Merge(int u, int v)
{
	u = this->Find(u);
	v = this->Find(v);
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

void 
disjointset::Reset()
{
	const int n = _parent.size();
	for (int i = 0; i != n; i++) _parent[i] = i;
	for (int i = 0; i != n; i++) _rank[i] = 1;
	for (int i = 0; i != n; i++) _size[i] = 1;
}

void
disjointset::Resize(int capacity)
{
	_parent.resize(capacity);
	_rank.resize(capacity);
	_size.resize(capacity);
}
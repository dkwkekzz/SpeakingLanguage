#pragma once
#include "IAllocator.h"

namespace SpeakingLanguage {
	namespace Utils {

		template<typename KEY_TYPE>
		class splay 
		{
		public:
			struct snode
			{
				KEY_TYPE key;
				snode* lchild;
				snode* rchild;
				int count;

				snode(KEY_TYPE k) : key(k), lchild(nullptr), rchild(nullptr) {}
			};

			snode* find_Kth(int k)
			{
				snode* x = _root;
				while (1)
				{
					while (nullptr != x->l && x->l->count > k)
						x = x->l;
					if (nullptr != x->l)
						k -= x->l->count;
					if (k-- == 0)
						break;
					x = x->r;
				}

				_splay(x);
				return _root;
			}

			snode* find(KEY_TYPE key)
			{
				snode* p = _root;
				if (nullptr == p) return nullptr;

				while (p)
				{
					if (key < p->key)
					{
						if (nullptr == p->l)
							break;
						p = p->l;
					}
					else if (key > p->key)
					{
						if (nullptr == p->r)
							break;
						p = p->r;
					}
					else break;
				}

				_splay(p);

				if (key != p->key)
					return nullptr;

				return p;
			}

			bool insert(KEY_TYPE key, bool overlap, snode* x = nullptr)
			{
				snode* p = _root;
				if (nullptr== p)
				{
					if (nullptr != _allocator)
						_root = _createNode(key, value);
					else
						_root = x;

					return true;
				}

				bool left;
				while (true)
				{
					int ret = key < p->key ? 1 : key > p->key ? -1 : 0;
					if (0 == ret)
					{
						if (overlap)
						{
							p->key = key;
							p->value = value;
							return true;
						}

						ret = -1;
					}

					if (1 == ret)
					{
						if (nullptr == p->l)
						{
							left = true;
							break;
						}
						p = p->l;
					}
					else
					{
						if (nullptr == p->r)
						{
							left = false;
							break;
						}
						p = p->r;
					}
				}

				if (nullptr != _allocator)
					x = _createNode(key, value);

				if (left)
					p->l = x;
				else
					p->r = x;
				x->p = p;
				_splay(x);
				return true;
			}

			bool erase(KEY_TYPE key)
			{
				if (nullptr == find(key))
					return false;

				snode* p = _root;
				if (nullptr!= p->l)
				{
					if (nullptr != p->r)
					{
						_root = p->l;
						_root->p = null;
						snode* x = _root;
						while (nullptr != x->r)
							x = x->r;
						x->r = p->r;
						p->r->p = x;
						_splay(x);
						_destroyNode(p);
						return true;
					}
					_root = p->l;
					_root->p = null;
					_destroyNode(p);
					return true;
				}

				if (nullptr!= p->r)
				{
					_root = p->r;
					_root->p = null;
					_destroyNode(p);
					return true;
				}

				_destroyNode(p);
				_root = null;
				return true;
			}

		private:
			void _splay(snode* x)
			{
				if (nullptr == x->p)
				{
					_update(x);
					return;
				}

				while (nullptr != x->p)
				{
					snode* p = x->p;
					snode* g = p->p;
					if (nullptr != g)
					{
						bool zigzig = (p->l == x) == (g->l == p);
						if (zigzig)
							_rotate(p);
						else
							_rotate(x);
					}

					_rotate(x);
				}
			}

			void _rotate(snode* x)
			{
				snode* p = x->p;
				snode* b;
				if (x == p->l)
				{
					p->l = b = x->r;
					x->r = p;
				}
				else
				{
					p->r = b = x->l;
					x->l = p;
				}

				x->p = p->p;
				p->p = x;
				if (nullptr != b)
					b->p = p;

				snode* g = x->p;
				if (nullptr != g)
				{
					if (p == g->l)
						g->l = x;
					else
						g->r = x;
				}
				else
				{
					_root = x;
				}

				_update(p);
				_update(x);
			}

			void _update(snode* x)
			{
				x->count = 1;
				if (nullptr!= x->l) x->count += x->l->count;
				if (nullptr!= x->r) x->count += x->r->count;
			}

			snode* _createNode(KEY_TYPE key)
			{
				auto* chk = _allocator->Alloc(sizeof(snode));
				chk->typeHandle = 0;

				snode* x = chk->Get<snode>();
				x->key = key;
				x->count = 0;
				return x;
			}

			void _destroyNode(snode* x)
			{
				x->l = nullptr;
				x->p = nullptr;
				x->r = nullptr;

				var chk = umnChunk.GetChunk(x);
				chk->Disposed = true;
			}
		private:
			snode* _root;
			IAllocator* _allocator;
		};

	}
}


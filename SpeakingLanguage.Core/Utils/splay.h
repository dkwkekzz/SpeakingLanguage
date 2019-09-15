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
			};

			splay(IAllocator* allocator = nullptr) : _root(nullptr), _allocator(allocator) {}
			~splay() {}

			/* RR(Y rotates to the right):
					k2                   k1
				   /  \                 /  \
				  k1   Z     ==>       X   k2
				 / \                      /  \
				X   Y                    Y    Z
			*/
			inline snode* RR_Rotate(snode* k2)
			{
				snode* k1 = k2->lchild;
				k2->lchild = k1->rchild;
				k1->rchild = k2;
				return k1;
			}

			/* LL(Y rotates to the left):
					k2                       k1
				   /  \                     /  \
				  X    k1         ==>      k2   Z
					  /  \                /  \
					 Y    Z              X    Y
			 */
			inline snode* LL_Rotate(snode* k2)
			{
				snode* k1 = k2->rchild;
				k2->rchild = k1->lchild;
				k1->lchild = k2;
				return k1;
			}

			snode* New_Node(KEY_TYPE key);
			snode* Splay(KEY_TYPE key);
			snode* Insert(KEY_TYPE key);
			snode* Delete(KEY_TYPE key);
			snode* Search(KEY_TYPE key);
			void InOrder();

		private:
			snode* _root;
			IAllocator* _allocator;
		};

		/* An implementation of top-down splay tree
		 If key is in the tree, then the node containing the key will be rotated to root,
		 else the last non-nullptr node (on the search path) will be rotated to root.
		 */
		template<typename KEY_TYPE>
		splay<KEY_TYPE>::snode*
		splay<KEY_TYPE>::Splay(KEY_TYPE key)
		{
			snode* root = _root;
			if (!root)
				return nullptr;
			snode header;
			/* header.rchild points to L tree; header.lchild points to R Tree */
			header.lchild = header.rchild = nullptr;
			snode* LeftTreeMax = &header;
			snode* RightTreeMin = &header;

			/* loop until root->lchild == nullptr || root->rchild == nullptr; then break!
			   (or when find the key, break too.)
			 The zig/zag mode would only happen when cannot find key and will reach
			 nullptr on one side after RR or LL Rotation.
			 */
			while (1)
			{
				if (key < root->key)
				{
					if (!root->lchild)
						break;
					if (key < root->lchild->key)
					{
						root = RR_Rotate(root); /* only zig-zig mode need to rotate once,
												   because zig-zag mode is handled as zig
												   mode, which doesn't require rotate,
												   just linking it to R Tree */
						if (!root->lchild)
							break;
					}
					/* Link to R Tree */
					RightTreeMin->lchild = root;
					RightTreeMin = RightTreeMin->lchild;
					root = root->lchild;
					RightTreeMin->lchild = nullptr;
				}
				else if (key > root->key)
				{
					if (!root->rchild)
						break;
					if (key > root->rchild->key)
					{
						root = LL_Rotate(root);/* only zag-zag mode need to rotate once,
												  because zag-zig mode is handled as zag
												  mode, which doesn't require rotate,
												  just linking it to L Tree */
						if (!root->rchild)
							break;
					}
					/* Link to L Tree */
					LeftTreeMax->rchild = root;
					LeftTreeMax = LeftTreeMax->rchild;
					root = root->rchild;
					LeftTreeMax->rchild = nullptr;
				}
				else
					break;
			}
			/* assemble L Tree, Middle Tree and R tree together */
			LeftTreeMax->rchild = root->lchild;
			RightTreeMin->lchild = root->rchild;
			root->lchild = header.rchild;
			root->rchild = header.lchild;

			return _root = root;
		}

		template<typename KEY_TYPE>
		splay<KEY_TYPE>::snode*
		splay<KEY_TYPE>::New_Node(KEY_TYPE key)
		{
			snode* p_node;
			if (_allocator == nullptr)
			{
				p_node = new snode();
				if (!p_node) return nullptr;
			}
			else
			{
				auto* chk = _allocator->Alloc(sizeof(snode));
				if (!chk) return nullptr;

				p_node = chk->Get<snode>();
			}

			p_node->key = key;
			p_node->lchild = p_node->rchild = nullptr;
			return p_node;
		}

		/* Implementation 1:
		   First Splay(key, root)(and assume the tree we get is called *), so root node and
		   its left child tree will contain nodes with keys <= key, so we could rebuild
		   the tree, using the newly alloced node as a root, the children of original tree
		   *(including root node of *) as this new node's children.
		NOTE: This implementation is much better! Reasons are as follows in implementation 2.
		NOTE: This implementation of splay tree doesn't allow nodes of duplicate keys!
		 */
		template<typename KEY_TYPE>
		splay<KEY_TYPE>::snode*
		splay<KEY_TYPE>::Insert(KEY_TYPE key)
		{
			snode* p_node = New_Node(key);
			if (p_node == nullptr) return nullptr;

			snode* root = _root;
			if (!root)
			{
				root = p_node;
				p_node = nullptr;
				return root;
			}

			root = Splay(key, root);
			/* This is BST that, all keys <= root->key is in root->lchild, all keys >
			   root->key is in root->rchild. (This BST doesn't allow duplicate keys.) */
			if (key < root->key)
			{
				p_node->lchild = root->lchild;
				p_node->rchild = root;
				root->lchild = nullptr;
				root = p_node;
			}
			else if (key > root->key)
			{
				p_node->rchild = root->rchild;
				p_node->lchild = root;
				root->rchild = nullptr;
				root = p_node;
			}
			return root;
		}

		 /*
		 Implementation: just Splay(key, root), if key doesn't exist in the tree(key !=
		 root->key), return root directly; else join the root->lchild and root->rchild
		 and then free(old_root).
		 To join T1 and T2 (where all elements in T1 < any element in T2) is easy if we
		 have the largest element in T1 as T1's root, and here's a trick to simplify code,
		 see "Note" below.
		  */
		template<typename KEY_TYPE>
		splay<KEY_TYPE>::snode*
		splay<KEY_TYPE>::Delete(KEY_TYPE key)
		{
			snode* root = _root;
			snode* temp;
			if (!root)
				return nullptr;

			root = Splay(key, root);
			if (key != root->key) // No such node in splay tree
				return root;
			else
			{
				if (!root->lchild)
				{
					temp = root;
					root = root->rchild;
				}
				else
				{
					temp = root;
					/*Note: Since key == root->key, so after Splay(key, root->lchild),
					  the tree we get will have no right child tree. (key > any key in
					  root->lchild)*/
					root = Splay(key, root->lchild);
					root->rchild = temp->rchild;
				}
				free(temp);
				return root;
			}
		}

		template<typename KEY_TYPE>
		splay<KEY_TYPE>::snode*
		splay<KEY_TYPE>::Search(KEY_TYPE key)
		{
			auto* ret = Splay(key);
			if (ret->key != key) return nullptr;
			return ret;
		}

		template<typename KEY_TYPE>
		void
		splay<KEY_TYPE>::InOrder()
		{
			snode* root = _root;
			if (root)
			{
				InOrder(root->lchild);
				std::cout << "key: " << root->key;
				if (root->lchild)
					std::cout << " | left child: " << root->lchild->key;
				if (root->rchild)
					std::cout << " | right child: " << root->rchild->key;
				std::cout << "\n";
				InOrder(root->rchild);
			}
		}
	}
}


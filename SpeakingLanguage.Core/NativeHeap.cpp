#include "stdafx.h"
#include "NativeHeap.h"
#include "slChunk.h"

using namespace SpeakingLanguage::Core;

NativeHeap::NativeHeap(int capacity) : _root(nullptr), _head(0)
{
	if (!this->Resize(capacity))
		throw std::bad_alloc();
}

NativeHeap::~NativeHeap() 
{
	free(_root);
}

// [!] 타입마다 핸들을 컴파일타임에 자동으로 계산되도록 해야한다.
slChunk* 
NativeHeap::Alloc(int size)
{
	const int last = _head;
	const int sz = size + sizeof(slChunk);
	_head += sz;

	const int remained = _capacity - _head;
	if (remained < 0 && !this->Resize(_capacity << 1))
		return nullptr;

	slChunk* chk = (slChunk*)((uint64_t)_root + last);
	memset(chk, 0, sz);
	chk->length = size;

	return chk;
}

bool 
NativeHeap::Resize(int capacity)
{	// [!] lock?
	auto* newRoot = malloc(capacity + sizeof(slChunk) + 4);
	if (newRoot == nullptr)
		return false;

	if (_root != nullptr) free(_root);
	_root = newRoot;
	_capacity = capacity;
	return true;
}

void 
NativeHeap::Reset()
{
	_head = 0;
}

void 
NativeHeap::Swap(NativeHeap& lhs, NativeHeap& rhs)
{
	std::swap(lhs._root, rhs._root);
	std::swap(lhs._head, rhs._head);
	std::swap(lhs._capacity, rhs._capacity);
}

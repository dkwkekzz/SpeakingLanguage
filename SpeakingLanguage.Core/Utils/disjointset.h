#pragma once
#include "stdafx.h"

namespace SpeakingLanguage {
	namespace Utils {

		class disjointset
		{
		public:
			explicit disjointset(int n);
			~disjointset();

			int Find(int u);
			void Merge(int u, int v);
			void Reset();
			void Resize(int);

		private:
			std::vector<int> _parent;
			std::vector<int> _rank;
			std::vector<int> _size;
		};
	}
}
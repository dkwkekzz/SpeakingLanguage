#pragma once
#include "slObject.h"

namespace SpeakingLanguage
{
	struct SyncPair
	{
		slObject::TSync subjectSync;
		slObject::TSync targetSync;

		inline bool IsZero() const { return !subjectSync && !targetSync; }
		inline bool Has(const SyncPair& other) const
		{ 
			return (subjectSync && other.subjectSync) > 0 && (targetSync && other.targetSync) > 0;
		}

		inline slObject::TSync GetLeastBit()
		{
			if (subjectSync == 0) return targetSync & -targetSync;
			return subjectSync & -subjectSync;
		}

		inline void DeleteOverlap(const SyncPair& other)
		{
			subjectSync &= ~other.subjectSync;
			targetSync &= ~other.targetSync;
		}

		inline slObject::TSync DeleteLeastBit()
		{
			if (subjectSync == 0) return targetSync &= (targetSync - 1);
			return subjectSync &= (subjectSync - 1);
		}

		static constexpr int MultiplyDeBruijnBitPosition[32] =
		{
		  0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
		  31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
		};

		inline int GetLeastBitPosition()
		{
			if (subjectSync == 0) MultiplyDeBruijnBitPosition[((uint32_t)((targetSync & -targetSync) * 0x077CB531U)) >> 27];
			return MultiplyDeBruijnBitPosition[((uint32_t)((subjectSync & -subjectSync) * 0x077CB531U)) >> 27];
		}

		inline bool operator==(const SyncPair &other) const
		{
			return subjectSync == other.subjectSync && targetSync == other.targetSync;
		}

		inline bool operator!=(const SyncPair &other) const
		{
			return !(*this == other);
		}

		inline bool operator<(const SyncPair &other) const
		{
			return subjectSync < other.subjectSync || (subjectSync == other.subjectSync && targetSync < other.targetSync);
		}

		inline bool operator>(const SyncPair &other) const
		{
			return subjectSync > other.subjectSync || (subjectSync == other.subjectSync && targetSync > other.targetSync);
		}

		inline bool operator<=(const SyncPair &other) const
		{
			return subjectSync <= other.subjectSync && targetSync <= other.targetSync;
		}

		inline bool operator>=(const SyncPair &other) const
		{
			return subjectSync >= other.subjectSync && targetSync >= other.targetSync;
		}
	};
}
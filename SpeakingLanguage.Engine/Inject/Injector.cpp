#include "stdafx.h"
#include "Injector.h"
#include "disjointset.h"

using namespace SpeakingLanguage;

// systemNode
struct Injector::systemNode
{
	std::shared_ptr<ISystem> system;
	std::vector<systemNode> children;

	systemNode() = default;
	systemNode(std::shared_ptr<ISystem> newSystem);

	Result<void> Regist(const ActionSource& action);
	Result<void> Insert(const SyncPair& sync, const InteractPair& itr);
};

Injector::systemNode::systemNode(std::shared_ptr<ISystem> newSystem) : system(newSystem)
{
}

Result<void> Injector::systemNode::Regist(const ActionSource& action)
{
	for (auto& child : children)
	{
		const auto childSync = child.system->GetSync();
		if (action.sync.Has(childSync))
		{
			return child.Regist(action);
		}
	}

	std::unique_ptr<ISystem> newSystem = FactoryHelper::CreateSystem<SimpleSystem>(action);
	std::shared_ptr<ISystem> shared(std::move(newSystem));
	children.emplace_back(shared);

	Locator::SystemCollector.Get()->Add(shared);
	return Done();
}

Result<void> Injector::systemNode::Insert(const SyncPair& sync, const InteractPair& itr)
{
	for (auto& child : children)
	{
		const auto childSync = child.system->GetSync();
		if (sync.Has(childSync))
		{
			child.system->Insert(itr);
			if (sync == childSync) return Done();

			return child.Insert(sync, itr);
		}
	}

	return Error::NullReferenceAction;
}

// Injector
Injector::Injector()
{
}


Injector::~Injector()
{
}

Result<void> Injector::Insert(const Interaction& itr)
{
	auto& map = this->_stmap;
	const auto& subIter = map.find(itr.subject);
	if (subIter == map.end()) return Error::NullReferenceObject;

	const auto& tarIter = map.find(itr.target);
	if (tarIter == map.end()) return Error::NullReferenceObject;

	for (const auto& stIter : (*subIter).second)
	{

	}
}

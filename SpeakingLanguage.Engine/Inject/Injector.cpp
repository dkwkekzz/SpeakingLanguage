#include "stdafx.h"
#include "Injector.h"
#include "FactoryHelper.h"

using namespace SpeakingLanguage;

// 1. regist는 systemcollection으로 뺀다. 곧, actioncollection이 된다.
// 2. 기존구조와 비슷하게 가져오되 레이아웃만 변경된 상태이어야 할 것이다. 
// 3. 기존의 스왑하던 방식을 하나의 버퍼만 사용하도록 바꾼다. add와 remove가 발생하면, 이는 한번에 처리하도록 한다.
// 4. 

// objectPool
struct Injector::objectPool
{	
	std::unordered_map<slObject::THandle, slObject> map;

	const slObject* find(slObject::THandle handle);
};

const slObject* Injector::objectPool::find(slObject::THandle handle)
{
	const auto& ret = map.find(handle);
	if (ret == map.cend()) return nullptr;

	return &(*ret).second;
}

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
Injector::Injector() : _objs(std::make_unique<objectPool>()), _root(std::make_unique<systemNode>())
{
}


Injector::~Injector()
{
}

Result<void> Injector::Insert(const ActionSource& action)
{
	return _root->Regist(action);
}

Result<void> Injector::Insert(const Interaction& interaction)
{
	InteractPair itrPair;
	itrPair.subject = _objs->find(interaction.subject);
	if (itrPair.subject == nullptr) return Error::NullReferenceObject;

	itrPair.target = _objs->find(interaction.target);
	if (itrPair.target == nullptr) return Error::NullReferenceObject;

	SyncPair syncPair{ itrPair.subject->sync, itrPair.target->sync };
	return _root->Insert(syncPair, itrPair);
}

#include "stdafx.h"
#include "CountdownEvent.h"

using namespace SpeakingLanguage::Core::Process;

CountdownEvent::CountdownEvent(int count) : _initCount(count), _currentCount(count) 
{
}

CountdownEvent::~CountdownEvent() 
{ 
	_currentCount.store(0); 
}

void
CountdownEvent::Wait()
{
	while (true)
	{
		int comparand = 0;
		int newValue = 0;
		bool exchanged = _currentCount.compare_exchange_weak(comparand, newValue);
		if (exchanged)
		{
			break;
		}
	}
}
// TestCpp.cpp : 이 파일에는 'main' 함수가 포함됩니다. 거기서 프로그램 실행이 시작되고 종료됩니다.
//

#include "pch.h"
#include <vector>
#include <functional>
#include <memory>
#include <unordered_map>
#include <map>
#include <bitset>
#include "../SpeakingLanguage.Engine/Engine.h"
#pragma comment(lib, "SpeakingLanguage.Engine.lib")

using TFunc = std::function<int(int)>;
std::vector<TFunc> filters;

class Calculator
{
public:
	int Get() { return value; }

private:
	int value{ 7777 };
};

void foo()
{
	int divisor = 5;
	filters.emplace_back([=](int value) { return value % divisor; });

	// shared_ptr이 복사되었으므로 2개가 참조되는데 sp가 사라져도 1개가 남으므로 람다의 캡처는 안전해야 한다.
	// 결과는... 예상대로 복사는 안전하나 참조는 그렇지 않다.
	auto sp = std::make_shared<Calculator>();
	filters.emplace_back([sp](int value) { return value % sp->Get(); });

	// 당연히 죽어버리는 코드...
	//auto rp = new Calculator;
	//filters.emplace_back([rp](int value) { return value % rp->Get(); });
	//delete rp;
}

struct ISystem
{
	int val;
};

template<typename T, int N>
constexpr int sizeOfArr(T (&arr) [N]) { return N; }

int main()
{
    std::cout << "Hello World!\n"; 

	//using namespace SpeakingLanguage;
	Sample(9999);

	std::cout << "size: " << sizeof(std::bitset<64>) << std::endl;

	std::cin.get();
}

// 프로그램 실행: <Ctrl+F5> 또는 [디버그] > [디버깅하지 않고 시작] 메뉴
// 프로그램 디버그: <F5> 키 또는 [디버그] > [디버깅 시작] 메뉴

// 시작을 위한 팁: 
//   1. [솔루션 탐색기] 창을 사용하여 파일을 추가/관리합니다.
//   2. [팀 탐색기] 창을 사용하여 소스 제어에 연결합니다.
//   3. [출력] 창을 사용하여 빌드 출력 및 기타 메시지를 확인합니다.
//   4. [오류 목록] 창을 사용하여 오류를 봅니다.
//   5. [프로젝트] > [새 항목 추가]로 이동하여 새 코드 파일을 만들거나, [프로젝트] > [기존 항목 추가]로 이동하여 기존 코드 파일을 프로젝트에 추가합니다.
//   6. 나중에 이 프로젝트를 다시 열려면 [파일] > [열기] > [프로젝트]로 이동하고 .sln 파일을 선택합니다.

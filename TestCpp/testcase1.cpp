#include <iostream>

class World
{
public:
	World() { std::cout << "world create." << std::endl; }
	~World() { std::cout << "world destroy." << std::endl; }
	std::shared_ptr<class Archer> a1;
};

class Archer
{
public:
	Archer() { std::cout << "archer create." << std::endl; }
	~Archer() { std::cout << "archer destroy." << std::endl; }
	std::shared_ptr<World> w1;
};

void battle(std::shared_ptr< World> w)
{
	auto a = std::make_shared<Archer>();	// a:1
	a->w1 = w;	// w:2
	w->a1 = a;	// a:2
}

int main()
{
	auto w = std::make_shared<World>();	// w:1
	w->a1 = std::make_shared<Archer>();
	w->a1->w1 = w;	// w:2
	w->a1.reset();
	w.reset();

	return 0;
}

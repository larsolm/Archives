#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Globals.h"

using namespace Pargon;

SuperMarioWar::Globals_::Globals_() :
	Window(1280, 720, "Super Mario War")
{
}

auto SuperMarioWar::Globals() -> SuperMarioWar::Globals_&
{
	static SuperMarioWar::Globals_ _globals;
	return _globals;
}

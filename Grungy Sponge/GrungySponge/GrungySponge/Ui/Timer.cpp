#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

void Timer::Start()
{
	_running = true;
}

void Timer::Stop()
{
	_running = false;
}

void Timer::Restart()
{
	_currentTime = 0;
	_running = true;
}

void Timer::Update(float elapsed)
{
	if (!_running)
		return;

	_currentTime += elapsed;
}

int Timer::ToMinutes(float time)
{
	return static_cast<int>(time / 60);
}

int Timer::ToSeconds(float time)
{
	return static_cast<int>(time) % 60;
}

String Timer::MinutesText()
{
	return ToString(ToMinutes(_currentTime));
}

String Timer::SecondsText()
{
	int seconds = ToSeconds(_currentTime);
	String text = ToString(seconds);
	return seconds < 10 ? '0' + text : text;
}

template<> void Pargon::CreateMembers<GrungySponge::Timer>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Timer>();

	type->AddMethod("GetMinutesText", &Timer::MinutesText);
	type->AddMethod("GetSecondsText", &Timer::SecondsText);
}
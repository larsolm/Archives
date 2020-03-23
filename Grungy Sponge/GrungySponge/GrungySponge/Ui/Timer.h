#pragma once

#include "Pargon/GameCore/GameCore.h"

namespace GrungySponge
{
	class Timer : public Pargon::Updater
	{
	private:
		float _currentTime = 0;
		bool _running = false;

	public:
		virtual void Update(float elapsed) override;

		static int ToMinutes(float time);
		static int ToSeconds(float time);

		void Restart();
		void Start();
		void Stop();

		int Minutes();
		int Seconds();
		float TotalTime();

		Pargon::String MinutesText();
		Pargon::String SecondsText();
	};
}

template<> void Pargon::CreateMembers<GrungySponge::Timer>(Reflection::Type* type);

inline
int GrungySponge::Timer::Minutes()
{
	return ToMinutes(_currentTime);
}

inline
int GrungySponge::Timer::Seconds()
{
	return ToSeconds(_currentTime);
}

inline
float GrungySponge::Timer::TotalTime()
{
	return _currentTime;
}
#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "Pargon/Types/List.h"
#include "Pargon/Types/String.h"

namespace Pargon
{
	class Cameara2D;
}

namespace GrungySponge
{
	class ScrollContainer : public Pargon::Updater
	{
	private:
		float _previousX = 0;
		float _scrollX = 0;
		float _destinationX = 0;
		bool _dragging = false;
		int _scrolling = 0;
		Pargon::Camera2D* _camera = nullptr;
		float _rightBounds = 1000;

		void ScrollChildren(float amount);

	public:
        ~ScrollContainer();
        
		Pargon::String ContainerName;

		float LeftBounds = 0;
		float PositionY = 0;
		float Spacing = 700;
		float ScrollSpeed = 5;

		virtual void Update(float elapsed) override;

	protected:
		virtual void Start() override;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::ScrollContainer>(Reflection::Type* type);

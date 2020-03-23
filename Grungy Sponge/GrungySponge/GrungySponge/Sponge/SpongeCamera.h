#pragma once

#include "Pargon/Game2D/Camera2D.h"
#include "Pargon/GameCore/GameCore.h"
#include "GrungySponge/Sponge/Sponge.h"

namespace GrungySponge
{
	class SpongeCamera : public Pargon::Camera2D
	{
	public:
		int XBounds = 0;
		int YBounds = 0;
		int WorldBoundsLeft = 0;
		int WorldBoundsRight = 0;
		int WorldBoundsTop = 0;
		int WorldBoundsBottom = 0;

		virtual void UpdateMatrices(int width, int height, Pargon::Matrix4x4& view, Pargon::Matrix4x4& projection) override;

	private:
		Pargon::GameObjectReference<SpongeData> _sponge;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::SpongeCamera>(Reflection::Type* type);

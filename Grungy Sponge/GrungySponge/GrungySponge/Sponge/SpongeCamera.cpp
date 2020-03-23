#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

void SpongeCamera::UpdateMatrices(int width, int height, Matrix4x4& view, Matrix4x4& projection)
{
	Spatial2D* spatial = _sponge->Entity()->GetComponent<Spatial2D>();

	if (spatial->Position.X > Position.X + XBounds)
		Position.X = spatial->Position.X - XBounds;
	else if (spatial->Position.X < Position.X - XBounds)
		Position.X = spatial->Position.X + XBounds;

	if (spatial->Position.Y > Position.Y + YBounds)
		Position.Y = spatial->Position.Y - YBounds;
	else if (spatial->Position.Y < Position.Y - YBounds)
		Position.Y = spatial->Position.Y + YBounds;

	if (Position.X - (width / 2) < WorldBoundsLeft)
		Position.X = static_cast<float>(WorldBoundsLeft + (width / 2));

	if (Position.X + (width / 2) > WorldBoundsRight)
		Position.X = static_cast<float>(WorldBoundsRight - (width / 2));

	if (Position.Y + (height / 2) > WorldBoundsTop)
		Position.Y = static_cast<float>(WorldBoundsTop - (height / 2));

	if (Position.Y - (height / 2) < WorldBoundsBottom)
		Position.Y = static_cast<float>(WorldBoundsBottom + (height / 2));

	Camera2D::UpdateMatrices(width, height, view, projection);
}

template<> void Pargon::CreateMembers<GrungySponge::SpongeCamera>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<SpongeCamera>();

	type->AddField("XBounds", &SpongeCamera::XBounds);
	type->AddField("YBounds", &SpongeCamera::YBounds);
	type->AddField("WorldBoundsLeft", &SpongeCamera::WorldBoundsLeft);
	type->AddField("WorldBoundsRight", &SpongeCamera::WorldBoundsRight);
	type->AddField("WorldBoundsTop", &SpongeCamera::WorldBoundsTop);
	type->AddField("WorldBoundsBottom", &SpongeCamera::WorldBoundsBottom);
}
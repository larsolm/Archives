#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

template<> void Pargon::CreateMembers<GrungySponge::SpongeData>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<SpongeData>();

	type->AddField("Acceleration", &SpongeData::Acceleration);
	type->AddField("Friction", &SpongeData::Friction);
	type->AddField("MaxSpeed", &SpongeData::MaxSpeed);
	type->AddField("Maneuverability", &SpongeData::Maneuverability);
	type->AddField("MaxAngularVelocity", &SpongeData::MaxAngularVelocity);
	type->AddField("AngularFriction", &SpongeData::AngularFriction);
	type->AddField("JumpScale", &SpongeData::JumpScale);
	type->AddField("JumpHeight", &SpongeData::JumpHeight);
	type->AddField("Gravity", &SpongeData::Gravity);
	type->AddField("SpeedRecharge", &SpongeData::SpeedRecharge);
	type->AddField("BurstRecharge", &SpongeData::BurstRecharge);
	type->AddField("MinimumSize", &SpongeData::MinimumSize);
}

template<>
void Pargon::CreateMembers<SpongeType>(Reflection::Type* type)
{
	Serialization::SerializeAsEnum<SpongeType>();

	type->AddConstant("Normal", SpongeType::Normal);
	type->AddConstant("Jumping", SpongeType::Jumping);
	type->AddConstant("Burst", SpongeType::Burst);
	type->AddConstant("Speed", SpongeType::Speed);
	type->AddConstant("Flip", SpongeType::Flip);
}

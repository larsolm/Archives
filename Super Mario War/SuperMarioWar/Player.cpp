#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Player.h"

using namespace SuperMarioWar;
using namespace Pargon;

template<>
void Pargon::CreateMembers<Player>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Player>();

	type->AddField("Agility", &Player::Agility);
	type->AddField("Gravity", &Player::Gravity);
	type->AddField("WalkSpeed", &Player::WalkSpeed);
	type->AddField("TurboSpeed", &Player::TurboSpeed);
	type->AddField("TerminalVelocity", &Player::TerminalVelocity);
	type->AddField("AirResistance", &Player::AirResistance);
	type->AddField("AirControl", &Player::AirControl);
	type->AddField("HangTime", &Player::HangTime);
	type->AddField("JumpImpulse", &Player::JumpImpulse);
	type->AddField("AnimationSpeed", &Player::AnimationSpeed);
	type->AddField("Traction", &Player::Traction);
	type->AddField("JumpSound", &Player::JumpSound);
	type->AddField("BonkSound", &Player::BonkSound);
	type->AddField("DeathSound", &Player::DeathSound);
	type->AddField("SkidSound", &Player::SkidSound);
	type->AddField("StompSound", &Player::StompSound);
}

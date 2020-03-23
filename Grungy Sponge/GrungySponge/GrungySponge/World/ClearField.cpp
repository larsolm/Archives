#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace Pargon::Graphics;
using namespace GrungySponge;

template<>
void Pargon::CreateMembers<ClearField>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<ClearField>();
	type->AddField("MaskFilename", &ClearField::MaskFilename);
}
